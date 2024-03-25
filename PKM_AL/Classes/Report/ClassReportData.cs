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
    }
}
