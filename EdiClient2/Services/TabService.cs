using DevExpress.Xpf.Core;
using EdiClient.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace EdiClient.Services
{
    public static class TabService 
    {
        public static ICollection<TabViewModel> Tabs { get; set; } = new ObservableCollection<TabViewModel>();
        private static MainWindow _window { get; set; }
        private static DXTabControl _tabControl { get; set; }

        public static void Configure(ref MainWindow window, ref DXTabControl tabControl)
        {
            _window = window;
            _tabControl = tabControl;
        }

        public static void Update()
        {
            _tabControl.UpdateLayout();
            _window.UpdateLayout();
        }

        public static void NewTab(Page view, string title = null)
        {
            if (view == null /*|| Tabs.Any( x=>x.View.GetType().IsEquivalentTo(view.GetType()) ) наверка на уже открытые вкладки*/  ) return;

            var newTab = new TabViewModel();
            newTab = new TabViewModel(view, title ?? (view.Title ?? "view"));
            Tabs.Add(newTab);
            Update();
        }

        public static void CloseTab(object view)
        {
            Tabs.Remove(Tabs.FirstOrDefault(x => x.View == view));
            Update();
        }

    }
}
