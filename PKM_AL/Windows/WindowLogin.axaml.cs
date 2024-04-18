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
        Users = MainWindow.DB.UsersLoad();
        this.Login.Focus();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
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
}