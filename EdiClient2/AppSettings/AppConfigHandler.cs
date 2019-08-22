using EdiClient.Services;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EdiClient.AppSettings
{
    public static class AppConfigHandler
    {
        private static string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string dirName = "EdiClient";
        private static string fileName = "EdiClientAppConfig.xml";
        private static string fullPath => Path.GetFullPath($"{directoryPath}\\{dirName}\\{fileName}");
        public static string LogPath => Path.GetFullPath($"{directoryPath}\\{dirName}");

        public static void ConfigureEdi()
        {
            Utilites.Logger.Log("[EdiService]");
            try { EdiService.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public static void ConfigureOracle()
        {
            Utilites.Logger.Log("[Connection]");
            try { DbService.Connection.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }
            Utilites.Logger.Log("\t\tClientVersion " + DbService.Connection.conn.ClientVersion);
            Utilites.Logger.Log("\t\tServer " + DbService.Connection.conn.Server);
        }

        public static void Load()
        {
            if (!Directory.Exists($"{directoryPath}\\{dirName}"))
                Directory.CreateDirectory($"{directoryPath}\\{dirName}");

            if (!File.Exists(fullPath))
                Create();

            Read();

            Utilites.Logger.Log($"\t\tconf_dir {directoryPath}\\{dirName}");
        }


        public static void Save()
        {
            Create();
        }


        public static void Read()
        {
            Utilites.Logger.Log("[APPCONFIG]");
            var newLoadedConfig = new Model.Common.AppConfig();
            XmlSerializer xml = new XmlSerializer(typeof(Model.Common.AppConfig));
            using (var stream = XmlReader.Create(fullPath))
            {
                newLoadedConfig = (Model.Common.AppConfig)xml.Deserialize(stream);
                stream.Close();
            }

            AppConfig.DbUserName = !string.IsNullOrEmpty(newLoadedConfig.DbUserName) ? newLoadedConfig.DbUserName : AppConfig.DbUserName;
            AppConfig.DbUserPassword = !string.IsNullOrEmpty(newLoadedConfig.DbUserPassword) ? newLoadedConfig.DbUserPassword : AppConfig.DbUserPassword;
            AppConfig.DbHost = !string.IsNullOrEmpty(newLoadedConfig.DbHost) ? newLoadedConfig.DbHost : AppConfig.DbHost;
            AppConfig.DbPort = !string.IsNullOrEmpty(newLoadedConfig.DbPort) ? newLoadedConfig.DbPort : AppConfig.DbPort;
            AppConfig.DbSID = !string.IsNullOrEmpty(newLoadedConfig.DbSID) ? newLoadedConfig.DbSID : AppConfig.DbSID;

            //AppConfig.TraderUserName = !string.IsNullOrEmpty(newLoadedConfig.TraderUserName) ? newLoadedConfig.TraderUserName : AppConfig.TraderUserName;
            //AppConfig.TraderUserPassword = !string.IsNullOrEmpty(newLoadedConfig.TraderUserPassword) ? newLoadedConfig.TraderUserPassword : AppConfig.TraderUserPassword;

            AppConfig.EdiTimeout = !string.IsNullOrEmpty(newLoadedConfig.EdiTimeout.ToString()) ? newLoadedConfig.EdiTimeout : AppConfig.EdiTimeout;
            AppConfig.EdiUser = !string.IsNullOrEmpty(newLoadedConfig.EdiUser) ? newLoadedConfig.EdiUser : AppConfig.EdiUser;
            AppConfig.EdiPassword = !string.IsNullOrEmpty(newLoadedConfig.EdiPassword) ? newLoadedConfig.EdiPassword : AppConfig.EdiPassword;
            AppConfig.EdiGLN = !string.IsNullOrEmpty(newLoadedConfig.EdiGLN) ? newLoadedConfig.EdiGLN : AppConfig.EdiGLN;
            AppConfig.EdiUrl = !string.IsNullOrEmpty(newLoadedConfig.EdiUrl) ? newLoadedConfig.EdiUrl : AppConfig.EdiUrl;

            //AppConfig.EnableAutoHandler = newLoadedConfig.EnableAutoHandler ?? AppConfig.EnableAutoHandler;
            //AppConfig.AutoHandlerPeriod = !string.IsNullOrEmpty(newLoadedConfig.AutoHandlerPeriod.ToString()) ? newLoadedConfig.AutoHandlerPeriod : AppConfig.AutoHandlerPeriod;
            AppConfig.EnableLogging = newLoadedConfig.EnableLogging ?? AppConfig.EnableLogging;

            AppConfig.Schema = !string.IsNullOrEmpty(newLoadedConfig.Schema) ? newLoadedConfig.Schema : AppConfig.Schema;

            AppConfig.EnableProxy = newLoadedConfig.EnableProxy ?? AppConfig.EnableProxy;
            AppConfig.ProxyUserName = !string.IsNullOrEmpty(newLoadedConfig.ProxyUserName) ? newLoadedConfig.ProxyUserName : AppConfig.ProxyUserName;
            AppConfig.ProxyUserPassword = !string.IsNullOrEmpty(newLoadedConfig.ProxyUserPassword) ? newLoadedConfig.ProxyUserPassword : AppConfig.ProxyUserPassword;


            Utilites.Logger.Log("\t\tDbUserName " + AppConfig.DbUserName);
            Utilites.Logger.Log("\t\tDbHost " + AppConfig.DbHost);
            Utilites.Logger.Log("\t\tEdiUser " + AppConfig.EdiUser);
            Utilites.Logger.Log("\t\tEdiGLN " + AppConfig.EdiGLN);
        }


        public static void Create()
        {
            XmlSerializer xml = new XmlSerializer(typeof(Model.Common.AppConfig));
            using (var stream = XmlWriter.Create(fullPath))
            {
                xml.Serialize(stream, new Model.Common.AppConfig());
                stream.Close();
            }
        }


    }
}
