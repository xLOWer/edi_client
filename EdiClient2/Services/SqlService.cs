using EdiClient.AppSettings;
using System;

namespace EdiClient.Services
{
    internal static class SqlService
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
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_FAILED_DETAILS WHERE SENDER_ILN={SENDER_ILN}";

        internal static string GET_GOODS =>
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_GOODS";

        internal static string GET_MATCHED(string CUSTOMER_GLN) =>
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_MATCHED WHERE CUSTOMER_GLN={CUSTOMER_GLN}";

        internal static string GET_MATCHED_PRICE_TYPES(string CUSTOMER_GLN) =>
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_MATCHED_PRICE_TYPES WHERE CUSTOMER_GLN={CUSTOMER_GLN}";

        internal static string GET_ORDERS(string SENDER_ILN, DateTime DateFrom, DateTime DateTo) =>
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_ORDERS WHERE SENDER_ILN like {SENDER_ILN} AND ORDER_DATE BETWEEN {OracleDateFormat(DateFrom)} AND {OracleDateFormat(DateTo)}";

        internal static string GET_ORDER_DETAILS(string ID_EDI_DOC) =>    
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_ORDER_DETAILS WHERE ID_EDI_DOC={ID_EDI_DOC}";

        internal static string GET_PRICE_TYPES =>
            $"SELECT * FROM {AppConfig.Schema}EDI_GET_PRICE_TYPES";


    }

}