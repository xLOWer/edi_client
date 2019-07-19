using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace EdiClient.Services
{
    public static class TabService
    {
        public static ICollection<TabViewModel> Tabs { get; set; } = new ObservableCollection<TabViewModel>();

        private static MainWindow _window { get; set; }
        private static TabControl _tabControl { get; set; }

        public static void Configure(ref MainWindow window, ref TabControl tabControl)
        {
            _window = window;
            _tabControl = tabControl;
        }

        public static void Update()
        {
            _tabControl.UpdateLayout();
            _window.UpdateLayout();
        }

        public static void NewTab(Type view, string title = null)
        {
            if (view == null) return;
            EdiClient.Services.LogService.Log($"[INIT-TabViewModel] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var newTab = new TabViewModel();
            EdiClient.Services.LogService.Log($"[INIT-TabViewModel] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            newTab = new TabViewModel(view, title);
            EdiClient.Services.LogService.Log($"[INIT-.Add(newTab)] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Tabs.Add(newTab);
            EdiClient.Services.LogService.Log($"[INIT-Update] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Update();
        }
        
    }
}
