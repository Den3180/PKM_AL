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
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewMap;
using PKM_AL.Mnemoscheme.ViewModelMap;

namespace PKM_AL.Mnemoscheme.ModelMap;

public sealed class ClassParamForListDevice:INotifyPropertyChanged
{
    private int _id;
    private decimal _paramValue;
    private decimal? _max;
    private decimal? _min;

    public string Nameparam{get; init; }=String.Empty;
    public int Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }


    public decimal ParamValue
    {
        get => _paramValue;
        set
        {
            if (value == _paramValue) return;
            _paramValue = value;
            OnPropertyChanged();
        }
    }

    public decimal? Max
    {
        get => _max;
        set
        {
            if (value == _max) return;
            _max = value;
            OnPropertyChanged();
        }
    }

    public decimal? Min
    {
        get => _min;
        set
        {
            if (value == _min) return;
            _min = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ListParamDevice : AbstractControl
{
    private EnumUnit _enumUnit;
    private ClassMap _map;
    

    public ObservableCollection<ClassParamForListDevice> ParamListDev { get; set; } = new ObservableCollection<ClassParamForListDevice>();
    public FlatTreeDataGridSource<ClassParamForListDevice> ParamDataSource { get; set; }

    /// <summary>
    /// Конструктор первичного создания.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="enumUnit"></param>
    /// <param name="stateWidget"></param>
    public ListParamDevice(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _map = map;
        _enumUnit = enumUnit;
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            LoadListParamDevice();
        }
        else
        {
            CreateListParamDevice();
        }
        DataContext = this;
        MainWindow.Widgets.Add(_stateWidget);
    }

    /// <summary>
    /// Создание юнита первичное.
    /// </summary>
    private void CreateListParamDevice()
    {
        _stateWidget = new ClassWidget()
        {
            GuidId = _map.GuidID,
            UnitType=_enumUnit,
            HeightUnit = Height,
            WidthUnit = Width,
            PositionX = Bounds.X,
            PositionY =Bounds.Y
        };
        WindowPropertyListParam listParam = new WindowPropertyListParam(_stateWidget);
        listParam.WindowShow(MainWindow.currentMainWindow);
        if (listParam.Tag is null) return;
        _stateWidget.BindingObjectUnit = (listParam.Tag as ClassWidget)?.BindingObjectUnit;
        _stateWidget.BindingObjects = (listParam.Tag as ClassWidget)?.BindingObjects;
        if(_stateWidget.BindingObjects==null) return;
        CreateTableParam();
        ContextMenu=CreateContextMenu();
        _map.Widgets.Add(_stateWidget);
    }

    /// <summary>
    /// Посторение тоблицы спичка параметров.
    /// </summary>
    private void CreateTableParam()
    {
        foreach (var bind in _stateWidget.BindingObjects)
        {
            ParamListDev.Add(new ClassParamForListDevice
            {
                Id=bind.IdParam,
                Nameparam = bind.NameParam,
            });
        }
        ParamDataSource = new FlatTreeDataGridSource<ClassParamForListDevice>(ParamListDev)
        {
            Columns =
            {
                new TextColumn<ClassParamForListDevice, string>
                ("Параметр", x => x.Nameparam,
                    options:new TextColumnOptions<ClassParamForListDevice>()
                    {
                        TextAlignment = TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap,
                    },
                    width:GridLength.Parse("150")),
                new TextColumn<ClassParamForListDevice, decimal>
                ("Значение", x => x.ParamValue,
                    options:new TextColumnOptions<ClassParamForListDevice>
                    {
                        TextAlignment = TextAlignment.Center
                    }),
                new TextColumn<ClassParamForListDevice, decimal?>
                ("Min", x => x.Min.Value,  options:new TextColumnOptions<ClassParamForListDevice>
                {
                    TextAlignment = TextAlignment.Center
                }),
                new TextColumn<ClassParamForListDevice, decimal?>
                ("Max", x => x.Max.Value,  options:new TextColumnOptions<ClassParamForListDevice>
                {
                    TextAlignment = TextAlignment.Center
                })
            },
        };
        ParamDataSource.Columns.Add(new TemplateColumn<ClassParamForListDevice>("Статус", "CheckBoxCell"));
    }

    /// <summary>
    /// Создание юнита вторичное.
    /// </summary>
    private void LoadListParamDevice()
    {
        //Установка размеров.
        Width = _stateWidget.WidthUnit;
        Height = _stateWidget.HeightUnit;
        _isBlocked=_stateWidget.IsBlocked;
        //Установка позиции.
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
        ContextMenu=CreateContextMenu();
        if (_stateWidget.BindingObjects.Count>0)
        {
            CreateTableParam();
        }
    }

    /// <summary>
    /// Конструктор копирования.
    /// </summary>
    /// <param name="stateWidge"></param>
    /// <param name="bounds"></param>
    /// <param name="map"></param>
    /// <param name="enumUnit"></param>
    public ListParamDevice(ClassWidget? stateWidge,Rect bounds, ClassMap map, EnumUnit enumUnit):this(map,enumUnit,stateWidge)
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(stateWidge);
    }

    /// <summary>
    /// Меню юнита.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override async void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit = new ListParamDevice(_stateWidget.Clone(),Bounds, _map,_enumUnit);
                break;
            case "Удалить":
                _map.Widgets.Remove(_stateWidget);
                MainWindow.Widgets.Remove(_stateWidget);
                MainWindow.MnemoUnit.Remove(this);
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                _stateWidget.IsBlocked=_isBlocked = !_isBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = _isBlocked;
                break;
            case "Свойства":
                WindowPropertyListParam listParam = new WindowPropertyListParam(_stateWidget);
                await listParam.ShowDialog(MainWindow.currentMainWindow);
                if(listParam.Tag is not null) 
                    RefreshListParamDevice((ClassWidget)listParam.Tag);
                break;
        }
    }

    private void RefreshListParamDevice(ClassWidget tag)
    {
        ParamListDev.Clear();
        _stateWidget.BindingObjectUnit = tag.BindingObjectUnit;
        _stateWidget.BindingObjects = tag.BindingObjects;
        CreateTableParam();
    }

    public ObservableCollection<BindingObject> GetBindingObjects()
    {
        return _stateWidget.BindingObjects;
    }
    
    public override EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }
    
    public override void SetValue(decimal value, object spare)
    {
        if (spare is not ClassChannel ch) return;
        var param = ParamListDev.FirstOrDefault(x => x.Id == ch.ID);
        if (param == null) return;
        param.ParamValue = value;
        if(ch.Max.HasValue) param.Max = ch.Max;
        if(ch.Min.HasValue) param.Min = ch.Min;
    }
}