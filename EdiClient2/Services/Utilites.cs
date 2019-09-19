using EdiClient.AppSettings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace EdiClient.Services.Utils
{
    public static class Utilites
    {

        static Utilites(){}
        internal static void Error(Exception ex)
        {
            Logger.Log($"===============================");
            var msg = $"Message: {ex.Message}\nSource: {ex.Source}\n{GetInnerExceptionMessage(ex)}\nTargetSite: {ex?.TargetSite}\n{ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}";
            Logger.Log(msg);
            Logger.Log($"===============================");
            DevExpress.Xpf.Core.DXMessageBox.Show(msg, "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        internal static void Error(string text)
        {
            Logger.Log($"===============================");
            Logger.Log($"[ERROR] {text}");
            Logger.Log($"===============================");
            DevExpress.Xpf.Core.DXMessageBox.Show($"[ОШИБКА] {text}", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private static string GetInnerExceptionMessage(Exception ex)
            => ex.InnerException != null ? ex.Message + GetInnerExceptionMessage(ex.InnerException) : $"\ninner: {ex.Message}\n{ex.Data}\n{ex.Source}\n{ex.TargetSite}\n===========\n";


        public static class Logger
        {
            // коллекция в которой хранится очередь на запись в лог
            private static BlockingCollection<string> log = new BlockingCollection<string>();

            private static string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            private static string dirName = "EdiClient";
            private static string fileName = "EdiClientLog.txt";
            private static string fullPath => Path.GetFullPath($"{directoryPath}\\{dirName}\\{fileName}");

            private static Thread[] threads;

            public static void Run()
            {
                threads = new[] { new Thread(Consumer) };
                foreach (var t in threads)
                    t.Start();
            }

            public static void Stop() // останавливает consumer
            {
                log.CompleteAdding();

                foreach (var t in threads)
                    t.Join();
            }

            // в блокировочную коллекцию ставим в очередь сообщения для записи в лог
            public static void Log(string msg)
            {
                log.Add(msg);
            }


            private static void Consumer()
            {
                foreach (var s in log.GetConsumingEnumerable())
                {
                    WriteFile(s);
                }
            }

            public static void WriteFile(string msg)
            {
                if (!AppConfigHandler.conf.EnableLogging) return;
                if (string.IsNullOrEmpty(msg)) return;
                string message = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] "
                    + $"{msg}\r\n";
                int c = message.Count();
                using (var stream = new FileStream(fullPath, FileMode.Append))
                {
                    stream.Write(Encoding.Default.GetBytes(message), 0, c);
                    stream.Close();
                }
            }

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

            public static void LogXml(string xml, string fileName)
            {
                if (!AppConfigHandler.conf.EnableLogging) return;
                if (string.IsNullOrEmpty(xml)) return;
                string message = xml;
                int c = message.Count();
                using (var stream = new FileStream($"{directoryPath}\\{dirName}\\{fileName}", FileMode.Append))
                {
                    stream.Write(Encoding.Default.GetBytes(message), 0, c);
                    stream.Close();
                }
            }
        }

        internal static class XmlService<TModel>
        {
            internal static List<TModel> Deserialize(string rawDocument)
            {
                List<TModel> Documents = new List<TModel>();
                XmlSerializer ser = new XmlSerializer(typeof(TModel));
                var stream = new StringReader(rawDocument);

                using (XmlReader reader = XmlReader.Create(stream))
                {
                    Documents.Add((TModel)ser.Deserialize(reader));
                }

                return Documents;
            }

            internal static bool Serialize(TModel order, string OutPath)
            {
                XmlSerializer ser = new XmlSerializer(typeof(TModel));
                try
                {
                    using (XmlWriter writer = XmlWriter.Create(OutPath)) ser.Serialize(writer, order);
                }
                catch (Exception ex)
                {
                    Utilites.Error(ex);
                    return false;
                }
                return true;
            }

            internal static string Serialize(TModel order)
            {
                XmlSerializer ser = new XmlSerializer(typeof(TModel));
                StringBuilder builder = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(builder, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
                {
                    ser.Serialize(writer, order);
                }
                return builder.ToString();
            }

        }
        

    }
}