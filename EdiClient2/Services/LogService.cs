using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiClient.Services
{
    public static class LogService
    {
        private static string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string dirName = "EdiClient";
        private static string fileName = "EdiClientLog.txt";
        private static string fullPath => Path.GetFullPath($"{directoryPath}\\{dirName}\\{fileName}");

        public static string Read()
        {
            var log = "";
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                long c = stream.Length;
                byte[] buffer = new byte[c + 1];
                stream.Read(buffer, 0, (int)c);
                log = Encoding.Default.GetString(buffer);
                stream.Close();
            }
            return log;
        }

        public static void Log(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string message = $"[{DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}.{DateTime.UtcNow.Millisecond}] "
                + $"{msg}\r\n";
            int c = message.Count();
            using (var stream = new FileStream(fullPath, FileMode.Append))
            {
                stream.Write(Encoding.Default.GetBytes(message), 0, c);
                stream.Close();
            }
        }

        public static string FormatArgsArray(Type[] types)
        {
            string ret = ""; int i = 1;
            foreach(var type in types)            
                ret += $"arg{i++}: {type.Name}";            
            return ret;
        }


    }
}
