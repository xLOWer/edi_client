using System;
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
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Logger.Log("\t\tcount=" + commands.Count.ToString());
            foreach (var command in commands)
            {
                command.Connection = DbService.Connection.conn;
                DbService.Connection.Check();
                command.ExecuteNonQuery();
            }
        }


        internal static void ExecuteCommand(OracleCommand command)
        {
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            using (command)
            {
                DbService.Connection.Check();
                command.Connection = DbService.Connection.conn;
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
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            DataTable DataGridItems = ObjToDataTable(typeof(string));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = DbService.Connection.conn;
                DbService.Connection.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, DbService.Connection.conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
                
            }            
            return DataGridItems;
        }

        internal static void Insert(string Sql)
        {
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            using (OracleCommand command = new OracleCommand(Sql))
            {
                command.Connection = DbService.Connection.conn;
                DbService.Connection.Check();
                command.ExecuteNonQuery();
                
            }
        }

        internal static void Insert(List<string> Sqls)
        {
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            int c = Sqls.Count();
            for (int i = 1; i <= c; ++i)
            {
                using (OracleCommand command = new OracleCommand(Sqls[i - 1]))
                {
                    command.Connection = DbService.Connection.conn;
                    DbService.Connection.Check();
                    command.ExecuteNonQuery();
                    
                }
            }

        }

        internal static string SelectSingleValue(string Sql)
        {
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            OracleDataReader reader;
            string retVal = "";
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = DbService.Connection.conn;
                command.CommandType = CommandType.Text;
                command.CommandText = Sql;
                DbService.Connection.conn.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    retVal = reader[0].ToString();
                }
                DbService.Connection.conn.Close();
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
                => $" BETWEEN {OracleDateFormat(Date)} AND {OracleDateFormat(Date)} ";

            internal static string DatesBetween(DateTime DateFrom, DateTime DateTo)
                => $" BETWEEN {OracleDateFormat(DateFrom)} AND {OracleDateFormat(DateTo)} ";

            internal static string OracleDateFormat(DateTime Date) => $" TO_DATE('{Date.Day}/{Date.Month}/{Date.Year}','DD/MM/YYYY') ";

            internal static string Sql_DateRange(string tableName, string fieldName, string sign, DateTime date1)
                => $"{tableName}.{fieldName} = {OracleDateFormat(date1)}\n";

            internal static string Sql_DateRange(string shortTableName, string fieldName, string sign, DateTime date1, DateTime date2)
                => $"{shortTableName}.{fieldName} {DatesBetween(date1, date2)}\n";

            internal static string ToOracleDate(DateTime Date)
            {
                var day = Date.Day < 10 ? $"0{Date.Day}" : Date.Day.ToString();
                var mouth = Date.Month < 10 ? $"0{Date.Month}" : Date.Month.ToString();
                return $"{day}.{mouth}. {Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}";
            }

            internal static string GET_FAILED_DETAILS(string SENDER_ILN) =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_FAILED_DETAILS WHERE SENDER_ILN={SENDER_ILN}";

            internal static string GET_GOODS =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_GOODS";

            internal static string GET_MATCHED(string CUSTOMER_GLN) =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_MATCHED WHERE CUSTOMER_GLN={CUSTOMER_GLN}";

            internal static string GET_MATCHED_PRICE_TYPES(string CUSTOMER_GLN) =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_MATCHED_PRICE_TYPES WHERE CUSTOMER_GLN={CUSTOMER_GLN}";

            internal static string GET_ORDERS(string SENDER_ILN, DateTime DateFrom, DateTime DateTo) =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_ORDERS WHERE SENDER_ILN like {SENDER_ILN} AND ORDER_DATE BETWEEN {OracleDateFormat(DateFrom)} AND {OracleDateFormat(DateTo)}";

            internal static string GET_ORDER_DETAILS(string ID_EDI_DOC) =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_ORDER_DETAILS WHERE ID_EDI_DOC={ID_EDI_DOC}";

            internal static string GET_PRICE_TYPES =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_PRICE_TYPES";

            internal static string GET_CLIENTS =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_DELIVERY_POINTS";

            internal static string GET_CUSTOMERS =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_CUSTOMERS";

            internal static string GET_CONTRACTORS =>
                $"SELECT * FROM {(AppConfig.Schema + ".")}EDI_GET_CONTRACTORS";

        }


        internal static class Connection
        {
            internal static OracleConnection conn { get; set; }
            internal static string Timeout => conn?.ConnectionTimeout.ToString() ?? "ошибка";

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
                    //DbService.SelectSingleValue("alter session set nls_numeric_characters = ',.'");
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
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Logger.Log("\t\t" + Sql);
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable<TModel>(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = DbService.Connection.conn;
                DbService.Connection.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, DbService.Connection.conn);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
            }

            Documents = ToListof<TModel>(DataGridItems).ToList();
            return Documents;
        }
        /*
        internal static TModel ObjectToClass(object[] obj)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            TModel inst = Activator.CreateInstance<TModel>();
            var objectProperties = typeof(TModel).GetProperties(flags);
            int c = obj.Count();
            for (int i = 0; i < c; ++i)            
                objectProperties[i].SetValue(inst, obj[i].ToString(), null);  
            return inst;
        }*/

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
            Logger.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Logger.Log("\t\tSqls.Count=" + Sqls.Count.ToString());
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable<TModel>(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                foreach (var Sql in Sqls)
                {
                    command.Connection = DbService.Connection.conn;
                    DbService.Connection.Check();
                    OracleDataAdapter adapter = new OracleDataAdapter(Sql, DbService.Connection.conn);
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
