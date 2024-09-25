using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGrathic.ServiceClasses;

namespace TestGrathic.ViewMap;

public partial class WindowImageProperty : Window, INotifyPropertyChanged
{
    private double _scaleUnit = 1;
    private double _widthUnit = 0;
    private double _heightUnit = 0;
    
    private readonly Lazy<SettingsUnitObject> _settingsUnitObject=new Lazy<SettingsUnitObject>();

    public double ScaleUnit
    {
        get => _scaleUnit;
        set
        {
            if (value.Equals(_scaleUnit)) return;
            _scaleUnit = value;
            OnPropertyChanged();
        }
    }

    public double WidthUnit
    {
        get => _widthUnit;
        set
        {
            if (value.Equals(_widthUnit)) return;
            _widthUnit = value;
            OnPropertyChanged();
        }
    }

    public double HeightUnit
    {
        get => _heightUnit;
        set
        {
            if (value.Equals(_heightUnit)) return;
            _heightUnit = value;
            OnPropertyChanged();
        }
    }

    public WindowImageProperty()
    {
        InitializeComponent();
    }

    public WindowImageProperty(SettingsUnitObject settingsUnitObject) : this()
    {
        _widthUnit = settingsUnitObject.WidthUnit;
        _heightUnit = settingsUnitObject.HeightUnit;
        DataContext = this;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            _settingsUnitObject.Value.ScaleUnit = ScaleUnit;
            _settingsUnitObject.Value.WidthUnit = WidthUnit;
            _settingsUnitObject.Value.HeightUnit = HeightUnit;
            Tag=_settingsUnitObject.Value;
        }
        Close(); 
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}