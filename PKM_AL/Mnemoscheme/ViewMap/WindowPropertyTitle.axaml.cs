using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using PKM_AL.Mnemoscheme.ServiceClasses;
using TestGrathic.ServiceClasses;


namespace TestGrathic.ViewMap;

public partial class WindowPropertyTitle : Window, INotifyPropertyChanged
{
    private readonly ClassWidget? _settingsUnitObject;
    public IFontCollection? FontFamilyList { get; set; }
    public List<string> FontSizeList { get; set; } = new List<string>() { 
        "8", "9", "10", "11", "12", "13", "14", "16", 
        "18", "20", "24", "26", "28", "36", "48", "72" };
    public FontWeight[]? FontWeightList { get; set; }
    
    private Color _fontColor;
    private string? _textTitle;
    private double _fontSizeTitle;
    private FontFamily? _fontStyle;
    private FontWeight _fontWeight;


    public WindowPropertyTitle()
    {
        InitializeComponent();
    }
    public WindowPropertyTitle(ClassWidget? settingsUnitObject):this()
    {
        _settingsUnitObject = settingsUnitObject;
        //Источники данных комбобоксов.
        FontFamilyList = FontManager.Current.SystemFonts;
        FontWeightList = Enum.GetValues<FontWeight>();
        //Входящий размер шрифта.
        _fontSizeTitle = _settingsUnitObject.FontSizeUnit;
        //Входящий цвет шрифта.
        _fontColor = _settingsUnitObject.FontBrushUnit;
        //Входящий текст.
        _textTitle= string.IsNullOrEmpty(_settingsUnitObject.TextUnit) ? "Текст" : _settingsUnitObject.TextUnit;
        //Входящий стиль шрифта.
        _fontStyle= _settingsUnitObject.FontStyleUnit==null ? FontFamily.Default :_settingsUnitObject.FontStyleUnit;
        //Входящий вес шрифта.
        _fontWeight=_settingsUnitObject.FontWeightUnit;
        DataContext = this;
    }
    
    public FontWeight FontWeightTitle
    {
        get => _fontWeight;
        set
        {
            if (value == _fontWeight) return;
            _fontWeight = value;
            OnPropertyChanged();
        }
    }
    
    public FontFamily? FontStyleTitle
    {
        get => _fontStyle;
        set
        {
            if (value == _fontStyle) return;
            _fontStyle = value;
            OnPropertyChanged();
        }
    }
    public double FontSizeTitle
    {
        get => _fontSizeTitle;
        set
        {
            if (value.Equals(_fontSizeTitle)) return;
            _fontSizeTitle = value;
            OnPropertyChanged();
        }
    }

    public string? TextTitle
    {
        get => _textTitle;
        set
        {
            if (value == _textTitle) return;
            _textTitle = value;
            OnPropertyChanged();
        }
    }

    public Color FontColor
    {
        get => _fontColor;
        set
        {
            _fontColor = value;
            OnPropertyChanged();
        }
    }
    
    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
         _settingsUnitObject.TextUnit = _textTitle;
         _settingsUnitObject.FontBrushUnit = _fontColor;
         _settingsUnitObject.FontSizeUnit = _fontSizeTitle;
         _settingsUnitObject.FontStyleUnit = _fontStyle;
         _settingsUnitObject.FontWeightUnit = _fontWeight;
         Tag=_settingsUnitObject;
        }
        Close();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}