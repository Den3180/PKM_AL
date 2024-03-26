using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PKM
{   
    /// <summary>
    /// Класс данных из таблиц БД отчетов.
    /// </summary>
    public class ClassReportData
    {
        public ClassReportData(EnumTypeReport typeReport)
        {
            DateRec = new ObservableCollection<string>();            
        }
        
        /// <summary>
        /// Список данных из отчетов.
        /// </summary>
        public ObservableCollection<string> DateRec {get;set;}

        public override string ToString()
        {
            StringBuilder str=new StringBuilder();
            for(int i=0;i<DateRec.Count;i++)
            {
                if (i == DateRec.Count - 1)
                {
                    str.Append(DateRec[i]);
                }
                else
                {
                    str.Append(DateRec[i]);
                    str.Append(';');
                }
            }
            return str.ToString();
        }
    }
}
