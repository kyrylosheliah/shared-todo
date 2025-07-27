using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoClientWpf.Converters;

public class TextNotEmptyVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = value is string s && !string.IsNullOrWhiteSpace(s);
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}
