using System;
using System.Globalization;
using Xamarin.Forms;

namespace Core
{
    public class ObjectIsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
                return !string.IsNullOrEmpty(strValue);

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
