using DevExpress.Xpf.Core;
using EdiClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ThemeManager.SetThemeName(this, "VS2017Light");
            Context = new DocumentListViewModel();
            DataContext = Context;
            InitializeComponent();
        }
    }
}
