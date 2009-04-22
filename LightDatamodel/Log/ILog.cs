using System;
namespace System.Data.LightDatamodel.Log
{
    public enum LogLevel
    {
        Profiling,
        Information,
        Warning,
        Error
    }

    public interface ILog : IDisposable
    {
        void WriteEntry(LogLevel type, string message);
        LogLevel Level { get; set; }
    }
}
