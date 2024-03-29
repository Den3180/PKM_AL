using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL.Controls.OBDControls.Trace_SituationTag;

public partial class UserControl_VEGETATION : UserControl
{
    public List<TextBox> lstTextBox;
    readonly ClassEventResource classEventResource = new ClassEventResource();
    public UserControl_VEGETATION()
    {
        InitializeComponent();
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
        ClassControlManager.SetToolTipText(lstTextBox);
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