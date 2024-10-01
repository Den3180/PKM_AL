using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Mysqlx.Datatypes;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ViewMap;

public partial class WindowMapProperty : Window, INotifyPropertyChanged
{
    private Color _backGroundColor = Colors.LightBlue;
    private string nameMap = "Мнемосхема";

    public WindowMapProperty()
    {
        InitializeComponent();
        DataContext = this;
    }
    
    public WindowMapProperty(string name="", string backGroundColor="")
    {
        InitializeComponent();
        _backGroundColor=Color.Parse(backGroundColor);
        nameMap = name;
        DataContext = this;
    }
    
    public Color BackGroundColor
    {
        get => _backGroundColor;
        set
        {
            if (value.Equals(_backGroundColor)) return;
            _backGroundColor = value;
            OnPropertyChanged();
        }
    }

    public string NameMap
    {
        get => nameMap;
        set
        {
            if (value == nameMap) return;
            nameMap = value;
            OnPropertyChanged();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button ?? throw new InvalidOperationException();
        if (button.Name is "SaveBtn")
        {
            Close((NameMap, BackGroundColor.ToString()));
        }
        else
        {
            Close();
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}