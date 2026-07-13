using System;
using System.IO;
using System.Text;

namespace TinynetbootServer.Core
{
    public static class LogManager
    {
        private static readonly object _lock = new();

        private static readonly string _logFolder =
            Path.Combine(AppContext.BaseDirectory, "logs");

        static LogManager()
        {
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }
        }

        public static void Write(string text)
        {
            try
            {
                string filePath = Path.Combine(
                    _logFolder,
                    $"log_{DateTime.Now:yyyy-MM-dd}.txt");

                lock (_lock)
                {
                    File.AppendAllText(
                        filePath,
                        text + Environment.NewLine,
                        Encoding.UTF8);
                }
            }
            catch
            {
                // Ignore log failures
            }
        }
    }
}