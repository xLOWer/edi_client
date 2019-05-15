namespace EdiClient.AppSettings
{
    public static class AppConfig
    {
        public static string OracleDbConnection_UserName { get; set; } // имя пользователя
        public static string OracleDbConnection_UserPassword { get; set; } // пароль пользователя
        public static string OracleDbConnection_Port { get; set; } = "1521"; // порт подключения  
        public static string OracleDbConnection_SID { get; set; } // имя сервиса 
        public static string OracleDbConnection_Host { get; set; } // ip/хост базы 

        public static string Edi_User { get; set; } // аккаунт EDI. без ЕС - обычный
        public static string Edi_Password { get; set; } // пароль edi
        public static string Edi_GLN { get; set; } = "4607971729990"; // GLN
        public static string Edi_Email { get; set; } // почта аккаунта EDI
        public static string Edi_Url { get; set; } = "https://soap.ediweb.ru:443/wsedi/services/platform"; // путь до платформы

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
    }
}
