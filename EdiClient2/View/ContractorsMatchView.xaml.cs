using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
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
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для MatchMakerView.xaml
    /// </summary>
    public partial class ContractorsMatchView : DXRibbonWindow
    {
        public ContractorsMatchViewModel Context { get; set; }

        public ContractorsMatchView()
        {
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Context = new ContractorsMatchViewModel( );
            DataContext = Context;
            InitializeComponent();
        }
        
        private void ClientsGridControlTableView_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            Logger.Log($"[CLIENT-MATCH] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Context.EditCommand.Execute(null);

        }
    }
}
