namespace EdiClient.AppSettings
{
    public static class AppConfig
    {
        public static string DbUserName { get; set; } // имя пользователя
        public static string DbUserPassword { get; set; } // пароль пользователя
        public static string DbPort { get; set; } = "1521"; // порт подключения  
        public static string DbSID { get; set; } // имя сервиса 
        public static string DbHost { get; set; } // ip/хост базы 

        public static string TraderUserName { get; set; } // имя пользователя в трейдере для создания документа 
        public static string TraderUserPassword { get; set; } // пароль пользователя в трейдере для создания документа 

        public static int? EdiTimeout { get; set; } = 5000; // таймаут соединения с сервисами
        public static string EdiUser { get; set; } // аккаунт EDI. без ЕС - обычный
        public static string EdiPassword { get; set; } // пароль edi
        public static string EdiGLN { get; set; } = "4607971729990"; // GLN
        public static string EdiEmail { get; set; } // почта аккаунта EDI
        public static string EdiUrl { get; set; } = "https://soap.ediweb.ru:443/wsedi/services/platform"; // путь до платформы

        public static bool?   EnableAutoHandler { get; set; } = false; // включен ли автообработчик (по-умолч. false)
        public static int?    AutoHandlerPeriod { get; set; } = 10; // время цикла(в минутах) автообработчика (по-умолч. 10)
        public static bool? EnableLogging { get; set; } = false; // включено ли логирование (выключено по-умолч.)

        public static string Schema { get; set; } = "HPCSERVICE";
        public static string connString => $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {DbHost})(PORT = {DbPort}))(CONNECT_DATA = "
                               + $"(SERVER = DEDICATED)(SERVICE_NAME = {DbSID})));Password={DbUserPassword};User ID={DbUserName}";

        //public static string connString => $"User ID={DbUserName};Password={DbUserPassword};"
        //    + $"Host={DbHost};Service Name={DbSID};Port={DbPort};";
    }
}
