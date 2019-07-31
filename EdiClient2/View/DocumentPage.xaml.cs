using DevExpress.Xpf.Core;
using EdiClient.Services;
using EdiClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для DocumentPage.xaml
    /// </summary>
    public partial class DocumentPage : Page
    {
        public DocumentListViewModel Context { get; set; }

        public DocumentPage()
        {
            EdiClient.Services.LogService.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            DevExpress.Xpf.Core.ThemeManager.SetThemeName(this, "VS2017Light");
            Context = new DocumentListViewModel();
            DataContext = Context;
            InitializeComponent();
        }

        private void DocumentsDataGrid_CustomColumnSort(object sender, DevExpress.Xpf.Grid.CustomColumnSortEventArgs e)
        {
            if (Convert.ToDouble(e.Value1) > Convert.ToDouble(e.Value2))
                e.Result = 1;
            else
                e.Result = Convert.ToDouble(e.Value1) == Convert.ToDouble(e.Value2) ? 0 : -1;
            e.Handled = true;
        }
    }
}
