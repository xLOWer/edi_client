using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Devart.Data.Oracle;
using System.Collections.Generic;

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
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            LogService.Log("\t\tcount=" + commands.Count.ToString());
            foreach (var command in commands)
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.Check();
                command.ExecuteNonQuery();
            }
        }


        internal static void ExecuteCommand(OracleCommand command)
        {
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            using (command)
            {
                OracleConnectionService.Check();
                command.Connection = OracleConnectionService.conn;
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
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            DataTable DataGridItems = ObjToDataTable(typeof(string));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
                
            }            
            return DataGridItems;
        }

        internal static void Insert(string Sql)
        {
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            using (OracleCommand command = new OracleCommand(Sql))
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.Check();
                command.ExecuteNonQuery();
                
            }
        }

        internal static void Insert(List<string> Sqls)
        {
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            int c = Sqls.Count();
            for (int i = 1; i <= c; ++i)
            {
                using (OracleCommand command = new OracleCommand(Sqls[i - 1]))
                {
                    command.Connection = OracleConnectionService.conn;
                    OracleConnectionService.Check();
                    command.ExecuteNonQuery();
                    
                }
            }

        }

        internal static string SelectSingleValue(string Sql)
        {
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            LogService.Log("\t\t"+Sql);
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = OracleConnectionService.conn;
                OracleConnectionService.Check();
                OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                DataGridItems.Clear();
                adapter.Fill(DataGridItems);
            }

            Documents = ToListof(DataGridItems).ToList();
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

        internal static List<TModel> DocumentSelect(List<string> Sqls)
        {
            LogService.Log($"[ORCL] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            LogService.Log("\t\tSqls.Count=" + Sqls.Count.ToString());
            List<TModel> Documents = new List<TModel>();
            DataTable DataGridItems = ObjToDataTable(typeof(TModel));
            using (OracleCommand command = new OracleCommand())
            {
                foreach (var Sql in Sqls)
                {
                    command.Connection = OracleConnectionService.conn;
                    OracleConnectionService.Check();
                    OracleDataAdapter adapter = new OracleDataAdapter(Sql, OracleConnectionService.conn);
                    OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                    DataGridItems.Clear();
                    adapter.Fill(DataGridItems);
                }
            }

            Documents = ToListof(DataGridItems).ToList();
            return Documents;
        }


    }
}
