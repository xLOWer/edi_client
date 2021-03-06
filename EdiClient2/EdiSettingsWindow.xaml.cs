﻿using EdiClient.AppSettings;
using System;
using System.Windows;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для EdiSettingsWindow.xaml
    /// </summary>
    public partial class EdiSettingsWindow : Window
    {
        public EdiSettingsWindow()
        {
            InitializeComponent();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            AppConfigHandler.Save();
            AppConfigHandler.Load();
            AppConfigHandler.ConfigureEdi();
            AppConfigHandler.ConfigureOracle();
        }
    }
}
