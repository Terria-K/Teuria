using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Teuria;


public static class SkyLog 
{
    public enum LogLevel { Debug, Warning, Error, Assert, Info }
    private static readonly StringBuilder writeLog = new StringBuilder();
    public static LogLevel Verbosity = LogLevel.Info;

    private static void LogInternal(LogLevel level, string message, string cfp, int cln) 
    {
        if (Verbosity < level)
            return;
        
        var logName = level switch
        {
            LogLevel.Debug => "[DEBUG]",
            LogLevel.Info => "[INFO]",
            LogLevel.Error => "[ERROR]",
            LogLevel.Warning => "[WARNING]",
            LogLevel.Assert => "[ASSERT]",
            _ => throw new InvalidOperationException()
        };
        var callSite = $"{Path.GetFileName(cfp)}:{cln}";

#if DEBUG
        {    
            Console.WriteLine(
                $"[{DateTime.Now.ToString("HH:mm:ss")}]{logName} {callSite} {message}"
            );
        }
#endif

        writeLog.AppendLine($"{logName}[{DateTime.Now.ToString("HH:mm:ss")}] {callSite} {message}");

        if (level == LogLevel.Error || level == LogLevel.Assert)
            Debugger.Break();
    }

    public static void Log(
        string log, 
        LogLevel logLevel = LogLevel.Debug,
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0
    ) 
    {
        LogInternal(logLevel, log, callerFilePath, callerLineNumber);
    }

    public static void Log(
        object log,
        LogLevel logLevel = LogLevel.Debug,
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0
    ) 
    {
        LogInternal(logLevel, log.ToString(), callerFilePath, callerLineNumber);
    }

    [Conditional("DEBUG")]
    public static void Assert(
        bool condition, 
        string message, 
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    ) 
    {
        if (!condition)
            LogInternal(LogLevel.Assert, message, callerFilePath, callerLineNumber);
    }

    public static void Error(
        string log, 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0
    ) 
    {
        LogInternal(LogLevel.Error, log, callerFilePath, callerLineNumber);
    }


    public static void Error(
        object log, 
        [CallerFilePath] string callerFilePath = "", 
        [CallerLineNumber] int callerLineNumber = 0
    ) 
    {
        LogInternal(LogLevel.Error, log.ToString(), callerFilePath, callerLineNumber);
    }

#if !ANDROID && !Blazor
    public static void OpenLog(string path) 
    {
        var process = new Process() 
        {
            StartInfo = new ProcessStartInfo(path) 
            {
                UseShellExecute = true
            }
        };
        if (File.Exists(path))
            process.Start();
    }
#endif

    public static void WriteToFile(string path) 
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        WriteToFile(fs);
    }

    public static void WriteToFile(Stream stream) 
    {
        using var textWriter = new StreamWriter(stream);
        textWriter.WriteLine(writeLog.ToString());
    }

    public static async Task WriteToFileAsync(string path, CancellationToken token = default) 
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        await WriteToFileAsync(fs, token);
    }

    public static async Task WriteToFileAsync(Stream stream, CancellationToken token = default) 
    {
        using var textWriter = new StreamWriter(stream);
        await textWriter.WriteLineAsync(writeLog.ToString().AsMemory(), token);
    }
}