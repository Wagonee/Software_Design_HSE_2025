using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace HSE_Bank.ConsoleApp
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddFileLogging(this ILoggingBuilder builder)
        {
            builder.AddProvider(new FileLoggerProvider());
            return builder;
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory); //  Create the directory if it doesn't exist
            string filePath = Path.Combine(logDirectory, "app.log"); //  Log to a single file
            return new FileLogger(filePath);
        }

        public void Dispose() {}
    }

    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public IDisposable? BeginScope<TState>(TState state)
        {
            return null; //  Not needed for simple file logging
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; //  Log all levels
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string logMessage = $"{DateTime.Now} [{logLevel}] {formatter(state, exception)}";
            
            try
            {
                File.AppendAllText(_filePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}