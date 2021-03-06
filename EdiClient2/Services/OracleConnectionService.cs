﻿//using Oracle.DataAccess.Client;
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
            //MessageBox.Show($"{AppSettings.AppConfig.connString}\n\n{AppSettings.AppConfig.OracleDbConnection_UserName}\n\n{AppSettings.AppConfig.Edi_Email}");
            if (!string.IsNullOrEmpty(AppConfig.connString)/* || 
                !String.IsNullOrEmpty(AppConfig.OracleDbConnection_UserName) ||
                !String.IsNullOrEmpty(AppConfig.OracleDbConnection_UserPassword) ||
                !String.IsNullOrEmpty(AppConfig.OracleDbConnection_SID) ||
                !String.IsNullOrEmpty(AppConfig.OracleDbConnection_Port) ||
                !String.IsNullOrEmpty(AppConfig.OracleDbConnection_Host)*/)
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
