using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGrathic.ServiceClasses;

namespace TestGrathic.ViewMap;

public partial class WindowPropertyButton : Window, INotifyPropertyChanged
{
    private double _scaleUnitUnit;

    public double ScaleUnit
    {
        get => _scaleUnitUnit;
        set
        {
            if (value.Equals(_scaleUnitUnit)) return;
            _scaleUnitUnit = value;
            OnPropertyChanged();
        }
    }

    public WindowPropertyButton()
    {
        InitializeComponent();
    } 
    
    public WindowPropertyButton(SettingsUnitObject settingsUnitObject):this()
    {
        //TODO Привязать объект привязки, когда будет интеграция в СОТКУ.
        DataContext = this;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            var settingsUnitObject = new SettingsUnitObject
            {
                ScaleUnit = this.ScaleUnit,
                BindingObjectUnit = new BindingObject()
                //TODO Добавить код привязки.
            };
            Tag=settingsUnitObject;
        }
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}