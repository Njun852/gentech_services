using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace gentech_services.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                if (parameter?.ToString() == "Text")
                {
                    return isActive ? "Active" : "Inactive";
                }
                else if (parameter?.ToString() == "Background")
                {
                    // Blue (#0000FF) with 10% opacity for both states
                    return new SolidColorBrush(Color.FromArgb(26, 0, 0, 255)); // 10% opacity = 26/255
                }
                else if (parameter?.ToString() == "Foreground")
                {
                    return isActive ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C206")) // Green
                                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000")); // Red
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
