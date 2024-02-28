using Avalonia.Controls;
using PKM_AL.Classes;

namespace PKM_AL.Windows;

public partial class WindowLink : ClassWindowPKM
{
    private ClassLink _Link;
    
    public WindowLink()
    {
        InitializeComponent();
    }
    
    public WindowLink(ClassLink newLink)
    {
        InitializeComponent();
        _Link = newLink;
        ComboEvent.ItemsSource = ClassEvent.GetEventNames();
        ComboEvent.SelectedIndex = (int)_Link.EventType - 1;
        foreach (ClassChannel item in MainWindow.Channels)
        {
            ComboSource.Items.Add(item.Name);
            if (_Link.SourceID == item.ID) ComboSource.SelectedItem = item.Name;
        }
        foreach (ClassCommand item in MainWindow.Commands)
        {
            ComboCommand.Items.Add(item.Name);
            if (_Link.Command.ID == item.ID) ComboCommand.SelectedItem = item.Name;
        }
    }

    

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.IsCancel == true)
        {
            Close();
            return;
        }
        _Link.EventType = (ClassEvent.EnumType)(ComboEvent.SelectedIndex + 1);
        _Link.SourceID =  MainWindow.Channels[ComboSource.SelectedIndex].ID;
        _Link.SourceName = ComboSource.SelectedItem?.ToString();
        _Link.Command = MainWindow.Commands[ComboCommand.SelectedIndex];
        if (_Link.ID == 0)
        {
            MainWindow.DB.LinkAdd(_Link);
            MainWindow.Links.Add(_Link);
        }
        else MainWindow.DB.LinkEdit(_Link);
        Close();

    }
}