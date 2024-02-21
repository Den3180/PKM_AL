using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using PKM;

namespace PKM_AL.Classes.TransferClasses;

public class ClassGSM
{
            public delegate void EventHandlerStateChanged(bool State);
        public delegate void EventHandlerSendCommand(string Command);
        public delegate void EventHandlerReceivedMessage(string Message);
        public event EventHandlerStateChanged EventStateChanged;
        public event EventHandlerSendCommand EventSendCommand;
        public event EventHandlerReceivedMessage EventReceivedMessage;

        private enum EnumCommand
        {
            None,
            Any,
            GetMemory,
            ReadSMS,
            ReadSMSAll,
            DeleteSMS,
            GetPositionSMS,
            DeleteOneSMS
        }

        public enum EModePortModem
        {
            None,
            PortModemOpen,
            PortModemClose
        }

        private string PortNumber;
        // private int PortNumber;
        private SerialPort port;
        private int CountSMS;
        private int StartNumberSMS;
        private EnumCommand ActiveCommand;
        private int ActiveIndex;
        private string ActivePhone;

        public EModePortModem StatusPortModem { get; set; } = EModePortModem.None;

        List<int> PositionSMSList=new List<int>();

        public ClassGSM()
        {
        }
        
        /// <summary>
        /// Прослушивание входящих сообщений для GSM-модема.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
{            
             lock (new object())
             {
                    try
                    {
                        while (port.BytesToRead > 0)
                        {
                            string s;
                                s = port.ReadLine();
                            Parse(s);
                            EventReceivedMessage?.Invoke(s+$"-> COMMAND: {ActiveCommand}");
                        }
                    }
                    catch(Exception ex)
                    {
                        MainWindow.ExeptionLogging(ex);
                    }
             }
        }

        /// <summary>
        /// Включение модема.
        /// </summary>
        /// <param name="PortNumber"></param>
        //public void Start(int PortNumber)
        public void Start(string PortNumber)
        {
            this.PortNumber = PortNumber;
            if (PortOpen(PortNumber)) SendInit();
        }

        /// <summary>
        /// Открытие порта и включение прослушки для GSM.
        /// </summary>
        /// <param name="PortNumber"></param>
        /// <returns></returns>
        public bool PortOpen(string PortNumber)
        {
            if (port is { IsOpen: true })
            {
                if (port.PortName == PortNumber) 
                    port.Close();
                else 
                    port = null;
            }
            
            //port = new SerialPort("COM" + PortNumber);
            port = new SerialPort(PortNumber);
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.NewLine = "\r\n";
            try
            {
                port.Open();
                port.DataReceived += Port_DataReceived;
                StatusPortModem = EModePortModem.PortModemOpen;
            }
            catch (Exception Ex)
            {
                return false;
            }
            if (EventStateChanged != null) EventStateChanged(true);
            return true;
        }        

        /// <summary>
        /// Распарсивание сообщений.
        /// </summary>
        /// <param name="s">Строка сообщения</param>
        private void Parse(string s)
        {           
            int pos1, pos2;           
            //БКМ-5.
            if (s.StartsWith("B5"))
            {
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x =>((x.Protocol == ClassDevice.EnumProtocol.SMS) ||
                    (x.Protocol == ClassDevice.EnumProtocol.GPRS_SMS)) && (x.SIM == ActivePhone));
                if (device == null) return;
                ClassLog.WriteSMSLog(device?.Name + "-->" + s);
                //Вызов метода отслеживания факта получения пакета в основном потоке.
                device.PacketReceived();
                //Фиксация стартового адреса.
                int startAddres = Convert.ToInt32(s.Substring(2,4),16);
                //Фиксация текущего адреса.
                int currentAddres = startAddres;
                //Фиксация стартовой позиции.
                int pos = 6;
                // Максимальный адрес в карте регистров.
                int maxAddressReg = device.Channels.Max(ch => ch.Address);
                //Обход карты регистров устройства.
                while (pos < s.Length && currentAddres<maxAddressReg)
                {
                    ClassChannel classChannel = device.Channels.FirstOrDefault(ch => ch.Address == currentAddres);
                    if (classChannel == null || classChannel.TypeRegistry!=ClassChannel.EnumTypeRegistry.InputRegistry)
                    {
                        currentAddres++;
                        continue;
                    }
                    string sHex = string.Empty;
                    sHex = s.Substring(pos, 4);
                    if (classChannel.Format == ClassChannel.EnumFormat.SINT)
                        classChannel.Value = Convert.ToInt16(sHex, 16);
                    else classChannel.Value = Convert.ToInt32(sHex, 16);
                    if (classChannel.Ext.HasValue)
                    {
                        int Number = classChannel.Ext.Value;
                        ushort uValue = Convert.ToUInt16(sHex, 16);
                        MainWindow.Slave.SetValue(Number, uValue);
                    }
                    currentAddres++;
                    pos += 4;
                }
            }
            
