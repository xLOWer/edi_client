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
            try { EdiService.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }            
        }

        public static void ConfigureOracle()
        {
            try { OracleConnectionService.Configure(); }
            catch (Exception ex) { Utilites.Error(ex); }
        }



        public static void Load()
        {
            if (!Directory.Exists($"{directoryPath}\\{dirName}"))
                Directory.CreateDirectory($"{directoryPath}\\{dirName}");

            if (!File.Exists(fullPath))
                Create();

            Read();
        }


        public static void Save()
        {
            Create();
        }


        public static void Read()
        {
            var newLoadedConfig = new Model.Common.AppConfig();
            XmlSerializer xml = new XmlSerializer(typeof(Model.Common.AppConfig));
            using (var stream = XmlReader.Create(fullPath))
            {
                newLoadedConfig = (Model.Common.AppConfig)xml.Deserialize(stream);
                stream.Close();
            }

            AppConfig.OracleDbConnection_UserName = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_UserName)
                        ? newLoadedConfig.OracleDbConnection_UserName
                        : AppConfig.OracleDbConnection_UserName;
            AppConfig.OracleDbConnection_UserPassword = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_UserPassword)
                        ? newLoadedConfig.OracleDbConnection_UserPassword
                        : AppConfig.OracleDbConnection_UserPassword;
            AppConfig.OracleDbConnection_Port = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_Port)
                        ? newLoadedConfig.OracleDbConnection_Port
                        : AppConfig.OracleDbConnection_Port;
            AppConfig.OracleDbConnection_SID = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_SID)
                        ? newLoadedConfig.OracleDbConnection_SID
                        : AppConfig.OracleDbConnection_SID;
            AppConfig.OracleDbConnection_Host = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_Host)
                        ? newLoadedConfig.OracleDbConnection_Host
                        : AppConfig.OracleDbConnection_Host;
            AppConfig.Edi_User = !string.IsNullOrEmpty(newLoadedConfig.Edi_User)
                        ? newLoadedConfig.Edi_User
                        : AppConfig.Edi_User;
            AppConfig.Edi_Password = !string.IsNullOrEmpty(newLoadedConfig.Edi_Password)
                        ? newLoadedConfig.Edi_Password
                        : AppConfig.Edi_Password;
            AppConfig.Edi_GLN = !string.IsNullOrEmpty(newLoadedConfig.Edi_GLN)
                        ? newLoadedConfig.Edi_GLN
                        : AppConfig.Edi_GLN;
            AppConfig.Edi_Email = !string.IsNullOrEmpty(newLoadedConfig.Edi_Email)
                        ? newLoadedConfig.Edi_Email
                        : AppConfig.Edi_Email;
            AppConfig.Edi_Url = !string.IsNullOrEmpty(newLoadedConfig.Edi_Url)
                        ? newLoadedConfig.Edi_Url
                        : AppConfig.Edi_Url;
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
