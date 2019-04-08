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
    public partial class MatchMakerView : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaiseNotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( info ) );
        }
        public MatchMakerViewModel Context { get; set; }

        public MatchMakerView()
        {
            Context = new MatchMakerViewModel( this );
            DataContext = Context;
            InitializeComponent();
        }
    }
}
