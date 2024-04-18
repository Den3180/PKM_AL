using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes;
using PKM_AL.Windows;

namespace PKM_AL.Controls;

public partial class UserControlLinks : UserControl
{
    private double _actualHeightUserControl; //Актуальная высота UserControl.
    
    public UserControlLinks()
    {
        InitializeComponent();
        GridLinks.ItemsSource = MainWindow.Links;
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
        WindowLink frm = new WindowLink(new ClassLink());
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        ClassLink obj = this.GridLinks.SelectedItem as ClassLink;
        WindowLink frm = new WindowLink(obj);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        ClassLink obj = this.GridLinks.SelectedItem as ClassLink;
        if (obj == null) return;
        Task<ButtonResult> res = ClassMessage.ShowMessage(text: $"Удалить связь?", owner: MainWindow.currentMainWindow,
            buttonEnum: ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Question);
        if (res.Result == ButtonResult.No) return;
        if (MainWindow.DB.LinkDel(obj)) MainWindow.Links.Remove(obj);
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _actualHeightUserControl = this.GetTransformedBounds().Value.Bounds.Height;
            GridLinks.Height = _actualHeightUserControl;
        }
        catch
        {
            _actualHeightUserControl=double.NaN;  
        }
    }
  
}