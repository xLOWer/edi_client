﻿using EdiClient.Model;
using Devart.Data.Oracle;
//using System.Data.OracleClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace EdiClient.Services.Repository
{
    /// <summary>
    /// Класс для работы с заказами
    /// </summary>
    public static class OrdersRepository
    {
        public static List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public static int RelationshipCount => EdiService.RelationshipCount;

        public static List<DocumentOrder> Orders { get; set; }
        private static List<Model.WebModel.DocumentInfo> NewOrders { get; set; }
        private static List<Task> NativeTaskList = new List<Task>();

        internal static void UpdateData(DateTime dateFrom, DateTime dateTo)
        {
            if (SelectedRelationship == null) return;
            if (SelectedRelationship.partnerIln == null || SelectedRelationship.documentType == null) return;

            NewOrders = EdiService.ListMBEx(
                                              SelectedRelationship.partnerIln
                                            , SelectedRelationship.documentType
                                            , ""
                                            , ""
                                            , ""
                                            , $"{dateFrom.Year}-{dateFrom.Month}-{dateFrom.Day}"
                                            , $"{dateTo.Year}-{dateTo.Month}-{dateTo.Day}"
                                            , ""
                                            , ""
                                            , "").Where(x => x?.documentStatus != "Ошибка" || !string.IsNullOrEmpty(x.fileName)).ToList();
        }

        /// <summary>
        /// Получить заказы
        /// </summary>
        /// <param name="dateFrom">начальная дата</param>
        /// <param name="dateTo">конечная дата</param>
        /// <returns>Список полученных заказов</returns>
        public static List<DocumentOrder> GetOrders(DateTime dateFrom, DateTime dateTo)
        {
            Orders = new List<DocumentOrder>();

            if (RelationshipCount > 0 && NewOrders.Count() > 0)
                foreach (var rel in Relationships)
                    foreach (var order in NewOrders)
                        NativeTaskList.Add(Task.Factory.StartNew(()
                            => AddOrders(rel.partnerIln, rel.documentType, order.trackingId, rel.documentStandard, order.partneriln)));
            
            Task.WaitAll(NativeTaskList.ToArray());
            
            NativeTaskList.Clear();

            var docNums = GetAllOrderIds();
            if (docNums == null) return Orders;
            var ex = Orders?.Where(x => !docNums.Contains(x?.OrderHeader?.OrderNumber))?.ToList() ?? throw new Exception("Ошибка при загрузке заказов. Повторите попытку");
            SetIncomingOrdersIntoBufferTable(ex);
            return Orders;
        }

        public static void UpdateFailedDetails(string P_EDI_DOC_ID)
        {
            DbService.ExecuteCommand(new OracleCommand()
            {
                Parameters = { new OracleParameter("P_EDI_DOC_ID", OracleDbType.NVarChar, P_EDI_DOC_ID, ParameterDirection.Input) },
                CommandType = CommandType.StoredProcedure,
                CommandText = "HPCSERVICE.EDI_REFRESH_DOC_DETAILS"
            });
        }

        /// <summary>
        /// Создать реальный документ для работы в trader
        /// </summary>
        /// <param name="orderNumber">номер заказа (не его ID в базе!)</param>
        internal static void CreateTraderDocument(string orderNumber)
        {
            var commands = new List<OracleCommand>()
                {
                        new OracleCommand()
                        {
                            Parameters =
                            {
                                new OracleParameter("p_id_edi_doc", OracleDbType.VarChar, orderNumber, ParameterDirection.Input)
                            },
                            CommandType = CommandType.StoredProcedure,
                            CommandText = "HPCSERVICE.EDI_MOVE_ORDER"
                        }
                };
            DbService.ExecuteCommand(commands);
            commands.Clear();

            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);

        }

        internal static List<string> GetAllOrderIds()
        {
            var list = new List<string>() { };
            var dt = DbService.Select(SqlConfiguratorService.Sql_SelectAllOrderIds());
            if (dt != null)
                if (dt.Rows.Count > 0)
                    foreach (DataRow r in dt.Rows)
                        list.Add((string)r.ItemArray[2] ?? "");
            dt.Clear();

            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Получает список деталей документов со статусом FAILED
        /// </summary>
        /// <returns>Список номеров заказов со статусом HAS_FAILED_DETAIL</returns>
        internal static List<string> GetDetailsFailed(string numbers)
        {
            var dt = DbService.Select(SqlConfiguratorService.Sql_SelectDetailsFailed(numbers));
            if (dt == null) return null;
            List<string> list = new List<string>() { };            
            if (dt.Rows.Count > 0)
                foreach (DataRow r in dt.Rows)
                    list.Add((string)r.ItemArray[2] ?? "");
            dt.Clear();
            return list.ToList();
        }

        /// <summary>
        /// Получает список документов со статусом HAS_FAILED_DETAIL чтобы нельзя было отправить его в базу
        /// </summary>
        /// <returns>Список номеров заказов со статусом HAS_FAILED_DETAIL</returns>
        internal static List<string> GetOrdersFailed()
        {
            var list = new List<string>() { };
            var dt = DbService.Select(SqlConfiguratorService.Sql_SelectOrdersFailed());
            if (dt == null) return null;
            if (dt.Rows.Count > 0)
                foreach (DataRow r in dt.Rows)
                    list.Add(r.ItemArray[2].ToString() ?? "");
            dt.Clear();
            return list;
        }

        /// <summary>
        /// Получает список документов уже находящихся в буферной таблице
        /// </summary>
        /// <returns>Список номеров заказов уже находящихся в буферной таблице</returns>
        internal static List<string> GetOrdersLocatedInBufferTable()
        {
            var list = new List<string>() { };
            var dt = DbService.Select(SqlConfiguratorService.Sql_SelectOrdersLocaterInBase());
            if (dt == null) return null;
            if (dt.Rows.Count > 0)
                foreach (DataRow r in dt.Rows)
                    list.Add((string)r.ItemArray[2] ?? "");
            dt.Clear();
            return list;

        }

        /// <summary>
        /// Отправить полученные заказы в буферную таблицу
        /// </summary>
        /// <param name="orders">Список заказов</param>
        private static void SetIncomingOrdersIntoBufferTable(List<DocumentOrder> orders)
        {
            List<OracleCommand> commands = new List<OracleCommand>();
            if (orders.Count > 0)
                foreach (var order in orders.Where(x => x != null))
                {
                    var coms = GenerateOrderCreatingCommands(order);
                    if (coms != null)
                    {
                        if (coms.Count > 0)
                            commands.AddRange(coms);
                        else throw new Exception("Не поступили команды, повторите загрузку");
                    }
                    else throw new Exception("Не поступили команды, повторите загрузку");
                }

            DbService.ExecuteCommand(commands);
            commands.Clear();
        }

        /// <summary>
        /// Создать список команд с запросами для отправки в базу
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Список сформированных команд</returns>
        private static List<OracleCommand> GenerateOrderCreatingCommands(DocumentOrder order)
        {
            if (order == null) return null;
            List<OracleCommand> commands = new List<OracleCommand>();

            commands.Add(new OracleCommand()
            {
                Parameters =
                    {
                        new OracleParameter("P_SENDER_ILN", OracleDbType.NVarChar, order?.DocumentParties?.Sender?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_PROMOTION_REFERENCE", OracleDbType.NVarChar, order?.OrderHeader?.PromotionReference ?? "", ParameterDirection.Input),
                        new OracleParameter("P_EXPECTED_DELIVERY_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.ExpectedDeliveryDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_PAYMENT_DUE_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.OrderPaymentDueDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_DELIVERY_POINT_ILN", OracleDbType.NVarChar, order?.OrderParties.DeliveryPoint?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_BUYER_ILN", OracleDbType.NVarChar, order?.OrderParties?.Buyer?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_CUSTOMER_ILN", OracleDbType.NVarChar, order?.OrderParties?.UltimateCustomer?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_SELLER_ILN", OracleDbType.NVarChar, order?.DocumentParties?.Receiver.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_ORDER_CURRENCY", OracleDbType.NVarChar, order?.OrderHeader?.OrderCurrency ?? "", ParameterDirection.Input),
                        new OracleParameter("P_ORDER_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.OrderDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_ORDER_NUMBER", OracleDbType.NVarChar, order?.OrderHeader?.OrderNumber ?? "", ParameterDirection.Input),
                        new OracleParameter("P_ORDER_PAYMENT_DUE_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.OrderPaymentDueDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_REMARKS", OracleDbType.NVarChar, order?.OrderHeader?.Remarks ?? "", ParameterDirection.Input),
                        new OracleParameter("P_TOTAL_GROSS_AMOUNT", OracleDbType.NVarChar, order?.OrderSummary?.TotalGrossAmount ?? "", ParameterDirection.Input)
                    },
                CommandType = CommandType.StoredProcedure,
                CommandText = "HPCSERVICE.EDI_ADD_ORDER"
            });

            if (order.OrderLines.Lines.Count > 0)
                foreach (var line in order.OrderLines.Lines)
                {


                    commands.Add(new OracleCommand()
                    {
                        Parameters =
                        {
                            new OracleParameter("P_EDI_DOC_NUMBER", OracleDbType.NVarChar, order.OrderHeader.OrderNumber, ParameterDirection.Input),
                            new OracleParameter("P_LINE_NUMBER", OracleDbType.NVarChar, line?.LineItem?.LineNumber ?? "", ParameterDirection.Input),
                            new OracleParameter("P_BUYER_ITEM_CODE", OracleDbType.NVarChar, line?.LineItem?.BuyerItemCode ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ITEM_DESCRIPTION", OracleDbType.NVarChar, line?.LineItem?.ItemDescription ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_UNIT_NET_PRICE", OracleDbType.NVarChar, line?.LineItem?.OrderedUnitNetPrice ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_UNIT_GROSS_PRICE", OracleDbType.NVarChar, line?.LineItem?.OrderedUnitGrossPrice ?? "", ParameterDirection.Input),
                            new OracleParameter("P_TAX_RATE", OracleDbType.NVarChar, line?.LineItem?.TaxRate ?? "", ParameterDirection.Input),
                            new OracleParameter("P_EAN", OracleDbType.NVarChar, line?.LineItem?.EAN ?? "", ParameterDirection.Input),
                            new OracleParameter("P_GROSS_WEIGHT", OracleDbType.NVarChar, line?.LineItem?.GrossWeight ?? "", ParameterDirection.Input),
                            new OracleParameter("P_NET_WEIGHT", OracleDbType.NVarChar, line?.LineItem?.NetWeight ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_GROSS_AMOUNT", OracleDbType.NVarChar, line?.LineItem?.OrderedGrossAmount ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_NET_AMOUNT", OracleDbType.NVarChar, line?.LineItem?.OrderedNetAmount ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_PALLETS", OracleDbType.NVarChar, line?.LineItem?.OrderedPallets ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_TAX_AMOUNT", OracleDbType.NVarChar, line?.LineItem?.OrderedTaxAmount ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_UNIT_PACKSIZE", OracleDbType.NVarChar, line?.LineItem?.OrderedUnitPacksize ?? "", ParameterDirection.Input),
                            new OracleParameter("P_SUPPLIER_ITEM_CODE", OracleDbType.NVarChar, line?.LineItem?.SupplierItemCode ?? "", ParameterDirection.Input),
                            new OracleParameter("P_UNIT_OF_MEASURE", OracleDbType.NVarChar, line?.LineItem?.UnitOfMeasure ?? "", ParameterDirection.Input),
                            new OracleParameter("P_VOLUME", OracleDbType.NVarChar, line?.LineItem?.Volume ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_BOXES", OracleDbType.NVarChar, line?.LineItem?.OrderedBoxes ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_QUANTITY", OracleDbType.Number, line?.LineItem?.OrderedQuantity ?? "0", ParameterDirection.Input)
                        },
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "HPCSERVICE.EDI_ADD_ORDER_DETAIL"
                    });
                }

            return commands;

        }

        private static void AddOrders(string relPartnerIln, string relDocumentType, string newOrderTrackingId, string relDocumentStandard, string orderPartnerIln)
        {
            if (relPartnerIln == orderPartnerIln)
            {
                var doc = EdiService.Receive<DocumentOrder>(relPartnerIln, relDocumentType, newOrderTrackingId, relDocumentStandard, "").First();
                Orders.Add(doc);
            }
                
        }
    }
}
