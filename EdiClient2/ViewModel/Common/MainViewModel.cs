using EdiClient.View;
using EdiClient.Services;
using System.ComponentModel;
using EdiClient.AppSettings;
using System;
using DevExpress.Xpf.Editors;

namespace EdiClient.ViewModel.Common
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CommandService OpenCommonSettingsCommand => new CommandService( OpenCommonSettings );
        public CommandService RefreshRelationshipsCommand => new CommandService(RefreshRelationships);
        public CommandService MatcherPriceOpenCommand => new CommandService(MatcherPriceOpen);
        public CommandService MatcherGoodsOpenCommand => new CommandService(MatcherGoodsOpen);
        public CommandService MatcherDeliveryPointsOpenCommand => new CommandService(MatcherDeliveryPointsOpen);
        public CommandService DocumentsOpenCommand => new CommandService(DocumentsOpen);
        
        private bool _IsNightTheme;
        public bool IsNightTheme
        {
            get => _IsNightTheme;
            set
            {
                _IsNightTheme = value;
                if (value)
                    AppConfig.ThemeName = "MetropolisDark";
                else
                    AppConfig.ThemeName = "MetropolisLight";                
            }
        }

        public void DocumentsOpen(object o = null)
        {
            try
            {
                TabService.NewTab(new DocumentPage(), "Документы");
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public void MatcherPriceOpen(object o = null)
        {
            try
            {
                TabService.NewTab(new PriceTypesView(), "Связи цен");
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public void MatcherGoodsOpen(object o = null)
        {
            try
            {
                TabService.NewTab(new MatchMakerView(), "Связи товаров");
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public void MatcherDeliveryPointsOpen(object o = null)
        {
            try
            {
                TabService.NewTab(new ContractorsMatchView(), "Связи точек доставки");
            }
            catch (Exception ex) { Utilites.Error(ex); }
        }

        public MainViewModel()
        {
            try
            {
                TabService.NewTab(new DocumentPage(), "Документы");
            }
            catch(Exception ex) { Utilites.Error(ex); }
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
            EdiClient.Services.Utilites.Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            if (!string.IsNullOrEmpty(AppConfig.EdiPassword) && !string.IsNullOrEmpty(AppConfig.EdiGLN) && !string.IsNullOrEmpty(AppConfig.EdiUser))
                EdiService.UpdateData();

            (o as ComboBoxEdit).ItemsSource = EdiService.Relationships;
        }
    }
}
