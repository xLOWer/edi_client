﻿using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
using EdiClient.AppSettings;
using EdiClient.Services;
using EdiClient.ViewModel;
using static EdiClient.Services.Utils.Utilites;

namespace EdiClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>(
    public partial class MainWindow : DXRibbonWindow
    {
        private MainViewModel Context { get; set; }

        public MainWindow()
        {
            ThemeManager.SetThemeName(this, "VS2017Light");
            InitializeComponent();
            Context = new MainViewModel();
            DataContext = Context;
            Title = $"Клиент EDI (версия {Assembly.GetEntryAssembly().GetName().Version})";
            try
            {
                DocumentsDataGrid.RestoreLayoutFromXml("Save_GridLayout.xml");
                //Context.RefreshRelationshipsCommand.Execute(SelectedRelationship_BarEditItem);

            }
            catch (Exception ex) { }
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"Продукт защищён лицензией MIT.

Copyright (c) 2019 Шишло Дмитрий

Данная лицензия разрешает лицам, получившим копию данного программного обеспечения и сопутствующей документации 
(в дальнейшем именуемыми ""Программное Обеспечение""), безвозмездно использовать Программное Обеспечение без ограничений, включая
неограниченное право на использование, копирование, изменение, слияние, публикацию, распространение, сублицензирование и/или продажу
копий Программного Обеспечения, а также лицам, которым предоставляется данное Программное Обеспечение, при соблюдении следующих условий:

Указанное выше уведомление об авторском праве и данные условия должны быть включены во все копии или значимые части данного Программного Обеспечения.

ДАННОЕ ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ ПРЕДОСТАВЛЯЕТСЯ ""КАК ЕСТЬ"", БЕЗ КАКИХ-ЛИБО ГАРАНТИЙ, ЯВНО ВЫРАЖЕННЫХ ИЛИ ПОДРАЗУМЕВАЕМЫХ,
ВКЛЮЧАЯ ГАРАНТИИ ТОВАРНОЙ ПРИГОДНОСТИ, СООТВЕТСТВИЯ ПО ЕГО КОНКРЕТНОМУ НАЗНАЧЕНИЮ И ОТСУТСТВИЯ НАРУШЕНИЙ, НО НЕ ОГРАНИЧИВАЯСЬ ИМИ.
НИ В КАКОМ СЛУЧАЕ АВТОРЫ ИЛИ ПРАВООБЛАДАТЕЛИ НЕ НЕСУТ ОТВЕТСТВЕННОСТИ ПО КАКИМ-ЛИБО ИСКАМ, ЗА УЩЕРБ ИЛИ ПО ИНЫМ ТРЕБОВАНИЯМ, В ТОМ ЧИСЛЕ,
ПРИ ДЕЙСТВИИ КОНТРАКТА, ДЕЛИКТЕ ИЛИ ИНОЙ СИТУАЦИИ, ВОЗНИКШИМ ИЗ-ЗА ИСПОЛЬЗОВАНИЯ ПРОГРАММНОГО ОБЕСПЕЧЕНИЯ ИЛИ ИНЫХ ДЕЙСТВИЙ С ПРОГРАММНЫМ ОБЕСПЕЧЕНИЕМ.


The product protected by MIT license.

Copyright 2019 Shishlo Dmitriy

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and / or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.", "Лицензирование продукта", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            DevExpress.Xpf.Core.DXMessageBox.Show($@"EDI клиент {Assembly.GetExecutingAssembly().GetName().Version.ToString()}
Разработана Шишло Дмитрием
Программист ООО ""ВИРЭЙ""
Владивосток
2019", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DocumentsDataGrid_CustomColumnSort(object sender, DevExpress.Xpf.Grid.CustomColumnSortEventArgs e)
        {
            double val1 = -999, val2 = -999;
            try
            {
                val1 = Convert.ToDouble(ToCultureDoubleString(e.Value1));
                val2 = Convert.ToDouble(ToCultureDoubleString(e.Value2));

            }
            catch (Exception ex) { }

            if (val1 > val2) e.Result = 1;
            else e.Result = val1 == val2 ? 0 : -1;

            e.Handled = true;
        }

        private string ToCultureDoubleString(object obj)
            => obj.ToString()
            .Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0])
            .Replace(',', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);

        private void Save_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DocumentsDataGrid.SaveLayoutToXml("Save_GridLayout.xml");
        }

        private void Load_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            DocumentsDataGrid.RestoreLayoutFromXml("Save_GridLayout.xml");
        }

    }
}
