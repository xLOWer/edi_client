using EdiClient.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для MatchMakerView.xaml
    /// </summary>
    public partial class ContractorsMatchView : Page
    {
        public ContractorsMatchViewModel Context { get; set; }

        public ContractorsMatchView()
        {
            Context = new ContractorsMatchViewModel( );
            DataContext = Context;
            InitializeComponent();
        }

        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.Edit();
        }
    }
}
