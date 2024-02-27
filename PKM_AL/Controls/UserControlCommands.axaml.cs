using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes;
using PKM_AL.Windows;

namespace PKM_AL.Controls;

public partial class UserControlCommands : UserControl
{
    public UserControlCommands()
    {
        InitializeComponent();
        GridCommands.ItemsSource = MainWindow.Commands;
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    private void сMenu_OnOpened(object sender, RoutedEventArgs e)
    {
        
    }

    private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
    {
        WindowCommand frm = new WindowCommand(new ClassCommand());
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void MenuItemRun_Click(object sender, RoutedEventArgs e)
    {
        
    }
}