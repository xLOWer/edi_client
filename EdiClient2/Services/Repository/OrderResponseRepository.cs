using EdiClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace EdiClient.Services.Repository
{
    internal static class OrderResponseRepository
    {
        public static List<Model.WebModel.RelationResponse.Relation> Relationships { get; set; }
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship { get; set; }
        public static List<Model.WebModel.DocumentInfo> Orders { get; set; }

        /// <summary>
        /// Отправить ответ на заказ в систему EDI
        /// </summary>
        /// <param name="order">отправляемый заказ</param>
        internal static void SendOrdrsp(DocumentOrderResponse order)
        {
            var sendOrder = XmlService<DocumentOrderResponse>.Serialize(order);
            EdiService.Send(order.DocumentParties?.Sender?.ILN ?? throw new Exception("Ошибка при отправке документа"),
                "ORDRSP", "", "", "T", "", sendOrder, 20);
            DbService.Insert(SqlConfiguratorService.Sql_UpdateEdiDocSetIsInEdiAsORDRSP(order.OrderResponseHeader.OrderResponseNumber));
            ////LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
        }

        /// <summary>
        /// Получить ответы на заказ из базы
        /// </summary>
        /// <param name="dateFrom">дата от</param>
        /// <param name="dateTo">дата до</param>
        /// <returns>Список ответов на заказ из базы</returns>
        internal static List<DocumentOrderResponse> GetOrdrsp(DateTime dateFrom, DateTime dateTo)
        {
            var sql = SqlConfiguratorService.Sql_SelectOrdrspHeader(dateFrom, dateTo);
            var headers = DbService<DbDocHeader>.DocumentSelect(sql);

            var ordrsp = new List<DocumentOrderResponse>();

            var orderLines = new DocumentOrderResponseLine();
            var lines = new List<Line>();
            
                if (headers.Count > 0)
                    foreach (var header in headers)
                    {
                        var details = DbService<DbDocDetail>.DocumentSelect(SqlConfiguratorService.Sql_SelectOrdrspDetails(header?.ID_TRADER));

                    lines = new List<Line>();
                    orderLines = new DocumentOrderResponseLine();
                        if (details.Count > 0)
                            foreach (var detail in details)
                            {
                                lines.Add(new Line()
                                {
                                    LineItem = new DocumentOrderResponseLineLineItem()
                                    {
                                        LineNumber = detail?.LineNumber ?? "",
                                        EAN = detail?.EAN ?? "",
                                        BuyerItemCode = detail?.BuyerItemCode ?? "",
                                        SupplierItemCode = detail?.ID_GOOD ?? "",
                                        ItemDescription = detail?.ItemDescription ?? "",
                                        OrderedQuantity = detail?.OrderedQuantity,
                                        QuantityToBeDelivered = detail?.QUANTITY,
                                        QuantityDifference = (double.Parse(detail?.OrderedQuantity) - double.Parse(detail?.QUANTITY)).ToString(),
                                        UnitOfMeasure = detail.UnitOfMeasure ?? "",
                                        OrderedUnitNetPrice = detail?.PRICE ?? "",
                                        TaxRate = detail?.TAX,
                                        OrderedUnitGrossPrice = (double.Parse(detail.PRICE) / 100 * (100 + double.Parse(detail.TAX))).ToString(),
                                        NetAmount = (double.Parse(detail.PRICE) * double.Parse(detail.QUANTITY)).ToString(),
                                        GrossAmount = (( double.Parse(detail.PRICE) / 100 * (100 + double.Parse(detail.TAX)) ) * double.Parse(detail.QUANTITY)).ToString(),
                                        TaxAmount = (((double.Parse(detail.PRICE) / 100 * (100 + double.Parse(detail.TAX))) - double.Parse(detail.PRICE)) * double.Parse(detail.QUANTITY)).ToString()
                                    }
                                });
                            }
                        orderLines.Lines = lines ?? new List<Line>();

                        ordrsp.Add(new DocumentOrderResponse()
                        {               
                            DocumentParties = new DocumentOrderResponseDocumentParties()
                            {
                                Sender = new DocumentOrderResponseDocumentPartiesSender()
                                {
                                    ILN = header?.SellerIln
                                },
                                Receiver = new DocumentOrderResponseDocumentPartiesReceiver()
                                {
                                    ILN = header?.SenderILN
                                }
                            },
                            OrderResponseHeader = new DocumentOrderResponseOrderResponseHeader()
                            {
                                DocumentFunctionCode = "9",
                                OrderResponseNumber = header?.CODE ?? "",
                                OrderResponseDate = DateTime.Parse(header?.DOC_DATETIME),
                                OrderResponseCurrency = header?.OrderCurrency ?? "",
                                Order = new DocumentOrderResponseOrderResponseHeaderOrder()
                                {
                                    BuyerOrderNumber = header.OrderNumber,                                    
                                }
                            },
                            OrderResponseParties = new DocumentOrderResponseOrderResponseParties()
                            {
                                Buyer = new DocumentOrderResponseOrderResponsePartiesBuyer()
                                {
                                    ILN = header?.BuyerIln ?? ""
                                },
                                Seller = new DocumentOrderResponseOrderResponsePartiesSeller()
                                {
                                    ILN = header?.SellerIln ?? ""
                                },
                                DeliveryPoint = new DocumentOrderResponseOrderResponsePartiesDeliveryPoint()
                                {
                                    ILN = header?.DeliveryPointIln ?? ""
                                }
                            },
                            OrderResponseLines = orderLines ?? new DocumentOrderResponseLine(),
                            OrderResponseSummary = new DocumentOrderResponseOrderResponseSummary()
                            {
                                TotalLines = header?.TOTAL_LINES ?? "",
                                TotalAmount = orderLines.Lines.Count.ToString(),
                                TotalNetAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                                TotalGrossAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.GrossAmount)).ToString(),
                                TotalTaxAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.TaxAmount)).ToString(),
                            }
                            ,
                            IsInEdiAsOrdrsp = bool.Parse((header?.IS_IN_EDI_AS_ORDRSP != null ? "true" : "false") ?? "false")

                        });
                }

            ////LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
            return ordrsp ?? new List<DocumentOrderResponse>();
        }

    }
}
