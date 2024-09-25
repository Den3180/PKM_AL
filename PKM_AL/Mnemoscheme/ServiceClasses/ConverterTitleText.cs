using System;
using System.Drawing;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TestGrathic.ServiceClasses;

public class ConverterTitleText : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null && !double.IsNaN((double)value)) return value.ToString();
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty(value?.ToString()))
        {
            if (CultureInfo.CurrentCulture.Name == culture.Name  && value.ToString().Contains('.'))
            {
                value = ((string)value).Replace('.',',');
            }
            try
            {
                return Double.Parse((string)value);
            }
            catch (Exception e)
            {
            }
        }
        return double.NaN;
    }
}