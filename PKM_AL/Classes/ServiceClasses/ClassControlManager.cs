using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PKM_AL
{
    public class ClassControlManager
    {
        /// <summary>
        /// Проверка разделителя десятичного числа. Если разделитель не корректен - замена.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separatorCurrent"></param>
        /// <returns></returns>
        public static string CheckCurrentSeparator(string text, string separatorCurrent)
        {
            if (Regex.IsMatch(text, @"\.|\,"))
            {
                return text.Contains(separatorCurrent) ? text : new Regex(@"\.|\,").Replace(text, separatorCurrent, 1);
            }
            return text;
        }

    }
}
