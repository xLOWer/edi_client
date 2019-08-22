using EdiClient.Model;
using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using EdiClient.AppSettings;
using EdiClient.Services;
using static EdiClient.Services.Utilites;
using System.Threading.Tasks;

namespace EdiClient.ViewModel.Common
{
    /// <summary>
    /// Класс для работы с заказами
    /// </summary>
    public static class DocumentRepository
    {
        public static List<Model.WebModel.RelationResponse.Relation> Relationships => EdiService.Relationships;
        public static Model.WebModel.RelationResponse.Relation SelectedRelationship => EdiService.SelectedRelationship;
        public static int RelationshipCount => EdiService.RelationshipCount;

        /// <summary>
        /// Получить новые заказы
        /// </summary>
        /// <param name="dateFrom">начальная дата</param>
        /// <param name="dateTo">конечная дата</param>x
        /// <returns>Список полученных заказов</returns>
        public static void GetNewOrders(DateTime dateFrom, DateTime dateTo)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (SelectedRelationship == null) return;
            if (SelectedRelationship.partnerIln == null || SelectedRelationship.documentType == null) return;

            List<Model.WebModel.DocumentInfo> NewOrders
               = EdiService.ListMBEx(SelectedRelationship.partnerIln
                                     , SelectedRelationship.documentType, "", "", ""
                                     , ToEdiDateString(dateFrom)
                                     , ToEdiDateString(dateTo)
                                     , "", "", "").Where(x => x?.documentStatus != "Ошибка" || !string.IsNullOrEmpty(x.fileName)).ToList();

            if (NewOrders.Count() > 0)
            {
                foreach (var order in GetDocuments(dateFrom, dateTo))
                {
                    NewOrders.RemoveAll(x => x.documentNumber == order.ORDER_NUMBER);                   
                }

                Utilites.tasks = new Task[NewOrders.Count()];
                int i = 0;
                foreach (var order in NewOrders)
                {
                    Utilites.tasks[i] = GetOrder(SelectedRelationship.partnerIln, SelectedRelationship.documentType, order.trackingId, SelectedRelationship.documentStandard);
                    Utilites.tasks[i++].Start();
                }
                
                Task.WaitAny(Utilites.tasks);
            }
        }

        private static Task GetOrder(string partnerIln, string documentType, string trackingId, string documentStandard)
        {             
            return new Task(() =>
            {
                var document = EdiService.Receive<DocumentOrder>(partnerIln, documentType, trackingId, documentStandard, "").First();
                InsertIncomingIntoDatabase(document);                
            });
        }


        private static string ToEdiDateString(DateTime date) => $"{date.Year}-{date.Month}-{date.Day}";

