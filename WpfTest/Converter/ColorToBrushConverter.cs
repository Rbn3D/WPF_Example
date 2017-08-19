using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfTest
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush).Color;
        }

        // Convenience method
        public static SolidColorBrush Convert(Color value)
        {
            return new SolidColorBrush((Color)value);
        }
    }
}
