using EdiClient.View;
using EdiClient.Services;
using System.ComponentModel;
using EdiClient.AppSettings;
using System.Windows.Controls;
using System.Linq;
using System;

namespace EdiClient.ViewModel.Common
{
    class MainViewModel : INotifyPropertyChanged
    {
        public string ImageRefresh => @"~\..\Images\img_refresh.png";
        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public CommandService OpenConnectionSettingsCommand => new CommandService( OpenConnectionSettings );
        public CommandService OpenEdiSettingsCommand => new CommandService( OpenEdiSettings );
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandService RefreshRelationshipsCommand => new CommandService(RefreshRelationships);

        public MainViewModel()
        {
            TabService.NewTab( typeof( OrdersListView ), "Заказы поставщика (ORDERS)" );
            TabService.NewTab( typeof( OrdrspListView ), "Ответы на заказы поставщика (ORDRSP)" );
            TabService.NewTab( typeof( DesadvListView ), "Уведомление об отгрузке (DESADV)" );
            TabService.NewTab( typeof( RecadvListView ), "Уведомление о приёмке (RECADV)" );
            TabService.NewTab( typeof( MatchMakerView ), "Связи товаров" );
            TabService.NewTab(typeof(PriceTypesView), "Связи цен");
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
            if (!string.IsNullOrEmpty(AppConfig.Edi_Password) && !string.IsNullOrEmpty(AppConfig.Edi_GLN) && !string.IsNullOrEmpty(AppConfig.Edi_User))
                EdiService.UpdateData();

            (o as ComboBox).ItemsSource = EdiService.Relationships;//.Where( x => !x.Name.ToUpper().Contains("тестовый".ToUpper()) );
        }
    }
}
