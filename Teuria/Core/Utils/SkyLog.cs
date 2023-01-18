using System;

namespace Teuria;

public static class SkyLog 
{
    public delegate void SkyLogger(string log, object[] args);
    private static SkyLogger logFn;
    static SkyLog() 
    {
        SkyLog.logFn ??= Console.WriteLine;
    }
    public static void Initialize(SkyLogger fn) 
    {
        SkyLog.logFn = fn;
    }

    public static void Log(string log) 
    {
        SkyLog.logFn(log, new object[0]);
    }

    public static void Log(string log, params object[] args) 
    {
        SkyLog.logFn(log, args);
    }
}