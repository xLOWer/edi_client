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
    }
}