        public static void UpdateFailedDetails(string P_EDI_DOC_ID)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Utilites.Logger.Log($"\t\tEDI_REFRESH_DOC_DETAILS.P_EDI_DOC_ID=" + P_EDI_DOC_ID);
            DbService.ExecuteCommand(new OracleCommand()
            {
                Connection = DbService.Connection.conn,
                Parameters = { new OracleParameter("P_EDI_DOC_ID", OracleDbType.NVarChar, P_EDI_DOC_ID, ParameterDirection.Input) },
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema+".") + "EDI_REFRESH_DOC_DETAILS"
            });
        }

        /// <summary>
        /// Создать реальный документ для работы в trader
        /// </summary>
        /// <param name="orderNumber">номер заказа (не его ID в базе!)</param>
        internal static void CreateTraderDocument(string orderID)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Utilites.Logger.Log($"\t\tEDI_MOVE_ORDER.P_ID=" + orderID);
            var commands = new List<OracleCommand>()
                {
                        new OracleCommand()
                        {
                            Parameters =
                            {
                                new OracleParameter("P_ID", OracleDbType.VarChar, orderID, ParameterDirection.Input),
                                new OracleParameter("P_USERNAME", OracleDbType.VarChar, "", ParameterDirection.Input)
                            },
                            Connection = DbService.Connection.conn,
                            CommandType = CommandType.StoredProcedure,
                            CommandText = (AppConfig.Schema+".") + "EDI_MOVE_ORDER"
                        }
                };
            DbService.ExecuteCommand(commands);
            commands.Clear();
        }

        /// <summary>
        /// Отправить полученные заказы в буферную таблицу
        /// </summary>
        /// <param name="orders">Список заказов</param>
        private static void InsertIncomingIntoDatabase(DocumentOrder order)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            List<OracleCommand> commands = new List<OracleCommand>();
            var coms = XmlOrdersToDatabase(order);
            if (coms != null && coms.Count > 0)
                commands.AddRange(coms);
            else
                throw new Exception("Не поступили команды, повторите загрузку");
            // в команды ещё можно забубенить те ,которые бы сразу проверяли "плохие" документы и товары и это всё выполнялось бы
            DbService.ExecuteCommand(commands);
            commands.Clear();
        }

        /// <summary>
        /// Создать список команд с запросами для отправки в базу
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Список сформированных команд</returns>
        private static List<OracleCommand> XmlOrdersToDatabase(DocumentOrder order)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (order == null) return new List<OracleCommand>();
            List<OracleCommand> commands = new List<OracleCommand>();

            commands.Add(new OracleCommand()
            {
                Parameters = {
                        new OracleParameter("P_SENDER_ILN", OracleDbType.NVarChar, order?.DocumentParties?.Sender?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_EXPECTED_DELIVERY_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.ExpectedDeliveryDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_DELIVERY_POINT_ILN", OracleDbType.NVarChar, order?.OrderParties.DeliveryPoint?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_BUYER_ILN", OracleDbType.NVarChar, order?.OrderParties?.Buyer?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_CUSTOMER_ILN", OracleDbType.NVarChar, order?.OrderParties?.UltimateCustomer?.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_SELLER_ILN", OracleDbType.NVarChar, order?.DocumentParties?.Receiver.ILN ?? "", ParameterDirection.Input),
                        new OracleParameter("P_ORDER_CURRENCY", OracleDbType.NVarChar, order?.OrderHeader?.OrderCurrency ?? "", ParameterDirection.Input),
                        new OracleParameter("P_ORDER_DATE", OracleDbType.NVarChar, (order?.OrderHeader?.OrderDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                        new OracleParameter("P_ORDER_NUMBER", OracleDbType.NVarChar, order?.OrderHeader?.OrderNumber ?? "", ParameterDirection.Input),
                        new OracleParameter("P_REMARKS", OracleDbType.NVarChar, order?.OrderHeader?.Remarks ?? "", ParameterDirection.Input),
                        new OracleParameter("P_TOTAL_GROSS_AMOUNT", OracleDbType.NVarChar, order?.OrderSummary?.TotalGrossAmount ?? "", ParameterDirection.Input)
                    },
                Connection = DbService.Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema+".") + "Edi_ADD_ORDER"
            });

            if (order.OrderLines.Lines.Count > 0)
                foreach (var line in order.OrderLines.Lines)
                {
                    commands.Add(new OracleCommand()
                    {
                        Parameters =
                        {
                            new OracleParameter("P_Edi_DOC_NUMBER", OracleDbType.NVarChar, order.OrderHeader.OrderNumber, ParameterDirection.Input),
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
                            new OracleParameter("P_ORDERED_TAX_AMOUNT", OracleDbType.NVarChar, line?.LineItem?.OrderedTaxAmount ?? "", ParameterDirection.Input),
                            new OracleParameter("P_SUPPLIER_ITEM_CODE", OracleDbType.NVarChar, line?.LineItem?.SupplierItemCode ?? "", ParameterDirection.Input),
                            new OracleParameter("P_ORDERED_QUANTITY", OracleDbType.Number, line?.LineItem?.OrderedQuantity ?? "0", ParameterDirection.Input)
                        },
                        Connection = DbService.Connection.conn,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = (AppConfig.Schema+".") + "Edi_ADD_ORDER_DETAIL"
                    });
                }

            return commands;
        }

        internal static DocumentDespatchAdvice DocumentToXmlDespatchAdvice(Document doc)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var Consignment = new DocumentDespatchAdviceDespatchAdviceConsignment();
            var PackingSequence = new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

            var details = doc.Details;

            PackingSequence = new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();
            Consignment = new DocumentDespatchAdviceDespatchAdviceConsignment();

            if (details.Count > 0)
                foreach (var detail in details)
                {
                    if (detail?.QUANTITY != "0")
                        PackingSequence.Add(new DocumentDespatchAdviceDespatchAdviceConsignmentLine()
                        {
                            LineItem = new DocumentDespatchAdviceDespatchAdviceConsignmentLineLineItem()
                            {
                                LineNumber = detail?.LINE_NUMBER ?? "",
                                EAN = detail?.EAN ?? "",
                                BuyerItemCode = detail?.BUYER_ITEM_CODE ?? "",
                                SupplierItemCode = detail?.ID_GOOD ?? "",
                                ItemDescription = detail?.ITEM_DESCRIPTION ?? "",
                                OrderedQuantity = detail?.ORDERED_QUANTITY,
                                QuantityDespatched = detail?.QUANTITY,
                                //ItemSize = detail?.GOOD_SIZE ?? "", 
                                UnitOfMeasure = "PCE",
                                UnitNetPrice = detail?.PRICE,
                                TaxRate = detail?.TAX_RATE.ToString(),
                                UnitGrossPrice = detail.UnitGrossPrice.ToString(),
                                NetAmount = detail.NetAmount.ToString(),
                                GrossAmount = detail.GrossAmount.ToString(),
                                TaxAmount = detail.TaxAmount.ToString()
                            }
                        }
                        );
                }
            Consignment.PackingSequence = PackingSequence ?? new List<DocumentDespatchAdviceDespatchAdviceConsignmentLine>();

            var advice = new DocumentDespatchAdvice()
            {
                DespatchAdviceHeader = new DocumentDespatchAdviceDespatchAdviceHeader()
                {
                    DocumentFunctionCode = "9",
                    DespatchAdviceNumber = doc?.CODE ?? "",
                    DespatchAdviceDate = DateTime.Parse(doc?.DOC_DATETIME),
                    BuyerOrderNumber = doc?.ORDER_NUMBER,
                    UTDnumber = doc?.CODE,
                    UTDDate = DateTime.Parse(doc?.DOC_DATETIME)
                },
                DocumentParties = new DocumentDespatchAdviceDocumentParties()
                {

                    Sender = new DocumentDespatchAdviceDocumentPartiesSender()
                    {
                        ILN = doc?.SELLER_ILN
                    },
                    Receiver = new DocumentDespatchAdviceDocumentPartiesReceiver()
                    {
                        ILN = doc?.SENDER_ILN
                    }
                },
                DespatchAdviceParties = new DocumentDespatchAdviceDespatchAdviceParties()
                {
                    Buyer = new DocumentDespatchAdviceDespatchAdvicePartiesBuyer() { ILN = doc?.BUYER_ILN ?? "" },
                    Seller = new DocumentDespatchAdviceDespatchAdvicePartiesSeller() { ILN = doc?.SELLER_ILN ?? "" },
                    DeliveryPoint = new DocumentDespatchAdviceDespatchAdvicePartiesDeliveryPoint() { ILN = doc?.DELIVERY_POINT_ILN ?? "" },
                },
                DespatchAdviceConsignment = Consignment ?? new DocumentDespatchAdviceDespatchAdviceConsignment(),
                DespatchAdviceSummary = new DocumentDespatchAdviceDespatchAdviceSummary()
                {
                    TotalLines = doc?.Details.Count.ToString() ?? "",
                    TotalNetAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                    TotalGrossAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.GrossAmount)).ToString(),
                    TotalGoodsDespatchedAmount = PackingSequence.Count().ToString(),
                    //TotalPSequence = PackingSequence.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                    TotalTaxAmount = PackingSequence.Sum(x => double.Parse(x.LineItem.TaxAmount)).ToString(),
                }
            };

            return advice;
        }

        /// <summary>
        /// Отправить извещение об отгрузке в систему EDI
        /// </summary>
        /// <param name="advice">отправляемый заказ</param>
        internal static void SendDesadv(Document doc)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            // преобразование выбранного документа в xml-desadv
            DocumentDespatchAdvice advice = DocumentToXmlDespatchAdvice(doc);

            if (advice == null) { Utilites.Error("При отправке уведомления об отгрузке: не выбран заказ"); return; }
            if (advice.DocumentParties == null) { Utilites.Error("При отправке уведомления об отгрузке: отсутсвуют части документа(DocumentParties)"); return; }
            if (advice.DocumentParties?.Receiver == null) { Utilites.Error("При отправке уведомления об отгрузке: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(advice.DocumentParties.Receiver.ILN)) { Utilites.Error("При отправке уведомления об отгрузке: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Utilites.Error("При отправке уведомления об отгрузке: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (advice.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Utilites.Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }


            EdiService.Send(SelectedRelationship?.partnerIln, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
            DbService.ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.NVarChar, doc.ID, ParameterDirection.Input)
                        },
                Connection = DbService.Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema+".") + "EDI_MAKE_DESADV"
            });
        }

        internal static DocumentOrderResponse DocumentToXmlOrderResponse(Document doc)
        {
            DocumentOrderResponse ordrsp = null;
            var orderLines = new DocumentOrderResponseLine();
            var lines = new List<Line>();
            lines = new List<Line>();
            orderLines = new DocumentOrderResponseLine();
            try
            {
                if (doc.Details.Count > 0)
                    foreach (var detail in doc.Details)
                    {
                        if (detail?.QUANTITY != "0")
                            lines.Add(new Line()
                            {
                                LineItem = new DocumentOrderResponseLineLineItem()
                                {
                                    LineNumber = detail?.LINE_NUMBER ?? "",
                                    EAN = detail?.EAN ?? "",
                                    BuyerItemCode = detail?.BUYER_ITEM_CODE ?? "",
                                    SupplierItemCode = detail?.ID_GOOD ?? "",
                                    ItemDescription = detail?.ITEM_DESCRIPTION ?? "",
                                    OrderedQuantity = detail?.ORDERED_QUANTITY,
                                    QuantityToBeDelivered = detail?.QUANTITY,
                                    AllocatedDelivered = detail?.QUANTITY,
                                    QuantityDifference = detail?.UnitsDifference.ToString(),
                                    UnitOfMeasure = "PCE",
                                    OrderedUnitNetPrice = detail?.PRICE.ToString() ?? "",
                                    TaxRate = detail?.TAX_RATE.ToString(),
                                    OrderedUnitGrossPrice = detail?.UnitGrossPrice.ToString(),
                                    NetAmount = detail?.NetAmount.ToString(),
                                    GrossAmount = detail?.GrossAmount.ToString(),
                                    TaxAmount = detail?.TaxAmount.ToString()
                                }
                            });
                    }

                orderLines.Lines = lines ?? new List<Line>();

                ordrsp = new DocumentOrderResponse()
                {
                    DocumentParties = new DocumentOrderResponseDocumentParties()
                    {
                        Sender = new DocumentOrderResponseDocumentPartiesSender()
                        {
                            ILN = doc?.SELLER_ILN
                        },
                        Receiver = new DocumentOrderResponseDocumentPartiesReceiver()
                        {
                            ILN = doc?.SENDER_ILN
                        }
                    },
                    OrderResponseHeader = new DocumentOrderResponseOrderResponseHeader()
                    {
                        DocumentFunctionCode = "9", // требование EDISOFT
                        OrderResponseNumber = doc?.CODE ?? "",
                        OrderResponseDate = DateTime.Parse(doc?.DOC_DATETIME),
                        OrderResponseCurrency = doc?.ORDER_CURRENCY ?? "",
                        Order = new DocumentOrderResponseOrderResponseHeaderOrder()
                        {
                            BuyerOrderNumber = doc.ORDER_NUMBER,
                        }
                    },
                    OrderResponseParties = new DocumentOrderResponseOrderResponseParties()
                    {
                        Buyer = new DocumentOrderResponseOrderResponsePartiesBuyer()
                        {
                            ILN = doc?.BUYER_ILN ?? ""
                        },
                        Seller = new DocumentOrderResponseOrderResponsePartiesSeller()
                        {
                            ILN = doc?.SELLER_ILN ?? ""
                        },
                        DeliveryPoint = new DocumentOrderResponseOrderResponsePartiesDeliveryPoint()
                        {
                            ILN = doc?.DELIVERY_POINT_ILN ?? ""
                        }
                    },
                    OrderResponseLines = orderLines ?? new DocumentOrderResponseLine(),
                    OrderResponseSummary = new DocumentOrderResponseOrderResponseSummary()
                    {
                        TotalLines = doc?.Details.Count.ToString() ?? "",
                        TotalAmount = orderLines.Lines.Count.ToString(),
                        TotalNetAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.NetAmount)).ToString(),
                        TotalGrossAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.GrossAmount)).ToString(),
                        TotalTaxAmount = orderLines.Lines.Sum(x => double.Parse(x.LineItem.TaxAmount)).ToString(),
                    }

                };
            }
            catch(Exception ex) { Utilites.Error(ex); }
            return ordrsp;
        }

        /// <summary>
        /// Отправить ответ на заказ в систему EDI
        /// </summary>
        /// <param name="order">отправляемый заказ</param>
        internal static void SendOrdrsp(Document doc)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            // преобразование выбранного документа в xml-desadv
            DocumentOrderResponse order = DocumentToXmlOrderResponse(doc);
            
            if (order == null) { Utilites.Error("При отправке ответа на заказ: заказ = null"); return; }
            if (order.DocumentParties == null) { Utilites.Error("При отправке ответа на заказ: отсутсвуют части документа(DocumentParties)"); return; }
            if (order.DocumentParties?.Receiver == null) { Utilites.Error("При отправке ответа на заказ: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(order.DocumentParties.Receiver.ILN)) { Utilites.Error("При отправке ответа на заказ: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Utilites.Error("При отправке ответа на заказ: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Utilites.Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (order.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Utilites.Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }
            
            var sendOrder = XmlService<DocumentOrderResponse>.Serialize(order);
            EdiService.Send(SelectedRelationship.partnerIln, "ORDRSP", "", "", "T", "", sendOrder, 20);
            DbService.ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.Number, doc.ID, ParameterDirection.Input)
                        },
                Connection = DbService.Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_ORDRSP"
            });
        }
        
        internal static List<Detail> GetDocumentDetails(string Id)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var sql = DbService.Sqls.GET_ORDER_DETAILS(Id);
            var result = DbService.DocumentSelect<Detail>(sql);
            return result;
        }

        internal static List<Document> GetDocuments(DateTime dateFrom, DateTime dateTo)
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var sql = DbService.Sqls.GET_ORDERS(SelectedRelationship?.partnerIln ?? "'%'", dateFrom, dateTo);
            var result = DbService.DocumentSelect<Document>(sql);
            if (result != null)
                if (result.Count > 0)
                    foreach (var doc in result)
                    {
                        doc.Details = GetDocumentDetails(doc.ID) ?? new List<Detail>();
                        //foreach (var detail in doc.Details)                        
                        //    detail.Doc = doc;                        
                    }
            Utilites.LoadedDocsCount = result.Count.ToString();
            return result;
        }
        
        internal static List<DocumentReceivingAdvice> GetRecadv()
        {
            Utilites.Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            return new List<DocumentReceivingAdvice>();
        }

        public static List<T> GetList<T>(string sql)
        {
            Utilites.Logger.Log($"[GOODS] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (string.IsNullOrEmpty(sql)) { Utilites.Error("Ошибка при выполнении загрузки"); return null; }
            var result = DbService.DocumentSelect<T>(new List<string> { sql }).Cast<T>().ToList();
            return result;
        }
    }
}
