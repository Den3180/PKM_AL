using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes;

namespace PKM_AL.Windows;

public partial class WindowCommand : ClassWindowPKM
{
    private ClassCommand _Command;

    public WindowCommand()
    {
        InitializeComponent();
    }
    public WindowCommand(ClassCommand newCommand)
    {
        InitializeComponent();
        _Command = newCommand;
        CommandName.Text = _Command.Name;
        foreach (ClassDevice item in MainWindow.Devices)
        {
            ComboDevice.Items.Add(item.Name);
            if (_Command.Device.ID == item.ID) ComboDevice.SelectedItem = item.Name;
        }
        ComboCommand.SelectedIndex = (int)_Command.CommandType - 1;
        Address.Text = _Command.Address.ToString();
        Val.Text = _Command.Value.ToString();
        Period.Text = _Command.Period.ToString();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.IsCancel == true)
        {
            Close();
            return;
        }
        _Command.Name = CommandName.Text;
        _Command.Device.ID = MainWindow.Devices[ComboDevice.SelectedIndex].ID;
        _Command.Device=MainWindow.Devices[ComboDevice.SelectedIndex];
        _Command.CommandType = (ClassCommand.EnumType)(ComboCommand.SelectedIndex + 1);
        _Command.Address = Convert.ToInt32(Address.Text);
        _Command.Value = Convert.ToInt32(Val.Text);
        _Command.Period = Convert.ToInt32(Period.Text);
        if (_Command.ID == 0)
        {
            MainWindow.DB.CommandAdd(_Command);
            MainWindow.Commands.Add(_Command);
        }
        else MainWindow.DB.CommandEdit(_Command);
        Tag = true;
        Close();
    }
}