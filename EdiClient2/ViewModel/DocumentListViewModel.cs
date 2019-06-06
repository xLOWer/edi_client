using EdiClient.Model;
using EdiClient.Services;
using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EdiClient.ViewModel
{
    public class DocumentListViewModel : INotifyPropertyChanged
    {
        public string ImageBack => @"~\..\..\Images\prev-day.png"; // путь относительно текущего файла (в данном случае ListViewModel.cs)
        public string ImageForward => @"~\..\..\Images\next-day.png";
        public string ImageRefresh => @"~\..\..\Images\refresh-50.png";
        public string ImageGetOrders => @"~\..\..\Images\get-orders-50.png";
        public string ImageToTrader => @"~\..\..\Images\to-trader-50.png";
        public string ImageSendOrdrsp => @"~\..\..\Images\send-ordrsp-50.png";
        public string ImageSendDesadv => @"~\..\..\Images\send-desadv-50.png";


        public DocumentListViewModel()
        {
            try
            {
                DateFrom = DateTime.Today;
                DateTo = DateTime.Today.AddDays(1);
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
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
        public CommandService GetORDERSCommand => new CommandService(GetORDERS);
        public CommandService SendORDRSPCommand => new CommandService(SendORDRSP);
        public CommandService SendDESADVCommand => new CommandService(SendDESADV);
        public CommandService GetRECADVCommand => new CommandService(GetRECADV);
        public CommandService ToTraderCommand => new CommandService(ToTrader);

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void ToTrader(object o = null) => ActionInTime(() => { DocumentRepository.CreateTraderDocument(SelectedDocument.ORDER_NUMBER); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void GetORDERS(object o = null) => ActionInTime(() => { DocumentRepository.GetNewOrders(dateFrom, dateTo); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void SendORDRSP(object o = null) => ActionInTime(() => { DocumentRepository.SendOrdrsp(SelectedDocument); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void SendDESADV(object o = null) => ActionInTime(() => { DocumentRepository.SendDesadv(SelectedDocument); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void GetRECADV(object o = null) => ActionInTime(() => { DocumentRepository.GetRecadv(); Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        public void GetDocuments(object o = null) => ActionInTime(() => {Documents = DocumentRepository.GetDocuments(DateFrom, DateTo); });
        
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        
        public void ActionInTime(Action act)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
            finally
            {
                watch.Stop();
                Time = ((double)(((double)watch.ElapsedMilliseconds) / 1000)).ToString() + " сек";
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
