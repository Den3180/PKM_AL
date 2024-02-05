using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System;
using Org.BouncyCastle.Asn1.Cmp;
using System.Linq;
using MsBox.Avalonia.Enums;
using Avalonia.Controls.Primitives;
using System.Diagnostics;
using System.Threading;

namespace PKM_AL;

public partial class WindowDevice : ClassWindowPKM
{
    private ClassDevice _Device = new ClassDevice();

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
        ConfigureWindowDeviceExist();
    }

    /// <summary>
    /// Заполнение окна данными сущ. устройства(при редактировании).
    /// </summary>
    private void ConfigureWindowDeviceExist()
    {
        this.DeviceName.Text = string.IsNullOrEmpty(_Device.Name) ? "Устройство" : _Device.Name;
        this.Protocol.SelectedIndex = (int)_Device.Protocol - 1;
        this.Period.Text = _Device.Period.ToString();
        this.IPAddress.Text = _Device.IPAddress;
        this.IPPort.Text = _Device.IPPort.ToString();
        this.Address.Text = _Device.Address.ToString();
        this.txtSIM.Text = _Device.SIM;
        this.Latitude.Text = _Device.Latitude.ToString();
        this.Longitude.Text = _Device.Longitude.ToString();
        this.Elevation.Text = _Device.Elevation.ToString();
        this.Model.SelectedIndex = (int)_Device.Model;
        this.Unom_supply.Text = _Device.UnomInSKZ.ToString();
        this.Nactiv.Text = _Device.NactiveSKZ.ToString();
        this.Nfull.Text = _Device.NfullInSKZ.ToString();
        this.Unom_output.Text = _Device.UnomOutSKZ.ToString();
        this.Inom_output.Text = _Device.InomOutSKZ.ToString();
        this.Nnom_output.Text = _Device.NnomOutSKZ.ToString();
        this.FactoryCode.Text = _Device.FactoryCode.ToString();
        this.FactoryNumber.Text = _Device.FactoryNumber.ToString();
        this.ModulesCount.Text = _Device.ModulesCount.ToString();
        this.DateStart.SelectedDate = _Device.DateStart;
        SetFactoryYear();
        //Распарсивание строки пикетов.
        if (!string.IsNullOrEmpty(_Device.Picket))
        {
            this.Picket_km.Text = _Device.Picket?.Split(new string[] { "+", "ПК" }, StringSplitOptions.RemoveEmptyEntries)[0];
            this.Picket_m.Text = _Device.Picket?.Split(new string[] { "+", "ПК" }, StringSplitOptions.RemoveEmptyEntries)[1];
        }
    }

    /// <summary>
    /// Кнопка применить.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string separatorCurrent = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        _Device.Name = this.DeviceName.Text;
        _Device.Protocol = (ClassDevice.EnumProtocol)this.Protocol.SelectedIndex + 1;
        _Device.Period = Convert.ToInt32(this.Period.Text);
        _Device.IPAddress = string.IsNullOrEmpty(this.IPAddress.Text) ? "" : this.IPAddress.Text;
        _Device.IPPort = string.IsNullOrEmpty(this.IPPort.Text) ? 502 : Convert.ToInt32(this.IPPort.Text);
        //Поля для конвертации в double проходят проверку на сепаратор чисел.
        _Device.Latitude = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Latitude.Text, separatorCurrent));
        _Device.Longitude = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Longitude.Text, separatorCurrent));
        _Device.Picket = $"ПК{Picket_km.Text}+{Picket_m.Text}";
        _Device.Elevation = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(Elevation.Text, separatorCurrent));
        _Device.SIM = string.IsNullOrEmpty(this.txtSIM.Text) ? "" : this.txtSIM.Text;
        //Выбрана вкладка СКЗ.
        if ((Model.SelectedItem as ComboBoxItem).Content.ToString().Contains("СКЗ"))
        { 
            SetSKZData(); 
        }
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
        this.Tag = true;
        this.Close();
    }

    /// <summary>
    /// Кнопка отмены.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Cancel(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Tag = false;
        this.Close();
    }

    /// <summary>
    ///Внесение данных СКЗ из формы.
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
    ///Проверка имени устройства на заполненность и адреса устройства на совпадение.
    /// </summary>
    /// <returns></returns>
    private bool CheckNameAndAddrDevice()
    {
        ClassDevice device = MainWindow.Devices.FirstOrDefault(x => (x.Address == Convert.ToInt32(this.Address.Text) && x.ProtocolName == _Device.ProtocolName));
        if (string.IsNullOrEmpty(_Device.Name))
        {
            ClassMessage.ShowMessage(text: "Введите имя устройства!", owner: MainWindow.currentMainWindow,icon: MsBox.Avalonia.Enums.Icon.Error);
            return false;
        }
        if (device != null && _Device.ID != device.ID && _Device.Address != Convert.ToInt32(Address.Text))
        {
            ClassMessage.ShowMessage(text: "Адреса устройств протокола ModbusRTU не должны совпадать!", owner: MainWindow.currentMainWindow, icon: MsBox.Avalonia.Enums.Icon.Error);
            return false;
        }
        return true;
    }

    /// <summary>
    ///  Обработка выбора типа устройства Combobox. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComboBox_SelectionChanged(object sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (Panel_SKZ == null) return;
        ComboBox comboBox = sender as ComboBox;
        Panel_SKZ.IsVisible = (comboBox.SelectedIndex == (int)ClassDevice.EnumModel.SKZ || comboBox.SelectedIndex == (int)ClassDevice.EnumModel.SKZ_IP);
    }

    /// <summary>
    /// Настройка ComboBox года ввода в эксплуатацию.
    /// </summary>
    private void SetFactoryYear()
    {
        int dtEnd = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

        for (int i = 1950; i <= dtEnd; i++)
        {
            FactoryYear.Items.Add(i);
        }
        if (_Device.FactoryYear == 0)
        {
            this.FactoryYear.SelectedIndex = this.FactoryYear.Items.Count - 1;
        }
        else
        { 
            this.FactoryYear.SelectedItem = this.FactoryYear.Items.FirstOrDefault(x=>(int)x== _Device.FactoryYear);
        }
    }
}