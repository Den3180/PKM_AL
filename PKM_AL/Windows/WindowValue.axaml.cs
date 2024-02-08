using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL.Windows;

public partial class WindowValue : ClassWindowPKM
{
    private readonly ClassChannel _channel; 
    public WindowValue()
    {
        InitializeComponent();
    }
    public WindowValue(ClassChannel newChannel)
    {
        InitializeComponent();
        _channel = newChannel;
        foreach (ClassDevice item in MainWindow.Devices)
        {
            if (_channel.Device.ID == item.ID) TextBoxDevice.Text = item.Name;
        }
        ChannelName.Text = _channel.Name;
        RegType.SelectedIndex = (int)_channel.TypeRegistry - 1;
        Address.Text = _channel.Address.ToString();
        Format.SelectedIndex = (int)_channel.Format;
        Koef.Text = _channel.Koef.ToString(CultureInfo.CurrentCulture);
        Val.Text = _channel.Value.ToString(CultureInfo.CurrentCulture);
        Val.Focus();
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
        
        decimal? decimalFromStr = ClassHelper.DecimalFromStr(this.Val.Text);
        if (!decimalFromStr.HasValue)
        {
            Close();
            return;
        }
        _channel.SendValue(decimalFromStr.Value);
        this.Close();
    }
}