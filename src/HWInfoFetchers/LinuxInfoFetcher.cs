using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Loggitt;

namespace Puter;

public class LinuxInfoFetcher : HWInfo.HWInfoFetcher
{
    public static string CPUModel()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LoggittLogger.Warn("Linux HWInfo called on non-Linux platform");
            return "Unkown";
        }

        try
        {
            string cpuInfo = File.ReadAllText("/proc/cpuinfo");
            var match = Regex.Match(cpuInfo, @"model name\s+:\s+(.+)");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }
        catch (Exception e)
        {
            LoggittLogger.Warn($"Failed to get Linux CPU model: {e.Message}");
        }
        return "Unknown";
    }

    public static int CPUCores()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LoggittLogger.Warn("Linux HWInfo called on non-Linux platform");
            return 0;
        }

        try
        {
            string cpuInfo = File.ReadAllText("/proc/cpuinfo");
            var physicalIds = new HashSet<string>();
            var coreIds = new HashSet<string>();
            string currentPhysicalId = "";

            foreach (var line in cpuInfo.Split('\n'))
            {
                if (line.StartsWith("physical id"))
                {
                    currentPhysicalId = line.Split(':')[1].Trim();
                    physicalIds.Add(currentPhysicalId);
                }
                else if (line.StartsWith("core id"))
                {
                    string coreId = line.Split(':')[1].Trim();
                    coreIds.Add($"{currentPhysicalId}:{coreId}");
                }
            }

            // If we found core ids, use that count (accounts for multi-socket systems)
            if (coreIds.Count > 0)
            {
                return coreIds.Count;
            }
            // If we only found physical ids, use that count (fallback for some systems)
            else if (physicalIds.Count > 0)
            {
                return physicalIds.Count;
            }
            // If all else fails, fall back to processor count
            else
            {
                return Environment.ProcessorCount;
            }
        }
        catch (Exception ex)
        {
            LoggittLogger.Error($"Error reading CPU core count: {ex.Message}");
            return Environment.ProcessorCount; // Fallback to logical processor count
        }
    }

    public static double CPUUsage()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            LoggittLogger.Warn("Linux HWInfo called on non-Linux platform");
            return 0;
        }

        try
        {
            string[] cpuUsage = File.ReadAllText("/proc/stat")
                .Split('\n')[0]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            long idle1 = long.Parse(cpuUsage[4]);
            long total1 = cpuUsage.Skip(1).Select(long.Parse).Sum();

            Thread.Sleep(250);

            cpuUsage = File.ReadAllText("/proc/stat")
                .Split('\n')[0]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            long idle2 = long.Parse(cpuUsage[4]);
            long total2 = cpuUsage.Skip(1).Select(long.Parse).Sum();

            return (1.0 - (idle2 - idle1) / (double)(total2 - total1)) * 100;
        }
        catch (Exception e)
        {
            LoggittLogger.Warn($"Failed to get Linux CPU usage: {e.Message}");
            return 0;
        }
    }

    public static int CPUActiveProcesses()
    {
        try
        {
            return Directory
                .GetDirectories("/proc")
                .Count(dir => int.TryParse(Path.GetFileName(dir), out _));
        }
        catch (Exception e)
        {
            LoggittLogger.Warn($"Failed to get Linux CPU active processes: {e.Message}");
            return 0;
        }
    }
}
