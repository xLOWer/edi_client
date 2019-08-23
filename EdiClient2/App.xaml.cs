using System.Globalization;
using System.Threading;
using System.Windows;
using EdiClient.AppSettings;
using EdiClient.Services;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-RU");

            // The following line provides localization for the application's user interface.  
            Thread.CurrentThread.CurrentUICulture = culture;

            // The following line provides localization for data formats.  
            Thread.CurrentThread.CurrentCulture = culture;
            


            // Хитронаёбочка. Чтобы регистрировать каждое включение программы.
            //                Может такие логи вести сразу в базу?
            bool? LogState = AppConfig.EnableLogging; //запоминаем что в логинге
            AppConfig.EnableLogging = true; // принудительно вырубаем его

            Logger.Log("========== START INIT APPLICATION ==========");
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
            Logger.Log("========== END INIT APPLICATION ==========");

            AppConfig.EnableLogging = LogState; // и возвращаем то что было до принудительного включения
            
        }        
        
    }
}