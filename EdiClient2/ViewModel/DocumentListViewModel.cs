using EdiClient.Model;
using EdiClient.Services;
using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace EdiClient.ViewModel
{
    public class DocumentListViewModel : INotifyPropertyChanged
    {
        public DocumentListViewModel()
        {
            LogService.Log($"[DOC] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);
        }

        private ListCollectionView groupDocs { get; set; }
        private string time;
        private DateTime dateTo;
        private DateTime dateFrom;
        private Document selectedDocument;
        private List<Document> documents;


        public ListCollectionView GroupDocs
        {
            get { return groupDocs; }
            set
            {
                groupDocs = value;
                NotifyPropertyChanged("GroupDocs");
            }
        }

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }
        public List<Document> Documents
        {
            get { return documents ?? new List<Document>(); }
            set
            {
                documents = value;
                NotifyPropertyChanged("Documents");
            }
        }
        public Document SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                selectedDocument = value;
                NotifyPropertyChanged("SelectedDocument");
            }
        }
        public DateTime DateFrom
        {
            get { return dateFrom; }
            set
            {
                dateFrom = value;
                NotifyPropertyChanged("DateFrom");
            }
        }
        public DateTime DateTo
        {
            get { return dateTo; }
            set
            {
                dateTo = value;
                NotifyPropertyChanged("DateTo");
            }
        }

        public CommandService NextDayCommand => new CommandService(NextDay);
        public CommandService PrevDayCommand => new CommandService(PrevDay);

        public CommandService GetDocumentsCommand => new CommandService(GetDocuments);

        public CommandService GetEDIDOCCommand => new CommandService(GetEDIDOC);
        public CommandService SendORDRSPCommand => new CommandService(SendORDRSP);
        public CommandService SendDESADVCommand => new CommandService(SendDESADV);
        public CommandService ToTraderCommand => new CommandService(ToTrader);

        public CommandService GroupDocumentTypeCommand => new CommandService(GroupDocumentType);
        public CommandService GroupContractorCommand => new CommandService(GroupContractor);
        public CommandService GroupContractorDocumentTypeCommand => new CommandService(GroupContractorDocumentType);
        public CommandService GroupDocumentTypeContractorCommand => new CommandService(GroupDocumentTypeContractor);
        public CommandService GroupClearCommand => new CommandService(GroupClear);

        private void GroupDocumentType(object o = null)
        {
            GroupClear();
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("DocumentType"));
        }

        private void GroupContractor(object o = null)
        {
            GroupClear();
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("CONTRACTOR_MANE"));
        }

        private void GroupDocumentTypeContractor(object o = null)
        {
            GroupClear();
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("DocumentType"));
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("CONTRACTOR_MANE"));
        }

        private void GroupContractorDocumentType(object o = null)
        {
            GroupClear();
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("CONTRACTOR_MANE"));
            GroupDocs.GroupDescriptions.Add(new PropertyGroupDescription("DocumentType"));
        }

        private void GroupClear(object o = null)
        {
            GroupDocs.GroupDescriptions.Clear();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void ToTrader(object o = null) => ActionInTime(() => 
        {
            DocumentRepository.CreateTraderDocument(SelectedDocument.ID);
            GetEDIDOC();
        });

        public void SendORDRSP(object o = null) => ActionInTime(() => 
        {
            DocumentRepository.SendOrdrsp(SelectedDocument);
            GetEDIDOC();
        });

        public void SendDESADV(object o = null) => ActionInTime(() => 
        {
            DocumentRepository.SendDesadv(SelectedDocument);
            GetEDIDOC();
        });

        public void GetDocuments(object o = null) => ActionInTime(() => 
        {
            GetEDIDOC();
        });
        public void GetEDIDOC(object o = null) => ActionInTime(() => 
        {
            //DocumentRepository.GetRecadv();
            Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            GroupDocs = new ListCollectionView(Documents);
        });

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        
        public void ActionInTime(Action act)
        {
            LogService.Log($"[DOC] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} => {act.Method.Name}");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                watch.Stop();
                Utilites.Error(ex);
            }
            finally
            {
                watch.Stop();
                Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
                Utilites.Time = Time;
            }
            NotifyPropertyChanged("Documents");
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
            NotifyPropertyChanged("SelectedDocument");
        }
        
        public void NextDay(object o = null)
        {
            DateFrom = DateFrom.AddDays(1);
            DateTo = DateTo.AddDays(1);
            ((DatePicker)o).UpdateLayout();
        }

        public void PrevDay(object o = null)
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
            ((DatePicker)o).UpdateLayout();
        }
    }
}
