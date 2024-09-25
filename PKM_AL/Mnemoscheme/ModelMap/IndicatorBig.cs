using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using TestGrathic.AbstractUnit;
using TestGrathic.Enums;
using TestGrathic.ServiceClasses;
using TestGrathic.ViewMap;
using TestGrathic.ViewModelMap;

namespace TestGrathic.ModelMap;

public class IndicatorBig : AbstractControl
{
    private decimal _paramValue;
    private string _paramName = "Параметр, ед.изм.";
    public BindingObject? BindingObject { get; set; }

    public IndicatorBig()
    {
        //_ = new TestCount();
        CreateIndicatorBig();
        DataContext = this; 
    }

    private void CreateIndicatorBig()
    {
        ContextMenu=CreateContextMenu(); 
    }

    public IndicatorBig(Rect bounds):this()
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
    }
    
    public string ParamName
    {
        get => _paramName;
        set
        {
            if (value == _paramName) return;
            OnPropertyChanged();
        }
    }

    public decimal ParamValue
    {
        get => _paramValue;
        set
        {
            _paramValue = value;
            OnPropertyChanged();
        }
    }
   
    protected override async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        var grid = tt?.Parent as Grid;
        var wnd=grid?.Parent as Window;
        if (wnd == null) return;
        switch (menuItem.Header)
        {
            case "Копировать" :
                MainViewModel.BufferCopiedOneUnit = new IndicatorBig(Bounds);
                break;
            case "Удалить":
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                IsBlocked = !IsBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = IsBlocked;
                break;
            case "Свойства":
                WindowPopertyIndicatorBig windowPropertyIndicatorBig = new WindowPopertyIndicatorBig();
                await windowPropertyIndicatorBig.ShowDialog(wnd);
                if(windowPropertyIndicatorBig.Tag is not null) 
                    RefreshIndicatorSmall((SettingsUnitObject)windowPropertyIndicatorBig.Tag);
                //TODO Поворот надписи.
                break;
        }
    }

    private void RefreshIndicatorSmall(SettingsUnitObject tag)
    {
        if (tag.BindingObjectUnit != null)
        {
            BindingObject = new BindingObject();
            BindingObject.IdDevice = tag.BindingObjectUnit.IdDevice;
            BindingObject.IdParam = tag.BindingObjectUnit.IdParam;
            BindingObject.NameParam = tag.BindingObjectUnit.NameParam;
        }
    }
   
}
