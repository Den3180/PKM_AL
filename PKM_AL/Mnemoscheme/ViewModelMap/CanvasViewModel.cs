using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Media;
using PKM_AL.Classes.ServiceClasses;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ModelMap;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewMap;

namespace PKM_AL.Mnemoscheme.ViewModelMap;

public sealed class CanvasViewModel :INotifyPropertyChanged
{
    private bool _isBlocked;
    public ObservableCollection<object> GraphicUnitObjects { get; set; } = new();
    // public static ObservableCollection<object> GraphicUnitObjects { get; set; } = new();
    public static List<object> BufferСopiedUnits { get; set; } = new();
    public static object? BufferCopiedOneUnit { get; set; }
    public ClassMap Map { get; set; }

    public CanvasViewModel()
    {
        Map = new ClassMap();
    }

    public CanvasViewModel(ClassMap map)
    {
        if (MainWindow.MnemoUnit != null)
        {
            //if(MainWindow.Maps.Count>0) MainWindow.Maps.Clear();
            // if(MainWindow.MnemoUnit.Count>0) MainWindow.MnemoUnit.Clear();
            // if(MainWindow.Widgets.Count>0) MainWindow.Widgets.Clear();
        }
        Map = map;
        MainWindow.Maps.Add(map);
        if (Map.Widgets.Count > 0)
        {
            foreach (var widget in Map.Widgets)
            {
                AddLoadShape(widget);
            }
        }
    }

    #region [Команды]

