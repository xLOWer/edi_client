using EdiClient.View;
using EdiClient.Services;
using System.ComponentModel;

namespace EdiClient.ViewModel.Common
{
    class MainViewModel : INotifyPropertyChanged
    {
        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public CommandService OpenConnectionSettingsCommand => new CommandService( OpenConnectionSettings );
        public CommandService OpenEdiSettingsCommand => new CommandService( OpenEdiSettings );
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            TabService.NewTab( typeof( OrdersListView ), "Заказы поставщика (ORDERS)" );
            //TabService.NewTab( typeof( OrdrspListView ), "Ответы на заказы поставщика (ORDRSP)" );
            //TabService.NewTab( typeof( DesadvListView ), "Уведомление об отгрузке (DESADV)" );
            //TabService.NewTab( typeof( RecadvListView ), "Уведомление о приёмке (RECADV)" );
            TabService.NewTab( typeof( MatchMakerView ), "Связи товаров" );
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
    }
}
