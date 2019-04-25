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
        public static List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public static int RelationshipCount => EdiService.RelationshipCount;
        public static List<Model.WebModel.DocumentInfo> Orders { get; set; }

        /// <summary>
        /// Отправить ответ на заказ в систему EDI
        /// </summary>
        /// <param name="order">отправляемый заказ</param>
        internal static void SendOrdrsp(DocumentOrderResponse order)
        {
            if (order == null) { Utilites.Error("При отправке ответа на заказ: не выбран заказ"); return; }
            if (order.DocumentParties == null) { Utilites.Error("При отправке ответа на заказ: отсутсвуют части документа(DocumentParties)"); return; }
            if (order.DocumentParties?.Receiver == null) { Utilites.Error("При отправке ответа на заказ: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(order.DocumentParties.Receiver.ILN)) { Utilites.Error("При отправке ответа на заказ: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Utilites.Error("При отправке ответа на заказ: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (order.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Utilites.Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }

            var sendOrder = XmlService<DocumentOrderResponse>.Serialize(order);
            EdiService.Send(SelectedRelationship.partnerIln, "ORDRSP", "", "", "T", "", sendOrder, 20);
            DbService.Insert(SqlConfiguratorService.Sql_UpdateEdiDocSetIsInEdiAsORDRSP(order.OrderResponseHeader.OrderResponseNumber));
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

                            /******************************************************/
                            /*********************** РАСЧЁТЫ **********************/

                            var TaxRate = double.Parse(detail.TAX);
                            var UnitNetPrice = double.Parse(detail.PRICE);
                            var UnitsCount = double.Parse(detail?.QUANTITY);
                            var UnitsDifference = double.Parse(detail?.OrderedQuantity) - UnitsCount;

                            var UnitGrossPrice = Math.Round(  UnitNetPrice / 100 * (100 + TaxRate) ,2  );
                            var GrossAmount = Math.Round(  UnitGrossPrice * UnitsCount ,2  );
                            var TaxAmount = Math.Round(GrossAmount * TaxRate / (100 + TaxRate), 2);
                            var NetAmount = GrossAmount - TaxAmount;

                            /*********************** РАСЧЁТЫ **********************/
                            /******************************************************/

                            lines.Add(new Line()
                            {
                                LineItem = new DocumentOrderResponseLineLineItem()
                                {
                                    LineNumber = detail?.LineNumber ?? "",
                                    EAN = detail?.EAN ?? "",
                                    BuyerItemCode = detail?.BuyerItemCode ?? "",
                                    SupplierItemCode = detail?.ID_GOOD ?? "",
                                    ItemDescription = detail?.ItemDescription ?? "",
                                    OrderedQuantity = detail?.QUANTITY,
                                    QuantityToBeDelivered = detail?.QUANTITY,
                                    AllocatedDelivered = detail?.QUANTITY,
                                    QuantityDifference = UnitsDifference.ToString(),
                                    UnitOfMeasure = detail.UnitOfMeasure ?? "",
                                    OrderedUnitNetPrice = UnitNetPrice.ToString() ?? "",
                                    TaxRate = TaxRate.ToString(),
                                    OrderedUnitGrossPrice = UnitGrossPrice.ToString(),
                                    NetAmount = NetAmount.ToString(),
                                    GrossAmount = GrossAmount.ToString(),
                                    TaxAmount = TaxAmount.ToString()
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
            return ordrsp.Where(x => x.DocumentParties.Receiver.ILN == SelectedRelationship.partnerIln).ToList() ?? new List<DocumentOrderResponse>();
        }

    }
}
