namespace EdiClient.Model.Common
{
    public class AppConfig
    {
        public AppConfig()
        {
            DbUserName = AppSettings.AppConfig.DbUserName;
            DbUserPassword = AppSettings.AppConfig.DbUserPassword;
            DbPort = AppSettings.AppConfig.DbPort;
            DbSID = AppSettings.AppConfig.DbSID;
            DbHost = AppSettings.AppConfig.DbHost;
            EdiUser = AppSettings.AppConfig.EdiUser;
            EdiPassword = AppSettings.AppConfig.EdiPassword;
            EdiGLN = AppSettings.AppConfig.EdiGLN;
            EdiEmail = AppSettings.AppConfig.EdiEmail;
            EdiUrl = AppSettings.AppConfig.EdiUrl;
            DebugLevel = AppSettings.AppConfig.DebugLevel;
        }

        public string DbUserName { get; set; }
        public string DbUserPassword { get; set; }
        public string DbPort { get; set; } 
        public string DbSID { get; set; }
        public string DbHost { get; set; } 

        public string EdiUser { get; set; } 
        public string EdiPassword { get; set; } 
        public string EdiGLN { get; set; } 
        public string EdiEmail { get; set; } 
        public string EdiUrl { get; set; }

        public ushort DebugLevel { get; set; }
    }
}
