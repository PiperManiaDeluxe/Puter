using Loggitt;

namespace Puter;

public static class Program
{
    public static void Main()
    {
        LoggittLogger.LogFile = "Puter.log";

        LoggittLogger.LogApplicationStart();

        LoggittLogger.Info("OS:");
        LoggittLogger.Info($"> OS: {HWInfo.Get(HWInfo.HWInfoType.OS)}");
        LoggittLogger.Info($"> Version: {HWInfo.Get(HWInfo.HWInfoType.OSVersion)}");
        LoggittLogger.Info($"> Architecture: {HWInfo.Get(HWInfo.HWInfoType.Architecture)}");
        LoggittLogger.Info($"> Machine name: {HWInfo.Get(HWInfo.HWInfoType.OSMachineName)}");
        LoggittLogger.Info($"> User name: {HWInfo.Get(HWInfo.HWInfoType.OSUserName)}\n");

        LoggittLogger.Info("CPU:");
        LoggittLogger.Info($"> Model: {HWInfo.Get(HWInfo.HWInfoType.CPUModel)}");
        LoggittLogger.Info($"> Cores: {HWInfo.Get(HWInfo.HWInfoType.CPUCores)}");
        LoggittLogger.Info($"> Threads: {HWInfo.Get(HWInfo.HWInfoType.CPUThreads)}");
        LoggittLogger.Info($"> Usage: {HWInfo.Get(HWInfo.HWInfoType.CPUUsage)}");
        LoggittLogger.Info(
            $"> Active processes: {HWInfo.Get(HWInfo.HWInfoType.CPUActiveProcesses)}\n"
        );

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
