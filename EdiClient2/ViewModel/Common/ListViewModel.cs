using EdiClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EdiClient.ViewModel.Common
{
    public class ListViewModel<TModel>: INotifyPropertyChanged
    {
        public virtual bool IsButtonEnabled { get; }

        public string ImageBack => @"~\..\..\Images\img_back.png"; // путь относительно текущего файла (в данном случае ListViewModel.cs)
        public string ImageForward => @"~\..\..\Images\img_forward.png";
        public string ImageRefresh => @"~\..\..\Images\img_refresh.png";
        public string ImageSendToDb => @"~\..\..\Images\img_create_doc.png";
        public string ImageSendToEdi => @"~\..\..\Images\img_to_edi.png";
        public string ImageSaveXml => @"~\..\..\Images\img_save_xml.png";

        public ListViewModel()
        {
            try
            {
                DateFrom = DateTime.Today;
                DateTo = DateTime.Today.AddDays(1);
                //Refresh(); // сделать обновление при загрузке всех вкладок. может сильно просаживать производительность при загрузке
            }
            catch (Exception ex)
            {
                Utilites.Error(ex);
            }
        }

        private bool isCanRefresh { get; set; } = true;
        private string time;
        private DateTime dateTo;
        private DateTime dateFrom;
        private TModel selectedItem;
        private List<TModel> documents;

        public bool IsCanRefresh
        {
            get { return isCanRefresh; }
            set
            {
                isCanRefresh = value;
                NotifyPropertyChanged("IsCanRefresh");
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
        public List<TModel> Documents
        {
            get { return documents; }
            set
            {
                documents = value;
                NotifyPropertyChanged("Documents");
            }
        }
        public TModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
                NotifyPropertyChanged("IsButtonEnabled");
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
        public CommandService RefreshCommand => new CommandService(Refresh);
        public CommandService PrevDayCommand => new CommandService(PrevDay);
        public CommandService GetORDERSCommand => new CommandService(GetORDERS);
        public CommandService SendORDRSPCommand => new CommandService(SendORDRSP);
        public CommandService SendDESADVCommand => new CommandService(SendDESADV);
        public CommandService GetDESADVCommand => new CommandService(GetDESADV);
        public CommandService CreateTraderDocumentCommand => new CommandService(CreateTraderDocument);

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void CreateTraderDocument(object o = null)
        {

        }
        public virtual void GetORDERS(object o = null)
        {

        }
        public virtual void SendORDRSP(object o = null)
        {

        }
        public virtual void SendDESADV(object o = null)
        {

        }
        public virtual void GetDESADV(object o = null)
        {

        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public virtual void Refresh(object o = null)
        {
            Task.Factory.StartNew(() => {
                Refresh();
            });
        }

        public virtual void UpdateView()
        {
            NotifyPropertyChanged("Documents");
            NotifyPropertyChanged("DateFrom");
            NotifyPropertyChanged("DateTo");
            NotifyPropertyChanged("SelectedItem");
        }

        public virtual void Refresh()
        {
            Task.Factory.StartNew(() => UpdateView());
        }

        public void NextDay(object o = null)
        {
            DateFrom = DateFrom.AddDays(1);
            DateTo = DateTo.AddDays(1);
            ((DatePicker)o).UpdateLayout();
            UpdateView();
        }

        public void PrevDay(object o = null)
        {
            DateFrom = DateFrom.AddDays(-1);
            DateTo = DateTo.AddDays(-1);
            ((DatePicker)o).UpdateLayout();
            UpdateView();
        }
    }
}
