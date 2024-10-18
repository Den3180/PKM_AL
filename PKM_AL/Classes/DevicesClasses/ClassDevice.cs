using Modbus.Device;
using PKM;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using PKM_AL.Classes;

namespace PKM_AL
{
    public class ClassDevice:MyPropertyChanged
    {
        /// <summary>
        /// Перечисление статусов подключения.
        /// </summary>
        public enum EnumLink
        {
            Unknown = 0,
            LinkNo = 1,
            LinkYes = 2,
            LinkConnect = 3,
            LinkSuspended=4
        }
        /// <summary>
        /// Перечисление протоколов подключения.
        /// </summary>
        public enum EnumProtocol
        {
            RTU = 1,
            TCP = 2,
            SMS = 3,
            GPRS = 4,
            GPRS_SMS = 5
        }
        /// <summary>
        /// Перечисление типов устройств.
        /// </summary>
        public enum EnumModel
        {
            None = 0,
            BKM_3 = 1,
            BKM_4 = 2,
            SKZ = 3,
            SKZ_IP = 4,
            BSZ = 5,
            USIKP = 6,
            BKM_5 = 7,
            KIP = 8,
            MMPR = 9
        }

        private string _Name;
        private int _Address;
        private EnumProtocol _Protocol;
        private int _Period;
        private string _IPAddress;
        private int _IPPort;
        private EnumLink _LinkState;
        private int _TxCounter;
        private int _RxCounter;
        private int _PacketLost;
        private DateTime _DTAct;
        private string _ComPort;
        private string _SIM;
        private EnumModel _Model;
        private DateTime _DTConnect;
        private bool _WaitAnswer;
        private string _Picket;
        private double _Latitude = 00.000000D;
        private double _Longitude = 00.000000D;
        private double _Elevation;
        private string commParam;
        private bool isPoll;
        private Avalonia.Media.IBrush _bColorLine = Brushes.Beige; //Цвет линии таблицы устройств.
        //private BitmapImage _bitmap;
        // private double _UnomInSKZ;
        // private double _NactiveSKZ;
        // private double _NfullInSKZ;
        // private double _UnomOutSKZ;
        // private double _InomOutSKZ;
        // private double _NnomOutSKZ;
        private string _LinkStateName = string.Empty;


        public double UnomInSKZ { get; set; }
        public double NactiveSKZ { get; set; }
        public double NfullInSKZ { get; set; }
        public double UnomOutSKZ { get; set; }
        public double InomOutSKZ { get; set; }
        public double NnomOutSKZ { get; set; }
        
        /// <summary>
        ///Количество элементов в группе.
        /// </summary>
        [XmlIgnore]
        public int CountGroups { get; set; }

        /// <summary>
        /// Флаг процесса опроса.
        /// </summary>
        [XmlIgnore]
        public bool InProcess { get; set; }

        [XmlIgnore]
        public int CountNumber { get; set; }

        [XmlIgnore]
        public int ID { get; set; }

        [XmlIgnore]
        public ModbusMaster Master { get; set; }

        [XmlIgnore]
        public string ServerID { get; set; }

        [XmlIgnore]
        public string NominalU { get; set; }

        [XmlIgnore]
        public string NominalI { get; set; }

        //Код предприятия (СКЗ)
        [XmlIgnore]
        public string FactoryCode { get; set; } = string.Empty;
        //Число модулей СКЗ.
        [XmlIgnore]
        public int ModulesCount { get; set; }
        //Год производства СКЗ.
        [XmlIgnore]
        public int FactoryYear { get; set; }
        //Дата начала эксплуатации СКЗ.
        [XmlIgnore]
        public DateTime DateStart { get; set; } = DateTime.Now;
        //Заводской номер СКЗ.
        [XmlIgnore]
        public int FactoryNumber { get; set; }
        //Срок службы.
        [XmlIgnore]
        public int Resource { get; set; }

        public List<ClassChannel> Channels { get; set; }

        [XmlIgnore]
        public DateTime LastRequestDT { get; set; }

        //Имя устройства.
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        
        /// <summary>
        /// Разрешение на опрос.
        /// </summary>
        [XmlIgnore]
        public bool IsPoll
        {
            get => isPoll;
            set
            {
                isPoll = value;
                _LinkState = isPoll switch
                {
                    true=>EnumLink.Unknown,
                    _=>EnumLink.LinkSuspended
                };
                OnPropertyChanged();
                OnPropertyChanged(nameof(LinkStateName));
                OnPropertyChanged(nameof(ColorLineDevice));
            }
        }
        
