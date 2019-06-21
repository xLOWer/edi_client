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

            DbUserName.Text = AppConfig.DbUserName;
            DbUserPassword.Text = AppConfig.DbUserPassword;
            Schema.Text = AppConfig.Schema;
            DbHost.Text = AppConfig.DbHost;
            DbPort.Text = AppConfig.DbPort;
            DbSID.Text = AppConfig.DbSID;
            EdiTimeout.Text = AppConfig.EdiTimeout.ToString();
            EdiUser.Text = AppConfig.EdiUser;
            EdiPassword.Text = AppConfig.EdiPassword;
            EdiGLN.Text = AppConfig.EdiGLN;
            EdiEmail.Text = AppConfig.EdiEmail;
            EdiUrl.Text = AppConfig.EdiUrl;
            EnableAutoHandler.IsChecked = AppConfig.EnableAutoHandler;
            AutoHandlerPeriod.Text = AppConfig.AutoHandlerPeriod.ToString();
            EnableLogging.IsChecked = AppConfig.EnableLogging;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            AppConfig.DbUserName = DbUserName.Text;
            AppConfig.DbUserPassword = DbUserPassword.Text;
            AppConfig.Schema = Schema.Text;
            AppConfig.DbHost = DbHost.Text;
            AppConfig.DbPort = DbPort.Text;
            AppConfig.DbSID = DbSID.Text;
            AppConfig.EdiTimeout = int.Parse(EdiTimeout.Text);
            AppConfig.EdiUser = EdiUser.Text;
            AppConfig.EdiPassword = EdiPassword.Text;
            AppConfig.EdiGLN = EdiGLN.Text;
            AppConfig.EdiEmail = EdiEmail.Text;
            AppConfig.EdiUrl = EdiUrl.Text;
            AppConfig.EnableAutoHandler = EnableAutoHandler.IsChecked ?? false;
            AppConfig.AutoHandlerPeriod = int.Parse(AutoHandlerPeriod.Text);
            AppConfig.EnableLogging = EnableLogging.IsChecked ?? false;

            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
        }
    }
}
