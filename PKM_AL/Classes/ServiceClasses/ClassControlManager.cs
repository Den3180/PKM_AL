using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace PKM_AL
{
    public enum TypeDataOBD
    {
        INTEGER=1,
        DOUBLE=2,
        TDATE=3,
        DATE=4,
        GPS=5,
        CDATA=0
    } 

    public class ClassControlManager
    {
                #region[Паттерны валидации]
        public static string DoublePattern = @"(^-$)|(^-?([1-9]+)\.?\d*$)|(^-?0?\.\d*$)|^0$|^-0$";//Для чисел с точкой и со знаком.
        public static string DecimalPattern { get; } = @"(^-?0\.\d{1,4}$)|(^-?\d+(\.\d{1,4})$)|(^-?[1-9]\d*$)|(^0$)";//Для чисел +/- с с точкой и без.
        public static string U_IntPattern { get; } = @"(^[1-9]\d*)$|^0$";//Для целых чисел без знака.
        public static string Date_YearPattern { get; } = @"^[1-2]\d{3}"; //Дата в формате гггг.
        public static string U_DecimalPattern { get; } = @"(^0\.\d{1,4}$)|(^\d+(\.\d{1,4})$)|(^[1-9]\d*$)|(^0$)"; //Для положительных целых чисел с точкой.
        public static string U_DecimalCoordPattern { get; } = @"(^0\.\d+$)|(^\d+(\.\d+)$)|(^[1-9]\d*$)|(^0$)"; //Для положительных целых чисел с точкой.
        public static string Date_PointFullPattern { get; } = @"^[0-3]\d\.[0-1]\d\.[1-2]\d{3}$"; //Дата в формате ДД.ММ.ГГГГ.
        public static string Date_TimeFullPattern { get; } = @"^[0-3]\d\.[0-1]\d\.[1-2]\d{3} ([0-1]\d|2[0-4]):[0-5]\d:[0-5]\d$"; //Дата в формате ДД.ММ.ГГГГ ЧЧ:ММ:СС
        public static string GPS_Pattern { get; } = @"^[1-9]?\d°[1-9]?\d\.\d{4}$";//ГГ°ММ.ММММ(в поле вводится пробел вместо градуса).
        public static string Date_FullPattern { get; } = @"^[1-2]\d{3}-[0-1]\d-[0-3]\d$"; //Дата в формате гггг-мм-дд.
        public static string U_Int_AdvancePattern { get; } = @"^\d+\W?\d*$";  //Целое число, не буквенно-цифровой символ,целое число.
        public static string DegreePattern { get; } = @"(^\d\W$)|(^\d\d\W$)";  //Любой пробельный или не буквенный символ.
        public static string PoinINGPSPattern { get; } = @"^\d\d?\d?°\d\d?\d?\W$";  //Любой пробельный или не буквенный символ.
        public static string PoinINNumberPattern { get; } = @"(^\d+\W$)";  //Любой пробельный или не буквенный символ.
        public static string AnySimbol_Pattern { get; } = @".";  //Целое число, не буквенно-цифровой символ,целое число.
        #endregion

        
        List<TextBox> textBoxes;
        
        public ClassControlManager()
        {
        }
        
        /// <summary>
        /// Контроль доступа к вкладкам.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CancelTabsControl(UserControl control, List<TextBox> lstTextBox)
        {
            if (control == null) return;
            ClassControlManager controlManagement = new ClassControlManager();
            TabItem owner = control.Parent as TabItem;
            TabControl tabC = owner.Parent as TabControl;
            if(tabC.Name != "tabC")
            {
                return;
            }
            foreach (var tab in tabC.Items)
            {
                if (!(tab as TabItem).IsSelected &&!controlManagement.CheckEnableTabs(lstTextBox))
                {
                    (tab as TabItem).IsEnabled = false;                    
                }
                else
                {
                    (tab as TabItem).IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Проверка полей значений.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool CheckEnableTabs(List<TextBox> lst)
        {
            foreach(var box in lst)
            {
                if (!string.IsNullOrEmpty(box.Text))
                {
                    return false;
                }
            }
            return true;
        }
        
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
        
        /// <summary>
        /// Рекурсивная функция обхода логического дерева контрола. Находит поля для ввода значений.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  List<TextBox> GetTextBox(ILogical obj)
        {
            if (obj is null) return null;
            textBoxes ??= new List<TextBox>();
            var childs = obj.GetLogicalChildren().ToList();
            //Если элемент TabControl - удаляем последний элемент коллекции потомков.
           if (obj is TabControl) 
            {
                childs.RemoveAt(childs.Count-1);
            }
            foreach (var elem in childs)
            {
                if((elem as TabItem)!=null && !(elem as TabItem).IsSelected)
                {
                    continue;
                }
                if (elem is TextBox box)
                {
                    textBoxes.Add(box);
                }
                else
                {
                    GetTextBox((ILogical)elem);
                }
            }
            return textBoxes;
        }
      
        
        /// <summary>
        /// Получение графических элементов из формы(ограничение - кнопки).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<T> GetUIElem<T>(ILogical obj, List<T>lst)
        {
            if (obj is null) return null;
            var childs = obj.GetLogicalChildren().ToList(); //LogicalTreeHelper.GetChildren(obj);
            if (obj is TabControl) 
            {
                childs.RemoveAt(childs.Count-1);
            }
            foreach (var elem in childs)
            {
                if((elem as TabItem)!=null && !(elem as TabItem).IsSelected)
                {
                    continue;
                }
                if (elem is T elem1)
                {
                    lst.Add(elem1);
                }
                else if((elem as Button)==null)
                {
                    GetUIElem<T>(elem as ILogical, lst);                                
                }
            }
            return lst;
        }
        
        /// <summary>
        /// Настройка подсказок для полей.
        /// </summary>
        /// <param name="lstTextBox"></param>
        public static void SetToolTipText(List<TextBox> lstTextBox)
        {           
            foreach (TextBox box in lstTextBox)
            {
                string t = box?.Tag?.ToString();
                if (t.Contains("*"))
                {
                    t = t.Remove(t.Length-1);
                }
                string toolTip  = t switch
                {
                    "INTEGER" => "Формат: целое положительное число. \nПример:1, 45, 200",
                    "DOUBLE" => "Формат: число с точкой(не более 4 знаков после точки). \nПример:0, 1.2564, -15.8",
                    "TDATE" => "Формат: ДД.ММ.ГГГГ ЧЧ:ММ:СС. \nПример:12.05.2023 15:02:45",
                    "DATE" => "Формат: ДД.ММ.ГГГГ. \nПример: 12.05.2023",
                    "GPS" => "Формат: ГГ.ГГГГ. \nПример: 55.4505",
                    "CDATA" => "Формат: Текст",
                    _ => string.Empty
                };
                ToolTip.SetTip(box,toolTip);
                ToolTip.SetShowDelay(box,500);
            }
        }

        public static string GetPatternOBDData(object tag)
        {
            string strTag = tag.ToString();
            if (strTag.Contains("*"))
            {
                strTag = strTag.Remove(strTag.Length - 1);
            }
            return strTag switch 
            {
                "INTEGER" => U_IntPattern,
                "DOUBLE" => DecimalPattern,
                "TDATE" => Date_TimeFullPattern, 
                "DATE" => Date_PointFullPattern,
                //"GPS" => GPS_Pattern,
                "GPS" => U_DecimalCoordPattern,
                "CDATA" => AnySimbol_Pattern,
                _ => string.Empty
            };
        }

        public static void CheckPointInValue(TextBox textBox)
        {
            if ((textBox.Tag.ToString().Contains("DOUBLE") || textBox.Tag.ToString().Contains("GPS")) && Regex.IsMatch(textBox.Text, ClassControlManager.PoinINNumberPattern))
            {
                string str = textBox.Text;
                str = str.Remove(str.Length - 1);
                str += ".";
                textBox.Text = str;
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        public static object CheckFillingTboxes(List<TextBox> textBoxes, List<Button> buttons)
        {
            if (buttons==null || buttons.Count == 0) return false;
            foreach (var box in textBoxes)
            {
                if(string.IsNullOrEmpty(box.Text)) 
                    box.Text=String.Empty;
                //Если поле обязательное и не соответствует паттерну.
                if ((box.Tag.ToString().Contains("*") && string.IsNullOrEmpty(box.Text)) 
                                                     && !Regex.IsMatch(box.Text,GetPatternOBDData(box.Tag)))
                {
                    //Доступ к кнопке "Добавить" запрещен.
                    buttons[0].IsEnabled = false;
                    //Выход из метода без дальнейшей проверки остальных полей.
                    return false;
                }
            }
            buttons[0].IsEnabled = true;
            return true;
        }

        public static List<TextBox> GetTextBoxFromDR_POINTS(ILogical obj, List<TextBox> lst)
        {
            if (obj is null) return null;
            System.Collections.IEnumerable childs = obj.GetLogicalChildren(); //LogicalTreeHelper.GetChildren(obj);
            foreach (var elem in childs)
            {               
                if (elem is TextBox)
                {
                    lst.Add(elem as TextBox);
                }
                else if ((elem as TabControl) == null)
                {
                    GetTextBoxFromDR_POINTS(elem as ILogical, lst);
                }
            }
            return lst;
        }
    }
}
