using System;
using System.Globalization;
using Avalonia.Controls;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL.Windows;

public partial class WindowChannel : ClassWindowPKM
{
    private readonly ClassChannel _channel;
    private readonly string _selectedColumn;
    public WindowChannel()
    {
        InitializeComponent();
    }

    public WindowChannel(ClassChannel newChannel, bool isEditChannel = false, string selectedColumn="")
    {
        InitializeComponent();
        _channel = newChannel;
        _selectedColumn = selectedColumn;
        foreach (ClassDevice item in MainWindow.Devices)
        {
            ComboDevice?.Items.Add(item.Name);
            if (_channel.Device?.ID == item.ID)
            {
                ComboBox comboDevice = ComboDevice;
                if (comboDevice != null)
                    comboDevice.SelectedItem = item.Name;
            }
        }
        if (_channel.Device == null) { ComboDevice.SelectedIndex = 0; }
        //Доступность выбора устройств в режиме редактирования.
        ComboDevice.IsEnabled = !isEditChannel; 
        ChannelName.Text = _channel.Name;
        RegType.SelectedIndex = (int)_channel.TypeRegistry - 1;
        Address.Text = _channel.Address.ToString();
        Format.SelectedIndex = (int)_channel.Format;
        Koef.Text = _channel.Koef.ToString(CultureInfo.CurrentCulture);
        Min.Text = _channel.Min.HasValue ? _channel.Min.Value.ToString(CultureInfo.CurrentCulture) : "";
        Max.Text = _channel.Max.HasValue ? _channel.Max.Value.ToString(CultureInfo.CurrentCulture) : "";
        Accuracy.Text = _channel.Accuracy.HasValue ? _channel.Accuracy.Value.ToString() : "";
        Ext.Text = _channel.Ext.HasValue ? _channel.Ext.Value.ToString() : "";
        chArchive.IsChecked = _channel.Archive; 
    }
    
    /// <summary>
    /// Выбор фокуса для поля(работает только в режиме редактирования регистра).
    /// </summary>
    /// <param name="selectedColumn"></param>
    private void GetFocusAndSelection(string selectedColumn)
    {
        switch(selectedColumn)
        {
            case "Наименование":
                ChannelName.Focus();
                ChannelName.SelectAll();
                break;
            case "Тип регистра":
                RegType.Focus();
                break;
            case "Адрес":
                Address.Focus();
                Address.SelectAll();
                break;
            case "Формат":
                Format.Focus();
                break;
            case "Коэф.":
                Koef.Focus();
                Koef.SelectAll();
                break;
            case "Знаков":
                Accuracy.Focus();
                Accuracy.SelectAll();
                break;
            case "Min":
                Min.Focus();
                Min.SelectAll();
                break;
            case "Max":
                Max.Focus();
                Max.SelectAll();
                break;
        };
    }

    private void ComboDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = sender as ComboBox;
        if(comboBox==null) return;
        _channel.Device = MainWindow.Devices[comboBox.SelectedIndex];
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        
        if(button==null) return;
        if (button.IsCancel == true)
        {
            Close();
            return;
        }
        
        _channel.Name = ChannelName.Text;
        _channel.Device.ID = MainWindow.Devices[ComboDevice.SelectedIndex].ID;
        _channel.TypeRegistry = (ClassChannel.EnumTypeRegistry)RegType.SelectedIndex + 1;
        _channel.Address = Convert.ToInt32(Address.Text);
        _channel.Format = (ClassChannel.EnumFormat)Format.SelectedIndex;
        decimal? Koef = ClassHelper.DecimalFromStr(this.Koef.Text);
        if (Koef.HasValue) _channel.Koef = Convert.ToSingle(Koef.Value);
        _channel.Min = ClassHelper.DecimalFromStr(Min.Text);
        _channel.Max = ClassHelper.DecimalFromStr(Max.Text);
        _channel.Accuracy = ClassHelper.IntFromStr(Accuracy.Text);
        _channel.Ext = ClassHelper.IntFromStr(Ext.Text);
        _channel.Archive = chArchive.IsChecked.Value;
        if (_channel.ID == 0)
        {
            MainWindow.DB.RegistryAdd(_channel);
            Tag = _channel;
        }
        else
        {
            MainWindow.DB.RegistryEdit(_channel);
            Tag = _channel;
        }
        Close();
    }

    /// <summary>
    /// Выделение поля в форме редактирования каналов.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        GetFocusAndSelection(_selectedColumn);
    }
}