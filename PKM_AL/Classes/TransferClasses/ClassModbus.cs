using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Threading;
using Modbus.Device;
using Modbus.Utility;
using PKM;

namespace PKM_AL.Classes.TransferClasses;

public class ClassModbus
{
    public delegate void PortErrorEventHandler(string ErrorMessage);
    public delegate void SendRequestEventHandler();
    public delegate void ReceivedAnswerEventHandler();
    public event PortErrorEventHandler PortErrorEvent;
    public event SendRequestEventHandler SendRequestEvent;
    public event ReceivedAnswerEventHandler ReceivedAnswerEvent;
    
    /// <summary>
    /// Режимы последовательного порта.
    /// </summary>
    public enum eMode
    {
        None = 0,
        PortOpen = 1,
        RequestServerID = 2,
        MasterInit = 3,
        PortClosed = 4            
    }
    /// <summary>
    /// Типы функций протокола Modbus.
    /// </summary>
    public enum eFunction
    {
        None = 0,
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleRegister = 6,
        WriteMultipleCoils = 15,
        WriteMultipleRegisters = 16
    }

    /// <summary>
    /// Режим порта.
    /// </summary>
    public eMode Mode { get; set; }

    private SerialPort port;
    private ModbusSerialMaster RTUMaster;
    //Массив для считываемых из регистров данных. 
    private ushort[] data;
    private bool[] dataBool;
    private ClassDevice CustomDevice;
    private byte[] CustomMessage;
    private long LastTickPoll;
    private System.Timers.Timer TimerSec;
    private int DelaySec;
    private bool WaitAnswer; //only COM
    private readonly int numOfRegMax=125;
    private readonly int numOfRegMin=1;

    public ClassModbus()
    {
        port = null;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        TimerSec = new System.Timers.Timer();
        TimerSec.Interval = 1000;
        TimerSec.Elapsed += TimerSec_Elapsed;
        TimerSec.Start();
    }
    
