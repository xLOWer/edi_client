namespace EdiClient.Model.Common
{
    public class AppConfig
    {
        public AppConfig()
        {
            DbUserName = AppSettings.AppConfig.DbUserName;
            DbUserPassword = AppSettings.AppConfig.DbUserPassword;
            Schema = AppSettings.AppConfig.Schema;
            DbHost = AppSettings.AppConfig.DbHost;
            DbPort = AppSettings.AppConfig.DbPort;
            DbSID = AppSettings.AppConfig.DbSID;

            TraderUserName = AppSettings.AppConfig.TraderUserName;
            TraderUserPassword = AppSettings.AppConfig.TraderUserPassword;

            EdiTimeout = AppSettings.AppConfig.EdiTimeout;
            EdiUser = AppSettings.AppConfig.EdiUser;
            EdiPassword = AppSettings.AppConfig.EdiPassword;
            EdiGLN = AppSettings.AppConfig.EdiGLN;
            EdiEmail = AppSettings.AppConfig.EdiEmail;
            EdiUrl = AppSettings.AppConfig.EdiUrl;

            EnableAutoHandler = AppSettings.AppConfig.EnableAutoHandler;
            AutoHandlerPeriod = AppSettings.AppConfig.AutoHandlerPeriod;

            EnableLogging = AppSettings.AppConfig.EnableLogging;
        }

        public string DbUserName { get; set; }
        public string DbUserPassword { get; set; }
        public string DbPort { get; set; }
        public string DbSID { get; set; } 
        public string DbHost { get; set; }

        public string TraderUserName { get; set; }
        public string TraderUserPassword { get; set; }

        public int? EdiTimeout { get; set; } 
        public string EdiUser { get; set; }
        public string EdiPassword { get; set; } 
        public string EdiGLN { get; set; } 
        public string EdiEmail { get; set; }
        public string EdiUrl { get; set; } 

        public bool? EnableAutoHandler { get; set; }
        public int? AutoHandlerPeriod { get; set; } 

        public bool? EnableLogging { get; set; } 

        public string Schema { get; set; } = "HPCSERVICE";
    }
}
