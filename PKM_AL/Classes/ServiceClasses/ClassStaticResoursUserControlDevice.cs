namespace PKM_AL.Classes.ServiceClasses;

/// <summary>
/// Класс-ресурс для чекбокса в заголовке столбца "Опрос" таблицы "Устройства".
/// </summary>
    public class ClassStaticResoursUserControlDevice : MyPropertyChanged
    {
        private bool isDevAll;
        
        /// <summary>
        /// Флаг чекбокса в заголовке столбца "Опрос" таблицы "Устройства".
        /// </summary>
        public bool IsDevAll
        {
            get=>isDevAll;
            set
            {
                isDevAll = value;
                OnPropertyChanged();
                if (MainWindow.currentMainWindow != null)
                {
                    foreach (var device in MainWindow.Devices)
                    {
                        device.IsPoll = value;
                    }
                }
            }
        }
    }