    /// <summary>
    /// Таймер модбас.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerSec_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        //Начальное значение 0.
        DelaySec++;
        if (Mode == eMode.RequestServerID)
        {
            if (DelaySec >= 2) MasterInit();
        }
    }
    
    /// <summary>
    /// Подключение по TCP протоколу каждого устройства.
    /// </summary>
    public void TCPConnect()
    {
        foreach (ClassDevice device in MainWindow.Devices)
        {
            if (device.Protocol != ClassDevice.EnumProtocol.TCP &&
                device.Protocol != ClassDevice.EnumProtocol.GPRS &&
                device.Protocol != ClassDevice.EnumProtocol.GPRS_SMS) continue;
            if (device.Master == null || device.LinkState == ClassDevice.EnumLink.LinkNo)
            {
                device.TCPConnect();
            }
        }
    }
    
    object locker = new object();
    
    /// <summary>
    /// Открытие последовательного порта.
    /// </summary>
    /// <param name="PortNumber"></param>
    /// <param name="BaudRate"></param>
    /// <param name="DataBits"></param>
    /// <param name="iParity"></param>
    /// <param name="iStopBits"></param>
    /// <returns></returns>
    // public bool PortOpen(int PortNumber, int BaudRate, int DataBits, int iParity, int iStopBits)
    public bool PortOpen(string PortNumber="", int BaudRate=9600, int DataBits=8, int iParity=0, int iStopBits=1)
    {
        if (port is { IsOpen: true })
        {
            if (port.PortName == PortNumber) 
                port.Close();
            else 
                port = null;
        }
      
        port = new SerialPort(PortNumber)
            {
                BaudRate = BaudRate,            //Скорость.
                DataBits = DataBits,            //Длина слова.
                Parity = (Parity)iParity,       //Четность.
                StopBits = (StopBits)iStopBits  //Стоп бит.
            };
            try
            {
                port.Open();
            }
            catch (Exception Ex)
            {
                PortErrorEvent?.Invoke(Ex.Message);
                return false;
            }
            Mode = eMode.PortOpen;
            return true;
    }
    
    /// <summary>
    /// Закрытие порта.
    /// </summary>
    public void PortClose()
    {
        if (port != null && port.IsOpen) port.Close();
        Mode = eMode.PortClosed;
    }

    /// <summary>
    /// Запрос на получение ID для СКЗ-ИП.
    /// </summary>
    public void RequestServerID()
    {
        port.DataReceived += Port_DataReceived;
        System.Threading.Thread.Sleep(10);
        for (int i = 0; i < MainWindow.Devices.Count; i++)
        {
            ClassDevice device = MainWindow.Devices[i];
            if (device.Model != ClassDevice.EnumModel.SKZ_IP) continue;
            //Если идентификационные данные уже есть в устройстве, то переход к следующему устройству списка.
            if (!string.IsNullOrEmpty(device.ServerID)) continue;
            //Массив для текущих сообщений в ноль.
            CustomMessage = new byte[0];
            CustomDevice = device;
            device.SendReportServerID(port);
            System.Threading.Thread.Sleep(10);
        }
        Mode = eMode.RequestServerID;
        DelaySec = 0;
    }

    /// <summary>
    /// Инициация мастера RTU.
    /// </summary>
    public void MasterInit()
    {
        //Отключаем прослушку порта.
        port.DataReceived -= Port_DataReceived;
        //Создаем объект мастера.
        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);            
        //Настройки мастера.
        master.Transport.Retries = 0;
        master.Transport.ReadTimeout = MainWindow.settings.Timeout;
        master.Transport.WriteTimeout = MainWindow.settings.Timeout;
        RTUMaster = master;
        Mode = eMode.MasterInit;
    }
    
    /// <summary>
    /// Прослушивание входящих сообщений с устройства.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        //Если нет устройства - выход.
        if (CustomDevice == null) return;
        //Количество байт в сообщении.
        int n = port.BytesToRead;
        byte[] b = new byte[n];
        //Чтение сообщения в буфер.
        port.Read(b, 0, b.Length);
        byte[] TempMessage = new byte[CustomMessage.Length];
        CustomMessage.CopyTo(TempMessage, 0);
        CustomMessage = new byte[CustomMessage.Length + b.Length];
        TempMessage.CopyTo(CustomMessage, 0);
        b.CopyTo(CustomMessage, CustomMessage.Length - b.Length);
        if (CustomMessage.Length < (5 + 21)) return;
        CustomDevice.ServerID = Encoding.GetEncoding(1251).GetString(CustomMessage, 3, 10);
        CustomDevice.NominalU = Encoding.GetEncoding(1251).GetString(CustomMessage, 13, 2);
        CustomDevice.NominalI = Encoding.GetEncoding(1251).GetString(CustomMessage, 15, 3);
        CustomDevice.FactoryCode = Encoding.GetEncoding(1251).GetString(CustomMessage, 18, 2);
        CustomDevice.ModulesCount = CustomMessage[20];
        CustomDevice.FactoryYear = CustomMessage[21];
        CustomDevice.FactoryNumber = CustomMessage[22] * 255 + CustomMessage[23];
    }
    
    /// <summary>
    /// Опрос устройств.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public async Task Poll(long t)
    {
        if (!port.IsOpen)
        {
            PortOpen(MainWindow.settings.PortModbus, MainWindow.settings.BaudRate, MainWindow.settings.DataBits,
                MainWindow.settings.Parity, MainWindow.settings.StopBits);
            return;
        }
        //Запись в лог.
        if (MainWindow.settings.RecordLog) _=Task.Run(()=> ClassLog.Write($"Master Poll {t}"));
        //Если токен отмены заполнен.
        if (MainWindow.cts != null)
        {
            if (MainWindow.settings.RecordLog) _ = Task.Run(() => ClassLog.Write($"Master Poll {t} canceled"));
            DateTime t1 = new DateTime(LastTickPoll);
            DateTime t2 = new DateTime(t);
            if (t2.Subtract(t1).TotalMilliseconds > 3000) MainWindow.cts = null;
            return;
        }
        CancellationTokenSource newCTS = new CancellationTokenSource();
        MainWindow.cts = newCTS;
        LastTickPoll = t;
        
        TCPConnect();
        //Цикл проверки наличия новых значений для записи в регистры.
        foreach (ClassDevice device in MainWindow.Devices)
        {
            //Если период опроса 0.
            if (device.Period == 0) continue;
            //Если период опроса не пройден.
            if (!device.IsNeedRequest()) continue;
            //Включено ожидание ответа.
            if (device.WaitAnswer) continue;
            //Список каналов устройства в котрых есть новое значение для записи.
            List<ClassChannel> channels = Task.Run(()=> MainWindow.DB.RegistriesLoadNew(device.ID)).Result;
            if (channels.Count == 0) continue;
            foreach (ClassChannel ch in channels)
            {
                if (ch.TypeRegistry != ClassChannel.EnumTypeRegistry.HoldingRegistry) continue;
                ClassChannel channel = device.Channels.FirstOrDefault(x => x.ID == ch.ID);
                if (channel == null) continue;
                ushort NewValue = (ushort)ch.NValue.Value;
                //Условие использования коэффициентов.
                if (channel.Koef != 1 && channel.Koef != 0)
                {
                    NewValue = (ushort)((float)channel.NValue/channel.Koef);
                }
                //Запись одного регистра типа AO.
                WriteRegistry(device, (ushort)channel.Address, NewValue);
            }
        }
        //Цикл проверки наличия команд в очереди команд.
        for (int i = 0; i < MainWindow.QueueCommands.Count; i++)
        {
            //Удаляет объект из начала очереди и возвращает его.
            ClassCommand cmd = MainWindow.QueueCommands.Dequeue();
            if (cmd.Device.WaitAnswer)
            {
                MainWindow.QueueCommands.Enqueue(cmd);
                continue;
            }
            switch (cmd.CommandType)
            {
                case ClassCommand.EnumType.WriteCoil when cmd.Device.Model == ClassDevice.EnumModel.SKZ_IP:
                    WriteCoils(cmd.Device, (ushort)cmd.Address, cmd.bValue, cmd);
                    break;
                case ClassCommand.EnumType.WriteCoil:
                    WriteCoil(cmd.Device, (ushort)cmd.Address, cmd.bValue, cmd);
                    break;
                case ClassCommand.EnumType.WriteRegistry when cmd.Device.Model == ClassDevice.EnumModel.SKZ_IP:
                    WriteRegistries(cmd.Device, (ushort)cmd.Address, (ushort)cmd.Value, cmd);
                    break;
                case ClassCommand.EnumType.WriteRegistry:
                    WriteRegistry(cmd.Device, (ushort)cmd.Address, (ushort)cmd.Value, cmd);
                    break;
                default:
                    continue;
            }
            System.Threading.Thread.Sleep(10);
        }
        //Цикл проверки и запуска команд из текущего списка команд(не очередь).
        foreach (ClassCommand cmd in MainWindow.Commands)
        {
            if (cmd.Period == 0 || cmd.Device == null) continue;
            if (cmd.DT.AddSeconds(cmd.Period).CompareTo(DateTime.Now) > 0) continue;
            if (cmd.Device.LinkState != ClassDevice.EnumLink.LinkYes) continue;
            switch (cmd.CommandType)
            {
                case ClassCommand.EnumType.WriteCoil when cmd.Device.Model == ClassDevice.EnumModel.SKZ_IP:
                    WriteCoils(cmd.Device, (ushort)cmd.Address, cmd.bValue, cmd);
                    break;
                case ClassCommand.EnumType.WriteCoil:
                    WriteCoil(cmd.Device, (ushort)cmd.Address, cmd.bValue, cmd);
                    break;
                case ClassCommand.EnumType.WriteRegistry when cmd.Device.Model == ClassDevice.EnumModel.SKZ_IP:
                    WriteRegistries(cmd.Device, (ushort)cmd.Address, (ushort)cmd.Value, cmd);
                    break;
                case ClassCommand.EnumType.WriteRegistry:
                    WriteRegistry(cmd.Device, (ushort)cmd.Address, (ushort)cmd.Value, cmd);
                    break;
                default:
                    continue;
            }
            System.Threading.Thread.Sleep(10);
        }
        //Цикл опроса регистров.
        for (int i = 0; i < MainWindow.Devices.Count; i++)
        {
            //Получение текущего устройства из списка.
            ClassDevice device = MainWindow.Devices[i];
            //Получаем модбас мастер.
            ModbusMaster master = GetMaster(device);
            //Если нет мастера, то пропуск итерации.
            if (master == null) continue;
            //Если протокол SMS, то проспуск итерации.
            if (device.Protocol == ClassDevice.EnumProtocol.SMS) continue;
            //Если период опроса 0, то пропуск итерации.
            if (device.Period == 0) continue;
            //Если время опроса устройства, указанное в настройках еще не наступило, то пропуск итерации.
            if (!device.IsNeedRequest()) continue;
            //Если устройство ожидает ответа, то пропуск итерации.
            if (device.WaitAnswer) continue;
            //Если не все пакеты отправлены.
            if (device.CountGroups > 0) continue;
            //Проверка на занятость порта каким либо устройством.
            if (SomeDeviceInTheProcess(MainWindow.Devices, device)) continue;
            //Разбиение карты регистров на группы.
            List<ClassGroupRequest> Groups = device.GetGroups();
            //Цикл запроса данных по группам регистров.
           await Task.Run(()=>ReadGroupRegistry(Groups,device,master));               
        }
        
        //????????????????????????????????????????????
        // for (int i = 0; i < MainWindow.Channels.Count; i++)
        // {
        //     //Если есть команды в очереди команд - выход из цикла.
        //     if (MainWindow.QueueCommands.Count > 0) break;
        //     ClassChannel item = MainWindow.Channels[i];
        //     //Если устройство работает по sms - следующая итерация.
        //     if (item.Device.Protocol == ClassDevice.EnumProtocol.SMS) continue;
        //     //Выбор мастера устройства.
        //     ModbusMaster master = GetMaster(item.Device);
        //     //Если мастер null - след. итерация.
        //     if (master == null) continue;
        //     //Если период опроса 0 -следю итерация.
        //     if (item.Device.Period == 0) continue;
        //     if (item.DTAct.AddSeconds(item.Device.Period).CompareTo(DateTime.Now) > 0) continue;
        //     if (item.Device.WaitAnswer) continue;
        //     if (item.Device.LinkState != ClassDevice.EnumLink.LinkYes) continue;
        //     ushort RegistryCount = 1;
        //     //Выбор, сколько регистров считать в зависимости от типа данных.
        //     if (item.Format == ClassChannel.EnumFormat.Float ||
        //         item.Format == ClassChannel.EnumFormat.swFloat ||
        //         item.Format == ClassChannel.EnumFormat.UInt32) RegistryCount = 2;
        //     try
        //     {
        //         if (SendRequestEvent != null) SendRequestEvent();
        //         item.Device.PacketSended();
        //         switch (item.TypeRegistry)
        //         {
        //             case ClassChannel.EnumTypeRegistry.InputRegistry:
        //             {
        //                 ClassMessage.SaveNewMessage(new ClassMessage() { Type = ClassMessage.EnumType.Request },
        //                     0x04, new ushort[] { (ushort)item.Address, 1 });
        //                 if (MainWindow.settings.RecordLog)
        //                     ClassLog.Write($"Request ID = {item.Device.Address} address {item.Address}");
        //                 data = await master.ReadInputRegistersAsync((byte)item.Device.Address,
        //                     (ushort)item.Address, RegistryCount);
        //                 if (MainWindow.settings.RecordLog)
        //                     ClassLog.Write($"Answer ID = {item.Device.Address} address {item.Address}");
        //                 ClassMessage.SaveNewMessage(new ClassMessage() { Type = ClassMessage.EnumType.Answer },
        //                     0x04, data);
        //                 break;
        //             }
        //             case ClassChannel.EnumTypeRegistry.HoldingRegistry:
        //                 ClassMessage.SaveNewMessage(new ClassMessage() { Type = ClassMessage.EnumType.Request },
        //                     0x03, new ushort[] { (ushort)item.Address, 1 });
        //                 data = await master.ReadHoldingRegistersAsync((byte)item.Device.Address,
        //                     (ushort)item.Address, RegistryCount);
        //                 ClassMessage.SaveNewMessage(new ClassMessage() { Type = ClassMessage.EnumType.Answer },
        //                     0x03, data);
        //                 break;
        //             case ClassChannel.EnumTypeRegistry.CoilOutput:
        //             {
        //                 bool[] b = await master.ReadCoilsAsync((byte)item.Device.Address,
        //                     (ushort)item.Address, 1);
        //                 data = new ushort[1];
        //                 if (b[0]) data[0] = 1;
        //                 break;
        //             }
        //             case ClassChannel.EnumTypeRegistry.DiscreteInput:
        //             {
        //                 bool[] b = await master.ReadInputsAsync((byte)item.Device.Address,
        //                     (ushort)item.Address, 1);
        //                 data = new ushort[1];
        //                 if (b[0]) data[0] = 1;
        //                 break;
        //             }
        //             default:
        //                 continue;
        //         }
        //         if (ReceivedAnswerEvent != null) ReceivedAnswerEvent();
        //     }
        //     catch (Exception Ex)
        //     {
        //         item.Device.PacketNotReceived();
        //         if (MainWindow.settings.RecordLog)
        //             ClassLog.Write($"Answer Error = {Ex.Message}");
        //         continue;
        //     }
        //     item.BaseValue = (ushort[])data.Clone();
        //     item.Device.PacketReceived();
        //     item.Value = GetDecimalFromBuffer(0, item.Format);
        //     System.Threading.Thread.Sleep(10);
        // }
        
        if (MainWindow.cts == newCTS) MainWindow.cts = null;
        await Task.Yield();
    }

    /// <summary>
        /// Проверка, ведет ли какое нибудь устройство опрос в текущий момент.
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="currentDevice"></param>
        /// <returns></returns>
    private bool SomeDeviceInTheProcess(ObservableCollection<ClassDevice> devices, ClassDevice currentDevice)
    {
        return devices.Any(dev => dev.InProcess == true && currentDevice.ID != dev.ID);
    }

    /// <summary>
    /// Чтение групп регистров типа AO/AI.
    /// </summary>
    /// <param name="Groups">Список групп регистров</param>
    /// <param name="device">Текущее устройство</param>
    /// <param name="master">Мастер Modbus</param>
    private void ReadGroupRegistry(List<ClassGroupRequest> Groups, ClassDevice device, ModbusMaster master)
    {
        device.InProcess = true;
        foreach (ClassGroupRequest group in Groups)
        {
            //Индикация отправки пакета.
            device.PacketSended();
            Dispatcher.UIThread.Invoke(()=>SendRequestEvent?.Invoke());
            device.CountGroups--;
            if (device.CountGroups <= 0)
            {
                device.DTAct = DateTime.Now;
                device.InProcess = false;
            }
            int numOfPoint = group.GetSize();                
            try
            {
                lock (locker)
                {
                    switch (group.TypeRegistry)
                    {
                        //Отправка запроса для InputRegistry.
                        case ClassChannel.EnumTypeRegistry.InputRegistry when numOfPoint < numOfRegMax:
                            data = master.ReadInputRegisters((byte)device.Address, (ushort)group.StartAddress,(ushort)numOfPoint);
                            break;
                        //Отправка запроса для HoldingRegistry.
                        case ClassChannel.EnumTypeRegistry.HoldingRegistry when numOfPoint < numOfRegMax:
                            data =  master.ReadHoldingRegisters((byte)device.Address,(ushort)group.StartAddress, (ushort)numOfPoint);
                            break;
                        case ClassChannel.EnumTypeRegistry.CoilOutput when numOfPoint < numOfRegMax:
                            dataBool = master.ReadCoils((byte)device.Address, (ushort)group.StartAddress, (ushort)numOfPoint);
                            break;
                        case ClassChannel.EnumTypeRegistry.DiscreteInput when numOfPoint < numOfRegMax:
                            dataBool = master.ReadInputs((byte)device.Address, (ushort)group.StartAddress, (ushort)numOfPoint);
                            break;
                        default:
                            continue;
                    }
                }
            }
            catch (Exception Ex)
            {
                //Индикация ошибки получения пакета.
                device.PacketNotReceived();
                if (MainWindow.settings.RecordLog)
                    ClassLog.Write($"Answer Error = {Ex.Message} Device name:{device.Name} Device address:{device.Address}");
                continue;
            }
            //Индикация получения пакета.
            device.PacketReceived();
            Dispatcher.UIThread.Invoke(()=>ReceivedAnswerEvent?.Invoke()); 
            
            if(group.TypeRegistry == ClassChannel.EnumTypeRegistry.HoldingRegistry || group.TypeRegistry == ClassChannel.EnumTypeRegistry.InputRegistry)
            {
                //Заполнение данных каналов, в соответсвии с их типом и адресом.
                for (int j = 0; j < group.Channels.Count; j++)
                {
                    ClassChannel channel = group.Channels[j];
                    int Offset = group.GetOffset(j);
                    channel.BaseValue = GetDataFromBuffer(Offset, channel.Format);
                    channel.Value = GetDecimalFromBuffer(Offset, channel.Format);
                }
                System.Threading.Thread.Sleep(10);
            }


            //Чтение DO/DI.
            if (group.TypeRegistry == ClassChannel.EnumTypeRegistry.CoilOutput || group.TypeRegistry == ClassChannel.EnumTypeRegistry.DiscreteInput)
            {
                for (int j = 0; j < group.Channels.Count; j++)
                {
                    ClassChannel channel = group.Channels[j];
                    int Offset = group.GetOffset(j);
                    channel.Value = Convert.ToDecimal(dataBool[Offset]);
                    //channel.Value = GetDecimalFromBuffer(Offset, channel.Format);
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }

    /// <summary>
    /// Запись одной катушки.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="Address"></param>
    /// <param name="Value"></param>
    /// <param name="Command"></param>
    public async void WriteCoil(ClassDevice device, ushort Address, bool Value, 
        ClassCommand Command = null)
    {
        ModbusMaster master = GetMaster(device);
        if (master == null) return;
        try
        {
            await master.WriteSingleCoilAsync((byte)device.Address, Address, Value);
            if (Command != null) Command.DT = DateTime.Now;
        }
        catch(Exception ex)
        {
        }
    }

    /// <summary>
    /// Запись катушек.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="Address"></param>
    /// <param name="Value"></param>
    /// <param name="Command"></param>
    public async void WriteCoils(ClassDevice device, ushort Address, bool Value,
        ClassCommand Command = null)
    {
        ModbusMaster master = GetMaster(device);
        if (master == null) return;
        bool[] data = new bool[1];
        data[0] = Value;
        try
        {
            await master.WriteMultipleCoilsAsync((byte)device.Address, Address, data);
            if (Command != null) Command.DT = DateTime.Now;
        }
        catch (Exception Ex)
        {
            ClassLog.Write(Ex.Message);
        }
    }
    /// <summary>
    /// Запись одного Holding Registry.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="Address"></param>
    /// <param name="Value"></param>
    /// <param name="Command"></param>
    public async void WriteRegistry(ClassDevice device, ushort Address, ushort Value,
        ClassCommand Command = null)
    {
        ModbusMaster master = GetMaster(device);
        if (master == null) return;
        try
        {
            await master.WriteSingleRegisterAsync((byte)device.Address, Address, Value);
            if (Command != null) Command.DT = DateTime.Now;//Устанавливаем время записи в сейчас.
        }
        catch (Exception Ex)
        {
            ClassLog.Write(Ex.Message); 
        }
        ClassChannel channel = device.Channels.FirstOrDefault(x => x.Address == Address);
        if (channel != null) MainWindow.DB.RegistrySaveNewValue(channel.ID, null);//Сбрасывем значение для запси в null.
    }

    public async void WriteRegistries(ClassDevice device, ushort Address, ushort Value,
        ClassCommand Command = null)
    {
        ModbusMaster master = GetMaster(device);
        if (master == null) return;
        ushort[] buf = new ushort[1];
        buf[0] = Value;
        try
        {
            device.PacketSended();
            Debug.Print("WriteRegistries - Before await");
            await master.WriteMultipleRegistersAsync((byte)device.Address, Address, buf);
            Debug.Print("WriteRegistries - After await");
            if (Command != null) Command.DT = DateTime.Now;
            data = await master.ReadHoldingRegistersAsync((byte)device.Address, Address, 1);
            Command.Channel.BaseValue = (ushort[])data.Clone();
            Command.Channel.Value = GetDecimalFromBuffer(0, Command.Channel.Format);
            device.PacketReceived();
        }
        catch (Exception Ex)
        {

            device.PacketNotReceived();
            ClassLog.Write(Ex.Message);
            Debug.Print("WriteRegistries - Exception");
        }
    }

    /// <summary>
    /// Получение для каждого регистра значения, согласно его адресу и точности измерения.
    /// </summary>
    /// <param name="Offset"></param>
    /// <param name="DataFormat"></param>
    /// <returns></returns>
    private decimal GetDecimalFromBuffer(int Offset, ClassChannel.EnumFormat DataFormat)
    {
        decimal V = 0;
        switch (DataFormat)
        {
            case ClassChannel.EnumFormat.UINT:
            //case ClassChannel.EnumFormat.UInt32:
            V = data[Offset];
                break;
            case ClassChannel.EnumFormat.SINT:
                V = (short)data[Offset];
                break;
            case ClassChannel.EnumFormat.Float:
                V = (decimal)ModbusUtility.GetSingle(data[Offset], data[Offset + 1]);
                break;
            case ClassChannel.EnumFormat.swFloat:
                V = (decimal)ModbusUtility.GetSingle(data[Offset + 1], data[Offset]);
                break;
            case ClassChannel.EnumFormat.UInt32:
            V = (decimal)(data[Offset + 1] << 16 | data[Offset]);
            break;
        }
        return V;
    }

    /// <summary>
    /// Формирование массива данных для каждого регистра(2 байта либо 1 байт).
    /// </summary>
    /// <param name="Offset"></param>
    /// <param name="DataFormat"></param>
    /// <returns></returns>
    private ushort[] GetDataFromBuffer(int Offset, ClassChannel.EnumFormat DataFormat)
    {
        //Создание временного массива.
        ushort[] d = new ushort[] { 0 };
        //Выход, если буфер данных пуст.
        if (data == null) return d;
        switch (DataFormat)
        {
            case ClassChannel.EnumFormat.UINT://8-битовое, без знака.
            case ClassChannel.EnumFormat.SINT://8-битовое, со знаком.
                d = new ushort[1];
                d[0] = data[Offset];
                break;
            case ClassChannel.EnumFormat.UInt32://32-битовое, без знака.
            case ClassChannel.EnumFormat.Float:
                d = new ushort[2];
                d[0] = data[Offset];
                d[1] = data[Offset + 1];
                break;
            case ClassChannel.EnumFormat.swFloat://Переворачиваем байты для этого формата.
                d = new ushort[2];
                d[0] = data[Offset + 1];
                d[1] = data[Offset];
                break;
        }
        return d;
    }

    /// <summary>
    /// Выбор типа мастера, в зависимости от типа соединения.
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    private ModbusMaster GetMaster(ClassDevice device)
    {
        if (device.Protocol == ClassDevice.EnumProtocol.TCP ||
            device.Protocol == ClassDevice.EnumProtocol.GPRS ||
            device.Protocol == ClassDevice.EnumProtocol.GPRS_SMS) return device.Master;
        else return RTUMaster;
    }

    private async void SendRequest(eFunction Function, ClassDevice Device, ushort Address,
        ushort Count) //debug
    {
        ModbusMaster master = GetMaster(Device);
        if (master == RTUMaster && WaitAnswer) return;
        try
        {
            if (master == RTUMaster) WaitAnswer = true;
            switch (Function)
            {
                case eFunction.ReadCoils:
                    bool[] b = await master.ReadCoilsAsync((byte)Device.Address, Address, 1);
                    data = new ushort[1];
                    if (b[0]) data[0] = 1;
                    break;
            }
        }
        catch (Exception Ex)
        {
            ClassLog.Write(Ex.Message);
            Debug.Print("SendRequest - Exception: " + Ex.Message);
        }
        finally 
        { 
            WaitAnswer = false; 
        }
    }


}