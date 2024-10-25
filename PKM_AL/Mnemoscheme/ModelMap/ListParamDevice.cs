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

public class ClassParamForListDevice:INotifyPropertyChanged
{
    private int _id;
    private decimal _paramValue;
    private string _max = "-";
    private string _min = "-";
    private IBrush _brush1 = Brushes.LightGray;
    private IBrush _brush2 = Brushes.LightGray;
    private bool _isFlag;

    public bool IsFlag
    {
        get => _isFlag;
        set
        {
            if (value == _isFlag) return;
            _isFlag = value;
            OnPropertyChanged();
        }
    }

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

    public string Max
    {
        get => _max;
        set
        {
            if (value == _max) return;
            _max = value;
            OnPropertyChanged();
        }
    }

    public string Min
    {
        get => _min;
        set
        {
            if (value == _min) return;
            _min = value;
            OnPropertyChanged();
        }
    }
    
    public IBrush Brush1
    {
        get => _brush1;
        set
        {
            if (Equals(value, _brush1)) return;
            _brush1 = value;
            OnPropertyChanged();
        }
    }

    public IBrush Brush2
    {
        get => _brush2;
        set
        {
            if (Equals(value, _brush2)) return;
            _brush2 = value;
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

    #region [temp]

    // private IBrush _brush1 = Brushes.LightGray;
    // private IBrush _brush2 = Brushes.LightGray;
    
    // public IBrush Brush1
    // {
    //     get => _brush1;
    //     set
    //     {
    //         if (Equals(value, _brush1)) return;
    //         _brush1 = value;
    //         OnPropertyChanged();
    //     }
    // }
    //
    // public IBrush Brush2
    // {
    //     get => _brush2;
    //     set
    //     {
    //         if (Equals(value, _brush2)) return;
    //         _brush2 = value;
    //         OnPropertyChanged();
    //     }
    // }

    #endregion
    
    

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
        DataContext = this;
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            LoadListParamDevice();
        }
        else
        {
            CreateListParamDevice();
        }
        MainWindow.Widgets.Add(_stateWidget);
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
    /// Посторение таблицы списка параметров.
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
                new TextColumn<ClassParamForListDevice, string>
                ("Min", x => x.Min,  options:new TextColumnOptions<ClassParamForListDevice>
                {
                    TextAlignment = TextAlignment.Center
                }),
                new TextColumn<ClassParamForListDevice, string>
                ("Max", x => x.Max,  options:new TextColumnOptions<ClassParamForListDevice>
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

    /// <summary>
    /// Обновление юнита.
    /// </summary>
    /// <param name="tag"></param>
    private void RefreshListParamDevice(ClassWidget tag)
    {
        ParamListDev.Clear();
        _stateWidget.BindingObjectUnit = tag.BindingObjectUnit;
        _stateWidget.BindingObjects = tag.BindingObjects;
        CreateTableParam();
    }
   
    /// <summary>
    /// Получить тип юнита.
    /// </summary>
    /// <returns></returns>
    public override EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }
 
    /// <summary>
    /// Обновление значений списка.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="spare"></param>
    public override void SetValue(decimal value, object spare)
    {
        if (spare is not ClassChannel ch) return;
        var param = ParamListDev.FirstOrDefault(x => x.Id == ch.ID);
        if (param == null) return;
        param.ParamValue = value;
        param.Min = ch.Min.HasValue ? ch.Min.Value.ToString(CultureInfo.InvariantCulture) : "-";
        param.Max = ch.Max.HasValue ? ch.Max.Value.ToString(CultureInfo.InvariantCulture) : "-";
        if (ch.State == ClassChannel.EnumState.Norma)
        {
            param.IsFlag=false;
            param.Brush1 = Brushes.LawnGreen;
            param.Brush2 = Brushes.LawnGreen;
        }
        else if (ch.State == ClassChannel.EnumState.Over)
        {
            param.IsFlag=true;
            param.Brush1 = Brushes.Red;
            param.Brush2 = Brushes.Brown;
        }
        else
        {
            param.IsFlag=false;
            param.Brush1 =  param.Brush2 = Brushes.LightGray;
        }
    }
}//Конец класса ListParamDevice