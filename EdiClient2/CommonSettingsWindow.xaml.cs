using Devart.Data.Oracle;
using EdiClient.AppSettings;
using EdiClient.Services;
using System;
using System.Diagnostics;
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
            DevExpress.Xpf.Core.ThemeManager.SetThemeName(this, "VS2017Light");
            InitializeComponent();
            ConfLoad();
        }


        private void WindowClosed(object sender, EventArgs e)
        {
            ConfSave();
        }

        //открыть папку с логами
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfSave();
            Process.Start("explorer.exe", AppConfigHandler.LogPath);
        }

        //Проверка пользователя трейдера
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ConfSave();
            bool failFlag = false;
            var connstr = $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {AppConfig.DbHost})(PORT = {AppConfig.DbPort}))(CONNECT_DATA = " +
                          $"(SERVER = DEDICATED)(SERVICE_NAME = {AppConfig.DbSID})));Password={AppConfig.TraderUserPassword};User ID={AppConfig.TraderUserName}";

            try
            {
                using (var conn = new OracleConnection(connstr))
                {                    
                    conn.Open();
                }
            }
            catch (Exception ex) { failFlag = true;  }

            if(failFlag)
                Utilites.Error($"Пользователь {AppConfig.TraderUserName}: нет доступа!");
            else
                MessageBox.Show($"Пользователь {AppConfig.TraderUserName}: OK!");
        }



        private void ConfLoad()
        {
            DbUserName.Text = AppConfig.DbUserName;
            DbUserPassword.Password = AppConfig.DbUserPassword;
            Schema.Text = AppConfig.Schema;
            DbHost.Text = AppConfig.DbHost;
            DbPort.Text = AppConfig.DbPort;
            DbSID.Text = AppConfig.DbSID;

            TraderUserName.Text = AppConfig.TraderUserName;
            TraderUserPassword.Password = AppConfig.TraderUserPassword;

            EdiTimeout.Text = AppConfig.EdiTimeout.ToString();
            EdiUser.Text = AppConfig.EdiUser;
            EdiPassword.Password = AppConfig.EdiPassword;
            EdiGLN.Text = AppConfig.EdiGLN;
            EdiEmail.Text = AppConfig.EdiEmail;
            EdiUrl.Text = AppConfig.EdiUrl;

            //EnableAutoHandler.IsChecked = AppConfig.EnableAutoHandler;
            //AutoHandlerPeriod.Text = AppConfig.AutoHandlerPeriod.ToString();
            EnableLogging.IsChecked = AppConfig.EnableLogging;

            LogPath.Text = AppConfigHandler.LogPath;

            EnableProxy.IsChecked = AppConfig.EnableProxy;
            ProxyUserName.Text = AppConfig.ProxyUserName;
            ProxyUserPassword.Password = AppConfig.ProxyUserPassword;

        }

        private void ConfSave()
        {
            AppConfig.DbUserName = DbUserName.Text;
            AppConfig.DbUserPassword = DbUserPassword.Password;
            AppConfig.Schema = Schema.Text;
            AppConfig.DbHost = DbHost.Text;
            AppConfig.DbPort = DbPort.Text;
            AppConfig.DbSID = DbSID.Text;

            AppConfig.TraderUserName = TraderUserName.Text;
            AppConfig.TraderUserPassword = TraderUserPassword.Password;

            AppConfig.EdiTimeout = int.Parse(EdiTimeout.Text);
            AppConfig.EdiUser = EdiUser.Text;
            AppConfig.EdiPassword = EdiPassword.Password;
            AppConfig.EdiGLN = EdiGLN.Text;
            AppConfig.EdiEmail = EdiEmail.Text;
            AppConfig.EdiUrl = EdiUrl.Text;

            //AppConfig.EnableAutoHandler = EnableAutoHandler.IsChecked ?? false;
            //AppConfig.AutoHandlerPeriod = int.Parse(AutoHandlerPeriod.Text);
            AppConfig.EnableLogging = EnableLogging.IsChecked;

            AppConfig.EnableProxy = EnableProxy.IsChecked;
            AppConfig.ProxyUserName = ProxyUserName.Text;
            AppConfig.ProxyUserPassword = ProxyUserPassword.Password;

            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
        }
    }
}
