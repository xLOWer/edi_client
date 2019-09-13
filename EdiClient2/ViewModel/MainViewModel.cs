﻿using EdiClient.Services;
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
using static System.Net.WebRequestMethods;
using System.Net;
using System.IO;
using System.Text;
using EdiClient.Model.WebModel;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Windows;

namespace EdiClient.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Command EditValueChangedCommand => new Command((o) =>
        {
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            EdiService.SelectedRelationship = cmb.SelectedItem as Relation;
        });

        public Command<BarEditItem> RefreshRelationshipsCommand => new Command<BarEditItem>((o) =>
        {
            UpdateData();
            ComboBoxEdit cmb = ((o as BarEditItem).Links[0] as BarEditItemLink).Editor as ComboBoxEdit;
            cmb.ItemsSource = EdiService.Relationships;
            RaiseAllProps();
        });

        public Command SaveConfigCommand => new Command((o) =>
        {
            //Logger.Log($"[CONFIG SAVED]");
            RaiseAllProps();
            AppConfigHandler.conf = AppConfig;
            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfig = AppConfigHandler.conf;
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
            RaiseAllProps();
        });


        public MainViewModel()
        {
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);
            AppConfig = AppConfigHandler.conf;
            RaiseAllProps();
        }

        public PriceTypesView _PriceTypesView { get; set; }
        public MatchMakerView _MatchMakerView { get; set; }
        public ContractorsMatchView _ContractorsMatchView { get; set; }

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

        public AppConfig AppConfig { get; set; }





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

        public string LoadedDocsCount =>  Documents.Count().ToString();


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


        public void LogDocument(string name, Document doc)
        {            
           Logger.Log("["+name+"] " +
               "id=" + doc.ID +
               "|number="+doc.ORDER_NUMBER +
               "|sender="+doc.SENDER_ILN +
               "|customer="+doc.CUSTOMER_ILN +
               "|deliveryp="+doc.DELIVERY_POINT_ILN +
               "|del_lines="+doc.Details_DeliveryLinesCount +
               "|ord_lines="+doc.Details_OrderedLinesCount +
               "|details="+doc.DetailsCount);
        } 


        public Command ToTraderCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("ToTraderCommand", SelectedDocument);
                CreateTraderDocument(SelectedDocument.ID);
                GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));


        public Command SendORDRSPCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("SendORDRSPCommand", SelectedDocument);
                SendOrdrsp(SelectedDocument);
                GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));
       


        public Command SendDESADVCommand => new Command((o) => ActionInTime(()
            => {
                if (SelectedDocument == null) return;
                LogDocument("SendDESADVCommand", SelectedDocument);
                SendDesadv(SelectedDocument);
                GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));


        public Command GetDocumentsCommand => new Command((o) => ActionInTime(()
            => {
                Logger.Log("[GetDocumentsCommand]count="+Documents.Count());
                GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));


        public Command GetEDIDOCCommand => new Command((o) => ActionInTime(()
            => {
                //GetRecadv();
                GetNewOrders(dateFrom, dateTo);
                //Documents = GetDocuments(DateFrom, DateTo);
                RaiseAllProps();
            }));


        public Command ReloadDocumentCommand => new Command((o) =>
        {
            if (SelectedDocument == null) return;
            LogDocument("ReloadDocumentCommand", SelectedDocument);
            var sql = "delete from EDI.EDI_DOC WHERE ID = "+SelectedDocument.ID;
            ExecuteLine(sql);
            GetNewOrders(dateFrom, dateTo);
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
            //Logger.Log($"{MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} => {act.Method.Name}");
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


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }


        [Flags]
        public enum DesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }
        [Flags]
        public enum ShareMode : uint
        {
            FILE_SHARE_NONE = 0x0,
            FILE_SHARE_READ = 0x1,
            FILE_SHARE_WRITE = 0x2,
            FILE_SHARE_DELETE = 0x4,

        }
        public enum CreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXSTING = 5
        }
        [Flags]
        public enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTES_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_HIDDEN = 0x2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 0x1,
            FILE_ATTRIBUTE_SYSTEM = 0x4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100,
            FILE_FLAG_WRITE_THROUGH = 0x80000000,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_NO_BUFFERING = 0x20000000,
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,
            FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
            FILE_FLAG_DELETE_ON = 0x4000000,
            FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
            FILE_FLAG_OPEN_NO_CALL = 0x100000
        }

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool ReadFile(SafeFileHandle hFile, Byte[] aBuffer, UInt32 cbToRead, UInt32 cbThatWereRead, IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes,
            CreationDisposition dwCreationDisposition, FlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport("kernel32", SetLastError = true)]
        internal static extern Int32 CloseHandle(SafeFileHandle hObject);


        public void GetNewOrders(DateTime dateFrom, DateTime dateTo)
        {








            if (SelectedRelationship == null) return;
            if (SelectedRelationship.partnerIln == null || SelectedRelationship.documentType == null) return;

            List<Model.WebModel.DocumentInfo> NewOrders
               = EdiService.ListMBEx(SelectedRelationship.partnerIln
                                     , SelectedRelationship.documentType, "", "", ""
                                     , ToEdiDateString(dateFrom)
                                     , ToEdiDateString(dateTo)
                                     , "", "", "");

            NewOrders = NewOrders.Where(x => x?.documentStatus != "Ошибка" || !string.IsNullOrEmpty(x.fileName)).ToList();















            if (SelectedRelationship == null) return;
            if (SelectedRelationship.partnerIln == null || SelectedRelationship.documentType == null) return;

            List<string> sqls = new List<string>();
            List<WIN32_FIND_DATA> files = new List<WIN32_FIND_DATA>();
            string dir = AppConfig.FtpDir+SelectedRelationship.partnerIln;
            byte[] aBuffer;
            DocumentOrder order;
            List<OracleCommand> commands = new List<OracleCommand>();
            WIN32_FIND_DATA w32file = new WIN32_FIND_DATA();
            SafeFileHandle hFile = null;
            DateTime w32fileCreateTime, minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;

            int frCount = 0, ftCount = 0, docaCount = 0, detaCount = 0, failCount = 0, ctCount = 0;
            uint btCount = 0;
            string fails = "";

            Documents.Clear();
            GetDocuments(dateFrom, dateTo);

            IntPtr h = FindFirstFile(dir+"\\*.*", out w32file);
            FindNextFile(h, out w32file); // пропускаем указатель на родительский каталог
            while (FindNextFile(h, out w32file))
            {
                ftCount++;
                w32fileCreateTime = DateTime.FromFileTime((((long)w32file.ftCreationTime.dwHighDateTime) << 32) | ((uint)w32file.ftCreationTime.dwLowDateTime));
                if (!(w32fileCreateTime > dateFrom.AddHours(-1) && w32fileCreateTime < dateTo.AddHours(1))) continue;
                // 1 - в скобках раньше, 0 - равно, -1 - позже
                if (minDate.CompareTo(w32fileCreateTime) >= 0) minDate = w32fileCreateTime;
                if (maxDate.CompareTo(w32fileCreateTime) <= 0) maxDate = w32fileCreateTime;
                files.Add(w32file);
            }
            FindClose(h);



            foreach (var w32f in files)
            {
                var name = w32f.cFileName;
                var size = w32f.nFileSizeLow;
                frCount++;
                hFile = CreateFile(dir+"\\"+name, DesiredAccess.GENERIC_READ, ShareMode.FILE_SHARE_READ, IntPtr.Zero, CreationDisposition.OPEN_EXISTING, 0, IntPtr.Zero);
                if (hFile.IsInvalid)
                {
                    int Err = Marshal.GetLastWin32Error();
                    failCount++;
                    fails += "\n#"+failCount+" : Win32Err#"+Err + ", file: " + name+"\n" + new System.ComponentModel.Win32Exception(Err).Message+"\n";
                    continue;
                }

                btCount += size;
                aBuffer = new byte[size];
                ReadFile(hFile, aBuffer, size, 0, IntPtr.Zero);
                hFile.Close();

                order = (DocumentOrder)new XmlSerializer(typeof(DocumentOrder))
                    .Deserialize(new StringReader(((XmlNode[])((Envelope)new XmlSerializer(typeof(Envelope))
                    .Deserialize(new StringReader(Encoding.UTF8.GetString(aBuffer)))).Body.receiveResponse.@return.cnt)
                    .First().OuterXml));

                if (!Documents.Any(x => order.OrderHeader.OrderNumber == x.ORDER_NUMBER))
                {
                    ctCount++;
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
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "EDI.Edi_MANAGER.ADD_ORDER"
                    });
                    docaCount++;
                    if (order.OrderLines.Lines.Count > 0)
                        foreach (var line in order.OrderLines.Lines)
                        {
                            detaCount++;
                            //Logger.Log($"[TODB] EAN={line?.LineItem?.EAN ?? ""}, UNIT_PRICE={line?.LineItem?.OrderedUnitGrossPrice ?? ""}, AMOUNT={line?.LineItem?.OrderedGrossAmount}, QUANTITY={line?.LineItem?.OrderedQuantity ?? "0"}");
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
                                CommandType = CommandType.StoredProcedure,
                                CommandText = "EDI.Edi_MANAGER.ADD_ORDER_DETAIL"
                            });
                        }

                }

            }
            ExecuteCommand(commands);
            GetDocuments(dateFrom, dateTo);

            commands.Clear();
            RaiseAllProps();

            Task.Factory.StartNew(() =>
            {
                string result = "[GetNewOrders]args|dateFrom="+dateFrom.ToShortDateString()+"|dateTo="+dateTo.ToShortDateString()+"|"+
"size="+Math.Round((double)btCount / 1024)+"Kb|"+
"read="+ftCount+"|handled="+frCount+"|orders="+ctCount+"|minDate="+minDate.ToShortDateString()+" "+minDate.ToLongTimeString()+"|"+
"maxDate=" + maxDate.ToShortDateString()+" " + maxDate.ToLongTimeString()+"doc=" + docaCount+"|detail="+detaCount+"\n"+fails;
                Logger.Log(result);
                //MessageBox.Show(result);
            });
        }


        internal void GetDocuments(DateTime dateFrom, DateTime dateTo)
        {
            //Logger.Log($"{MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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

        private string ToEdiDateString(DateTime date) => $"{date.Year}-{date.Month}-{date.Day}";

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

        /// <summary>
        /// Создать реальный документ для работы в trader
        /// </summary>
        /// <param name="orderNumber">номер заказа (не его ID в базе!)</param>
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

        /// <summary>
        /// Отправить извещение об отгрузке в систему EDI
        /// </summary>
        /// <param name="advice">отправляемый заказ</param>
        internal void SendDesadv(Document doc)
        {
            //Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
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
            EdiService.Send(SelectedRelationship.partnerIln, "ORDRSP", "", "", "T", "", sendOrder, 20);
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



        internal List<DocumentReceivingAdvice> GetRecadv()
        {
            //Logger.Log($"[DOCREP] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            return new List<DocumentReceivingAdvice>();
        }
    }
}
