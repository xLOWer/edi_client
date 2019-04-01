using EdiClient.ViewModel.Orders;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
