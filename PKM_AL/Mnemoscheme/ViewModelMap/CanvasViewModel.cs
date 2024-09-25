using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PKM_AL.Mnemoscheme.Enums;
using TestGrathic.ModelMap;

namespace TestGrathic.ViewModelMap;

public sealed class CanvasViewModel :INotifyPropertyChanged
{
    public static ObservableCollection<object> GraphicUnitObjects { get; set; } = new();
    public static List<object> BufferСopiedUnits { get; set; } = new();
    public static object? BufferCopiedOneUnit { get; set; }
    
    public CanvasViewModel()
    {
    }
    public void InsertUnit(object obj)
    {
        if(BufferCopiedOneUnit == null) return;
        GraphicUnitObjects.Add(BufferCopiedOneUnit);
        BufferCopiedOneUnit = null;
    }

    /// <summary>
    /// Добавить элемент на форму.
    /// </summary>
    /// <param name="obj">тип элемента</param>
    public void AddShape(object obj)
    {
        EnumUnit enumUnit = (EnumUnit)Convert.ToInt32(obj);
        object newUnit = enumUnit switch
        {
            EnumUnit.Title=>new TitleUnit(),
            EnumUnit.ImageKip=>new ImageUnit(enumUnit),
            EnumUnit.ImagePipe=>new ImageUnit(enumUnit),
            EnumUnit.Panel=>new PanelUnit(),
            EnumUnit.IndicatorDigital=> new IndicatorSmall(),
            EnumUnit.IndicatorAnalog=> new IndicatorBig(),
            _=>new ButtonUnit()
        };
        GraphicUnitObjects.Add(newUnit);
    }

    /// <summary>
    /// Очистить форму. 
    /// </summary>
    /// <param name="obj">необязательный коммандный параметр</param>
    /// <returns></returns>
    public void DeleteAllShape(object obj)
    {
        GraphicUnitObjects.Clear();
    }

    /// <summary>
    /// Доступность команды.<see cref="DeleteAllShape"/>
    /// </summary>
    /// <param name="obj">необязательный коммандный параметр</param>
    /// <returns></returns>
    public bool CanDeleteAllShape(object obj)
    {
        return GraphicUnitObjects.Count>0;
    }

    /// <summary>
    /// Доступность команды.<see cref="InsertUnit"/>
    /// </summary>
    /// <param name="obj">необязательный коммандный параметр</param>
    /// <returns></returns>
    public bool CanInsertUnit(object obj)
    {
        return BufferCopiedOneUnit != null;
    }

    /// <summary>
    /// Доступность команды.<see cref="AddShape"/>
    /// </summary>
    /// <param name="obj">необязательный коммандный параметр</param>
    /// <returns></returns>
    public bool CanAddShape(object obj)
    {
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}