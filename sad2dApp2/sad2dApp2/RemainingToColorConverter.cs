using System;
using Microsoft.Maui.Controls;

namespace sad2dApp2
{
    public class RemainingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double remaining)
            {
                return remaining < 0 ? Colors.Red : Colors.ForestGreen;
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
