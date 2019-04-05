using EdiClient.ViewModel;
using System.Windows.Controls;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для MatchMakerView.xaml
    /// </summary>
    public partial class MatchMakerView : Page
    {
        MatchMakerViewModel Context { get; set; }

        public MatchMakerView()
        {
            Context = new MatchMakerViewModel();
            DataContext = Context;
            InitializeComponent();
        }
    }
}
