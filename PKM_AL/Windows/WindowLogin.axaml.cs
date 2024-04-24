using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes;

namespace PKM_AL.Windows;

public partial class WindowLogin : ClassWindowPKM
{
    private List<ClassUser> Users;
    public ClassUser User { get; set; }
    public WindowLogin()
    {
        InitializeComponent();
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Users = MainWindow.DB.UsersLoad();
        foreach (ClassUser item in Users)
        {
            if (this.Login.Text == item.Login && this.Pass.Text == item.Pass)
            {
                User = item;
                break;
            }
        }
        if (User == null)
        {
            ClassMessage.ShowMessage(this, "Вход не разрешен", "Регистрация",icon:MsBox.Avalonia.Enums.Icon.Error);
            this.Login.Text = "";
            this.Pass.Text = "";
            this.Login.Focus();
        }
        else
        {
            this.Close();
        }
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        //Фокус на поле с логином.
        Login.Focus();
        //Выбрать все в поле с логином.
        this.Login.SelectAll();
        
    }
}
