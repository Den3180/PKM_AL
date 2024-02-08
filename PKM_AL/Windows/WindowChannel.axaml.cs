using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL.Windows;

public partial class WindowChannel : ClassWindowPKM
{
    private ClassChannel _Channel;
    private string selectedColumn;
    public WindowChannel()
    {
        InitializeComponent();
    }

    public WindowChannel(ClassChannel newChannel, bool isEditChannel = false, string selectedColumn="")
    {
        InitializeComponent();
        _Channel = newChannel;
        this.selectedColumn = selectedColumn;
        foreach (ClassDevice item in MainWindow.Devices)
        {
            ComboDevice?.Items.Add(item.Name);
            if (_Channel.Device?.ID == item.ID)
            {
                ComboBox comboDevice = this.ComboDevice;
                if (comboDevice != null)
                    comboDevice.SelectedItem = item.Name;
            }
        }
        if (_Channel.Device == null) { this.ComboDevice.SelectedIndex = 0; }
        //Доступность выбора устройств в режиме редактирования.
        this.ComboDevice.IsEnabled = !isEditChannel; 
        this.ChannelName.Text = _Channel.Name;
        this.RegType.SelectedIndex = (int)_Channel.TypeRegistry - 1;
        this.Address.Text = _Channel.Address.ToString();
        this.Format.SelectedIndex = (int)_Channel.Format;
        this.Koef.Text = _Channel.Koef.ToString();
        if (_Channel.Min.HasValue) this.Min.Text = _Channel.Min.Value.ToString();
        else this.Min.Text = "";
        if (_Channel.Max.HasValue) this.Max.Text = _Channel.Max.Value.ToString();
        else this.Max.Text = "";
        if (_Channel.Accuracy.HasValue) this.Accuracy.Text = _Channel.Accuracy.Value.ToString();
        else this.Accuracy.Text = "";
        if (_Channel.Ext.HasValue) this.Ext.Text = _Channel.Ext.Value.ToString();
        else this.Ext.Text = "";
        this.chArchive.IsChecked = _Channel.Archive; 
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
                this.ChannelName.Focus();
                this.ChannelName.SelectAll();
                break;
            case "Тип регистра":
                this.RegType.Focus();
                break;
            case "Адрес":
                this.Address.Focus();
                this.Address.SelectAll();
                break;
            case "Формат":
                this.Format.Focus();
                break;
            case "Коэф.":
                this.Koef.Focus();
                this.Koef.SelectAll();
                break;
            case "Знаков":
                this.Accuracy.Focus();
                this.Accuracy.SelectAll();
                break;
            case "Min":
                this.Min.Focus();
                this.Min.SelectAll();
                break;
            case "Max":
                this.Max.Focus();
                this.Max.SelectAll();
                break;
        };
    }


    private void ComboDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = sender as ComboBox;
        if(comboBox==null) return;
        _Channel.Device = MainWindow.Devices[comboBox.SelectedIndex];
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        
        if(button==null) return;
        if (button.IsCancel == true)
        {
            this.Close();
            return;
        }
        
        _Channel.Name = this.ChannelName.Text;
        _Channel.Device.ID = MainWindow.Devices[this.ComboDevice.SelectedIndex].ID;
        _Channel.TypeRegistry = (ClassChannel.EnumTypeRegistry)this.RegType.SelectedIndex + 1;
        _Channel.Address = Convert.ToInt32(this.Address.Text);
        _Channel.Format = (ClassChannel.EnumFormat)this.Format.SelectedIndex;
        decimal? Koef = ClassHelper.DecimalFromStr(this.Koef.Text);
        if (Koef.HasValue) _Channel.Koef = Convert.ToSingle(Koef.Value);
        _Channel.Min = ClassHelper.DecimalFromStr(this.Min.Text);
        _Channel.Max = ClassHelper.DecimalFromStr(this.Max.Text);
        _Channel.Accuracy = ClassHelper.IntFromStr(this.Accuracy.Text);
        _Channel.Ext = ClassHelper.IntFromStr(this.Ext.Text);
        _Channel.Archive = this.chArchive.IsChecked.Value;
        if (_Channel.ID == 0)
        {
            MainWindow.DB.RegistryAdd(_Channel);
            this.Tag = _Channel;
        }
        else
        {
            MainWindow.DB.RegistryEdit(_Channel);
            this.Tag = _Channel;
        }
        this.Close();
    }

    /// <summary>
    /// Выделение поля в форме редактирования каналов.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        GetFocusAndSelection(selectedColumn);
    }
}