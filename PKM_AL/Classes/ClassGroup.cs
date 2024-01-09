using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PKM_AL
{
    public class ClassGroup:MyPropertyChanged
    {
        //Тип узла.
        public enum eType
        {
            None = 0,
            Devices = 1,
            Map = 2,
        }

        private string _expand = "False";
        public string Expand
        {
            get => _expand;
            set
            {
                _expand = value;
                OnPropertyChanged("Expand");
            }
        }

        public int ID { get; set; }
        //Коллекция подузлов.
        public ObservableCollection<ClassItem> SubGroups { get; set; }
        public string IconUri { get; set; }
        public string Name { get; set; }
        public eType GroupType { get; set; }

        public ClassGroup()
        {
            ID = 0;
            SubGroups = new ObservableCollection<ClassItem>();
        }
    }

    public class ClassItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public enum eType
        {
            None = 0,
            Device = 1,
            Command = 2,
            Log = 3,
            Map = 4,
            Archive = 5,
            Graph = 6,
            Messages = 7,
            Alarms = 8,
            Links = 9
        }
        public int ID { get; set; }
        public string IconUri { get; set; }
        public ClassGroup Group { get; set; }
        public eType ItemType { get; set; }
        public int Param { get; set; }

        private string _NameCh;

        public string NameCh
        {
            get { return _NameCh; }
            set
            {
                _NameCh = value;
                OnPropertyChanged("NameCh");
            }
        }
    }
}
