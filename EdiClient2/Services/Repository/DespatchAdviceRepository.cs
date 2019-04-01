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
        public static List<Model.WebModel.RelationResponse.Relation> Relationships { get; set; }
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship { get; set; }
        public static List<Model.WebModel.DocumentInfo> Advices { get; set; }

        internal static void UpdateData(DateTime dateFrom, DateTime dateTo)
        {
            Relationships = EdiService.Relationships().Where(x => x.documentType == "DESADV").ToList() ?? throw new Exception("При загрузке связей возникла ошибка");
            SelectedRelationship = SelectedRelationship ?? (Relationships[0] ?? throw new Exception("Не выбрана связь с покупателем"));

            LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
        }

        /// <summary>
        /// Отправить извещение об отгрузке в систему EDI
        /// </summary>
        /// <param name="advice">отправляемый заказ</param>
        internal static void SendDesadv(DocumentDespatchAdvice advice)
        {
            EdiService.Send(advice.DespatchAdviceParties.Buyer.ILN, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
            DbService.Insert($@"UPDATE EDI_DOC SET IS_IN_EDI_AS_DESADV = {SqlConfiguratorService.OracleDateFormat(DateTime.UtcNow)} WHERE ORDER_NUMBER 
= (SELECT ORDER_NUMBER FROM edi_doc WHERE ID_TRADER
= (SELECT ID FROM DOC_JOURNAL DJ WHERE CODE = '{advice.DespatchAdviceHeader.DespatchAdviceNumber}' and rownum = 1) and rownum = 1)");
            LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{advice}", 2);
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

            var advice = new List<DocumentDespatchAdvice>();

            var Consignment = new DocumentDespatchAdviceDespatchAdviceConsignment();
            var PackingSequence = new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

            if (headers.Count > 0)
                foreach (var header in headers)
                {
                    var details = DbService<DbDocDetail>.DocumentSelect(SqlConfiguratorService.Sql_SelectOrdrspDetails(header?.ID_TRADER));

                    PackingSequence.Clear();
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
                                    BuyerItemCode = detail?.BuyerItemCode ?? "", // Неверный формат кода товара Реми:  <BuyerItemCode>
                                    SupplierItemCode = detail?.ID_GOOD ?? "",
                                    ItemDescription = detail?.ItemDescription ?? "",
                                    OrderedQuantity = detail?.OrderedQuantity,
                                    QuantityDespatched =  detail?.QUANTITY, // Указано нулевое количество к отгрузке: <QuantityDespatched>
                                    //ItemSize = detail?.GOOD_SIZE ?? "", 
                                    UnitOfMeasure =  detail?.UnitOfMeasure, // Не указана единица измерения: <UnitOfMeasure>
                                    UnitNetPrice =  detail?.PRICE,
                                    TaxRate =  detail?.TAX, // ставка НДС
                                    UnitGrossPrice = (double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX))).ToString(),
                                    NetAmount = (double.Parse( detail?.PRICE) * double.Parse( detail?.QUANTITY)).ToString(),
                                    GrossAmount = ((double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX))) * double.Parse( detail?.QUANTITY)).ToString(),
                                    TaxAmount = (((double.Parse( detail?.PRICE) / 100 * (100 + double.Parse( detail?.TAX))) - double.Parse( detail?.PRICE)) * double.Parse( detail?.QUANTITY)).ToString()
                                }
                            }
                            );
                        }
                    Consignment.PackingSequence = PackingSequence ?? new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

                    advice.Add(new DocumentDespatchAdvice()
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
            LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} args:{LogService.FormatArgsArray(MethodBase.GetCurrentMethod().GetGenericArguments())}", 2);
            return advice ?? new List<DocumentDespatchAdvice>();
        }

    }
}
