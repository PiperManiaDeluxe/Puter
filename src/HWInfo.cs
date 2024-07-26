using System.Runtime.InteropServices;
using Loggitt;

namespace Puter;

public static class HWInfo
{
    internal interface HWInfoFetcher
    {
        static abstract string CPUModel();
        static abstract int CPUCores();
        static abstract double CPUUsage();
        static abstract int CPUActiveProcesses();
    }

    public enum HWInfoType
    {
        OS,
        OSVersion,
        OSMachineName,
        OSUserName,
        Architecture,
        CPUModel,
        CPUCores,
        CPUThreads,
        CPUUsage,
        CPUActiveProcesses
    }

    public static string Get(HWInfoType type)
    {
        if (type == HWInfoType.OS)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "macOS";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return "FreeBSD";
        }
        else if (type == HWInfoType.OSVersion)
        {
            return Environment.OSVersion.VersionString;
        }
        else if (type == HWInfoType.OSMachineName)
        {
            return Environment.MachineName;
        }
        else if (type == HWInfoType.OSUserName)
        {
            return Environment.UserName;
        }
        else if (type == HWInfoType.Architecture)
        {
            return RuntimeInformation.OSArchitecture.ToString();
        }
        else if (type == HWInfoType.CPUThreads)
        {
            return Environment.ProcessorCount.ToString();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsInfo(type);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxInfo(type);
        }
        else
        {
            LoggittLogger.Error("HWInfo.Get: OS not supported");
            return "Unknown";
        }

        return "Unknown";
    }

    private static string GetWindowsInfo(HWInfoType type)
    {
        switch (type)
        {
            case HWInfoType.CPUModel:
                return WinInfoFetcher.CPUModel();
            case HWInfoType.CPUCores:
                return WinInfoFetcher.CPUCores().ToString();
            case HWInfoType.CPUUsage:
                return $"{WinInfoFetcher.CPUUsage():F2}%";
            case HWInfoType.CPUActiveProcesses:
                return WinInfoFetcher.CPUActiveProcesses().ToString();
            default:
                LoggittLogger.Error($"HWInfo.Get: {type}, is not implemented for Windows");
                return "Unknown";
        }
    }

    private static string GetLinuxInfo(HWInfoType type)
    {
        switch (type)
        {
            case HWInfoType.CPUModel:
                return LinuxInfoFetcher.CPUModel();
            case HWInfoType.CPUCores:
                return LinuxInfoFetcher.CPUCores().ToString();
            case HWInfoType.CPUUsage:
                return $"{LinuxInfoFetcher.CPUUsage():F2}%";
            case HWInfoType.CPUActiveProcesses:
                return LinuxInfoFetcher.CPUActiveProcesses().ToString();
            default:
                LoggittLogger.Error($"HWInfo.Get: {type}, is not implemented for Linux");
                return "Unknown";
        }
    }
}
