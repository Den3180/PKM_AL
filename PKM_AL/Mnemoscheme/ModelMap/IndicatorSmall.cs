﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PKM_AL;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;

namespace TestGrathic.ModelMap;

public class IndicatorSmall : AbstractControl
{
    private readonly Lazy<ClassWidget> _settingsUnitObject = new Lazy<ClassWidget>();
    private double _sizeUnit;
    private ClassWidget _settings;
    public BindingObject BindingObject { get; set; }

    public double SizeUnit
    {
        get => _sizeUnit;
        set
        {
            if (value.Equals(_sizeUnit)) return;
            _sizeUnit = value;
            OnPropertyChanged();
        }
    }

    public IndicatorSmall()
    {
        CreateIndicatorD();
    }

    public IndicatorSmall(Rect bounds, double sizeUnit):this()
    {
        SizeUnit = sizeUnit;
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
    }

    private void CreateIndicatorD()
    {
        _settings = new ClassWidget();
        _sizeUnit = 24;
        ContextMenu=CreateContextMenu();
        DataContext = this; 
    }

    protected override async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit = new IndicatorSmall(Bounds,SizeUnit);
                break;
            case "Удалить":
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                IsBlocked = !IsBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = IsBlocked;
                break;
            case "Свойства":
                _settingsUnitObject.Value.FontSizeUnit = _sizeUnit;
                WindowPropertyIndicatorSmall windowPropertyIndicatorSmall = new WindowPropertyIndicatorSmall(_settingsUnitObject.Value);
                await windowPropertyIndicatorSmall.ShowDialog(MainWindow.currentMainWindow);
                if(windowPropertyIndicatorSmall.Tag is not null) 
                    RefreshIndicatorSmall((ClassWidget)windowPropertyIndicatorSmall.Tag);
                break;
        }
    }

    private void RefreshIndicatorSmall(ClassWidget tag)
    {
        SizeUnit = tag.FontSizeUnit;
        if (tag.BindingObjectUnit != null) BindingObject = tag.BindingObjectUnit;
    }
   
}