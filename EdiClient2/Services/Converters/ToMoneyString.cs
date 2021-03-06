﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace EdiClient.Services.Converters
{
    public class ToMoneyString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (value == null) return "0 ₽";
            return double.Parse((string)value).ToString() + " ₽";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
