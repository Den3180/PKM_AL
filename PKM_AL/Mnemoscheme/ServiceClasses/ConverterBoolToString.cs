using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PKM_AL.Mnemoscheme.ServiceClasses;

public class ConverterBoolToString : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Опрос вкл." : "Опрос откл.";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}