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
        
        public static void ConfigureEdi()
        {
            LogService.Log("[EdiService]");
            try { EdiService.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }
            LogService.Log("BindingName " + EdiService.Client.Endpoint.Name);
            LogService.Log("Uri " + EdiService.Client.Endpoint.ListenUri.AbsoluteUri);
        }

        public static void ConfigureOracle()
        {
            LogService.Log("[OracleConnectionService]");
            try { OracleConnectionService.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }
            LogService.Log("ClientVersion " + OracleConnectionService.conn.ClientVersion);
            LogService.Log("Server " + OracleConnectionService.conn.Server);
            LogService.Log("ServerVersion " + OracleConnectionService.conn.ServerVersion);
            LogService.Log("UserId " + OracleConnectionService.conn.UserId);
        }

        public static void Load()
        {
            if (!Directory.Exists($"{directoryPath}\\{dirName}"))
                Directory.CreateDirectory($"{directoryPath}\\{dirName}");

            if (!File.Exists(fullPath))
                Create();

            Read();

            LogService.Log($"conf_dir {directoryPath}\\{dirName}");
        }


        public static void Save()
        {
            Create();
        }


        public static void Read()
        {
            LogService.Log("[APPCONFIG]");
            var newLoadedConfig = new Model.Common.AppConfig();
            XmlSerializer xml = new XmlSerializer(typeof(Model.Common.AppConfig));
            using (var stream = XmlReader.Create(fullPath))
            {
                newLoadedConfig = (Model.Common.AppConfig)xml.Deserialize(stream);
                stream.Close();
            }

            AppConfig.DbUserName = !string.IsNullOrEmpty(newLoadedConfig.DbUserName) ? newLoadedConfig.DbUserName : AppConfig.DbUserName;
            AppConfig.DbUserPassword = !string.IsNullOrEmpty(newLoadedConfig.DbUserPassword) ? newLoadedConfig.DbUserPassword : AppConfig.DbUserPassword;
            AppConfig.DbPort = !string.IsNullOrEmpty(newLoadedConfig.DbPort) ? newLoadedConfig.DbPort : AppConfig.DbPort;
            AppConfig.DbSID = !string.IsNullOrEmpty(newLoadedConfig.DbSID) ? newLoadedConfig.DbSID : AppConfig.DbSID;
            AppConfig.DbHost = !string.IsNullOrEmpty(newLoadedConfig.DbHost) ? newLoadedConfig.DbHost : AppConfig.DbHost;
            AppConfig.EdiUser = !string.IsNullOrEmpty(newLoadedConfig.EdiUser) ? newLoadedConfig.EdiUser : AppConfig.EdiUser;
            AppConfig.EdiPassword = !string.IsNullOrEmpty(newLoadedConfig.EdiPassword) ? newLoadedConfig.EdiPassword : AppConfig.EdiPassword;
            AppConfig.EdiGLN = !string.IsNullOrEmpty(newLoadedConfig.EdiGLN) ? newLoadedConfig.EdiGLN : AppConfig.EdiGLN;
            AppConfig.EdiEmail = !string.IsNullOrEmpty(newLoadedConfig.EdiEmail) ? newLoadedConfig.EdiEmail : AppConfig.EdiEmail;
            AppConfig.EdiUrl = !string.IsNullOrEmpty(newLoadedConfig.EdiUrl) ? newLoadedConfig.EdiUrl : AppConfig.EdiUrl;

            LogService.Log("DbUserName " + AppConfig.DbUserName);
            LogService.Log("DbPort " + AppConfig.DbPort);
            LogService.Log("DbSID " + AppConfig.DbSID);
            LogService.Log("DbHost " + AppConfig.DbHost);
            LogService.Log("EdiUser " + AppConfig.EdiUser);
            LogService.Log("EdiGLN " + AppConfig.EdiGLN);
            LogService.Log("EdiEmail " + AppConfig.EdiEmail);
            LogService.Log("EdiUrl " + AppConfig.EdiUrl);
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
