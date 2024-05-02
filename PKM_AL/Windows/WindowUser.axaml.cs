using System.Collections.Generic;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes;
using Brushes = Avalonia.Media.Brushes;

namespace PKM_AL.Windows;

public partial class WindowUser : ClassWindowPKM
{
    private ClassUser User;
    public WindowUser()
    {
        InitializeComponent();
    }
    
    public WindowUser(ClassUser obj)
    {
        InitializeComponent();
        User = obj;
        this.UserName.Text = User.Name;
        this.Login.Text = User.Login;
        this.Pass.Text = User.Pass;
        if ((User.Grant & 0x01) != 0) this.Grant1.IsChecked = true;
        if ((User.Grant & 0x02) != 0) this.Grant2.IsChecked = true;
        if ((User.Grant & 0x04) != 0) this.Grant3.IsChecked = true;
        Tag = false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button=sender as Button;
        if (button?.IsCancel == true)
        {
            Close();
            return;
        }
        var childs= ClassControlManager.GetUIElem<TextBox>(this, new List<TextBox>());
        foreach (var elem in childs)
        {
            if (elem is TextBox box)
            {
                if (string.IsNullOrEmpty(box.Text))
                {
                    ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Заполните все поля!", "", 
                        ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Warning);
                    return;
                }
            }
        }
        
        User.Name = UserName.Text;
        User.Login =Login.Text;
        User.Pass = Pass.Text;
        int Grants = 0x00;
        if (Grant1.IsChecked != null && Grant1.IsChecked.Value) Grants += 0x01;
        if (Grant2.IsChecked != null && Grant2.IsChecked.Value) Grants += 0x02;
        if (Grant3.IsChecked != null && Grant3.IsChecked.Value) Grants += 0x04;
        User.Grant = Grants;
        if (User.ID == 0)
        {
            MainWindow.DB.UserAdd(User);
            Tag = true;
        }
        else
        {
            MainWindow.DB.UserEdit(User);
            Tag = false;
        }
        this.Close();
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        UserName.Focus();
    }
}