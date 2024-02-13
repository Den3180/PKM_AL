using System;
using System.IO.Ports;
using PKM;
using PKM_AL.Classes.TransferClasses;

namespace PKM_AL.Classes;

public class ClassUSIKP
{
    private SerialPort port;
    private System.Timers.Timer TimerSec;
    private DateTime _USIKPLastTime = DateTime.MinValue;
    private bool FlagRun = false;
    private bool FlagWait = false;
    private int DelaySec;

    public ClassUSIKP()
    {
        port = null;
        TimerSec = new System.Timers.Timer();
        TimerSec.Interval = 1000;
        TimerSec.Elapsed += TimerSec_Elapsed;
    }

    private void TimerSec_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (!FlagRun) return;
        if (FlagWait) DelaySec++;
        if (!FlagWait || (FlagWait && DelaySec >= 4))
        {
            ClassDevice device = SelectDevice();
            if (device == null)
            {
                Off();
                return;
            }
            FlagWait = true;
            DelaySec = 0;
            //SendSetCoil(device.Address, 5);
            if (MainWindow.settings.PeriodUSIKP % 2 == 0)
                SendCheck(device.Address, 0x16); //0x16,0x23,0x2E,0x2F
            else SendCheck(device.Address, 0x23); //0x16,0x23,0x2E,0x2F
        }
    }

            private bool PortOpen()
        {
            if (port != null && port.IsOpen) port.Close();
            port = new SerialPort("COM" + MainWindow.settings.PortModbus);
            port.BaudRate = MainWindow.settings.BaudRate;
            port.DataBits = MainWindow.settings.DataBits;
            port.Parity = (Parity)MainWindow.settings.Parity;
            port.StopBits = (StopBits)MainWindow.settings.StopBits;
            try { port.Open(); }
            catch (Exception Ex)
            {
                ClassLog.Write("USIKP PortOpen Error");
                return false;
            }
            return true;
        }

        private void Off()
        {            
            _USIKPLastTime = DateTime.Now;
            TimerSec.Stop();
            FlagRun = false;
            if (port != null && port.IsOpen) port.Close();
            MainWindow.modbus.Mode = ClassModbus.eMode.None;
            if (MainWindow.settings.RecordLog) ClassLog.Write("Stop poll USIKP");
        }    

        public void Poll()
        {
            if (_USIKPLastTime.AddSeconds(MainWindow.settings.PeriodUSIKP).CompareTo(DateTime.Now) > 0) return;
            if (FlagRun) return;
            if (MainWindow.settings.RecordLog) ClassLog.Write("Start poll USIKP");
            MainWindow.modbus.PortClose();
            if (!PortOpen())
            {
                Off();
                return;
            }
            FlagRun = true;
            DelaySec = 0;
            TimerSec.Start();
        }

        private ClassDevice SelectDevice()
        {
            foreach (ClassDevice device in MainWindow.Devices)
            {
                if (device.Model != ClassDevice.EnumModel.USIKP) continue;
                if (device.LastRequestDT.AddSeconds(MainWindow.settings.PeriodUSIKP).CompareTo(DateTime.Now) > 0)
                    continue;
                device.LastRequestDT = DateTime.Now;
                return device;
            }
            return null;
        }

        private void SendCheck(int DeviceAddress, int FunctionCode)
        {
            byte[] b = new byte[5];
            b[0] = (byte)DeviceAddress;
            b[1] = (byte)FunctionCode;
            b[2] = (byte)(DateTime.Now.Year - 2000);
            b[3] = (byte)DateTime.Now.Month;
            b[4] = (byte)DateTime.Now.Day;
            byte[] crc = Modbus.Utility.ModbusUtility.CalculateCrc(b); //0-low, 1-high
            byte[] mes = new byte[b.Length + crc.Length];
            b.CopyTo(mes, 0);
            crc.CopyTo(mes, mes.Length - crc.Length);
            port.Write(mes, 0, mes.Length);
        }

        private void SendSetCoil(int DeviceAddress, int CoilAddress)
        {
            byte[] b = new byte[6];
            b[0] = (byte)DeviceAddress;
            b[1] = 0x05;
            b[2] = 0x00;
            b[3] = (byte)CoilAddress;
            b[4] = 0xFF;
            b[5] = 0x00;
            byte[] crc = Modbus.Utility.ModbusUtility.CalculateCrc(b); //0-low, 1-high
            byte[] mes = new byte[b.Length + crc.Length];
            b.CopyTo(mes, 0);
            crc.CopyTo(mes, mes.Length - crc.Length);
            port.Write(mes, 0, mes.Length);
        }

}