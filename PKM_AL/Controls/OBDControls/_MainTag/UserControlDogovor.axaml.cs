using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes.ServiceClasses;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlDogovor : UserControl
{
    public List<TextBox> lstTextBox;
    public List<CalendarDatePicker> datePickers;
    readonly ClassEventResource classEventResource = new ClassEventResource();
    
    public UserControlDogovor()
    {
        InitializeComponent();
        datePickers = new List<CalendarDatePicker>();
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
        ClassControlManager.GetUIElem<CalendarDatePicker>(this,datePickers);
        ClassControlManager.SetToolTipText(lstTextBox);
    }
    
    public void TagAssembly()
    {
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
        foreach (CalendarDatePicker picker in datePickers)
        {
            lstTextBox.Add(new TextBox{ Name=picker.Name,Text=picker.Text});
        }
        if (WindowRepOBD.DOGOVOR.HasAttributes)
        {
            WindowRepOBD.DOGOVOR.RemoveAll();
        }
        foreach(var tbox in lstTextBox)
        {
            WindowRepOBD.DOGOVOR.Add(new XAttribute(tbox.Name,tbox.Text));
        }
    }
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        classEventResource.TextBox_LostFocus(sender,e);
    }

    private void TextBox_GotFocus(object sender, GotFocusEventArgs e)
    {
        classEventResource.TextBox_GotFocus(sender,e);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        classEventResource.TextBox_TextChanged(sender,e);
    }
    
}