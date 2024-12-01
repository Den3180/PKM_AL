using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes.ServiceClasses;
using PKM;

namespace PKM_AL.Windows;

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
        Task < IReadOnlyList < IStorageFile >> files;
    private void ButtonDB_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string path = ClassDialogWindows.ChooseDbDialog(this);
        TxtDB.Text = string.IsNullOrEmpty(path)? TxtDB.Text: path;
    }

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Button button = (Button)sender;
        if (button?.IsCancel==true)
        {
            Close();
            return;
        }
        ClassSettings settings = ClassSettings.Load();
        settings.TypeDB = (ClassSettings.EnumTypeDB)this.TypeDB.SelectedIndex;
        settings.DB = this.TxtDB.Text;
        settings.Server = this.TxtServer.Text;
        settings.Login = this.TxtLogin.Text;
        settings.Password = this.Pass.Text;
        settings.Save();
        ClassMessage.ShowMessageCustom(this, "Подключение к базе данных будет выполнено после перезапуска программы.",
                                "", 
                                ButtonEnum.Ok,icon: MsBox.Avalonia.Enums.Icon.Info);
        this.Close();
        Environment.Exit(0);
    }
}