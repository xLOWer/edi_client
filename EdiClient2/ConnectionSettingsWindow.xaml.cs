using EdiClient.AppSettings;
using EdiClient.Services;
using System;
using System.Windows;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для ConnectionSettingsWindow.xaml
    /// </summary>
    public partial class ConnectionSettingsWindow : Window
    {
        public ConnectionSettingsWindow()
        {
            InitializeComponent();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            OracleConnectionService.Configure();
        }
    }
}
