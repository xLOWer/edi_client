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
        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandService RefreshRelationshipsCommand => new CommandService(RefreshRelationships);

        public MainViewModel()
        {
            try
            {
                EdiClient.Services.LogService.Log($"[INIT-DocumentPage] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                TabService.NewTab(typeof(DocumentPage), "Документы");
                EdiClient.Services.LogService.Log($"[INIT-MatchMakerView] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                TabService.NewTab(typeof(MatchMakerView), "Связи товаров");
                EdiClient.Services.LogService.Log($"[INIT-PriceTypesView] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                TabService.NewTab(typeof(PriceTypesView), "Связи цен");
                EdiClient.Services.LogService.Log($"[INIT-ContractorsMatchView] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                TabService.NewTab(typeof(ContractorsMatchView), "Связи точек доставки");
            }
            catch(Exception ex) { EdiClient.Services.Utilites.Error(ex); }
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
            EdiClient.Services.LogService.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            if (!string.IsNullOrEmpty(AppConfig.EdiPassword) && !string.IsNullOrEmpty(AppConfig.EdiGLN) && !string.IsNullOrEmpty(AppConfig.EdiUser))
                EdiService.UpdateData();

            (o as ComboBox).ItemsSource = EdiService.Relationships;
        }
    }
}
