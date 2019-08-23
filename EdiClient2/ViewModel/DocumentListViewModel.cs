using EdiClient.Model;
using EdiClient.Services;
using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.ViewModel
{
    public class DocumentListViewModel : INotifyPropertyChanged
    {
        public DocumentListViewModel()
        {
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today.AddDays(1);
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
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
        
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }



        public CommandService ToTraderCommand => new CommandService((o) => ActionInTime(()
            => {
                DocumentRepository.CreateTraderDocument(SelectedDocument.ID);
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            }));


        public CommandService SendORDRSPCommand => new CommandService((o) => ActionInTime(()
            => {
                DocumentRepository.SendOrdrsp(SelectedDocument);
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            }));


        public CommandService SendDESADVCommand => new CommandService((o) => ActionInTime(()
            => {
                DocumentRepository.SendDesadv(SelectedDocument);
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            }));


        public CommandService GetDocumentsCommand => new CommandService((o) => ActionInTime(()
            => {
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            }));


        public CommandService GetEDIDOCCommand => new CommandService((o) => ActionInTime(()
            => {
                //DocumentRepository.GetRecadv();
                DocumentRepository.GetNewOrders(dateFrom, dateTo);
                Documents = DocumentRepository.GetDocuments(DateFrom, DateTo);
            }));


        public CommandService NextDayCommand => new CommandService((o) =>
        {
            DateFrom = DateFrom.AddDays(1);
            DateTo = DateTo.AddDays(1);
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
        });

        public CommandService PrevDayCommand => new CommandService((o) =>
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
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
                Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
            }
            NotifyPropertyChanged("Documents");
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
            NotifyPropertyChanged("SelectedDocument");
            NotifyStaticPropertyChanged("tasks");
            NotifyStaticPropertyChanged("RunnedCount");
            NotifyStaticPropertyChanged("tasksCount");
            NotifyStaticPropertyChanged("RanToCompletionCount");
        }

    }
}
