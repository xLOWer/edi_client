using EdiClient.ViewModel.Ordrsp;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для OrdrspListView.xaml
    /// </summary>
    public partial class OrdrspListView : Page
    {
        public OrdrspListViewModel Context { get; set; }

        public OrdrspListView()
        {
            Context = new OrdrspListViewModel();
            DataContext = Context;
            InitializeComponent();
        }
    }
}
