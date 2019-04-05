using EdiClient.ViewModel.Orders;
using System.Windows.Controls;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для OrdersListView.xaml
    /// </summary>
    public partial class OrdersListView : Page
    {
        OrdersListViewModel Context { get; set; }

        public OrdersListView()
        {
            Context = new OrdersListViewModel();
            DataContext = Context;
            InitializeComponent();
        }

    }
}
