using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PKM_AL.Classes.ServiceClasses;

public class DateConvert :IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var dt = System.Convert.ToDateTime(value);
        return dt.ToString("dd.MM.yyyy hh:mm:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new object();
    }
}