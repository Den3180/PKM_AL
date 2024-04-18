using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes;
using PKM_AL.Windows;

namespace PKM_AL.Controls;

public partial class UserControlUsers : UserControl
{
    private ObservableCollection<ClassUser> _Users;
    public UserControlUsers()
    {
        InitializeComponent();
        _Users = new ObservableCollection<ClassUser>(MainWindow.DB.UsersLoad());
        this.GridUsers.ItemsSource = _Users;
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
    {
        ClassUser obj = new ClassUser();
        WindowUser frm = new WindowUser(obj);
        frm.WindowShow(MainWindow.currentMainWindow);
        if((bool)frm.Tag) 
            _Users.Add(obj);  
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        ClassUser obj = this.GridUsers.SelectedItem as ClassUser;
        WindowUser frm = new WindowUser(obj);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        ClassUser obj = this.GridUsers.SelectedItem as ClassUser;
        if (obj == null) return;
        var res = ClassMessage.ShowMessage
            (MainWindow.currentMainWindow, "Удалить пользователя '" + obj.Name + "'?", "Удалить", 
            ButtonEnum.YesNo,Icon.Question).Result;
        if (res == ButtonResult.No) return;
        if (MainWindow.DB.UserDel(obj)) _Users.Remove(obj);
    }
}