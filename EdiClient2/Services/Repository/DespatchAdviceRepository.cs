using EdiClient.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace EdiClient.Services.Repository
{
    internal static class DespatchAdviceRepository
    {
        public static List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public static int RelationshipCount => EdiService.RelationshipCount;
        public static List<Model.WebModel.DocumentInfo> Advices { get; set; }

        /// <summary>
        /// Отправить извещение об отгрузке в систему EDI
        /// </summary>
        /// <param name="advice">отправляемый заказ</param>
        internal static void SendDesadv(DocumentDespatchAdvice advice)
        {
            if (advice == null) { Utilites.Error("При отправке уведомления об отгрузке: не выбран заказ"); return; }
            if (advice.DocumentParties == null) { Utilites.Error("При отправке уведомления об отгрузке: отсутсвуют части документа(DocumentParties)"); return; }
            if (advice.DocumentParties?.Receiver == null) { Utilites.Error("При отправке уведомления об отгрузке: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(advice.DocumentParties.Receiver.ILN)) { Utilites.Error("При отправке уведомления об отгрузке: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Utilites.Error("При отправке уведомления об отгрузке: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (advice.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Utilites.Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }


            EdiService.Send(SelectedRelationship?.partnerIln, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
            DbService.Insert($@"UPDATE EDI_DOC SET IS_IN_EDI_AS_DESADV = {SqlConfiguratorService.OracleDateFormat(DateTime.UtcNow)} WHERE ORDER_NUMBER 
= (SELECT ORDER_NUMBER FROM edi_doc WHERE ID_TRADER
= (SELECT ID FROM DOC_JOURNAL DJ WHERE CODE = '{advice.DespatchAdviceHeader.DespatchAdviceNumber}' and rownum = 1) and rownum = 1)");
        }

        /// <summary>
        /// Получить извещения об отгрузке из базы
        /// </summary>
        /// <param name="dateFrom">дата от</param>
        /// <param name="dateTo">дата до</param>
        /// <returns>Список ответов на заказ из базы</returns>
        internal static List<DocumentDespatchAdvice> GetDesadv(DateTime dateFrom, DateTime dateTo)
        {
            var sql = SqlConfiguratorService.Sql_SelectDesadvHeader(dateFrom, dateTo);
            var headers = DbService<DbDocHeader>.DocumentSelect(sql);

            var adviceList = new List<DocumentDespatchAdvice>();

            var Consignment = new DocumentDespatchAdviceDespatchAdviceConsignment();
            var PackingSequence = new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

            if (headers.Count > 0)
                foreach (var header in headers)
                {
                    var details = DbService<DbDocDetail>.DocumentSelect(SqlConfiguratorService.Sql_SelectOrdrspDetails(header?.ID_TRADER));

                    PackingSequence = new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();
                    Consignment = new DocumentDespatchAdviceDespatchAdviceConsignment();
                    if (details.Count > 0)
                        foreach (var detail in details)
                        {
                            PackingSequence.Add(new DocumentDespatchAdviceDespatchAdviceConsignmentLine()
                            {
                                LineItem = new DocumentDespatchAdviceDespatchAdviceConsignmentLineLineItem()
                                {
                                    LineNumber = detail?.LineNumber ?? "",
                                    EAN = detail?.EAN ?? "",
                                    BuyerItemCode = detail?.BuyerItemCode ?? "", 
                                    SupplierItemCode = detail?.ID_GOOD ?? "",
                                    ItemDescription = detail?.ItemDescription ?? "",
                                    OrderedQuantity = detail?.OrderedQuantity,
                                    QuantityDespatched =  detail?.QUANTITY, 
                                    //ItemSize = detail?.GOOD_SIZE ?? "", 
                                    UnitOfMeasure =  detail?.UnitOfMeasure,
                                    UnitNetPrice =  detail?.PRICE,
                                    TaxRate =  detail?.TAX, // ставка НДС
                                    UnitGrossPrice = Math.Round(double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX)), 4).ToString(),
                                    NetAmount = Math.Round(double.Parse( detail?.PRICE) * double.Parse( detail?.QUANTITY), 4).ToString(),
                                    GrossAmount = Math.Round((double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX))) * double.Parse( detail?.QUANTITY), 4).ToString(),
                                    TaxAmount = Math.Round(((double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX))) - double.Parse( detail?.PRICE)) * double.Parse( detail?.QUANTITY),4).ToString()
                                }
                            }
                            );
                        }
                    Consignment.PackingSequence = PackingSequence ?? new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

                    adviceList.Add(new DocumentDespatchAdvice()
                    {
                        DespatchAdviceHeader = new DocumentDespatchAdviceDespatchAdviceHeader()
                        {
                            DocumentFunctionCode = "9",
                            DespatchAdviceNumber = header?.CODE ?? "",
                            DespatchAdviceDate = DateTime.Parse(header?.DOC_DATETIME),
                            BuyerOrderNumber = header?.OrderNumber,
                            UTDnumber = header?.CODE,
                            UTDDate = DateTime.Parse(header?.DOC_DATETIME)
                        },
                        DocumentParties = new DocumentDespatchAdviceDocumentParties()
                        {

                            Sender = new DocumentDespatchAdviceDocumentPartiesSender()
                            {
                                ILN = header?.SellerIln
                            },
                            Receiver = new DocumentDespatchAdviceDocumentPartiesReceiver()
                            {
                                ILN = header?.SenderILN
                            }
                        },
                        DespatchAdviceParties = new DocumentDespatchAdviceDespatchAdviceParties()
                        {
                            Buyer = new DocumentDespatchAdviceDespatchAdvicePartiesBuyer() { ILN = header?.BuyerIln ?? "" },
                            Seller = new DocumentDespatchAdviceDespatchAdvicePartiesSeller() { ILN = header?.SellerIln ?? "" },
                            DeliveryPoint = new DocumentDespatchAdviceDespatchAdvicePartiesDeliveryPoint() { ILN = header?.DeliveryPointIln ?? "" },
                        },
                        DespatchAdviceConsignment = Consignment ?? new DocumentDespatchAdviceDespatchAdviceConsignment(),
                        DespatchAdviceSummary = new DocumentDespatchAdviceDespatchAdviceSummary()
                        {
                            TotalLines = header?.TOTAL_LINES ?? "",
                            TotalNetAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                            TotalGrossAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.GrossAmount)).ToString(),
                            TotalGoodsDespatchedAmount = PackingSequence.Count().ToString(),
                            //TotalPSequence = PackingSequence.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                            TotalTaxAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.TaxAmount)).ToString(),
                        }
                        ,
                        IsInEdiAsDesadv = bool.Parse((header?.IS_IN_EDI_AS_DESADV != null ? "true" : "false") ?? "false")

                    });
                }
            return adviceList
                .Where(x=>x.DocumentParties.Receiver.ILN == SelectedRelationship.partnerIln)
                .ToList() ?? new List<DocumentDespatchAdvice>();
        }

    }
}
