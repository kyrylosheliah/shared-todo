using System;
using System.Globalization;
using System.Windows.Data;

namespace TodoClientWpf.Converters;

public class TextNotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value as string;
        return !string.IsNullOrWhiteSpace(text);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
