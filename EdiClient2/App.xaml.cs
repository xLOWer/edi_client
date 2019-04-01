using System;
using System.Reflection;
using System.Windows;
using EdiClient.AppSettings;
using EdiClient.Services;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LogService.Log($" START APP {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 5);

            AppConfig.Load();
            EdiService.Configure();
            try
            {
                OracleConnectionService.Configure();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message+"\n\n"+ex.Source + "\n\n" + ex.StackTrace + "\n\n" + ex.TargetSite); }

        }

        ~App()
        {
            LogService.Log($" CLOSE APP {MethodBase.GetCurrentMethod().DeclaringType} {MethodBase.GetCurrentMethod().Name}", 5);
        }
    }
}