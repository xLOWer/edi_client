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
            // Хитронаёбочка. Чтобы регистрировать каждое включение программы.
            //                Может такие логи вести сразу в базу?
            bool? LogState = AppConfig.EnableLogging; //запоминаем что в логинге
            AppConfig.EnableLogging = true; // принудительно вырубаем его

            LogService.Log("========== START INIT APPLICATION ==========");
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
            LogService.Log("========== END INIT APPLICATION ==========");

            AppConfig.EnableLogging = LogState; // и возвращаем то что было до принудительного включения
        }
        
    }
}