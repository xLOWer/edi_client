using EdiClient.ViewModel.Recadv;
using System.Windows.Controls;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для RecadvListView.xaml
    /// </summary>
    public partial class RecadvListView : Page
    {
        public RecadvListViewModel Context { get; set; }

        public RecadvListView()
        {
            Context = new RecadvListViewModel();
            DataContext = Context;
            InitializeComponent();
        }
    }
}
