using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes;
using PKM_AL.Windows;

namespace PKM_AL.Controls;

public partial class UserControlCommands : UserControl
{
    private double _actualHeightUserControl; //Актуальная высота UserControl.
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
        bool FlagEnabled = MainWindow.User == null || MainWindow.User.GrantConfig;
        foreach (object item in cMenu.Items)
            if (item is MenuItem) ((MenuItem)item).IsEnabled = FlagEnabled;
    }

    private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
    {
        WindowCommand frm = new WindowCommand(new ClassCommand());
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        ClassCommand obj = GridCommands.SelectedItem as ClassCommand;
        if(obj==null) return;
        WindowCommand frm = new WindowCommand(obj);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        ClassCommand obj = this.GridCommands.SelectedItem as ClassCommand;
        if (obj == null) return;
        Task<string> res = ClassMessage.ShowMessageCustom(text: $"Удалить команду\' {obj.Name}\'?", owner: MainWindow.currentMainWindow,
            buttonEnum: ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Question);
        if (res.Result == "Нет") return;
        if (MainWindow.DB.CommandDel(obj)) MainWindow.Commands.Remove(obj);
    }

    private void MenuItemRun_Click(object sender, RoutedEventArgs e)
    {
        ClassCommand obj = this.GridCommands.SelectedItem as ClassCommand;
        if (obj == null) return;
        //Вставка команды в очередь первоочередных команд.
        MainWindow.QueueCommands.Enqueue(obj);
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _actualHeightUserControl = this.GetTransformedBounds().Value.Bounds.Height;
            GridCommands.Height = _actualHeightUserControl;
        }
        catch
        {
            _actualHeightUserControl=double.NaN;  
        }
    }
}