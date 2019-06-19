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
        public static string Timeout => conn?.ConnectionTimeout.ToString() ?? "ошибка";

        internal static void Configure()
        {
            if (!string.IsNullOrEmpty(AppConfig.connString) || 
                !String.IsNullOrEmpty(AppConfig.DbUserName) ||
                !String.IsNullOrEmpty(AppConfig.DbUserPassword) ||
                !String.IsNullOrEmpty(AppConfig.DbSID) ||
                !String.IsNullOrEmpty(AppConfig.DbPort) ||
                !String.IsNullOrEmpty(AppConfig.DbHost))
            {
                conn = new OracleConnection(AppConfig.connString);               
            }
            else
                MessageBox.Show("Соединение с базой не создано. Не верные параметры в строке соединения");            
        }

        internal static void Open() => conn.Open();
        internal static void Close() => conn.Close();

        internal static void Check()
        {
            if(conn.State != System.Data.ConnectionState.Open)
                conn.Open();  
        }
        
    }
}
