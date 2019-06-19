using EdiClient.View;
using EdiClient.Services;
using System.ComponentModel;
using EdiClient.AppSettings;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Reflection;

namespace EdiClient.ViewModel.Common
{
    class MainViewModel : INotifyPropertyChanged
    {
        public string ImageRefresh => @"~\..\Images\refresh-50.png";
        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public CommandService OpenConnectionSettingsCommand => new CommandService( OpenConnectionSettings );
        public CommandService OpenEdiSettingsCommand => new CommandService( OpenEdiSettings );
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandService RefreshRelationshipsCommand => new CommandService(RefreshRelationships);

        public MainViewModel()
        {
            TabService.NewTab( typeof(DocumentPage  ), "Документы" );
            TabService.NewTab( typeof(MatchMakerView), "Связи товаров" );
            TabService.NewTab( typeof(PriceTypesView), "Связи цен");
        }

        public void OpenCommonSettings(object o)
        {
            CommonSettingsWindow setWin = new CommonSettingsWindow();
            setWin.Show();
            setWin.UpdateLayout();
        }

        public void OpenConnectionSettings(object o)
        {
            ConnectionSettingsWindow setWin = new ConnectionSettingsWindow();
            setWin.Show();
            setWin.UpdateLayout();
        }

        public void OpenEdiSettings(object o)
        {
            EdiSettingsWindow setWin = new EdiSettingsWindow();
            setWin.Show();
            setWin.UpdateLayout();
        }

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged( this, new PropertyChangedEventArgs( prop ) );
        }

        public void RefreshRelationships(object o)
        {
            LogService.Log($"[INFO] {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}");
            if (!string.IsNullOrEmpty(AppConfig.EdiPassword) && !string.IsNullOrEmpty(AppConfig.EdiGLN) && !string.IsNullOrEmpty(AppConfig.EdiUser))
                EdiService.UpdateData();

            (o as ComboBox).ItemsSource = EdiService.Relationships;
        }
    }
}
