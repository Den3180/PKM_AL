using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTest1.Service;
using MsBox.Avalonia.Enums;
using PKM;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL;

public partial class WindowDB : ClassWindowPKM
{
    public WindowDB()
    {
        InitializeComponent();
        ClassSettings settings = ClassSettings.Load();
        this.TypeDB.SelectedIndex = (int)settings.TypeDB;
        this.TxtDB.Text = settings.DB;
        this.TxtServer.Text = settings.Server;
        this.TxtLogin.Text = settings.Login;
        this.Pass.Text = settings.Password;
    }

    private void ButtonDB_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string path = ClassDialogWindows.ChooseDBDialog(this);
        TxtDB.Text = string.IsNullOrEmpty(path)? TxtDB.Text: path;
    }

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassSettings settings = ClassSettings.Load();
        settings.TypeDB = (ClassSettings.EnumTypeDB)this.TypeDB.SelectedIndex;
        settings.DB = this.TxtDB.Text;
        settings.Server = this.TxtServer.Text;
        settings.Login = this.TxtLogin.Text;
        settings.Password = this.Pass.Text;
        settings.Save();
        ClassMessage.ShowMessage(this, "Подключение к БД будет выполнено после перезапуска программы.", "", ButtonEnum.Ok,
                                          icon: MsBox.Avalonia.Enums.Icon.Info);
        this.Close();
    }
}