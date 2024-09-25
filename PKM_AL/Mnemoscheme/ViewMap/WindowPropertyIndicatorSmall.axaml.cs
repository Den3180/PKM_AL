using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGrathic.ServiceClasses;

namespace TestGrathic.ViewMap;

public partial class WindowPropertyIndicatorSmall : Window, INotifyPropertyChanged
{
    private double _currentSize;


    public List<double> FontSizeList { get; set; } = new List<double>() { 
       8,9,10,11,12,13,14,15,16,18,20,24,26,28,36,48,72 };
    
    public WindowPropertyIndicatorSmall()
    {
        InitializeComponent();
    }
    
    public WindowPropertyIndicatorSmall(SettingsUnitObject settingsUnitObject):this()
    {
        //TODO Привязать объект привязки, когда будет интеграция в СОТКУ.
        _currentSize = settingsUnitObject.FontSizeUnit;
        DataContext = this;
    }
    
    public double CurrentSize
    {
        get => _currentSize;
        set
        {
            if (value.Equals(_currentSize)) return;
            _currentSize = value;
            OnPropertyChanged();
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            var settingsUnitObject = new SettingsUnitObject
            {
                FontSizeUnit = _currentSize,
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