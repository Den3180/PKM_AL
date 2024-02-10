using System;

namespace PKM_AL.Classes;

public class ClassCommand : MyPropertyChanged
{
            public enum EnumType
        {
            WriteCoil = 1,
            WriteRegistry = 2
        }

        public int ID { get; set; }
        public int Address { get; set; }

        private string _Name;
        private ClassDevice _Device;
        private DateTime _DT;
        private EnumType _CommandType;
        private int _Value;
        private int _Period;
        private ClassChannel _Channel;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public ClassDevice Device
        {
            get { return _Device; }
            set
            {
                _Device = value;
                OnPropertyChanged("Device");
                OnPropertyChanged("DeviceName");
            }
        }

        public DateTime DT
        {
            get { return _DT; }
            set
            {
                _DT = value;
                OnPropertyChanged("DT");
                OnPropertyChanged("StrDT");
            }
        }

        public EnumType CommandType
        {
            get { return _CommandType; }
            set
            {
                _CommandType = value;
                OnPropertyChanged("CommandType");
                OnPropertyChanged("StrCommandType");
            }
        }

        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        public int Period
        {
            get { return _Period; }
            set
            {
                _Period = value;
                OnPropertyChanged("Period");
            }
        }

        public ClassChannel Channel
        {
            get { return _Channel; }
            set
            {
                _Channel = value;
                OnPropertyChanged("Channel");
                OnPropertyChanged("ChannelName");
            }
        }

        public string DeviceName { get { return Device.Name; } }
        public string StrDT
        {
            get
            {
                if (_DT == DateTime.MinValue) return "";
                else return DT.ToString("dd.MM.yyyy HH:mm:ss"); 
            } 
        }

        public string StrCommandType
        {
            get
            {
                switch (_CommandType)
                {
                    case EnumType.WriteCoil: return "Write Coil (DO)";
                    case EnumType.WriteRegistry: return "Write Registry (AO)";
                    default: return "";
                }
            } 
        }

        public bool bValue 
        {
            get 
            {
                if (Value != 0) return true;
                else return false;
            } 
        }

        public ClassCommand()
        {
            ID = 0;
            Name = "Команда 1";
            Device = new ClassDevice();
            Address = 0;
            Value = 0;
            Period = 3600;
            _DT = DateTime.MinValue;
            _CommandType = EnumType.WriteCoil;
            _Channel = new ClassChannel();
        }
    }

