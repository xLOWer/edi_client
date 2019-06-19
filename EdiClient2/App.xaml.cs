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
            LogService.Log("========== START INIT APPLICATION ==========");
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
            LogService.Log("========== END INIT APPLICATION ==========");
        }
    }
}