namespace SDKore.Helper
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class Logger
    {
        public readonly string Path;
        public readonly string FileName;

        public Logger()
        {
            this.Path = Environment.CurrentDirectory.ToString() + @"\Log";
            if (!Directory.Exists(this.Path))
                Directory.CreateDirectory(this.Path);

            this.FileName = System.IO.Path.Combine(this.Path, string.Format("{0}-{1}.txt", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"), FileName));

            File.Delete(this.FileName);
            File.CreateText(this.FileName).Close();
        }

        public Logger(string fileName, bool deleteIfExists)
        {
            this.Path = Environment.CurrentDirectory.ToString() + @"\Log";
            if (!Directory.Exists(this.Path))
                Directory.CreateDirectory(this.Path);

            this.FileName = System.IO.Path.Combine(this.Path, string.Format("{0}-{1}.txt", DateTime.Now.ToString("yyyy-MM-dd_HHmmss"), fileName));

            if (!File.Exists(this.FileName))
            {
                File.CreateText(this.FileName).Close();
            }
            else if (deleteIfExists)
            {
                File.Delete(this.FileName);
                File.CreateText(this.FileName).Close();
            }
        }

        public void Log(string logMessage, bool stampTime, bool printConsole)
        {
            lock (this.FileName)
            {
                StreamWriter writer = File.AppendText(this.FileName);
                if (stampTime)
                {
                    writer.WriteLine(logMessage + "\t(" + DateTime.Now.ToString() + ")");
                }
                else
                {
                    writer.WriteLine(logMessage);
                }
                writer.Close();
                if (printConsole)
                {
                    Console.WriteLine(logMessage);
                }
            }
        }
    }
}

