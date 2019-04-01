namespace EdiClient.Model.Common
{
    public class AppConfig
    {
        public AppConfig()
        {
            OracleDbConnection_UserName = AppSettings.AppConfig.OracleDbConnection_UserName;
            OracleDbConnection_UserPassword = AppSettings.AppConfig.OracleDbConnection_UserPassword;
            OracleDbConnection_Port = AppSettings.AppConfig.OracleDbConnection_Port;
            OracleDbConnection_SID = AppSettings.AppConfig.OracleDbConnection_SID;
            OracleDbConnection_Host = AppSettings.AppConfig.OracleDbConnection_Host;
            Edi_User = AppSettings.AppConfig.Edi_User;
            Edi_Password = AppSettings.AppConfig.Edi_Password;
            Edi_GLN = AppSettings.AppConfig.Edi_GLN;
            Edi_Email = AppSettings.AppConfig.Edi_Email;
            Edi_Url = AppSettings.AppConfig.Edi_Url;
            DebugLevel = AppSettings.AppConfig.DebugLevel;
        }

        public string OracleDbConnection_UserName { get; set; }
        public string OracleDbConnection_UserPassword { get; set; }
        public string OracleDbConnection_Port { get; set; } 
        public string OracleDbConnection_SID { get; set; }
        public string OracleDbConnection_Host { get; set; } 

        public string Edi_User { get; set; } 
        public string Edi_Password { get; set; } 
        public string Edi_GLN { get; set; } 
        public string Edi_Email { get; set; } 
        public string Edi_Url { get; set; }

        public ushort DebugLevel { get; set; }
    }
}
