﻿using DevExpress.Xpf.Grid;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EdiClient.Services.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {         
            return double.Parse(((EditGridCellData)value).Value?.ToString() ?? "0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}