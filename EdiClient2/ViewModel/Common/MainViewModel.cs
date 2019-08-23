using EdiClient.View;
using EdiClient.Services;
using static EdiClient.Services.Utils.Utilites;
using System.ComponentModel;
using EdiClient.AppSettings;
using System;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Settings;

namespace EdiClient.ViewModel.Common
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public CommandService RefreshRelationshipsCommand => new CommandService(RefreshRelationships);
        
        public MainViewModel()
        {
            try
            {
                TabService.NewTab(new DocumentPage(), "Документы");
                TabService.NewTab(new MatchMakerView(), "Связи товаров");
                TabService.NewTab(new PriceTypesView(), "Связи цен");
                TabService.NewTab(new ContractorsMatchView(), "Связи точек доставки");
            }
            catch(Exception ex) { Error(ex); }
        }

        public void OpenCommonSettings(object o)
        {
            CommonSettingsWindow setWin = new CommonSettingsWindow();
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
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            if (!string.IsNullOrEmpty(AppConfig.EdiPassword) && !string.IsNullOrEmpty(AppConfig.EdiGLN) && !string.IsNullOrEmpty(AppConfig.EdiUser))
                EdiService.UpdateData();
            (o as ComboBoxEdit).ItemsSource = EdiService.Relationships;
        }
    }
}
