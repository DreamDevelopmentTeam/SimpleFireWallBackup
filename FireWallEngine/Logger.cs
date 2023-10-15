using System;
using System.IO;

namespace FireWallEngine
{
    public enum LogLevel
    {
        Info,
        Error,
        Warn,
        Debug,
        Fatal
    }

    public class Logger
    {
        private static Logger instance;
        private FileStream logFile;
        private bool printOut;
        private bool writeOut;
        private bool enabled;

        private Logger()
        {
            this.logFile = null;
            this.printOut = false;
            this.writeOut = false;
            this.enabled = false;
        }
        
        private Logger(bool printOut, bool writeOut, string fileName = null, FileStream fileStream = null)
        {
            this.enabled = true;
            this.printOut = printOut;
            this.writeOut = writeOut;
            if (fileName != null)
            {
                this.logFile = new FileStream(fileName, FileMode.OpenOrCreate);
            }
            else
            {
                this.logFile = fileStream;
            }
        }

        public static Logger GetInstance(bool printOut = false, bool writeOut = false, string fileName = null, FileStream fileStream = null)
        {
            if (instance == null)
            {
                instance = new Logger(printOut, writeOut, fileName, fileStream);
            }
            return instance;
        }

        private void WriteLog(LogLevel level, string name, string message, Exception ex = null)
        {
            if (enabled)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logEntry = $"[{level}]{timestamp}> {message}";
                if (ex != null)
                {
                    logEntry += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
                }
                if (this.writeOut && this.logFile != null)
                {
                    using (StreamWriter writer = new StreamWriter(logFile))
                    {
                        writer.WriteLine(logEntry);
                    }
                }
                if (this.printOut)
                {
                    switch (level)
                    {
                        case LogLevel.Info:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case LogLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case LogLevel.Warn:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case LogLevel.Debug:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case LogLevel.Fatal:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                    }
                    Console.WriteLine(logEntry);
                    Console.ResetColor();
                }
            }
        }

        public void Info(string name, string message)
        {
            WriteLog(LogLevel.Info, name, message);
        }

        public void Error(string name, string message, Exception ex = null)
        {
            WriteLog(LogLevel.Error, name, message, ex);
        }

        public void Warn(string name, string message)
        {
            WriteLog(LogLevel.Warn, name, message);
        }

        public void Debug(string name, string message)
        {
            WriteLog(LogLevel.Debug, name, message);
        }

        public void Fatal(string name, string message, Exception ex = null)
        {
            WriteLog(LogLevel.Fatal, name, message, ex);
        }

        public void Close()
        {
            if (this.logFile != null)
            {
                this.logFile.Close();
            }
        }
    }
}