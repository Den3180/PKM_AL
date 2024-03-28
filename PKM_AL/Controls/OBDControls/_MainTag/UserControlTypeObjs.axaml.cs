using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PKM_AL.Classes.ServiceClasses;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlTypeObjs : UserControl
{
    public List<TextBox> lstTextBox;
    public List<XElement> lstTypeObj; 

    public UserControlTypeObjs()
    {
        InitializeComponent();
        lstTypeObj = new List<XElement>();
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
        ClassControlManager.SetToolTipText(lstTextBox);
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
       // lstObjs.MaxHeight = this.Height-(0.2* this.Height);
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.TYPEOBJS.HasElements)
        {
            WindowRepOBD.TYPEOBJS.RemoveAll();
        }
        foreach (var elem in lstTypeObj)
        {
            WindowRepOBD.TYPEOBJS.Add(elem);
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        classEventResource.TextBox_LostFocus(sender,e);
        // if ((sender as DatePicker) != null) return;
        // TextBox textBox = sender as TextBox;
        // if (!string.IsNullOrEmpty(textBox.Text) && !Regex.IsMatch(textBox.Text, ClassControlManager.GetPatternOBDData(textBox.Tag)))
        // {
        //     Brush brushRed = new SolidColorBrush(Color.FromRgb(252, 170, 178));
        //     textBox.Background = brushRed;
        //     textBox.Foreground = Brushes.White;
        //     textBox.FontStyle = FontStyle.Italic;
        // }
        // else if(!string.IsNullOrEmpty(textBox.Text))
        // {
        //     textBox.Background = new SolidColorBrush(Color.FromRgb(217, 250, 205));
        // }
    }

    readonly ClassEventResource classEventResource = new ClassEventResource();
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        classEventResource.Button_Click(sender,e);
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