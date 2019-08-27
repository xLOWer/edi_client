using DevExpress.Xpf.Ribbon;
using EdiClient.ViewModel;
using System.Windows.Controls;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для PriceTypesView.xaml
    /// </summary>
    public partial class PriceTypesView : DXRibbonWindow
    {
        public PriceTypesViewModel Context { get; set; }

        public PriceTypesView()
        {
            Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Context = new PriceTypesViewModel(this);
            DataContext = Context;
            InitializeComponent();
        }
    }
}
