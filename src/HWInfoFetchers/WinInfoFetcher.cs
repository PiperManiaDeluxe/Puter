using System.Diagnostics;
using System.Runtime.InteropServices;
using Loggitt;

namespace Puter;

public class WinInfoFetcher : HWInfo.HWInfoFetcher
{
    public static string CPUModel()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoggittLogger.Warn("Windows HWInfo called on non-Windows platform");
            return "Unkown";
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "cpu get name",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split('\n');
            return lines[1].Trim();
        }
        catch (Exception e)
        {
            LoggittLogger.Warn($"Failed to get Windows CPU model: {e.Message}");
        }

        return "Unknown";
    }

    public static int CPUCores()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoggittLogger.Warn("Windows HWInfo called on non-Windows platform");
            return 0;
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "cpu get NumberOfCores",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split('\n');
            return int.Parse(lines[1].Trim());
        }
        catch (Exception e)
        {
            LoggittLogger.Warn($"Failed to get Windows CPU cores: {e.Message}");
        }

        return 0;
    }

    public static double CPUUsage()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoggittLogger.Warn("Windows HWInfo called on non-Windows platform");
            return 0;
        }

        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        cpuCounter.NextValue(); // First call will always return 0
        Thread.Sleep(250); // Wait for a second
        return cpuCounter.NextValue();
    }

    public static int CPUActiveProcesses()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            LoggittLogger.Warn("Windows HWInfo called on non-Windows platform");
            return 0;
        }

        return Process.GetProcesses().Length;
    }
}