        ///<summary>
        ///Адрес устройства.
        /// </summary>
        public int Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        //Протокол связи.
        public EnumProtocol Protocol
        {
            get { return _Protocol; }
            set
            {
                _Protocol = value;
                OnPropertyChanged(nameof(Protocol));
                OnPropertyChanged(nameof(ProtocolName));
                OnPropertyChanged(nameof(CommParam));
            }
        }
        //Период опроса.
        public int Period
        {
            get { return _Period; }
            set
            {
                _Period = value;
                OnPropertyChanged(nameof(Period));
            }
        }
        //IP адрес.
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                _IPAddress = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }
        //IP порт.
        public int IPPort
        {
            get { return _IPPort; }
            set
            {
                _IPPort = value;
                OnPropertyChanged(nameof(IPPort));
            }
        }
        //COM порт.
        public string ComPort
        {
            get { return _ComPort; }
            set
            {
                _ComPort = value;
                OnPropertyChanged(nameof(ComPort));
            }
        }
        //Номер сим-карты.        
        public string SIM
        {
            get { return _SIM; }
            set
            {
                _SIM = value;
                OnPropertyChanged(nameof(SIM));
            }
        }
        //Модель устройства.
        public EnumModel Model
        {
            get { return _Model; }
            set
            {
                _Model = value;
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(ModelName));
            }
        }
        //Пикет.
        public string Picket
        {
            get { return _Picket; }
            set
            {
                _Picket = value;
                OnPropertyChanged(nameof(Picket));
            }
        }
        //Широта(координата).
        public double Latitude
        {
            get { return _Latitude; }
            set
            {
                _Latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
        }
        // Высота(координата).
        public double Longitude
        {
            get { return _Longitude; }
            set
            {
                _Longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
        }
        //Высота над уровнем моря.
        public double Elevation
        {
            get { return _Elevation; }
            set
            {
                _Elevation = value;
                OnPropertyChanged(nameof(Elevation));
            }
        }
        
        //Карта для маркера.
        //[XmlIgnore]
        //public BitmapImage Bitmap
        //{
        //    get { return _bitmap; }
        //    set
        //    {
        //        _bitmap = value;
        //        OnPropertyChanged("Bitmap");
        //    }
        //}

        [XmlIgnore]
        public DateTime DTAct
        {
            get { return _DTAct; }
            set
            {
                _DTAct = value;
                OnPropertyChanged(nameof(DTAct));
            }
        }
        /// <summary>
        /// Тип протокола обмена данными.
        /// </summary>
        public string ProtocolName
        {
            get
            {
                switch (_Protocol)
                {
                    case EnumProtocol.RTU: return "Modbus RTU";
                    case EnumProtocol.TCP: return "Modbus TCP";
                    case EnumProtocol.SMS: return "GSM SMS";
                    case EnumProtocol.GPRS: return "GPRS";
                    case EnumProtocol.GPRS_SMS: return "GPRS SMS";
                    default: return "";
                }
            }
        }

        public string ModelName
        {
            get
            {
                switch (_Model)
                {
                    case EnumModel.None: return "";
                    case EnumModel.BKM_3: return "БКМ-3";
                    case EnumModel.BKM_4: return "БКМ-4";
                    case EnumModel.SKZ: return "СКЗ";
                    case EnumModel.SKZ_IP: return "СКЗ-ИП";
                    case EnumModel.BSZ: return "БСЗЭ";
                    case EnumModel.USIKP: return "УСИКП";
                    case EnumModel.BKM_5: return "БКМ-5";
                    case EnumModel.KIP: return "КИП";
                    case EnumModel.MMPR: return "ММПР";
                    default: return "";
                }
            }
        }

        public EnumLink LinkState { get { return _LinkState; } }

        [XmlIgnore]
        public Avalonia.Media.IBrush ColorLineDevice
        {
            get
            {
               _bColorLine= _LinkState switch
                {
                    EnumLink.LinkNo => Brushes.Red,
                    EnumLink.LinkYes => Brushes.Chartreuse,
                    _ => Brushes.Transparent
                };
                return _bColorLine;
            }
            set
            {
                _bColorLine = value;
                OnPropertyChanged();
            } 
        }

        
        public string LinkStateName
        {
            get
            {
                _LinkStateName = _LinkState switch
                {
                    EnumLink.LinkNo => "Нет связи",
                    EnumLink.LinkYes => "На связи",
                    EnumLink.LinkConnect => "Подключение",
                    EnumLink.LinkSuspended=>"Отключено",
                    _ => "Неизвестно"
                };
                return _LinkStateName;
            }
            set
            {
                _LinkStateName = value;
                OnPropertyChanged(nameof(LinkStateName));
            }
        }

        public string PacketStatistics
        {
            get { return _TxCounter.ToString() + "/" + _RxCounter.ToString(); }
        }

        public int PacketTxCount { get { return _TxCounter; } }

        public int PacketRxCount { get { return _RxCounter; } }

        public string CommParam
        {
            get
            {
                if ((_Protocol == EnumProtocol.TCP) || (_Protocol == EnumProtocol.GPRS)
                                                    || (_Protocol == EnumProtocol.GPRS_SMS))
                {
                    commParam = _IPAddress + ":" + _IPPort.ToString();
                }
                else if (_ComPort != "")
                {
                    commParam = _ComPort;
                }
                else if (_Protocol == EnumProtocol.SMS)
                {
                    commParam = MainWindow.settings.PortModem;
                }
                else if (MainWindow.settings.PortModbus == "")
                {
                    commParam = "Нет";
                }
                else
                {
                    commParam =MainWindow.settings.PortModbus;
                }
                return commParam;
            }
            set
            {
                commParam = value;
                OnPropertyChanged(nameof(CommParam));
            }
        }

        public bool WaitAnswer { get { return _WaitAnswer; } }

        public ClassDevice()
        {
            ID = 0;
            Channels = new List<ClassChannel>();
            _Protocol = EnumProtocol.RTU;
            _Period = 60;
            _IPAddress = "192.168.0.1";
            _IPPort = 502;
            _Address = 1;
            _LinkState = EnumLink.Unknown;
            _ComPort = "";
            _SIM = "";
            _Model = EnumModel.None;
            _DTConnect = DateTime.MinValue;
            _WaitAnswer = false;
            //_bitmap = new BitmapImage(new Uri(@"/Resources/Markers/Marker_gray.png", UriKind.Relative));
            _Picket = string.Empty;
            CountGroups = 0;
            InProcess = false;
            isPoll = false;
        }


        /// <summary>
        /// Подсчет отправленных пакетов.
        /// </summary>
        public void PacketSended()
        {
            _TxCounter++;
            if (_TxCounter > 10000) _TxCounter = 0;
            OnPropertyChanged(nameof(PacketStatistics));
            _WaitAnswer = true;
        }
      
        /// <summary>
        /// Увеличение счетчика пакетов, выставление индикации, если пришел пакет, добавление события.
        /// </summary>
        public void PacketReceived()
        {
            _WaitAnswer = false;
            if(!IsPoll) return;
            _RxCounter++;
            if (_RxCounter > 10000) _RxCounter = 0;
            OnPropertyChanged(nameof(PacketStatistics));
            if (_LinkState != EnumLink.LinkYes)
            {
                _LinkState = EnumLink.LinkYes;
                OnPropertyChanged(nameof(LinkState));
                OnPropertyChanged(nameof(LinkStateName));
                OnPropertyChanged(nameof(ColorLineDevice));
                ClassEvent ev = new ClassEvent() { Type = ClassEvent.EnumType.Connect, Param = _Name, NameDevice = Name };
                MainWindow.DB.EventAdd(ev);
                Dispatcher.UIThread.Invoke(()=>MainWindow.Events.Add(ev));
            }
            _PacketLost = 0;
            _DTAct = DateTime.Now;
            //Изменение маркера на карте.
            //Bitmap = new BitmapImage(new Uri(@"/Resources/Markers/Marker_blue.png", UriKind.Relative));
        }

        /// <summary>
        /// Счетчик потеряных пакетов, индикация контрола устройств.
        /// </summary>
        public void PacketNotReceived()
        {
            int PACKET_LOST_MAX = 1000;
            int PACKET_LOST_ALARM = 3;
            if (_Model == EnumModel.BKM_3) PACKET_LOST_ALARM = 5;
            _WaitAnswer = false;
            if(!IsPoll) return;
            if (_PacketLost < PACKET_LOST_MAX) _PacketLost++;
            if (_PacketLost < PACKET_LOST_ALARM) return;
            if (_LinkState != EnumLink.LinkNo)
            {
                _LinkState = EnumLink.LinkNo;
                OnPropertyChanged(nameof(LinkState));
                OnPropertyChanged(nameof(LinkStateName));
                OnPropertyChanged(nameof(ColorLineDevice));
                ClassEvent ev = new ClassEvent() { Type = ClassEvent.EnumType.Disconnect, Param = _Name, NameDevice = Name };
                MainWindow.DB.EventAdd(ev);
                Dispatcher.UIThread.Invoke(()=> MainWindow.Events.Add(ev));
                if (ev.Category is ClassEvent.EnumCategory.Fault or ClassEvent.EnumCategory.Alarm)
                {
                    Dispatcher.UIThread.Invoke(()=>MainWindow.EventsAlarm.Add(ev));
                }
            }
            //Изменение маркера на карте.
            //Bitmap = new BitmapImage(new Uri(@"/Assets/Markers/Marker_gray.png", UriKind.Relative));
        }

        /// <summary>
        /// Индикация доступности порта в контроле устройств.
        /// </summary>
        public void PortDisable()
        {
            _LinkState = EnumLink.Unknown;
            OnPropertyChanged(nameof(LinkState));
            OnPropertyChanged(nameof(LinkStateName));
            OnPropertyChanged(nameof(ColorLineDevice));
        }
        
        /// <summary>
        ///Сохранение профиля устройства в файл. 
        /// </summary>
        /// <param name="FileName">Путь к файлу</param>
        /// <returns></returns>
        public bool SaveProfile(string FileName)
        {
            TextWriter writer = new StreamWriter(FileName);
            XmlSerializer serializer = new XmlSerializer(typeof(ClassDevice));
            serializer.Serialize(writer, this);
            writer.Close();
            return true;
        }

        /// <summary>
        /// Загрузка устройства из файла.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static ClassDevice Load(string FileName)
        {
            ClassDevice device = null;
            using TextReader reader = new StreamReader(FileName);
            XmlSerializer serializer = new XmlSerializer(typeof(ClassDevice));
            try
            {
                device = (ClassDevice)serializer.Deserialize(reader);
            }
            catch (Exception Ex)
            {
                ClassLog.Write(Ex.Message);
            }
            return device;
        }

        /// <summary>
        /// Соединение TCP с устройством. 
        /// </summary>
        public void TCPConnect()
        {
            //????
            if (_DTConnect.AddSeconds(_Period).CompareTo(DateTime.Now) > 0) return;
            _DTConnect = DateTime.Now;
            _LinkState = EnumLink.LinkConnect;
            OnPropertyChanged(nameof(LinkState));
            OnPropertyChanged(nameof(LinkStateName));
            TcpClient tcpClient = new TcpClient();
            IAsyncResult asyncResult = tcpClient.BeginConnect(_IPAddress, _IPPort,
                OnTCPConnectCallback, tcpClient);
        }

        /// <summary>
        /// Функция обратного вызова. Соединение с устройством TCP. 
        /// </summary>
        /// <param name="result"></param>
        private void OnTCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = (TcpClient)result.AsyncState;
            if (client.Connected)
            {
                client.EndConnect(result);
                ModbusIpMaster master = ModbusIpMaster.CreateIp(client);
                master.Transport.ReadTimeout = MainWindow.settings.Timeout;
                master.Transport.WriteTimeout = MainWindow.settings.Timeout;
                master.Transport.Retries = 0;
                Master = master;
            }
            else
            {
                Master = null;
                if (_Protocol == EnumProtocol.TCP) _LinkState = EnumLink.LinkNo;
                else _LinkState = EnumLink.Unknown;
                OnPropertyChanged(nameof(LinkState));
                OnPropertyChanged(nameof(LinkStateName));
            }
        }

        /// <summary>
        /// Разбиение списка регистров устройства на группы по 100 или по типу.
        /// </summary>
        /// <returns></returns>
        public List<ClassGroupRequest> GetGroups()
        {
            int MaxCount = 100;
            int MaxSpace = 1;
            List<ClassGroupRequest> Groups = new List<ClassGroupRequest>();
            List<ClassChannel> lstChannels = new List<ClassChannel>();
            lstChannels = Channels.OrderBy(x => x.TypeRegistry).ThenBy(x => x.Address).ToList();
            if (lstChannels.Count == 0) return Groups;
            Groups.Add(new ClassGroupRequest(lstChannels[0].TypeRegistry));
            foreach (ClassChannel item in lstChannels)
            {
                ClassGroupRequest group = Groups[^1];
                if ((group.TypeRegistry != item.TypeRegistry) || (group.GetSize() > MaxCount) ||
                    ((item.Address - group.GetLastAddress()) > MaxSpace))
                {
                    Groups.Add(new ClassGroupRequest(item.TypeRegistry));
                }
                Groups[^1].AddChannel(item);
            }
            CountGroups = Groups.Count;
            return Groups;
        }

        /// <summary>
        /// Определение текущего режима работы СКЗ.
        /// </summary>
        /// <returns></returns>
        public string GetModeName()
        {
            if (_Model != EnumModel.SKZ_IP) return "";
            if (_LinkState != EnumLink.LinkYes) return "";
            //Получить регистр адрес, которого совпадает с 7 и тип регистра DI.
            ClassChannel ch1 = Channels.FirstOrDefault(x => x.Address == 0x07 &&
                x.TypeRegistry == ClassChannel.EnumTypeRegistry.DiscreteInput);
            //Получить регистр адрес, которого совпадает с 8 и тип регистра DI.
            ClassChannel ch2 = Channels.FirstOrDefault(x => x.Address == 0x08 &&
                x.TypeRegistry == ClassChannel.EnumTypeRegistry.DiscreteInput);
            //Выход если регистры пусты.
            if (ch1 == null || ch2 == null) return "";
            string s = "";
            //Определение текущего режима работы СКЗ.
            if (ch1.Value == 0 && ch2.Value == 0) s = "Стабилизация тока";
            if (ch1.Value == 0 && ch2.Value == 1) s = "Стабилизация суммарного потенциала";
            if (ch1.Value == 1 && ch2.Value == 0) s = "Стабилизация поляризационного потенциала";
            if (ch1.Value == 1 && ch2.Value == 1) s = "Стабилизация напряжения";
            return s;
        }

        /// <summary>
        /// Отправка запроса на чтение серийного номера к устройству(СКЗ).
        /// </summary>
        /// <param name="port"></param>
        public void SendReportServerID(SerialPort port)
        {
            byte[] b = new byte[2];
            //Адрес устройства.
            b[0] = (byte)_Address;
            //Функция чтения серийного номера.
            b[1] = 0x11;
            //Вычисление контрольной суммы.
            byte[] crc = Modbus.Utility.ModbusUtility.CalculateCrc(b); //0-low, 1-high
            byte[] mes = new byte[b.Length + crc.Length];
            b.CopyTo(mes, 0);
            crc.CopyTo(mes, mes.Length - crc.Length);
            port.Write(mes, 0, mes.Length);
        }

        /// <summary>
        /// Определяет необходимость сделать опрос каналов в соответствии с выбраным периодом опроса.
        /// </summary>
        /// <returns></returns>
        public bool IsNeedRequest()
        {
            // foreach (ClassChannel channel in Channels)
            // {
            //     if (DateTime.Now.Subtract(channel.DTAct).TotalSeconds > Period) return true;
            // }
            return DateTime.Now.Subtract(DTAct).TotalSeconds > Period;
        }
        
        /// <summary>
        /// Конвертация данных от ММПР. 
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="typeRegistry"></param>
        /// <returns></returns>
        public static decimal ConvertMMPRData(decimal _value, ClassChannel channel)
        {
            if(channel.TypeRegistry==ClassChannel.EnumTypeRegistry.CoilOutput || channel.TypeRegistry == ClassChannel.EnumTypeRegistry.DiscreteInput)
            {
                return _value;
            }
            decimal res= channel.Address switch
            {
                0 => (decimal)((_value / 4095) * 75),
                1=> (decimal)((_value/4095)*100),
                2=> (decimal)(5-(_value*10)/4095),
                3=> (decimal)((_value/4095)*10),
                _=>_value
            };
            return Decimal.Round(res, 2);
        }

        public decimal GetParam(int Param)
        {
            if (_Model == EnumModel.USIKP)
            {
                ClassChannel channelLow = null;
                ClassChannel channelHigh = null;
                if (Param == 1)
                {
                    channelLow = Channels.FirstOrDefault(x => x.Address == 49);
                    channelHigh = Channels.FirstOrDefault(x => x.Address == 50);
                }
                if (Param == 2)
                {
                    channelLow = Channels.FirstOrDefault(x => x.Address == 51);
                    channelHigh = Channels.FirstOrDefault(x => x.Address == 52);
                }
                if (channelLow != null && channelHigh != null)
                    return (channelHigh.Value * 256 + channelLow.Value);
            }
            return 0;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }
}
