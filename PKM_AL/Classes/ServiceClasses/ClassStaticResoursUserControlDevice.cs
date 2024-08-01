namespace PKM_AL.Classes.ServiceClasses;
    public class ClassStaticResoursUserControlDevice : MyPropertyChanged
    {
        private bool isDevAll;

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
