using System;
using System.Reflection;
using Avalonia.Controls;

namespace PKM_AL.Windows;

public partial class WindowAbout: Window
{
    public WindowAbout()
    {
        InitializeComponent();
        Version ver = Assembly.GetExecutingAssembly().GetName().Version;
        var sVersion = ver != null ? $"{ver.Major}.{ver.Minor}.{ver.Build}" : "нет данных";
        this.lblVersion.Text += " " + sVersion;
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}