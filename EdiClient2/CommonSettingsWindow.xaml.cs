using EdiClient.AppSettings;
using System;
using System.Windows;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для CommonSettingsWindow.xaml
    /// </summary>
    public partial class CommonSettingsWindow : Window
    {
        public CommonSettingsWindow()
        {
            InitializeComponent();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            AppConfigHandler.Save();
        }
    }
}
