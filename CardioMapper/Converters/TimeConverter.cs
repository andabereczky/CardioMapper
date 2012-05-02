namespace CardioMapper
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan ts = (TimeSpan) value;

            // Round the timespan to the nearest second.
            if (ts.Milliseconds >= 500)
            {
                ts = ts.Add(new TimeSpan(0, 0, 1));
            }

            ts = ts.Subtract(new TimeSpan(0, 0, 0, 0, ts.Milliseconds));

            // Return the general short format string.
            return string.Format(culture, "Time: {0:g}", ts);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
