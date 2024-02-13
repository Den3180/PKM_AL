using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IO.Ports;
using System.Linq;
using PKM;

namespace PKM_AL.Windows;

public partial class WindowConfig : ClassWindowPKM
{
    private ClassSettings settings;
    public WindowConfig()
    {
        InitializeComponent();
        Ports.Items.Clear();
        PortModem.Items.Clear();
        Ports.Items.Add("Нет");
        PortModem.Items.Add("Нет");
        
        var portList = SerialPort.GetPortNames();
         foreach (var port in portList)
         {
             Ports.Items.Add(port);
             PortModem.Items.Add(port);
         }
        // for (int i = 1; i <= 32; i++)
        // {
        //     this.Ports.Items.Add("COM" + i.ToString());
        //     this.PortModem.Items.Add("COM" + i.ToString());
        // }
        
        settings = ClassSettings.Load();

        Ports.SelectedItem = Ports.Items.FirstOrDefault(p=>p.ToString()==settings.PortModbus);
        PortModem.SelectedItem = Ports.Items.FirstOrDefault(p=>p.ToString()==settings.PortModem);
       
        // Ports.SelectedIndex = settings.PortModbus;
        // PortModem.SelectedIndex = settings.PortModem;
        BaudRate.SelectedValue =BaudRate.Items.FirstOrDefault(item=>(item as ComboBoxItem)?.Content?.ToString()==settings.BaudRate.ToString());
        DataBits.SelectedValue =DataBits.Items.FirstOrDefault(item=>(item as ComboBoxItem)?.Content?.ToString()==settings.DataBits.ToString());
        Parity.SelectedIndex = settings.Parity;
        StopBits.SelectedIndex = settings.StopBits - 1;
        Timeout.Text = settings.Timeout.ToString();
        RequestLogin.IsChecked = settings.RequestLogin;
        RecordLog.IsChecked = settings.RecordLog;
        if (settings.Interpol) Inter.SelectedIndex = 1;
        else Inter.SelectedIndex = 0;
        Interface.IsChecked = settings.Interface;
        ModbusSlave.IsChecked = settings.ModbusSlave;
        txtPeriodUSIKP.Text = settings.PeriodUSIKP.ToString();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button=sender as Button;
        if (button?.IsCancel == true)
        {
            Close();
            return;
        }
        settings.PortModbus = Ports.SelectedItem?.ToString();
        settings.PortModem = PortModem.SelectedItem?.ToString();
        settings.BaudRate = Convert.ToInt32((BaudRate.SelectedItem as ComboBoxItem)?.Content);
        settings.DataBits = Convert.ToInt32((DataBits.SelectedItem as ComboBoxItem)?.Content);
        settings.Parity = Parity.SelectedIndex;
        settings.StopBits = StopBits.SelectedIndex + 1;
        settings.Timeout = Convert.ToInt32(Timeout.Text);
        settings.RequestLogin = RequestLogin.IsChecked.Value;
        settings.RecordLog = RecordLog.IsChecked.Value;
        settings.Interpol = Inter.SelectedIndex == 1 ? true : false;
        settings.Interface = Interface.IsChecked.Value;
        settings.ModbusSlave = ModbusSlave.IsChecked.Value;
        settings.PeriodUSIKP = Convert.ToInt32(txtPeriodUSIKP.Text);
        settings.Save();
        MainWindow.settings = settings;
        foreach (var dev in MainWindow.Devices)
        {
            //Изменение значение параметра происходит в блоке "get". Привязка в этом блоке происходит
            //к полям объекта MainWindow.setting.
            dev.OnPropertyChanged("CommParam");
        }
        Close();
    }
}