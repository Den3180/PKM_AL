using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Media;
using Avalonia.Threading;
using PKM_AL.Classes;
using PKM_AL.Controls;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ModelMap;

namespace PKM_AL
{
    public class ClassChannel : MyPropertyChanged
    {
        public delegate void NewValueEventHandler(ClassChannel channel);
        public event NewValueEventHandler NewValueEvent;
        /// <summary>
        /// Тип регистра.
        /// </summary>
        public enum EnumTypeRegistry
        {
            DiscreteInput = 1,
            CoilOutput = 2,
            InputRegistry = 3,
            HoldingRegistry = 4,
        }
        /// <summary>
        /// Формат значения канала.
        /// </summary>
        public enum EnumFormat
        {
            UINT = 0,
            SINT = 1,
            Float = 2,
            swFloat = 3,
            UInt32 = 4
        }
        /// <summary>
        /// Состояние значения канала.
        /// </summary>
        public enum EnumState
        {
            Unknown = 0,
            Norma = 1,
            Over = 2,
            Less = 3
        }
        private string _Name;
        private DateTime _DTAct;
        private int _Address;
        private EnumFormat _Format;
        private float _Koef;
        private decimal _Value;
        private decimal? _Max;
        private decimal? _Min;
        private EnumState _State;
        private EnumTypeRegistry _TypeRegistry;
        private ClassDevice _Device;
        private ushort[] _BaseValue;
        private decimal _PreviousValue;
        private bool _Archive;
        private int? _Ext;
        private int? _Accuracy;
        private decimal? _NValue;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public DateTime DTAct
        {
            get { return _DTAct; }
        }

