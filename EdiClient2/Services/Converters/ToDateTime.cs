using System;
using System.Globalization;
using System.Windows.Data;

namespace EdiClient.Services.Converters
{
    class ToDateTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is string)
            {
                DateTime dt;
                DateTime.TryParse(value as string, out dt);
                return dt;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
