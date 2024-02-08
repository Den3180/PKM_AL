using System.Threading;

namespace PKM_AL.Classes.ServiceClasses;

public class ClassHelper
{
    public static decimal? DecimalFromStr(string strValue)
    {
        decimal? result = null;
        string Separator = Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
        strValue = strValue.Contains('.') ? strValue.Replace(".", Separator) : strValue;
        strValue = strValue.Contains(',') ? strValue.Replace(",", Separator) : strValue;
        if (decimal.TryParse(strValue, out decimal value)) result = value;
        return result;
    }

    public static int? IntFromStr(string strValue)
    {
        int? result = null;
        if (int.TryParse(strValue, out int value)) result = value;
        return result;
    }
}