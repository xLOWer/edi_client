using EdiClient.Model;
using EdiClient.Services;
using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;

namespace EdiClient.ViewModel
{
    public class DocumentListViewModel : INotifyPropertyChanged
    {
        public DocumentListViewModel()
        {
            EdiClient.Services.Utilites.Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);
        }

        private string time;
        private DateTime dateTo;
        private DateTime dateFrom;
        private Document selectedDocument;
        private List<Document> documents;

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
        public CommandService GetDocumentsCommand => new CommandService(GetDocuments);
        public CommandService PrevDayCommand => new CommandService(PrevDay);
        public CommandService GetEDIDOCCommand => new CommandService(GetEDIDOC);
        public CommandService SendORDRSPCommand => new CommandService(SendORDRSP);
        public CommandService SendDESADVCommand => new CommandService(SendDESADV);
        public CommandService ToTraderCommand => new CommandService(ToTrader);

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void ToTrader(object o = null) => ActionInTime(()
            => { DocumentRepository.CreateTraderDocument(SelectedDocument.ID); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void SendORDRSP(object o = null) => ActionInTime(()
            => { DocumentRepository.SendOrdrsp(SelectedDocument); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void SendDESADV(object o = null) => ActionInTime(()
            => { DocumentRepository.SendDesadv(SelectedDocument); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void GetDocuments(object o = null) => ActionInTime(()
            => { Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void GetEDIDOC(object o = null) => ActionInTime(()
            => {
                //DocumentRepository.GetRecadv();
                DocumentRepository.GetNewOrders(dateFrom, dateTo);
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            });

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public void ActionInTime(Action act)
        {
            Utilites.Logger.Log($"[DOC] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name} => {act.Method.Name}");
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
                Utilites.Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
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
        }

        public void PrevDay(object o = null)
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
        }
    }
}
