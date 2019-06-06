//using Oracle.DataAccess.Client;
using Devart.Data.Oracle;
using EdiClient.AppSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdiClient.Services
{
    public static class OracleConnectionService
    {
        internal static OracleConnection conn { get; set; }

        internal static void Configure()
        {
            //MessageBox.Show($"{AppSettings.AppConfig.connString}\n\n{AppSettings.AppConfig.DbUserName}\n\n{AppSettings.AppConfig.EdiEmail}");
            if (!string.IsNullOrEmpty(AppConfig.connString)/* || 
                !String.IsNullOrEmpty(AppConfig.DbUserName) ||
                !String.IsNullOrEmpty(AppConfig.DbUserPassword) ||
                !String.IsNullOrEmpty(AppConfig.DbSID) ||
                !String.IsNullOrEmpty(AppConfig.DbPort) ||
                !String.IsNullOrEmpty(AppConfig.DbHost)*/)
                conn = new OracleConnection(AppConfig.connString);
            else
                MessageBox.Show("Соединение с базой не создано. Не верные параметры в строке соединения");

            
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
        }

        internal static void OpenDatabaseConnect()
        {
            conn.Open();            
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
        }

        internal static void CloseDatabaseConnect()
        {
            conn.Close();
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
        }
    }
}
