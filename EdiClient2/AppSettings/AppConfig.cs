using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System;

namespace EdiClient.AppSettings
{
    public static class AppConfig
    {
        public static string OracleDbConnection_UserName { get; set; } = "abt"; // имя пользователя          developer3
        public static string OracleDbConnection_UserPassword { get; set; } = "tujybrnjytpyftn"; // пароль пользователя    shishl0Dm
        public static string OracleDbConnection_Port { get; set; } = "1521"; // порт подключения                    1521
        public static string OracleDbConnection_SID { get; set; } = "xe"; // имя сервиса         orcl.vladivostok.wera
        public static string OracleDbConnection_Host { get; set; } = "192.168.2.115"; // ip/хост базы                192.168.2.13

        public static string Edi_User { get; set; } = "4607971729990"; // тестовый. без ЕС - обычный
        public static string Edi_Password { get; set; } = "996b542d";
        public static string Edi_GLN { get; set; } = "4607971729990";
        public static string Edi_Email { get; set; } = "testuser@ediweb.eu";
        public static string Edi_Url { get; set; } = "https://soap.ediweb.ru:443/wsedi/services/platform";

        /// <summary>
        /// Уровень лога: 
        /// 0 - откл.,
        /// 1 - DEBUG,
        /// 2 - INFO (все действия),
        /// 3 - WARNING(предупрежд-я),
        /// 4 - ERROR(ошибки, по-умолч.),
        /// 5 - CRITICAL(крит. наруш-я работы).
        /// Например 3: будет писать WARNING, ERROR, CRITICAL
        /// </summary>
        public static ushort DebugLevel { get; set; } = 0;

        public static string connString => $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {OracleDbConnection_Host})(PORT = {OracleDbConnection_Port}))(CONNECT_DATA = "
                               + $"(SERVER = DEDICATED)(SERVICE_NAME = {OracleDbConnection_SID})));Password={OracleDbConnection_UserPassword};User ID={OracleDbConnection_UserName}";

        //public static string connString => $"User ID={OracleDbConnection_UserName};Password={OracleDbConnection_UserPassword};"
        //    + $"Host={OracleDbConnection_Host};Service Name={OracleDbConnection_SID};Port={OracleDbConnection_Port};";

        private static string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);      
        private static string dirName = "EdiClient";        
        private static string fileName = "EdiClientAppConfig.xml";        
        private static string fullPath => Path.GetFullPath($"{directoryPath}\\{dirName}\\{fileName}");

        public static void Load()
        {
            if (!Directory.Exists($"{directoryPath}\\{dirName}"))            
                Directory.CreateDirectory($"{directoryPath}\\{dirName}");            

            if(!File.Exists(fullPath))            
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

            OracleDbConnection_UserName = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_UserName)
                        ? newLoadedConfig.OracleDbConnection_UserName
                        : OracleDbConnection_UserName;
            OracleDbConnection_UserPassword = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_UserPassword)
                        ? newLoadedConfig.OracleDbConnection_UserPassword
                        : OracleDbConnection_UserPassword;
            OracleDbConnection_Port = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_Port)
                        ? newLoadedConfig.OracleDbConnection_Port
                        : OracleDbConnection_Port;
            OracleDbConnection_SID = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_SID)
                        ? newLoadedConfig.OracleDbConnection_SID
                        : OracleDbConnection_SID;
            OracleDbConnection_Host = !string.IsNullOrEmpty(newLoadedConfig.OracleDbConnection_Host)
                        ? newLoadedConfig.OracleDbConnection_Host
                        : OracleDbConnection_Host;
            Edi_User = !string.IsNullOrEmpty(newLoadedConfig.Edi_User)
                        ? newLoadedConfig.Edi_User
                        : Edi_User;
            Edi_Password = !string.IsNullOrEmpty(newLoadedConfig.Edi_Password)
                        ? newLoadedConfig.Edi_Password
                        : Edi_Password;
            Edi_GLN = !string.IsNullOrEmpty(newLoadedConfig.Edi_GLN)
                        ? newLoadedConfig.Edi_GLN
                        : Edi_GLN;
            Edi_Email = !string.IsNullOrEmpty(newLoadedConfig.Edi_Email)
                        ? newLoadedConfig.Edi_Email
                        : Edi_Email;
            Edi_Url = !string.IsNullOrEmpty(newLoadedConfig.Edi_Url)
                        ? newLoadedConfig.Edi_Url
                        : Edi_Url;
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
