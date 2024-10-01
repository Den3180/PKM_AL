using System;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowPopertyIndicatorBig : Window
{
    public WindowPopertyIndicatorBig()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            var settingsObject = new ClassWidget
            {
                BindingObjectUnit = new BindingObject()
            };
            Tag = settingsObject;
        }
        Close();
    }
}