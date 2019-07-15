using System;
using System.Globalization;
using System.Windows.Data;

namespace EdiClient.Services.Converters
{
    public class ToMoneyString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return "0 Р";
                if (value is double) return value.ToString() + " Р";
                else return double.Parse((string)value).ToString() + " Р";
            }
            catch (Exception ex) { }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
