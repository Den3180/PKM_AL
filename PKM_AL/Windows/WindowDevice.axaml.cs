using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System;
using Org.BouncyCastle.Asn1.Cmp;
using System.Linq;
using MsBox.Avalonia.Enums;

namespace PKM_AL;

public partial class WindowDevice : ClassWindowPKM
{
    private ClassDevice _Device;

    public ClassDevice Device { get { return _Device; } }

    public WindowDevice()
    {
        InitializeComponent();
    }

    public WindowDevice(ClassDevice newDevice)
    {
        InitializeComponent();
        _Device = newDevice;
        this.Tag = false;
    }

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string separatorCurrent = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        _Device.Name = this.DeviceName.Text;
        _Device.Protocol = (ClassDevice.EnumProtocol)this.Protocol.SelectedIndex + 1;
        _Device.Period = Convert.ToInt32(this.Period.Text);
        _Device.IPAddress = this.IPAddress.Text;
        _Device.IPPort = Convert.ToInt32(this.IPPort.Text);
        //Поля для конвертации в double проходят проверку на сепаратор чисел.
        _Device.Latitude = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Latitude.Text, separatorCurrent));
        _Device.Longitude = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Longitude.Text, separatorCurrent));
        _Device.Picket = $"ПК{Picket_km.Text}+{Picket_m.Text}";
        _Device.Elevation = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Elevation.Text, separatorCurrent));
        _Device.SIM = this.txtSIM.Text;
        //Выбрана вкладка СКЗ.
        if (Model.SelectedItem.ToString().Contains("СКЗ"))
        { SetSKZData(); }
        //Проверка адреса устройства.
        _Device.Model = (ClassDevice.EnumModel)this.Model.SelectedIndex;
        if (!CheckNameAndAddrDevice())
        { return; }
        _Device.Address = Convert.ToInt32(this.Address.Text);
        if (_Device.ID == 0)
        {
            bool res = MainWindow.DB.DeviceAdd(_Device);
            MainWindow.Devices.Add(_Device);
            _Device.CountNumber = MainWindow.Devices[^1].CountNumber + 1;
        }
        else
        {
            MainWindow.DB.DeviceEdit(_Device);
            ClassItem item = MainWindow.Groups[0].SubGroups.FirstOrDefault(x => x.ID == _Device.ID);
            item.NameCh = _Device.Name;
        }


        Button button = sender as Button;
        this.Tag = button.IsDefault != true;
        this.Close();
    }

    /// <summary>
    /// Внесение данных СКЗ из формы.
    /// </summary>
    private void SetSKZData()
    {
        _Device.UnomInSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Unom_supply.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.NactiveSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Nactiv.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.NfullInSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Nfull.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.UnomOutSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Unom_output.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.InomOutSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Inom_output.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.NnomOutSKZ = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Nnom_output.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
        _Device.FactoryCode = FactoryCode.Text;//Код предприятия.
        _Device.FactoryNumber = Convert.ToInt32(FactoryNumber.Text);//Заводской номер изделия.
        _Device.ModulesCount = Convert.ToInt32(ModulesCount.Text);//Число модулей СКЗ.
        _Device.DateStart = DateStart.SelectedDate.Value;
        _Device.FactoryYear = Convert.ToInt32(FactoryYear.SelectedValue);//Год выпуска.
        _Device.Resource = Convert.ToInt32(Resource.Text);
    }

    /// <summary>
    /// Проверка имени устройства на заполненность и адреса устройства на совпадение.
    /// </summary>
    /// <returns></returns>
    private bool CheckNameAndAddrDevice()
    {
        ClassDevice device = MainWindow.Devices.FirstOrDefault(x => (x.Address == Convert.ToInt32(this.Address.Text) && x.ProtocolName == _Device.ProtocolName));
        if (string.IsNullOrEmpty(_Device.Name))
        {
            ClassMessage.ShowMessage(text:"Введите имя устройства!",owner: MainWindow.currentMainWindow,icon: MsBox.Avalonia.Enums.Icon.Error);
            return false;
        }
        if (device != null && _Device.ID != device.ID && _Device.Address != Convert.ToInt32(Address.Text))
        {
            ClassMessage.ShowMessage(text: "Адреса устройств протокола ModbusRTU не должны совпадать!", owner: MainWindow.currentMainWindow, icon: MsBox.Avalonia.Enums.Icon.Error);
            return false;
        }
        return true;
    }

}