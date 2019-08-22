﻿using EdiClient.ViewModel;
using System.Windows.Controls;

namespace EdiClient.View
{
    /// <summary>
    /// Логика взаимодействия для PriceTypesView.xaml
    /// </summary>
    public partial class PriceTypesView : Page
    {
        public PriceTypesViewModel Context { get; set; }

        public PriceTypesView()
        {
            EdiClient.Services.Utilites.Logger.Log($"[INIT] {System.Reflection.MethodBase.GetCurrentMethod().DeclaringType} {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Context = new PriceTypesViewModel(this);
            DataContext = Context;
            InitializeComponent();
        }
    }
}
