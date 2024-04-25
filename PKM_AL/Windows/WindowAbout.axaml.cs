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
        string sVersion;
        if (ver != null)
        {
            sVersion = ver.Major.ToString() + "." + ver.Minor.ToString();
            if (ver.Build > 0 || ver.Revision > 0) sVersion += "." + ver.Build.ToString();
            if (ver.Revision > 0) sVersion += "." + ver.Revision.ToString();
        }
        else
        {
            sVersion = "нет данных";
        }
        this.lblVersion.Text += " " + sVersion;
    }
}