            //БКМ-3.
            else if (s.StartsWith("55"))
            {
                //Если длина строки меньше 52 знаков.
                if (s.Length < 52) return;
                //Поиск среди устройств(выбран ли протокол sms)
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x =>
                    ((x.Protocol == ClassDevice.EnumProtocol.SMS) ||
                    (x.Protocol == ClassDevice.EnumProtocol.GPRS_SMS)) && (x.SIM == ActivePhone));
                //Если нет - выход.
                if (device == null) return;
                //Подсчет пакетов.
                device.PacketReceived();
                foreach (ClassChannel channel in device.Channels)
                {                    
                    string sHex=string.Empty;
                    //Если адрес канала меньше 54.
                    if (channel.Address <= 54)
                        sHex = s.Substring(channel.Address, 4);
                    //1 символ(вскрытие корпуса)
                    else if (channel.Address == 58)
                        sHex = s.Substring(channel.Address, 1);
                    //1 символ(превышение уставки)
                    else if (channel.Address == 59)
                        sHex = s.Substring(channel.Address, 1);
                    //Количество попыток смс.
                    else if (channel.Address == 60)
                    {
                        channel.Value = Convert.ToInt32(s.Substring(channel.Address, 4));
                        continue;
                    }
                    //Уровень сигнала GSM. Конец карты регистров. 
                    else if (channel.Address == 64)
                    {
                        channel.Value = Convert.ToInt32(s.Substring(channel.Address, 4));
                        break;
                    }
                    else continue;
                    if (channel.Format == ClassChannel.EnumFormat.SINT)
                        channel.Value = Convert.ToInt16(sHex, 16);
                    else channel.Value = Convert.ToInt32(sHex, 16);
                    if (channel.Ext.HasValue)
                    {
                        int Number = channel.Ext.Value;
                        ushort uValue = Convert.ToUInt16(sHex, 16);
                        MainWindow.Slave.SetValue(Number, uValue);
                    }
                }
            return;
            }

            //Перехват ошибки модема.
            if (s.Contains("ERROR")) 
            {
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x => ((x.Protocol == ClassDevice.EnumProtocol.SMS) ||
                                    (x.Protocol == ClassDevice.EnumProtocol.GPRS_SMS)) && (x.SIM == ActivePhone));
                ClassLog.WriteSMSLog(device?.Name+"-->" + s);
                if ((ActiveCommand == EnumCommand.GetMemory || ActiveCommand == EnumCommand.ReadSMS) && CountSMS>0)
                {
                    ActiveIndex++;
                    SendReadSMS(ActiveIndex);                    
                    return;
                }
            }

            if (s.StartsWith("OPEN DOOR"))
            {
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x =>
                    ((x.Protocol == ClassDevice.EnumProtocol.SMS) ||
                    (x.Protocol == ClassDevice.EnumProtocol.GPRS_SMS)) && (x.SIM == ActivePhone));
                if (device == null) return;
                if (device.Model != ClassDevice.EnumModel.BKM_4) return;
                device.PacketReceived();
                foreach (ClassChannel channel in device.Channels)
                {
                    if (channel.Address == 53) channel.Value = 1;
                }
                return;
            }

            if (s.StartsWith("IP="))
            {
                if (s.Length < 90) return;
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x =>
                    ((x.Protocol == ClassDevice.EnumProtocol.SMS) ||
                    (x.Protocol == ClassDevice.EnumProtocol.GPRS_SMS)) && (x.SIM == ActivePhone));
                if (device == null) return;
                if (device.Model != ClassDevice.EnumModel.BKM_4) return;
                device.PacketReceived();
                pos1 = s.IndexOf("potencial=");
                if (pos1 <= 0) return;
                string[] sP = new string[8];
                for (int i = 0; i < 8; i++)
                    sP[i] = s.Substring(pos1 + 10 + (i * 5), 4);
                foreach (ClassChannel channel in device.Channels)
                {
                    if (channel.Address >= 0 && channel.Address < 8) 
                        channel.Value = Convert.ToInt16(sP[channel.Address]);
                }
                return;
            }

            
            
            //Заголовочная строка перед строкой с данными.
            if (s.StartsWith("+CMGR:"))
            {
                if(s.Contains("REC READ"))
                {
                    ActivePhone = string.Empty;
                    SendDeleteSMS();
                    return;
                }
                pos1 = s.IndexOf(',');
                if (pos1 > 0)
                {
                    pos2 = s.IndexOf(',', pos1 + 1);
                    if (pos2 > pos1)
                    {
                        //Парсим номер телефона.
                        ActivePhone = s.Substring(pos1 + 2, pos2 - pos1 - 3);
                        return;
                    } 
                }               
            }

            //Отчет о количестве смс.
            if (s.StartsWith("+CPMS:"))
            {
                pos1 = s.IndexOf(',');
                if (pos1 > 0)
                {
                    pos2 = s.IndexOf(',', pos1 + 1);
                    if (pos2 > pos1)
                    {
                        //Парсим количество СМС.
                        string a = s.Substring(pos1 + 1, pos2 - pos1 - 1);
                        CountSMS = Convert.ToInt32(a);
                        return;
                    }
                }
            }

            if (s.StartsWith("+CMGD"))
            {
                int indexStart = s.IndexOf("(")+1;
                for(int i= indexStart; i<s.Length;i++)
                {
                    if (s[i] == ')') break;
                    if (s[i] == ',') continue;
                    PositionSMSList.Add(Convert.ToInt32(s[i].ToString()));
                }
                return;
            }

            if (s.StartsWith("OK"))
            {
                if (ActiveCommand == EnumCommand.GetMemory)
                {
                    if (CountSMS > 0)
                    {
                        SendInfo();
                        return;
                    }
                }

                if(ActiveCommand == EnumCommand.GetPositionSMS)
                {
                    SendReadSMS(PositionSMSList[0]);
                    PositionSMSList.RemoveAt(0);
                }
                if (ActiveCommand == EnumCommand.ReadSMS)
                {
                    if (PositionSMSList.Count>0)
                    {
                        SendReadSMS(PositionSMSList[0]);
                        PositionSMSList.RemoveAt(0);                        
                    }
                    else
                    {
                        SendDeleteSMS();                        
                    }
                    return;
                }               
            }
        }

        /// <summary>
        /// Отправка команд на модем.
        /// </summary>
        /// <param name="Command"></param>
        private void Send(string Command)
        {
            if (port.IsOpen)
            {
                port.WriteLine(Command);
                EventSendCommand?.Invoke(Command);
            }
            else
            {
                StatusPortModem = EModePortModem.None;
                if (EventStateChanged != null) EventStateChanged(false);
            }
        }

        /// <summary>
        /// Пинг на модем.
        /// </summary>
        public void SendPoll()
        {
            ActiveCommand = EnumCommand.Any;
            Send("AT");
        }

        /// <summary>
        /// Hачальная инициация модема. Перевод его в текстовый формат.
        /// </summary>
        public void SendInit()
        {
            ActiveCommand = EnumCommand.Any;
            Send("AT+CMGF=1");
        }

        /// <summary>
         /// Запрос о качестве сигнала.
         /// </summary>
        public void SendSignal()
        {
            ActiveCommand = EnumCommand.Any;
            Send("AT+CSQ");
        }

        /// <summary>
        /// Сообщает о наличие сообщений в хранилище.
        /// </summary>
        public void SendGetMemory()
        {
            ActiveCommand = EnumCommand.GetMemory;
            Send("AT+CPMS?");
        }

        /// <summary>
        /// Чтение сообщений по номеру.
        /// </summary>
        /// <param name="Index"></param>
        public void SendReadSMS(int Index)
        {
            //Назначение активной команды.
            ActiveCommand = EnumCommand.ReadSMS;
            //Присваивание индекса сообщения.
            ActiveIndex = Index;
            Send("AT+CMGR=" + Index.ToString());
        }

        /// <summary>
        /// Чтение всех не прочитанных SMS. 
        /// </summary>
        public void SendReadSMSAll()
        {
            Send("AT+CMGL=\"REC UNREAD\"");
            ActiveCommand = EnumCommand.ReadSMSAll;
        }

        /// <summary>
        /// Запрос информации об индексах входящих сообщений.
        /// </summary>
        public void SendInfo()
        {
            Send("AT+CMGD=?");
            ActiveCommand = EnumCommand.GetPositionSMS;
        }

        /// <summary>
        /// Удаляет все смс из памяти модема.
        /// </summary>
        public void SendDeleteSMS()
        {
            ActiveCommand = EnumCommand.DeleteSMS;
            Send("AT+CMGD=1,3");           
            ActiveIndex = 0;
        }

}