namespace PKM_AL.Classes.ServiceClasses;
    public class ClassStaticResoursUserControlDevice : MyPropertyChanged
    {
        private bool isDevAll;
        //private bool firstTime = true;

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
                        //if (firstTime && device.IsPoll) value = true;
                        device.IsPoll = value;
                    }
                }
                //if (firstTime) firstTime = false;
            }
        }
    }
