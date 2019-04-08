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

        public string ImageBack => @"~\..\..\Images\img_back.png"; // путь, относительно текущего файла
        public string ImageForward => @"~\..\..\Images\img_forward.png";
        public string ImageRefresh => @"~\..\..\Images\img_refresh.png";
        public string ImageSendToDb => @"~\..\..\Images\img_create_doc.png";
        public string ImageSendToEdi => @"~\..\..\Images\img_to_edi.png";
        public string ImageSaveXml => @"~\..\..\Images\img_save_xml.png";

        public ListViewModel()
        {
            try
            {
                DateFrom = DateTime.Parse("01.01.2019");
                DateTo = DateTime.Today.AddDays(1);
                //Refresh();
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
        public CommandService ToEdiCommand => new CommandService(ToEdi);
        public CommandService SaveToXmlCommand => new CommandService(SaveToXml);
        public CommandService CreateTraderDocumentCommand => new CommandService(CreateTraderDocument);

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void CreateTraderDocument(object o = null)
        {
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().Name} {typeof(TModel).ToString()}", 2);
        }
        public virtual void ToEdi(object o = null)
        {
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().Name} {typeof(TModel).ToString()}", 2);
        }
        public virtual void SaveToXml(object o = null)
        {
            //LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().Name} {typeof(TModel).ToString()}", 2);
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
