namespace CardioMapper
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(culture, "Distance: {0:0.##} meters", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
