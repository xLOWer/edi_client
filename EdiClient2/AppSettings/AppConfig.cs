namespace EdiClient.AppSettings
{
    public static class AppConfig
    {
        public static string DbUserName { get; set; } // имя пользователя
        public static string DbUserPassword { get; set; } // пароль пользователя
        public static string DbPort { get; set; } = "1521"; // порт подключения  
        public static string DbSID { get; set; } // имя сервиса 
        public static string DbHost { get; set; } // ip/хост базы 

        public static string EdiUser { get; set; } // аккаунт EDI. без ЕС - обычный
        public static string EdiPassword { get; set; } // пароль edi
        public static string EdiGLN { get; set; } = "4607971729990"; // GLN
        public static string EdiEmail { get; set; } // почта аккаунта EDI
        public static string EdiUrl { get; set; } = "https://soap.ediweb.ru:443/wsedi/services/platform"; // путь до платформы

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
        public static string Schema { get; } = "HPCSERVICE.";
        public static string connString => $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {DbHost})(PORT = {DbPort}))(CONNECT_DATA = "
                               + $"(SERVER = DEDICATED)(SERVICE_NAME = {DbSID})));Password={DbUserPassword};User ID={DbUserName}";

        //public static string connString => $"User ID={DbUserName};Password={DbUserPassword};"
        //    + $"Host={DbHost};Service Name={DbSID};Port={DbPort};";
    }
}
