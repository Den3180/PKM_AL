using System;
using System.Threading;
using Avalonia.Threading;
using Modbus.Device;
using PKM_AL.Classes.TransferClasses;

namespace PKM_AL.Classes.Demo;

public class ClassDemo
{
    public event ClassModbus.SendRequestEventHandler SendRequestEvent;
    public event ClassModbus.ReceivedAnswerEventHandler ReceivedAnswerEvent;
    
    private const decimal PolPotMin = 0.7M;  
    private const decimal PolPotMax = 1.25M;  
    private const decimal SumPotMin = 0.8M;  
    private const decimal SumPotMax = 2.7M;  
        
        
    /// <summary>
    /// Выполнить демонстрацию.
    /// </summary>
    public void ActionDemo()
    {
        for (int i = 0; i < MainWindow.Devices.Count; i++)
        {
            //Получение текущего устройства из списка.
            ClassDevice device = MainWindow.Devices[i];
            //Если нет каналов у устройства - пропуск итерации.
            if (device.Channels.Count == 0) continue;
            //Если протокол SMS, то проспуск итерации.
            if (device.Protocol == ClassDevice.EnumProtocol.SMS) continue;
            //Если период опроса 0, то пропуск итерации.
            if (device.Period == 0) continue;
            //Если время опроса устройства, указанное в настройках еще не наступило, то пропуск итерации.
            if (!device.IsNeedRequest()) continue;
            //Если устройство ожидает ответа, то пропуск итерации.
            if (device.WaitAnswer) continue;
            //Проверка на разрешения опроса. 
            if(!device.IsPoll) continue;
            device.PacketSended();
            Thread.Sleep(50);
            FillingRegistersDevices(device);
            device.PacketReceived();
            Dispatcher.UIThread.Invoke(()=>ReceivedAnswerEvent?.Invoke());
            Dispatcher.UIThread.Invoke(()=>SendRequestEvent?.Invoke());
            Thread.Sleep(50);
        }
    }

    /// <summary>
    /// Заполнение значение регистров устройств.
    /// </summary>
    /// <param name="device"></param>
    private void FillingRegistersDevices(ClassDevice device)
    {
        switch (device.Model)
        {
            case ClassDevice.EnumModel.BKM_3:
                FillBKM_3(device);
                break;
            case ClassDevice.EnumModel.BKM_4:
                break;
            case ClassDevice.EnumModel.SKZ:
                break;
            case ClassDevice.EnumModel.SKZ_IP:
                break;
            case ClassDevice.EnumModel.BSZ:
                break;
            case ClassDevice.EnumModel.USIKP:
                break;
            case ClassDevice.EnumModel.BKM_5:
                FillBKM_5(device);
                break;
            case ClassDevice.EnumModel.KIP:
                break;
            case ClassDevice.EnumModel.MMPR:
                break;
            default:
                _ = 0;
                break;
        }
    }

    private void FillBKM_3(ClassDevice device)
    {
        foreach (var channel in device.Channels)
        {
            channel.Value = channel.Address switch
            {
                5 => 11M,
                7=> 3,
                9 => DateTime.Now.Year,
                10 => DateTime.Now.Month,
                11 => DateTime.Now.Day,
                12 => DateTime.Now.Hour,
                13 => DateTime.Now.Minute,
                14 => DateTime.Now.Second,
                69 => (decimal)new Random().Next((int)(PolPotMin*100),(int)(PolPotMax*100))/100,
                70 => (decimal)new Random().Next((int)(0.03*100),(int)(0.05*100))/100,
                72 => (decimal)new Random().Next((int)(SumPotMin*100),(int)(SumPotMax*100))/100,
                68 => 0.009M,
                _ => channel.Value,
            };
        }
    }

    private void FillBKM_5(ClassDevice device)
    {
        foreach (var channel in device.Channels)
        {
            channel.Value = channel.Address switch
            {
                11 => 11M,
                19 => DateTime.Now.Year,
                20 => DateTime.Now.Month,
                21 => DateTime.Now.Day,
                22 => DateTime.Now.Hour,
                23 => DateTime.Now.Minute,
                24 => DateTime.Now.Second,
                26 => (decimal)new Random().Next((int)(PolPotMin * 100), (int)(PolPotMax * 100)) / 100,
                28 => (decimal)new Random().Next((int)(0.03 * 100), (int)(0.05 * 100)) / 100,
                328 => (decimal)new Random().Next(15, 25),
                1025 => (decimal)new Random().Next((int)(SumPotMin * 100), (int)(SumPotMax * 100)) / 100,
                _ => channel.Value,
            };
        }
    }
}