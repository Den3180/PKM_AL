using System;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlBegin : UserControl
{
    public UserControlBegin()
    {
        InitializeComponent();
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.ReportOBD.FirstNode != null) return;
        if (WindowRepOBD.EHZ_EXHG.HasElements || WindowRepOBD.EHZ_EXHG.HasAttributes)
        {
            WindowRepOBD.EHZ_EXHG.RemoveAll();
        }
        WindowRepOBD.EHZ_EXHG.Add(new XAttribute(XML_RELOAD.Name, Convert.ToInt32(XML_RELOAD.IsChecked)));
    }

}