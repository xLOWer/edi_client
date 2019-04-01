using System.Windows;
using EdiClient.Services;
using EdiClient.ViewModel.Common;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>(
    public partial class MainWindow : Window
    {
        private MainViewModel Context { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            TabService.Configure(ref mainWindow, ref MainTabControl);
            Context = new MainViewModel();
            DataContext = Context;
            UpdateLayout();
        }
        
        //private void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Context.NewTab();            
        //}

        //private void LeftMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    MenuItemViewModel selectedMenuItem = (MenuItemViewModel)(sender as TreeView).SelectedItem;

        //    if (String.IsNullOrEmpty(selectedMenuItem.Title) || selectedMenuItem.View == null) return;

        //    Context.SelectedMenuItem = (MenuItemViewModel)(sender as TreeView).SelectedItem;
        //    this.MainTabControl.UpdateLayout();
        //}

    }
}
