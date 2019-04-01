using EdiClient.ViewModel.Desadv;
using System;
using System.Windows.Controls;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для DesadvListView.xaml
    /// </summary>
    public partial class DesadvListView : Page
    {
        public DesadvListViewModel Context { get; set; }

        public DesadvListView()
        {
            Context = new DesadvListViewModel();
            DataContext = Context;
            InitializeComponent();
        }

    }
}
