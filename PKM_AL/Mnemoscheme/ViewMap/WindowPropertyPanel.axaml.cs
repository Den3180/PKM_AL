using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using TestGrathic.ServiceClasses;

namespace TestGrathic.ViewMap;

public partial class WindowPropertyPanel : Window, INotifyPropertyChanged
{
    private double _widthPan=200D;
    private double _heightPan=300D;
    private Color _colorPan=Colors.Beige; 

    public double WidthPan
    {
        get => _widthPan;
        set
        {
            if (value.Equals(_widthPan)) return;
            _widthPan = value;
            OnPropertyChanged();
        }
    }

    public double HeightPan
    {
        get => _heightPan;
        set
        {
            if (value.Equals(_heightPan)) return;
            _heightPan = value;
            OnPropertyChanged();
        }
    }

    public Color ColorPan
    {
        get => _colorPan;
        set
        {
            if (value.Equals(_colorPan)) return;
            _colorPan = value;
            OnPropertyChanged();
        }
    }

    public WindowPropertyPanel()
    {
        InitializeComponent();
    }

    public WindowPropertyPanel(SettingsUnitObject? settingsUnitObject) : this()
    {
        if (settingsUnitObject != null)
        {
            _widthPan = settingsUnitObject.WidthUnit;
            _heightPan = settingsUnitObject.HeightUnit;
            _colorPan = settingsUnitObject.FontBrushUnit;
        }
        DataContext = this;
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            Tag = new SettingsUnitObject()
            {
                WidthUnit = _widthPan,
                HeightUnit = _heightPan,
                FontBrushUnit = _colorPan
            };
        }
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}