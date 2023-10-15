using System;
using System.IO;

namespace FireWallEngine
{
    public class Logger
    {
        private FileStream logFile;
        private bool printOut = false;

        public Logger(FileStream file)
        {
            this.logFile = file;
        }

        public Logger(string fileName)
        {
            this.logFile = new FileStream(fileName, FileMode.OpenOrCreate);
        }
        
        
        public Logger(FileStream file, bool printOut)
        {
            this.logFile = file;
            this.printOut = this.printOut;
        }

        public Logger(string fileName, bool printOut)
        {
            this.logFile = new FileStream(fileName, FileMode.OpenOrCreate);
            this.printOut = this.printOut;
        }


        public void Info(string name,string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[INFO][{timestamp}]({name})> {message}";
            using (StreamWriter writer = new StreamWriter(logFile))
            {
                writer.WriteLine(logEntry);
            }
        }

        public void Error(string name, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[ERROR][{timestamp}]({name})> {message}";
            using (StreamWriter writer = new StreamWriter(logFile))
            {
                writer.WriteLine(logEntry);
            }
        }
        
        public void Debug(string name, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[DEBUG][{timestamp}]({name})> {message}";
            using (StreamWriter writer = new StreamWriter(logFile))
            {
                writer.WriteLine(logEntry);
            }
        }
        
        public void Warn(string name, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[WARN][{timestamp}]({name})> {message}";
            using (StreamWriter writer = new StreamWriter(logFile))
            {
                writer.WriteLine(logEntry);
            }
        }
        
        public void Fatal(string name, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[Fatal][{timestamp}]({name})> {message}";
            using (StreamWriter writer = new StreamWriter(logFile))
            {
                writer.WriteLine(logEntry);
            }
        }
    }
}