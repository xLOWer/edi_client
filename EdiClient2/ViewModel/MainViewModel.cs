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

namespace EdiClient.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
      
        public CommandService EditValueChangedCommand => new CommandService(o =>
        {
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            EdiService.SelectedRelationship = cmb.SelectedItem as Relation;
        });

        public CommandService RefreshRelationshipsCommand => new CommandService(o => 
        {
            EdiService.UpdateData();
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            cmb.ItemsSource = EdiService.Relationships;
            RaiseAllProps();
        });
        public CommandService SaveConfigCommand => new CommandService(o => 
        {
            Logger.Log($"[CONFIG SAVED]");
            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
            RaiseAllProps();
        });

        public MainViewModel()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);
            RaiseAllProps();
        }

        public PriceTypesView _PriceTypesView { get; set; }
        public MatchMakerView _MatchMakerView { get; set; }
        public ContractorsMatchView _ContractorsMatchView { get; set; }

        public CommandService OpenPriceTypesViewCommand => new CommandService( o  =>
        {
            try
            {
                _PriceTypesView = new PriceTypesView();
                _PriceTypesView.Activate();
                _PriceTypesView.Show();
            }
            catch (Exception ex) { Error(ex); }
        });

        public CommandService OpenMatchMakerViewCommand => new CommandService(o =>
        {
            try
            {
                _MatchMakerView = new MatchMakerView();
                _MatchMakerView.Activate();
                _MatchMakerView.Show();
            }
            catch (Exception ex) { Error(ex); }
        });

        public CommandService OpenContractorsMatchViewCommand => new CommandService(o =>
        {
            try
            {
                _ContractorsMatchView = new ContractorsMatchView();
                _ContractorsMatchView.Activate();
                _ContractorsMatchView.Show();
            }
            catch (Exception ex) { Error(ex); }
        });





        private DateTime dateTo;
        private DateTime dateFrom;
        private Document selectedDocument;
        private List<Document> documents;
        private string _Time = "0";

        public string RunnedCount => tasks?.Where(x => x.Status == TaskStatus.Running)?.Count().ToString() ?? "0";
        public string tasksCount => tasks?.Count().ToString() ?? "0";
        public string RanToCompletionCount => tasks?.Where(x => x.Status == TaskStatus.RanToCompletion)?.Count().ToString() ?? "0";

        public Task[] tasks { get; set; }

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

        public string LoadedDocsCount { get; set; } = "0";


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



        public CommandService ToTraderCommand => new CommandService((o) => ActionInTime(()
            => {
                CreateTraderDocument(SelectedDocument.ID);
                Documents = GetDocuments(DateFrom, DateTo);
            }));


        public CommandService SendORDRSPCommand => new CommandService((o) => ActionInTime(()
            => {
                SendOrdrsp(SelectedDocument);
                Documents = GetDocuments(DateFrom, DateTo);
            }));


        public CommandService SendDESADVCommand => new CommandService((o) => ActionInTime(()
            => {
                SendDesadv(SelectedDocument);
                Documents = GetDocuments(DateFrom, DateTo);
            }));


        public CommandService GetDocumentsCommand => new CommandService((o) => ActionInTime(()
            => {
                Documents = GetDocuments(DateFrom, DateTo);
            }));


        public CommandService GetEDIDOCCommand => new CommandService((o) => ActionInTime(()
            => {
                //GetRecadv();
                GetNewOrders(dateFrom, dateTo);
                Documents = GetDocuments(DateFrom, DateTo);
            }));


        public CommandService NextDayCommand => new CommandService((o) =>
        {
            DateFrom = DateFrom.AddDays(1);
            DateTo = DateTo.AddDays(1);
            RaiseAllProps();
        });

        public CommandService PrevDayCommand => new CommandService((o) =>
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
            RaiseAllProps();
        });

        public void ActionInTime(Action act)
        {
            Logger.Log($"[DOC] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} => {act.Method.Name}");
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

        private void RaiseAllProps()
        {
            foreach(var prop in typeof(MainViewModel).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                NotifyPropertyChanged(prop.Name);
            }
        }

        public void GetNewOrders(DateTime dateFrom, DateTime dateTo)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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

                tasks = new Task[NewOrders.Count()];
                int i = 0;
                foreach (var order in NewOrders)
                {
                    tasks[i] = new Task(() =>
                    {
                        var document = Receive<DocumentOrder>(SelectedRelationship.partnerIln,
                                                                            SelectedRelationship.documentType,
                                                                            order.trackingId,
                                                                            SelectedRelationship.documentStandard, "").First();
                        InsertIncomingIntoDatabase(document);
                        RaiseAllProps();
                    });

                    tasks[i++].Start();
                }

            }
        }


        internal List<Document> GetDocuments(DateTime dateFrom, DateTime dateTo)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
            LoadedDocsCount = result.Count.ToString();
            NotifyPropertyChanged("LoadedDocsCount");
            return result;
        }

        private string ToEdiDateString(DateTime date) => $"{date.Year}-{date.Month}-{date.Day}";
        public void UpdateFailedDetails(string P_EDI_DOC_ID)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Logger.Log($"\t\tEDI_REFRESH_DOC_DETAILS.P_EDI_DOC_ID=" + P_EDI_DOC_ID);
            DbService.ExecuteCommand(new OracleCommand()
            {
                Connection = DbService.Connection.conn,
                Parameters = { new OracleParameter("P_EDI_DOC_ID", OracleDbType.NVarChar, P_EDI_DOC_ID, ParameterDirection.Input) },
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema + ".") + "EDI_REFRESH_DOC_DETAILS"
            });
        }

        /// <summary>
        /// Создать реальный документ для работы в trader
        /// </summary>
        /// <param name="orderNumber">номер заказа (не его ID в базе!)</param>
        internal void CreateTraderDocument(string orderID)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            Logger.Log($"\t\tEDI_MOVE_ORDER.P_ID=" + orderID);
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
        private void InsertIncomingIntoDatabase(DocumentOrder order)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
        private List<OracleCommand> XmlOrdersToDatabase(DocumentOrder order)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
                CommandText = (AppConfig.Schema + ".") + "Edi_ADD_ORDER"
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
                        CommandText = (AppConfig.Schema + ".") + "Edi_ADD_ORDER_DETAIL"
                    });
                }

            return commands;
        }

        internal DocumentDespatchAdvice DocumentToXmlDespatchAdvice(Document doc)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
        internal void SendDesadv(Document doc)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            // преобразование выбранного документа в xml-desadv
            DocumentDespatchAdvice advice = DocumentToXmlDespatchAdvice(doc);

            if (advice == null) { Error("При отправке уведомления об отгрузке: не выбран заказ"); return; }
            if (advice.DocumentParties == null) { Error("При отправке уведомления об отгрузке: отсутсвуют части документа(DocumentParties)"); return; }
            if (advice.DocumentParties?.Receiver == null) { Error("При отправке уведомления об отгрузке: отсутствует отправитель"); return; }
            if (String.IsNullOrEmpty(advice.DocumentParties.Receiver.ILN)) { Error("При отправке уведомления об отгрузке: у отправителя отсутствует GLN"); return; }
            if (SelectedRelationship == null) { Error("При отправке уведомления об отгрузке: не выбран покупатель"); return; }
            if (SelectedRelationship.partnerIln == null) { Error("Невозможная ошибка: у покупателя отсутствует GLN (звоните в IT-отдел!)"); return; }
            if (advice.DocumentParties.Receiver.ILN != SelectedRelationship.partnerIln) { Error("Нельзя отправить документ другому покупателю! Выберите соответствующего документу покупателя и повторите отправку."); return; }


            EdiService.Send(SelectedRelationship?.partnerIln, "DESADV", "", "", "T", "", XmlService<DocumentDespatchAdvice>.Serialize(advice), 20);
            DbService.ExecuteCommand(new OracleCommand()
            {
                Parameters =
                        {
                            new OracleParameter("P_ID", OracleDbType.NVarChar, doc.ID, ParameterDirection.Input)
                        },
                Connection = Connection.conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = (AppConfig.Schema + ".") + "EDI_MAKE_DESADV"
            });
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
            catch (Exception ex) { Error(ex); }
            return ordrsp;
        }

        /// <summary>
        /// Отправить ответ на заказ в систему EDI
        /// </summary>
        /// <param name="order">отправляемый заказ</param>
        internal void SendOrdrsp(Document doc)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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

        internal List<Detail> GetDocumentDetails(string Id)
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            var sql = DbService.Sqls.GET_ORDER_DETAILS(Id);
            var result = DbService.DocumentSelect<Detail>(sql);
            return result;
        }


        internal List<DocumentReceivingAdvice> GetRecadv()
        {
            Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            return new List<DocumentReceivingAdvice>();
        }
    }
}
