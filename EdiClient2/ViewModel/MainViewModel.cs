using EdiClient.Services;
using System.ComponentModel;
using EdiClient.AppSettings;
using System;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Bars;
using System.Collections.Generic;
using EdiClient.Model;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Devart.Data.Oracle;
using static EdiClient.Services.EdiService;
using static EdiClient.Services.DbService;
using static EdiClient.Model.WebModel.RelationResponse;
using static EdiClient.Services.Utils.Utilites;
using EdiClient.View;
using System.Threading;

namespace EdiClient.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        public MainViewModel()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);

            tokenSource2 = new CancellationTokenSource();
            ct = tokenSource2.Token;

            AutoHandlerTask = new Task(() =>
            {
                int c1 = 0, c2 = 0, c3 = 0;
                while (true)
                {
                    // если токен запросил завершение
                    if (ct.IsCancellationRequested) tokenSource2.Cancel();
                    c1 = GetNewOrders();
                    if (ct.IsCancellationRequested) tokenSource2.Cancel();
                    c2 = SendOrderResponses();
                    if (ct.IsCancellationRequested) tokenSource2.Cancel();
                    c3 = SendDespatchAdvice();
                    Time = $"[{DateTime.Now.ToShortTimeString()}] ORDERS: {c1} | ORDRSP: {c2} | DESADV: {c3}";
                    Logger.Log($"[AUTOHANDLER]ORDERS:{c1}|ORDRSP:{c2}|DESADV: {c3}");
                    NotifyPropertyChanged("Time");
                    RaiseAllProps();
                    Thread.Sleep(1000*60*10);
                }
            }, tokenSource2.Token);

            AppConfig = AppConfigHandler.conf;
            if (AppConfig.EnableAutoHandler) AutoHandlerTask.Start();
            RaiseAllProps();
        }

        private Task AutoHandlerTask;
        private CancellationTokenSource tokenSource2;
        private CancellationToken ct;

        public Command EditValueChangedCommand => new Command((o) =>
        {
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            SelectedRelationship = cmb.SelectedItem as Relation;
        });

        public Command<BarEditItem> RefreshRelationshipsCommand => new Command<BarEditItem>((o) =>
        {
            UpdateData();
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            cmb.ItemsSource = Relationships;
            RaiseAllProps();
        });

        public Command SaveConfigCommand => new Command((o) =>
        {
            RaiseAllProps();
            AppConfigHandler.conf = AppConfig;
            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfig = AppConfigHandler.conf;
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();

            // эта логика необходима для случаев, когда идёт работа нескольких опреаторов
            // и чтобы не возникало задвоений отправленных или полученных документов
            // то запускать автообработку надо только на одном компьютере
            if (AppConfig.EnableAutoHandler)// если стоит галка "автообработка"
            { // то
                if(AutoHandlerTask.Status != TaskStatus.Running) // если она НЕ запущена - запустить
                    AutoHandlerTask.Start();
            }
            else
            {// иначе если НЕ стоит галка "автообработка"
                if (AutoHandlerTask.Status == TaskStatus.Running) // и она запущена - остановить
                    tokenSource2.Cancel();
            }

            RaiseAllProps();
        });
        
        public Command OpenPriceTypesViewCommand => new Command((o) =>
       {
           try
           {
               _PriceTypesView = new PriceTypesView();
               _PriceTypesView.Activate();
               _PriceTypesView.Show();
           }
           catch (Exception ex) { Error(ex); }
       });

        public Command OpenMatchMakerViewCommand => new Command((o) =>
        {
            try
            {
                _MatchMakerView = new MatchMakerView();
                _MatchMakerView.Activate();
                _MatchMakerView.Show();
            }
            catch (Exception ex) { Error(ex); }
        });
        
        public Command OpenContractorsMatchViewCommand => new Command((o) =>
        {
            try
            {
                _ContractorsMatchView = new ContractorsMatchView();
                _ContractorsMatchView.Activate();
                _ContractorsMatchView.Show();
            }
            catch (Exception ex) { Error(ex); }
        });
               
        public void LogDocument(string name)
        {            
           Logger.Log("["+name+"] " +
               "id=" + SelectedDocument.ID +
               "|number="+ SelectedDocument.ORDER_NUMBER +
               "|sender="+ SelectedDocument.SENDER_ILN +
               "|customer="+ SelectedDocument.CUSTOMER_ILN +
               "|deliveryp="+ SelectedDocument.DELIVERY_POINT_ILN +
               "|del_lines="+ SelectedDocument.Details_DeliveryLinesCount +
               "|ord_lines="+ SelectedDocument.Details_OrderedLinesCount +
               "|details="+ SelectedDocument.DetailsCount);
        } 
        
        public Command ToTraderCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("ToTraderCommand");
                CreateTraderDocument(SelectedDocument.ID);
                GetDocuments();
                RaiseAllProps();
            }));
        
        public Command SendORDRSPCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("SendORDRSPCommand");
                SendSelectedOrderResponse(SelectedDocument);
                GetDocuments();
                RaiseAllProps();
            }));
       
        public Command SendDESADVCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("SendDESADVCommand");
                SendSelectedDespatchAdvice(SelectedDocument);
                GetDocuments();
                RaiseAllProps();
            }));
        
        public Command GetDocumentsCommand => new Command((o) => ActionInTime(()
            => {
                Logger.Log("[GetDocumentsCommand]count="+Documents.Count());
                GetDocuments();
                RaiseAllProps();
            }));
        
        public Command GetEDIDOCCommand => new Command((o) => ActionInTime(()
            => {
                //GetRecadv();
                GetNewOrders(true);
                //Documents = GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));
        
        public Command ReloadDocumentCommand => new Command((o) =>
        {
            if (SelectedDocument == null) return;
            LogDocument("ReloadDocumentCommand");
            var sql = "delete from EDI.EDI_DOC WHERE ID = "+SelectedDocument.ID;
            ExecuteLine(sql);
            GetNewOrders(true);
            RaiseAllProps();

        });

        public Command NextDayCommand => new Command((o) =>
        {
            DateFrom = DateFrom.AddDays(1);
            DateTo = DateTo.AddDays(1);
            RaiseAllProps();
        });

        public Command PrevDayCommand => new Command((o) =>
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
            RaiseAllProps();
        });

        public void ActionInTime(Action act)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                watch.Stop();
                Error(ex);
            }
            finally
            {
                watch.Stop();
                Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
                RaiseAllProps();
            }
        }
        
        
        internal void GetDocuments()
        {
            var sql = Sqls.GET_ORDERS(SelectedRelationship?.partnerIln ?? "'%'", dateFrom, dateTo);
            Documents = DocumentSelect<Document>(sql);
                if (Documents.Count > 0)
                    foreach (var doc in Documents)
                    {
                        doc.Details = GetDocumentDetails(doc.ID) ?? new List<Detail>();                  
                    }
        }

        internal List<Detail> GetDocumentDetails(string Id)
        {
            var sql = Sqls.GET_ORDER_DETAILS(Id);
            var result = DocumentSelect<Detail>(sql);
            return result;
        }

        public Command UpdateFailedDetailsCommand = new Command((o) =>
        {
            try
            {
                ExecuteCommand(new OracleCommand()
                {
                    Parameters = { new OracleParameter("P_EDI_DOC_ID", OracleDbType.NVarChar, o.ToString(), ParameterDirection.Input) },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "EDI.EDI_MANAGER.REFRESH_DOC_DETAILS"
                });
                Logger.Log("[UpdateFailedDetailsCommand]P_EDI_DOC_ID=" + o.ToString());
            }
            catch(Exception ex) { Error(ex); }

        });

        internal void CreateTraderDocument(string orderID)
        {
            var commands = new List<OracleCommand>()
                {
                        new OracleCommand()
                        {
                            Parameters =
                            {
                                new OracleParameter("P_ID", OracleDbType.VarChar, orderID, ParameterDirection.Input),
                                new OracleParameter("P_USERNAME", OracleDbType.VarChar, "", ParameterDirection.Input)
                            },
                            Connection = Connection.conn,
                            CommandType = CommandType.StoredProcedure,
                            CommandText = "EDI.EDI_MANAGER.MOVE_ORDER"
                        }
                };
            ExecuteCommand(commands);
            commands.Clear();
        }



        internal DocumentDespatchAdvice DocumentToXmlDespatchAdvice(Document doc)
        {
            //Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
        
        internal DocumentOrderResponse DocumentToXmlOrderResponse(Document doc)
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
                        DocumentFunctionCode = "9",
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
            catch (Exception ex) { Error(ex); }
            return ordrsp;
        }



        public int GetNewOrders(bool selectedDates = false)
        {
            int count = 0;

            List<OracleCommand> commands = new List<OracleCommand>();


            foreach (var rel in Relationships)
            {
                if (Relationships == null) return count;

                var sql = Sqls.GET_ORDERS(rel?.partnerIln ?? "'%'",
                            !selectedDates ? DateTime.Now.AddDays(-14) : dateFrom,
                            !selectedDates ? DateTime.Now.AddDays(2) : dateTo);
                var docs = DocumentSelect<Document>(sql);

                List<Model.WebModel.DocumentInfo> NewOrders
                   = ListMBEx(rel.partnerIln, "ORDER", "", "", "", ToEdiDateString(!selectedDates ? DateTime.Now.AddDays(-14) : dateFrom),
                   ToEdiDateString(!selectedDates ? DateTime.Now.AddDays(2) : dateTo), "", "", "")
                   .Where(x => x?.documentStatus != "Ошибка" || !string.IsNullOrEmpty(x.fileName)).Where(x => !docs.Any(d => d.ORDER_NUMBER == x.documentNumber)).ToList();

                if (NewOrders.Count() > 0)
                {
                    foreach (var order in NewOrders)
                    {
                        var document = Receive<DocumentOrder>(rel.partnerIln, rel.documentType, order.trackingId, rel.documentStandard, "").First();

                        if (!docs.Any(x => document.OrderHeader.OrderNumber == x.ORDER_NUMBER))
                        {
                            count++;
                            commands.Add(new OracleCommand()
                            {
                                Parameters = {
                                    new OracleParameter("P_SENDER_ILN", OracleDbType.NVarChar, document?.DocumentParties?.Sender?.ILN ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_EXPECTED_DELIVERY_DATE", OracleDbType.NVarChar, (document?.OrderHeader?.ExpectedDeliveryDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                                    new OracleParameter("P_DELIVERY_POINT_ILN", OracleDbType.NVarChar, document?.OrderParties.DeliveryPoint?.ILN ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_BUYER_ILN", OracleDbType.NVarChar, document?.OrderParties?.Buyer?.ILN ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_CUSTOMER_ILN", OracleDbType.NVarChar, document?.OrderParties?.UltimateCustomer?.ILN ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_SELLER_ILN", OracleDbType.NVarChar, document?.DocumentParties?.Receiver.ILN ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_ORDER_CURRENCY", OracleDbType.NVarChar, document?.OrderHeader?.OrderCurrency ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_ORDER_DATE", OracleDbType.NVarChar, (document?.OrderHeader?.OrderDate ?? DateTime.Now).ToShortDateString(), ParameterDirection.Input),
                                    new OracleParameter("P_ORDER_NUMBER", OracleDbType.NVarChar, document?.OrderHeader?.OrderNumber ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_REMARKS", OracleDbType.NVarChar, document?.OrderHeader?.Remarks ?? "", ParameterDirection.Input),
                                    new OracleParameter("P_TOTAL_GROSS_AMOUNT", OracleDbType.NVarChar, document?.OrderSummary?.TotalGrossAmount ?? "", ParameterDirection.Input)
                                },
                                CommandType = CommandType.StoredProcedure,
                                CommandText = "EDI.Edi_MANAGER.ADD_ORDER"
                            });
                            if (document.OrderLines.Lines.Count > 0)
                                foreach (var line in document.OrderLines.Lines)
                                {
                                    commands.Add(new OracleCommand()
                                    {
                                        Parameters =
                                        {
                                            new OracleParameter("P_Edi_DOC_NUMBER", OracleDbType.NVarChar, document.OrderHeader.OrderNumber, ParameterDirection.Input),
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
                                        CommandType = CommandType.StoredProcedure,
                                        CommandText = "EDI.Edi_MANAGER.ADD_ORDER_DETAIL"
                                    });
                                }
                        }

                    }

                }

                try
                {
                    ExecuteCommand(commands);
                    commands.Clear();
                    RaiseAllProps();
                }
                catch (Exception ex)
                {
                    Error(ex);
                }

            }
            return count;
        }

        internal void SendSelectedOrderResponse(Document doc)
        {
            //Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            // преобразование выбранного документа в xml-desadv
            DocumentOrderResponse order = DocumentToXmlOrderResponse(doc);

            if (order == null) { Error("При отправке ответа на заказ: заказ = null"); return; }
            if (order.DocumentParties == null) { Error("При отправке ответа на заказ: отсутсвуют части документа(DocumentParties)"); return; }
            if (order.DocumentParties?.Receiver == null) { Error("При отправке ответа на заказ: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(order.DocumentParties.Receiver.ILN)) { Error("При отправке ответа на заказ: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Error("При отправке ответа на заказ: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (order.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }

            var sendOrder = XmlService<DocumentOrderResponse>.Serialize(order);
            Send(SelectedRelationship.partnerIln, "ORDRSP", "", "", "T", "", sendOrder, 20);
            ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.Number, doc.ID, ParameterDirection.Input)
                        },
                Connection = Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "EDI.EDI_MANAGER.MAKE_ORDRSP"
            });
        }

        internal void SendSelectedDespatchAdvice(Document doc)
        {
            DocumentDespatchAdvice advice = DocumentToXmlDespatchAdvice(doc);

            if (advice == null) { Error("При отправке уведомления об отгрузке: не выбран заказ"); return; }
            if (advice.DocumentParties == null) { Error("При отправке уведомления об отгрузке: отсутсвуют части документа(DocumentParties)"); return; }
            if (advice.DocumentParties?.Receiver == null) { Error("При отправке уведомления об отгрузке: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(advice.DocumentParties.Receiver.ILN)) { Error("При отправке уведомления об отгрузке: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Error("При отправке уведомления об отгрузке: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (advice.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }


            Send(SelectedRelationship?.partnerIln, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
            ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.NVarChar, doc.ID, ParameterDirection.Input)
                        },
                Connection = Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "EDI.EDI_MANAGER.MAKE_DESADV"
            });
        }

        private int SendOrderResponses()
        {
            List<Document> docs; // словить с базы все документы подходящие под отправку
            var sql = "SELECT * FROM EDI.EDI_GET_ORDERS WHERE ORDRSP IS NULL " +
    "AND DESADV IS NULL " +
    "AND ID_TRADER IS NOT NULL " +
    "AND ACT_STATUS >= 3 " +
    "AND CONTRACTOR_MANE IS NOT NULL ";//да-да, это ошибка: MANE->NAME
            //кому какое дело? я тут разработчик!

            docs = DocumentSelect<Document>(sql);
            foreach (var doc in docs)
            {
                doc.Details = GetDocumentDetails(doc.ID) ?? new List<Detail>();
                DocumentOrderResponse order = DocumentToXmlOrderResponse(doc);
                Send(doc.SENDER_ILN, "ORDRSP", "", "", "T", "", XmlService<DocumentOrderResponse>.Serialize(order));
                ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.Number, doc.ID, ParameterDirection.Input)
                        },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "EDI.EDI_MANAGER.MAKE_ORDRSP"
                });
            }
            return docs.Count;
        }

        private int SendDespatchAdvice()
        {
            List<Document> docs;

            var sql = "SELECT * FROM EDI.EDI_GET_ORDERS WHERE ORDRSP IS NOT NULL " +
"AND DESADV IS NULL " +
"AND ID_TRADER IS NOT NULL " +
"AND ACT_STATUS >= 4 " +
"AND CONTRACTOR_MANE IS NOT NULL ";//да-да, это ошибка: MANE->NAME
            //кому какое дело? я тут разработчик!

            docs = DocumentSelect<Document>(sql);
            if (docs != null)
                if (docs.Count > 0)
                    foreach (var doc in docs)
                        doc.Details = GetDocumentDetails(doc.ID) ?? new List<Detail>();

            foreach (var doc in docs)
            {
                doc.Details = GetDocumentDetails(doc.ID) ?? new List<Detail>();
                DocumentDespatchAdvice advice = DocumentToXmlDespatchAdvice(doc);
                Send(doc.SENDER_ILN, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
                ExecuteCommand(new OracleCommand()
                {
                    Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.Number, doc.ID, ParameterDirection.Input)
                        },
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "EDI.EDI_MANAGER.MAKE_DESADV"
                });
            }
            return docs.Count;
        }



        private string ToEdiDateString(DateTime date) => $"{date.Year}-{date.Month}-{date.Day}";

        private void RaiseAllProps()
        {
            foreach (var prop in typeof(MainViewModel).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                NotifyPropertyChanged(prop.Name);
            }
        }








        public AppConfig AppConfig { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public PriceTypesView _PriceTypesView { get; set; }
        public MatchMakerView _MatchMakerView { get; set; }
        public ContractorsMatchView _ContractorsMatchView { get; set; }
        private DateTime dateTo;
        private DateTime dateFrom;
        private Document selectedDocument;
        private List<Document> documents;
        private string _Time = "0";

        public string Time
        {
            get { return _Time; }
            set
            {
                if (value != _Time)
                {
                    _Time = value;
                    RaiseAllProps();
                }
            }
        }

        public string LoadedDocsCount => Documents.Count().ToString();

        public List<Document> Documents
        {
            get { return documents ?? new List<Document>(); }
            set
            {
                documents = value;
                RaiseAllProps();
            }
        }

        public Document SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                selectedDocument = value;
                RaiseAllProps();
            }
        }

        public DateTime DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                RaiseAllProps();
            }
        }

        public DateTime DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                RaiseAllProps();
            }
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}
