﻿using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Reflection;
//using Oracle.DataAccess.Client;
using Devart.Data.Oracle;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EdiClient.Services
{
    internal static partial class DbService
    {
        private static List<Task> NativeTaskList = new List<Task>();

        /// <summary>
        /// Запускает массив команд с возможностью задавать параметры
        /// </summary>
        /// <param name="commands">Команды</param>
        internal static void ExecuteCommand(List<OracleCommand> commands)
        {
            int c = commands.Count();
            for (int i = 1; i <= c; ++i)
            {
                ExecuteCommand(commands[i - 1]);
            }

        }


        internal static void ExecuteCommand(OracleCommand command)
        {
            using (command)
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.OpenDatabaseConnect();
                command.ExecuteNonQuery();
                OracleConnectionService.CloseDatabaseConnect();
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
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.OpenDatabaseConnect();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
                OracleConnectionService.CloseDatabaseConnect();
            }

            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            return DataGridItems;
        }

        internal static void Insert(string Sql)
        {

            using (OracleCommand command = new OracleCommand(Sql))
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.OpenDatabaseConnect();
                command.ExecuteNonQuery();
                OracleConnectionService.CloseDatabaseConnect();
            }
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
        }

        internal static void Insert(List<string> Sqls)
        {
            int c = Sqls.Count();
            for (int i = 1; i <= c; ++i)
            {
                using (OracleCommand command = new OracleCommand(Sqls[i - 1]))
                {
                    command.Connection = OracleConnectionService.conn;
                    OracleConnectionService.OpenDatabaseConnect();
                    command.ExecuteNonQuery();
                    OracleConnectionService.CloseDatabaseConnect();
                }
            }
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);

        }

        internal static string SelectSingleValue(string Sql)
        {
            OracleDataReader reader;
            string retVal = "";
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = OracleConnectionService.conn;
                command.CommandType = CommandType.Text;
                command.CommandText = Sql;
                OracleConnectionService.conn.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    retVal = reader[0].ToString();
                }
                OracleConnectionService.conn.Close();
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
    }

    internal static partial class DbService<TModel>
    {
        internal static List<TModel> DocumentSelect(string Sql)
        {
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.OpenDatabaseConnect();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
                OracleConnectionService.CloseDatabaseConnect();

            }

            Documents = ToListof(DataGridItems).ToList();
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            return Documents;
        }

        internal static List<TModel> DocumentSelect(List<string> Sqls)
        {
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                foreach (var Sql in Sqls)
                {
                    command.Connection = OracleConnectionService.conn;
                    OracleConnectionService.OpenDatabaseConnect();
                    OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                    OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                    DataGridItems.Clear();
                    adapter.Fill(DataGridItems);
                    OracleConnectionService.CloseDatabaseConnect();
                }
            }

            Documents = ToListof(DataGridItems).ToList();
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 2);
            return Documents;
        }
        
        internal static List<TModel> ToListof(DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select( c => c.ColumnName )
                .ToList();
            var objectProperties = typeof( TModel ).GetProperties( flags );
            var targetList = dt.AsEnumerable().Select( dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<TModel>();

                foreach (var properties in objectProperties.Where( properties => columnNames.Contains( properties.Name ) && dataRow[properties.Name] != DBNull.Value ))
                {
                    properties.SetValue( instanceOfT, dataRow[properties.Name], null );
                }
                return instanceOfT;
            } ).ToList();

            return targetList;
        }


        internal static DataTable ObjToDataTable(Type type)
        {
            var dt = new DataTable();
            foreach (var info in type.GetProperties())
                dt.Columns.Add( info.Name );
            dt.AcceptChanges();
            return dt;
        }
        

    }
}