    /// <summary>
    /// Вставить юнит.
    /// </summary>
    /// <param name="obj"></param>
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
            EnumUnit.Title=>new TitleUnit(Map),
            EnumUnit.ImageKip=>new ImageUnit(Map,enumUnit),
            EnumUnit.ImagePipe=>new ImageUnit(Map,enumUnit),
            EnumUnit.Electrod => new ImageUnit(Map,enumUnit),
            EnumUnit.Shell => new ImageUnit(Map,enumUnit),
            EnumUnit.Panel=>new PanelUnit(Map,enumUnit),
            EnumUnit.IndicatorSmall=> new IndicatorSmall(Map,enumUnit),
            EnumUnit.IndicatorBig=> new IndicatorBig(Map,enumUnit),
            EnumUnit.ListParamDevice => new ListParamDevice(Map,enumUnit),
            _=>new ButtonUnit(Map,enumUnit)
        };
        GraphicUnitObjects.Add(newUnit);
        MainWindow.MnemoUnit.Add((IUnitService)newUnit);
    }

    /// <summary>
    /// Зафиксировать все элементы.
    /// </summary>
    /// <param name="obj"></param>
    public void FixAll(object obj)
    {
        IsBlocked = !IsBlocked;
        foreach (IUnitService unit in GraphicUnitObjects)
        {
                int index=GraphicUnitObjects.IndexOf(unit);
                unit.SetFixUnit(IsBlocked);
                Map.Widgets[index].IsBlocked = IsBlocked;
        }
    }

    /// <summary>
    /// Очистить форму. 
    /// </summary>
    /// <param name="obj">необязательный коммандный параметр</param>
    /// <returns></returns>
    public void DeleteAllShape(object obj)
    {
        GraphicUnitObjects.Clear();
        if(MainWindow.MnemoUnit.Count>0) MainWindow.MnemoUnit.Clear();
        if(MainWindow.Widgets.Count>0) MainWindow.Widgets.Clear();
        if(obj!=null && Map.Widgets.Count>0) Map.Widgets.Clear();
    }

    /// <summary>
    /// Сохранить мнемосхему.
    /// </summary>
    public async void SaveMnemoScheme()
    {
        //TODO на наличие директории мнемосхем,сохранять по имени мнемосхемы
        //var ss = Map.GetJson();
        if(MainWindow.currentMainWindow==null) return;
        var path= await ClassDialogWindows.SaveDialogSampleAsync
            (MainWindow.currentMainWindow, MainWindow.MapsPath,Map.Name,"sch" );
        if(!string.IsNullOrEmpty(path))  Map.SaveProfile(path);
    }

    /// <summary>
    /// Загружает мнемосхему из файла.
    /// </summary>
    /// <param name="obj"></param>
    public async void LoadMapXML(object obj)
    {
        if(MainWindow.currentMainWindow==null) return;
        var path= await ClassDialogWindows.ChooseDialogSampleAsync(MainWindow.currentMainWindow, MainWindow.MapsPath);
        if(string.IsNullOrEmpty(path)) return;
        var map = ClassMap.Load(path);
        DeleteAllShape(null);
        //if(MainWindow.Maps.Count>0) MainWindow.Maps.Clear();
        Map.MapClone(map);
        MainWindow.Maps.Add(map);
        foreach (var widget in Map.Widgets)
        {
            AddLoadShape(widget);
        }
    }

    /// <summary>
    /// Свойства мнемосхемы.
    /// </summary>
    public async void SetPropertiesMaps()
    {
        if(MainWindow.currentMainWindow==null) return;
        WindowMapProperty windowMapProperty = new WindowMapProperty(Map.Name,Map.MapColorString);
        (string,string)? res= await windowMapProperty.ShowDialog<(string,string)?>(MainWindow.currentMainWindow);
        if (!res.HasValue) return;
        Map.Name = res.Value.Item1;
        Map.BackgroundColor = Brush.Parse(res.Value.Item2);
    }

    #endregion

    /// <summary>
    /// Создает новую мнемосхему.
    /// </summary>
    public async void NewMnemoScheme()
    {
        if(MainWindow.currentMainWindow==null) return;
        WindowMapProperty windowMapProperty = new WindowMapProperty(Map.Name,Map.MapColorString);
        (string,string)? res= await windowMapProperty.ShowDialog<(string,string)?>(MainWindow.currentMainWindow);
        if (!res.HasValue) return;
        Map.Name = res.Value.Item1;
        Map.BackgroundColor = Brush.Parse(res.Value.Item2);
        Map.Widgets.Clear();
        Map.GuidID=Guid.NewGuid();
        DeleteAllShape(null);
        MainWindow.Maps.Add(Map);
    }
    
    /// <summary>
    /// Добавить юнит в немосхему, используя настройки виджета.
    /// </summary>
    /// <param name="widget"></param>
    public void AddLoadShape(ClassWidget widget)
    {
        EnumUnit enumUnit = widget.UnitType;
        object newUnit = enumUnit switch
        {
            EnumUnit.Title=>new TitleUnit(Map,widget),
            EnumUnit.ImageKip=>new ImageUnit(Map,enumUnit, widget),
            EnumUnit.ImagePipe=>new ImageUnit(Map,enumUnit, widget),
            EnumUnit.Electrod => new ImageUnit(Map,enumUnit,widget),
            EnumUnit.Shell => new ImageUnit(Map,enumUnit,widget),
            EnumUnit.Panel=>new PanelUnit(Map,enumUnit,widget),
            EnumUnit.IndicatorSmall=> new IndicatorSmall(Map,enumUnit,widget),
            EnumUnit.IndicatorBig=> new IndicatorBig(Map,enumUnit,widget),
            EnumUnit.ListParamDevice => new ListParamDevice(Map,enumUnit,widget),
            _=>new ButtonUnit(Map,enumUnit,widget)
        };
        GraphicUnitObjects.Add(newUnit);
        MainWindow.MnemoUnit.Add((IUnitService)newUnit);
    }

    public bool IsBlocked
    {
        get => _isBlocked;
        set
        {
            if (value == _isBlocked) return;
            _isBlocked = value;
            OnPropertyChanged();
        }
    }

    #region [Методы доступности команд]

    /// <summary>
    /// Доступность команды загрузки мнемосхемы из файла.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool CanLoadMapXML(object obj)
    {
        return true;
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

    public bool CanFixAll()
    {
        return GraphicUnitObjects.Count>0;
    }

    #endregion

    #region [PropertyChanged]

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

    #endregion
}