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
            System.Collections.IEnumerable childs = obj.GetLogicalChildren(); //LogicalTreeHelper.GetChildren(obj);
            foreach (var elem in childs)
            {
                if((elem as TabItem)!=null && !(elem as TabItem).IsSelected)
                {
                    continue;
                }
                if (elem is T)
                {
                    lst.Add((T)elem);
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
    }
}
