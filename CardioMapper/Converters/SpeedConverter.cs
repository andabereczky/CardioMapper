namespace CardioMapper
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (!string.IsNullOrEmpty(param))
            {
                if (param.Equals("average"))
                {
                    return string.Format(culture, "Average speed: {0:0.##} m/s", value);
                }
                else if (param.Equals("max"))
                {
                    return string.Format(culture, "Maximum speed: {0:0.##} m/s", value);
                }
                else
                {
                    return string.Format(culture, "Speed: {0:0.##} m/s", value);
                }
            }
            else
            {
                return string.Format(culture, "{0} m/s", value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
