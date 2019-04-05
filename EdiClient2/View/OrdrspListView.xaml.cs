using EdiClient.ViewModel.Ordrsp;
using System.Windows.Controls;

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
