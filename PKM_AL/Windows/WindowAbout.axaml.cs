using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL.Windows;

public partial class WindowAbout : ClassWindowPKM
{
    public WindowAbout()
    {
        InitializeComponent();
        Version ver = Assembly.GetExecutingAssembly().GetName().Version;
        var sVersion = ver != null ? $"{ver.Major}.{ver.Minor}.{ver.Build}" : "нет данных";
        this.lblVersion.Text += " " + sVersion;
    }
}