using AvaloniaTest1.Service;
using Modbus.Data;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PKM_AL
{
    public class ClassSlave
    {
        private ModbusTcpSlave _Slave;
        public ClassSlave()
        {
            int port = 50502;

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            _Slave = ModbusTcpSlave.CreateTcp(1, listener);
            _Slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
            //Обработка входящих.
            _Slave.ModbusSlaveRequestReceived += _Slave_ModbusSlaveRequestReceived;
            //Прослушивание входящих.
            try
            {
                _Slave.Listen();
            }
            catch (Exception ex)
            {
                ClassMessage.ShowMessage(MainWindow.currentMainWindow,ex.Message);
            }
        }

        private void _Slave_ModbusSlaveRequestReceived(object sender, ModbusSlaveRequestEventArgs e)
        {
            var t = _Slave.Masters;
            byte slaveID = e.Message.SlaveAddress;
            byte fCode = e.Message.FunctionCode;
            byte[] data = e.Message.MessageFrame;
            byte[] byteStartAddress = new byte[] { data[3], data[2] };
            byte[] byteNum = new byte[] { data[5], data[4] };
            short StartAddress = BitConverter.ToInt16(byteStartAddress, 0);
            short NumOfPoint = BitConverter.ToInt16(byteNum, 0);
            if (slaveID == 250) //devices 
            {
                short devAddress = (short)(NumOfPoint - 3);
                ClassDevice device = MainWindow.Devices.FirstOrDefault(x => x.Address == devAddress);
                if (device == null) return;
                _Slave.DataStore.HoldingRegisters[StartAddress + 1] = (ushort)device.LinkState;
                _Slave.DataStore.HoldingRegisters[StartAddress + 2] = (ushort)device.PacketTxCount;
                _Slave.DataStore.HoldingRegisters[StartAddress + 3] = (ushort)device.PacketRxCount;

                _Slave.DataStore.InputRegisters[StartAddress + 1] = (ushort)device.LinkState;
                _Slave.DataStore.InputRegisters[StartAddress + 2] = (ushort)device.PacketTxCount;
                _Slave.DataStore.InputRegisters[StartAddress + 3] = (ushort)device.PacketRxCount;
            }
            else if (slaveID == 1 && MainWindow.settings.ModbusSlave) //addresses 1 и шлюз
            {
                //slaveID = (byte)(StartAddress / 100);
                //StartAddress = (short)(StartAddress % 100);
                RequestData(slaveID, StartAddress, NumOfPoint);
            }
            else //channels
            {
                RequestData(slaveID, StartAddress, NumOfPoint);
            }
        }

        private void RequestData(byte slaveID, short StartAddress, short NumOfPoint)
        {
            ClassDevice device = MainWindow.Devices.FirstOrDefault(x => x.Address == slaveID);
            if (device == null) return;
            ClassChannel.EnumTypeRegistry enumType = ClassChannel.EnumTypeRegistry.HoldingRegistry;
            int countIndex = 0;
            for (int i = 0; i < NumOfPoint; i++)
            {
                int RegistryAddress = StartAddress + i;
                ClassChannel channel = device.Channels.FirstOrDefault(x => x.Address == RegistryAddress);
                if (channel == null) continue;
                enumType = channel.TypeRegistry;
                ushort uValue = 0;
                if (channel.Value < 0)
                {
                    short Value = Convert.ToInt16(channel.Value);
                    uValue = BitConverter.ToUInt16(BitConverter.GetBytes(Value), 0);
                }
                else uValue = Convert.ToUInt16(channel.Value);
                if (channel.TypeRegistry == ClassChannel.EnumTypeRegistry.HoldingRegistry)
                {
                    countIndex = StartAddress + i + 1;
                    _Slave.DataStore.HoldingRegisters[countIndex] = uValue;
                }

                else if (channel.TypeRegistry == ClassChannel.EnumTypeRegistry.InputRegistry)
                {
                    countIndex = StartAddress + i + 1;
                    _Slave.DataStore.InputRegisters[countIndex] = uValue;
                }
                if (enumType == ClassChannel.EnumTypeRegistry.HoldingRegistry)
                {
                    _Slave.DataStore.HoldingRegisters[countIndex + 1] = (ushort)device.LinkState;
                }
                else if (enumType == ClassChannel.EnumTypeRegistry.InputRegistry)
                {
                    _Slave.DataStore.InputRegisters[countIndex + 1] = (ushort)device.LinkState;

                }

            }
        }
        /// <summary>
        /// Заполнение хранилища AI.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool SetValue(int Number, ushort Value)
        {
            if (_Slave == null) return false;
            if (Number <= 0) return false;
            _Slave.DataStore.InputRegisters[Number] = Value;
            return true;
        }
    }
}
