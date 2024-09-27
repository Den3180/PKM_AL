using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PKM_AL.Mnemoscheme.ServiceClasses;
using TestGrathic.ServiceClasses;

namespace TestGrathic.ViewMap;

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