        public int Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged();
            }
        }

        public EnumFormat Format
        {
            get { return _Format; }
            set
            {
                _Format = value;
                OnPropertyChanged();
            }
        }

        public float Koef
        {
            get { return _Koef; }
            set
            {
                _Koef = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public decimal Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                if (_Value is Decimal vd)
                {
                    //Конвертация значений в знаковые величины.
                    _Value = ConvertMinus(_Value);
                }
                
                if (this.Device.Model == ClassDevice.EnumModel.MMPR)
                {
                    _Value = ClassDevice.ConvertMMPRData(_Value,this);
                }
                
                //Умножение на коэффициент.
                if (_Koef != 1) _Value = _Value * (decimal)_Koef;
                //Округление значений.
                if (_Accuracy.HasValue) _Value = Decimal.Round(_Value, _Accuracy.Value);
                OnPropertyChanged();
                _DTAct = DateTime.Now;
                OnPropertyChanged("DTAct");
                OnPropertyChanged("StrDTAct");
                //Сохранение нового значение в таблицу регистров(только в том случае если у регистра есть имя и оно не "Резерв").
                if (Name != "" && Name != "Резерв" && !MainWindow.settings.Demo)
                {
                    Task.Run(() => MainWindow.DB.RegistrySaveValue(this));
                }
                _PreviousValue = _Value; //Странное место?
                //Создание объекта события.
                ClassEvent ev = new ClassEvent
                {
                    Type = ClassEvent.EnumType.Measure,
                    Param = _Name,
                    Val = _Value.ToString(CultureInfo.CurrentCulture),
                    SourceID = ID,
                    NameDevice = DeviceName
                };
                ClassEvent.SaveNewEvent(ev, _Archive);

                //Изменение значений на мнемосхеме.
                NewValueEvent?.Invoke(this);
                //Если задано максимальное ограничение величины и оно превышено.
                if (_Max.HasValue && _Value > _Max.Value)
                {
                    if (State != EnumState.Over) State = EnumState.Over;
                    ev = new ClassEvent
                    {
                        Type = ClassEvent.EnumType.Over,
                        Param = _Name,
                        Val = _Value.ToString(CultureInfo.CurrentCulture),
                        SourceID = ID,
                        NameDevice = DeviceName
                    };
                    ClassEvent.SaveNewEvent(ev, _Archive);
                    Dispatcher.UIThread.Invoke(() => MainWindow.Events.Add(ev));
                    OnPropertyChanged(nameof(ColorLineChannel));
                }
                //Если задано минимальное ограничение величины и оно пройдено.
                else if (_Min.HasValue && _Value < _Min.Value)
                {
                    if (State != EnumState.Less) State = EnumState.Less;
                    ev = new ClassEvent
                    {
                        Type = ClassEvent.EnumType.Less,
                        Param = _Name,
                        Val = _Value.ToString(CultureInfo.CurrentCulture),
                        SourceID = ID,
                        NameDevice = DeviceName
                    };
                    ClassEvent.SaveNewEvent(ev, _Archive);
                    Dispatcher.UIThread.Invoke(() => MainWindow.Events.Add(ev));
                    OnPropertyChanged(nameof(ColorLineChannel));
                }
                //Если величина в пределах допуска и границы заданы.
                else if (_Max.HasValue || _Min.HasValue)
                {
                    if (State != EnumState.Norma) State = EnumState.Norma;
                    OnPropertyChanged(nameof(ColorLineChannel));
                }
                //Во всех других случаях.
                else
                {
                    if (State != EnumState.Unknown) State = EnumState.Unknown;
                    OnPropertyChanged(nameof(ColorLineChannel));
                }
                //Передача данных в мнемосхему.
                if (MainWindow.MnemoUnit.Count <= 0 ||
                    MainWindow.currentMainWindow.ContentArea.Content is not UserControlCanvas) return;
                foreach (var widget in MainWindow.MnemoUnit )
                {
                    var bindObj = widget.GetBindingObject();
                    if(bindObj==null) continue;
                    if (widget.GetTypeUnit().Equals(EnumUnit.ListParamDevice))
                    {
                        //var obj = new object[] { ID,Max,Min};
                        //Dispatcher.UIThread.Invoke(()=> widget.SetValue(Value,obj));
                        Dispatcher.UIThread.Invoke(()=> widget.SetValue(Value,this));
                    }
                    else
                    {
                        if (bindObj.IdParam==ID && bindObj.IdDevice==Device.ID )
                        {
                            Dispatcher.UIThread.Invoke(()=> widget.SetValue(Value));
                        } 
                    }
                }
            }
        }

        public decimal? Max
        {
            get { return _Max; }
            set
            {
                _Max = value;
                OnPropertyChanged();
            }
        }

        public decimal? Min
        {
            get { return _Min; }
            set
            {
                _Min = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public EnumState State
        {
            get { return _State; }
            set
            {
                _State = value;
                OnPropertyChanged();
            }
        }
        public EnumTypeRegistry TypeRegistry
        {
            get { return _TypeRegistry; }
            set
            {
                _TypeRegistry = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TypeRegistryName));
                OnPropertyChanged(nameof(TypeRegistryShortName));
                OnPropertyChanged(nameof(TypeRegistryFullName));
            }
        }

        [XmlIgnore]
        public ClassDevice Device
        {
            get { return _Device; }
            set
            {
                _Device = value;
                OnPropertyChanged("DeviceName");
            }
        }

       /// <summary>
       /// Данные в необработанном виде(массив байтов), полученные с устройств.
       /// </summary>
        [XmlIgnore]
        public ushort[] BaseValue
        {
            get { return _BaseValue; }
            set
            {
                _BaseValue = value;
                OnPropertyChanged();
                OnPropertyChanged("StrBaseValue");
            }
        }

        [XmlIgnore]
        public int ID { get; set; }
        
        /// <summary>
        /// Флаг, для записи входящего значения в архив.
        /// </summary>
        [XmlIgnore]
        public bool Archive
        {
            get { return _Archive; }
            set
            {
                _Archive = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Шлюз.
        /// </summary>
        [XmlIgnore]
        public int? Ext
        {
            get { return _Ext; }
            set
            {
                _Ext = value;
                OnPropertyChanged();
            }
        }

        public int? Accuracy
        {
            get { return _Accuracy; }
            set
            {
                _Accuracy = value;
                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public decimal? NValue
        {
            get { return _NValue; }
            set { _NValue = value; }
        }

        /// <summary>
        /// Тип регистра.
        /// </summary>
        public string TypeRegistryName
        {
            get
            {
                switch (TypeRegistry)
                {
                    case EnumTypeRegistry.DiscreteInput: return "Discrete Input";
                    case EnumTypeRegistry.CoilOutput: return "Coil Output";
                    case EnumTypeRegistry.InputRegistry: return "Input Registry";
                    case EnumTypeRegistry.HoldingRegistry: return "Holding Registry";
                    default: return "Unknown";
                }
            }
        }
        
        /// <summary>
        /// Тип регистра(короткое имя).
        /// </summary>
        public string TypeRegistryShortName
        {
            get
            {
                switch (TypeRegistry)
                {
                    case EnumTypeRegistry.DiscreteInput: return "DI";
                    case EnumTypeRegistry.CoilOutput: return "DO";
                    case EnumTypeRegistry.InputRegistry: return "AI";
                    case EnumTypeRegistry.HoldingRegistry: return "AO";
                    default: return "Unknown";
                }
            }
        }
        
        /// <summary>
        /// Тип регистра(полное имя).
        /// </summary>
        public string TypeRegistryFullName
        {
            get
            {
                switch (TypeRegistry)
                {
                    case EnumTypeRegistry.DiscreteInput: return "DI - Discrete Input";
                    case EnumTypeRegistry.CoilOutput: return "DO - Coil Output";
                    case EnumTypeRegistry.InputRegistry: return "AI - Input Registry";
                    case EnumTypeRegistry.HoldingRegistry: return "AO - Holding Registry";
                    default: return "Unknown";
                }
            }
        }
        public string DeviceName { get { return Device.Name; } }
        public string AddressHex { get { return "0x" + _Address.ToString("X4"); } }
        
        /// <summary>
        /// Актуальная дата в строковом формате(использована в табличном представлении регистров).
        /// </summary>
        /// <returns>string</returns>
        public string StrDTAct
        {
            get
            {
                if (DTAct == DateTime.MinValue) return "";
                return DTAct.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }
        
        /// <summary>
        /// Отображение полученных данных в формате hex.
        /// </summary>
        public string StrBaseValue
        {
            get
            {
                if (BaseValue == null) return "";
                string sFormat = "X4";
                if (_TypeRegistry == EnumTypeRegistry.CoilOutput ||
                    _TypeRegistry == EnumTypeRegistry.DiscreteInput) sFormat = "X2";
                string s = "0x";
                for (int i = 0; i < BaseValue.Length; i++)
                    s += BaseValue[i].ToString(sFormat);
                return s;
            }
        }

        public ClassChannel()
        {
            ID = 0;
            Name = "Канал 1";
            _Address = 0;
            TypeRegistry = EnumTypeRegistry.HoldingRegistry;
            Device = new ClassDevice();
            _Format = EnumFormat.UINT;
            _Koef = 1;
            _Value = 0;
            _DTAct = DateTime.MinValue;
            _PreviousValue = decimal.MinValue;
            _Archive = false;
        }

        public void LoadSavedValue(decimal SavedValue, DateTime dt)
        {
            _Value = SavedValue;
            _DTAct = dt;
            if (_Ext.HasValue)
            {
                int Number = _Ext.Value;
                if (_Value < 0) _Value *= (-1);
                ushort uValue = Convert.ToUInt16(_Value);
                //MainWindow.Slave.SetValue(Number, uValue);
            }
        }

        /// <summary>
        /// Вставка команды записи в регистр в очередь команд.
        /// </summary>
        /// <param name="Value"></param>
        public void SendValue(decimal Value)
        {
            ClassCommand cmd = new ClassCommand();
            cmd.Device = _Device;
            cmd.Channel = this;
            cmd.Address = _Address;
            if (_TypeRegistry == ClassChannel.EnumTypeRegistry.HoldingRegistry)
            {
                cmd.CommandType = ClassCommand.EnumType.WriteRegistry;
                if (_Koef == 1 || _Koef == 0) cmd.Value = (int)Value;
                else cmd.Value = (int)((float)Value / _Koef);
            }
            else if (_TypeRegistry == ClassChannel.EnumTypeRegistry.CoilOutput)
            {
                cmd.CommandType = ClassCommand.EnumType.WriteCoil;
                cmd.Value = (int)Value;
            }
            else return;
            MainWindow.QueueCommands.Enqueue(cmd);
           
        }

        /// <summary>
        /// Конвертация значений в знаковые величины.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private decimal ConvertMinus(decimal val)
        {
            if (val > 32767)
            {
                return val - ushort.MaxValue - 1;
            }
            return val;
        }

        private Avalonia.Media.IBrush _bColorLine = Brushes.Transparent;
        [XmlIgnore]
        public Avalonia.Media.IBrush ColorLineChannel
        {
            get
            {
                _bColorLine= State switch
                {
                    EnumState.Less => Brushes.Red,
                    EnumState.Over => Brushes.Red,
                    EnumState.Norma => Brushes.Chartreuse,
                    _ =>  MainWindow.Devices.FirstOrDefault(dev=>dev.ID==Device.ID)
                        !.Channels.IndexOf(this)%2==0 ? Brushes.Transparent: Brush.Parse("#20808080")
                };
                return _bColorLine;
            }
            set
            {
                _bColorLine = value;
                OnPropertyChanged(prop:nameof(ColorLineChannel));
            } 
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
