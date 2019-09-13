﻿using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Devart.Data.Oracle;
using System.Collections.Generic;
using EdiClient.AppSettings;
using DevExpress.Xpf.Core;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.Services
{
    internal static partial class DbService
    {
        /// <summary>
        /// Запускает массив команд с возможностью задавать параметры
        /// </summary>
        /// <param name="commands">Команды</param>
        internal static void ExecuteCommand(List<OracleCommand> commands)
        {
            int i = 0;
            Connection.Check();
            foreach (var command in commands)
            {
                // каждую сотню запросов проверим, а не отвалилось ли соединение
                if ((i % 100) == 0) Connection.Check();
                i++;
                command.Connection = Connection.conn;
                command.ExecuteNonQuery();
            }
        }


        internal static void ExecuteCommand(OracleCommand command)
        {
            using (command)
            {
                Connection.Check();
                command.Connection = Connection.conn;
                var res = command.ExecuteNonQuery();                
            }
        }
        /// <summary>
        /// Запускает одиночный запрос
        /// </summary>
        /// <param name="Sql">Запрос</param>
        /// <returns>DataTable с резльтатом запроса</returns>
        internal static DataTable Select(string Sql)
        {
            DataTable DataGridItems = ObjToDataTable(typeof(string));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = Connection.conn;
                Connection.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, Connection.conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
            }
            return DataGridItems;
        }

        internal static void ExecuteLine(string Sql)
        {
            using (OracleCommand command = new OracleCommand(Sql))
            {
                command.Connection = Connection.conn;
                Connection.Check();
                command.ExecuteNonQuery();
            }
        }

        internal static void ExecuteLines(List<string> Sqls)
        {
            int c = Sqls.Count();
            for (int i = 1; i <= c; ++i)
            {
                using (OracleCommand command = new OracleCommand(Sqls[i - 1]))
                {
                    command.Connection = Connection.conn;
                    Connection.Check();
                    command.ExecuteNonQuery();                    
                }
            }
        }

        internal static string SelectSingleValue(string Sql)
        {
            OracleDataReader reader;
            string retVal = "";
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = Connection.conn;
                command.CommandType = CommandType.Text;
                command.CommandText = Sql;
                Connection.conn.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    retVal = reader[0].ToString();
                }
                Connection.conn.Close();
            }
            return retVal;
        }

        internal static DataTable ObjToDataTable(Type type)
        {
            var dt = new DataTable();
            foreach (var info in type.GetProperties())
                dt.Columns.Add(info.Name);
            dt.AcceptChanges();
            return dt;
        }

        internal static class Sqls
        {
            internal static string DatesBetween(DateTime Date)
                => " BETWEEN " + OracleDateFormat(Date) + " AND " + OracleDateFormat(Date) + " ";

            internal static string DatesBetween(DateTime DateFrom, DateTime DateTo)
                => " BETWEEN " + OracleDateFormat(DateFrom) + " AND " + OracleDateFormat(DateTo) + " ";

            internal static string OracleDateFormat(DateTime Date) => " TO_DATE('" + Date.Day + "/" + Date.Month + "/" + Date.Year + "','DD/MM/YYYY') ";

            internal static string Sql_DateRange(string tableName, string fieldName, string sign, DateTime date1)
                => "" + tableName + "." + fieldName + " = " + OracleDateFormat(date1) + "\n";

            internal static string Sql_DateRange(string shortTableName, string fieldName, string sign, DateTime date1, DateTime date2)
                => "" + shortTableName + "." + fieldName + " " + DatesBetween(date1, date2) + "\n";

            internal static string ToOracleDate(DateTime Date)
            {
                var day = Date.Day < 10 ? "0"+Date.Day : Date.Day.ToString();
                var mouth = Date.Month < 10 ? "0" + Date.Month : Date.Month.ToString();
                return day+ "."+mouth+ ". " + Date.Year+ " " + Date.Hour+ ":" + Date.Minute+ ":" + Date.Second;
            }

            internal static string GET_FAILED_DETAILS(string SENDER_ILN) =>
                "SELECT * FROM EDI.EDI_GET_FAILED_DETAILS WHERE SENDER_ILN=" + SENDER_ILN;

            internal static string GET_GOODS =>
                "SELECT * FROM EDI.EDI_GET_GOODS";

            internal static string GET_MATCHED(string CUSTOMER_GLN) =>
                "SELECT * FROM EDI.EDI_GET_MATCHED WHERE CUSTOMER_GLN=" + CUSTOMER_GLN;

            internal static string GET_MATCHED_PRICE_TYPES(string CUSTOMER_GLN) =>
                "SELECT * FROM EDI.EDI_GET_MATCHED_PRICE_TYPES WHERE CUSTOMER_GLN=" + CUSTOMER_GLN;

            internal static string GET_ORDERS(string SENDER_ILN, DateTime DateFrom, DateTime DateTo) =>
                "SELECT * FROM EDI.EDI_GET_ORDERS WHERE SENDER_ILN like "+SENDER_ILN+" AND ORDER_DATE BETWEEN "+OracleDateFormat(DateFrom) + " AND " + OracleDateFormat(DateTo);

            internal static string GET_ORDER_DETAILS(string ID_EDI_DOC) =>
                "SELECT * FROM EDI.EDI_GET_ORDER_DETAILS WHERE ID_EDI_DOC=" + ID_EDI_DOC;

            internal static string GET_PRICE_TYPES =>
                "SELECT * FROM EDI.EDI_GET_PRICE_TYPES";

            internal static string GET_CLIENTS =>
                "SELECT * FROM EDI.EDI_GET_DELIVERY_POINTS";

            internal static string GET_CUSTOMERS =>
                "SELECT * FROM EDI.EDI_GET_CUSTOMERS";

            internal static string GET_CONTRACTORS =>
                "SELECT * FROM EDI.EDI_GET_CONTRACTORS";

        }


        internal static class Connection
        {
            internal static OracleConnection conn { get; set; }
            internal static string Timeout => conn?.ConnectionTimeout.ToString() ?? "ошибка";

            internal static void Configure()
            {
                if (!string.IsNullOrEmpty(AppConfigHandler.conf.connString) ||
                    !String.IsNullOrEmpty(AppConfigHandler.conf.DbUserName) ||
                    !String.IsNullOrEmpty(AppConfigHandler.conf.DbUserPassword) ||
                    !String.IsNullOrEmpty(AppConfigHandler.conf.DbSID) ||
                    !String.IsNullOrEmpty(AppConfigHandler.conf.DbPort) ||
                    !String.IsNullOrEmpty(AppConfigHandler.conf.DbHost))
                {
                    conn = new OracleConnection(AppConfigHandler.conf.connString);
                    //SelectSingleValue("alter session set nls_numeric_characters = ',.'");
                }
                else
                    DXMessageBox.Show("Соединение с базой не создано. Не верные параметры в строке соединения");
            }

            internal static void Open() => conn.Open();
            internal static void Close() => conn.Close();

            internal static void Check()
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                }
                catch (Exception ex) { Error(ex); }
            }

        }


        internal static List<TModel> DocumentSelect<TModel>(string Sql)
        {
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable<TModel>(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = Connection.conn;
                Connection.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, Connection.conn);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
            }

            Documents = ToListof<TModel>(DataGridItems).ToList();
            return Documents;
        }

        internal static List<TModel> ToListof<TModel>(DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(TModel).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<TModel>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }


        internal static DataTable ObjToDataTable<TModel>(Type type)
        {
            var dt = new DataTable();
            foreach (var info in type.GetProperties())
                dt.Columns.Add(info.Name);
            dt.AcceptChanges();
            return dt;
        }

        internal static List<TModel> DocumentSelect<TModel>(List<string> Sqls)
        {
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable<TModel>(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                foreach (var Sql in Sqls)
                {
                    command.Connection = Connection.conn;
                    Connection.Check();
                    OracleDataAdapter adapter = new OracleDataAdapter(Sql, Connection.conn);
                    OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                    DataGridItems.Clear();
                    adapter.Fill(DataGridItems);
                }
            }

            Documents = ToListof<TModel>(DataGridItems).ToList();
            return Documents;
        }
    }


}
