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
    /// Логика взаимодействия для PriceTypesView.xaml
    /// </summary>
    public partial class PriceTypesView : Page
    {
        public PriceTypesViewModel Context { get; set; }

        public PriceTypesView()
        {
            ThemeManager.SetThemeName(this, "VS2017Light");
            Context = new PriceTypesViewModel(this);
            DataContext = Context;
            InitializeComponent();
        }
    }
}
