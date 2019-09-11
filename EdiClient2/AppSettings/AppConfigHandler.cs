using EdiClient.Services;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.AppSettings
{
    public static class AppConfigHandler
    {
        private static string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string dirName = "EdiClient";
        private static string fileName = "EdiClientAppConfig.xml";
        private static string fullPath => Path.GetFullPath($"{directoryPath}\\{dirName}\\{fileName}");
        public static string LogPath => Path.GetFullPath($"{directoryPath}\\{dirName}");
        public static AppConfig conf = new AppConfig();

        public static void ConfigureEdi()
        {
            string log = "[APPCONFIG]ConfigureEdi";
            try { EdiService.Configure(); }
            catch (Exception ex) { Error(ex); }
            log += $"|EdiUser=" + conf.EdiUser;
            log += $"|EdiGLN=" + conf.EdiGLN;
            Logger.Log(log);
        }

        public static void ConfigureOracle()
        {
            string log = "[APPCONFIG]ConfigureOracle";
            try { DbService.Connection.Configure(); }
            catch (Exception ex) { Error(ex); }
            log += $"|ClientVersion=" + DbService.Connection.conn.ClientVersion;
            log += $"|DbUserName=" + conf.DbUserName;
            log += $"|DbHost=" + conf.DbHost;
            log += $"|DbSID=" + conf.DbSID;
            log += $"|DbPort=" + conf.DbPort;
            Logger.Log(log);
        }

        public static void Load()
        {
            Logger.Log("[APPCONFIG]LOAD");
            if (!Directory.Exists($"{directoryPath}\\{dirName}"))
                Directory.CreateDirectory($"{directoryPath}\\{dirName}");

            if (!File.Exists(fullPath))
                Create();

            Read();
            
        }


        public static void Save()
        {
            Logger.Log("[APPCONFIG]SAVE");
            Create();
        }


        public static void Read()
        {
            string log = "[APPCONFIG]READ";
            var newLoadedConfig = new AppConfig();
            XmlSerializer xml = new XmlSerializer(typeof(AppConfig));
            using (var stream = XmlReader.Create(fullPath))
            {
                newLoadedConfig = (AppConfig)xml.Deserialize(stream);
                stream.Close();
            }
            conf = newLoadedConfig;

            Logger.Log(log);
        }


        public static void Create()
        {
            XmlSerializer xml = new XmlSerializer(typeof(AppConfig));
            using (var stream = XmlWriter.Create(fullPath))
            {
                if(conf == null)                
                    xml.Serialize(stream, new AppConfig());    
                else
                    xml.Serialize(stream, conf);

                stream.Close();
            }
        }
    }



    public class AppConfig
    {
        public const string AppVersion = "3.3.2.27";
        public static string ThemeName { get; set; } = "VS2017Light"; //VS2017Light  MetropolisDark

        public string DbUserName { get; set; } // имя пользователя
        public string DbUserPassword { get; set; } // пароль пользователя
        public string DbPort { get; set; } // порт подключения  
        public string DbSID { get; set; } // имя сервиса 
        public string DbHost { get; set; } // ip/хост базы 

        //public string TraderUserName { get; set; } // имя пользователя в трейдере для создания документа 
        //public string TraderUserPassword { get; set; } // пароль пользователя в трейдере для создания документа 

        public int EdiTimeout { get; set; } = 5000; // таймаут соединения с сервисами
        public string EdiUser { get; set; } // аккаунт EDI. без ЕС - обычный
        public string EdiPassword { get; set; } // пароль edi
        public string EdiGLN { get; set; } // GLN
        public string EdiUrl { get; set; } // путь до платформы

        //public bool?  EnableAutoHandler { get; set; } = false; // включен ли автообработчик (по-умолч. false)
        //public int?   AutoHandlerPeriod { get; set; } = 10; // время цикла(в минутах) автообработчика (по-умолч. 10)
        public bool EnableLogging { get; set; } = false; // включено ли логирование (выключено по-умолч.)

        public string Schema { get; set; }
        public string connString => $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {DbHost})(PORT = {DbPort}))(CONNECT_DATA = "
                               + $"(SERVER = DEDICATED)(SERVICE_NAME = {DbSID})));Password={DbUserPassword};User ID={DbUserName}";

        public bool EnableProxy { get; set; } = false; // включен ли трафик через прокси
        public string ProxyUserName { get; set; } // имя пользователя для прокси
        public string ProxyUserPassword { get; set; } // пароль пользователя прокси
        
        public string FtpDir { get; set; } // директория, откуда забираются документы для загрузки в базу

        /*виртуальные свойства, не идущие в файл*/

    }
}
