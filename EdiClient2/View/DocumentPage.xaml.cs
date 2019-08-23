using DevExpress.Xpf.Core;
using EdiClient.Services;
using EdiClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static EdiClient.Services.Utils.Utilites;

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
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Context = new DocumentListViewModel();
            DataContext = Context;
            InitializeComponent();
//            string format = $@"
//CultureTypes='{Thread.CurrentThread.CurrentCulture.CultureTypes}'
//NativeName='{Thread.CurrentThread.CurrentCulture.NativeName}'
//Name='{Thread.CurrentThread.CurrentCulture.Name}'

//CurrencyDecimalSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator}'
//CurrencyGroupSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyGroupSeparator}'
//NumberDecimalSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}'
//NumberGroupSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator}'
//PercentDecimalSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.PercentDecimalSeparator}'
//PercentGroupSeparator='{Thread.CurrentThread.CurrentCulture.NumberFormat.PercentGroupSeparator}'
//CurrencySymbol='{Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol}'
//";
//            DXMessageBox.Show(format);
        }

        private void DocumentsDataGrid_CustomColumnSort(object sender, DevExpress.Xpf.Grid.CustomColumnSortEventArgs e)
        {
            double val1=-999, val2 = -999;
            try
            {
                val1 = Convert.ToDouble(ToCultureDoubleString(e.Value1));
                val2 = Convert.ToDouble(ToCultureDoubleString(e.Value2));

            }
            catch (Exception ex) { }

            if (val1 > val2) e.Result = 1;
                else e.Result = val1 == val2 ? 0 : -1;

                e.Handled = true;            
        }

        private string ToCultureDoubleString(object obj)
            => obj.ToString()
            .Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0])
            .Replace(',', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);
    }
}
