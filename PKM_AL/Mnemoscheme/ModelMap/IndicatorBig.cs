using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;
using ZstdSharp.Unsafe;
using WindowPopertyIndicatorBig = PKM_AL.Mnemoscheme.ViewMap.WindowPopertyIndicatorBig;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class IndicatorBig : AbstractControl
{
    private decimal _paramValue;
    private string _paramName = "Параметр, ед.изм.";
    private EnumUnit _enumUnit;
    private ClassMap _map;
    public BindingObject? BindingObject { get; set; }

    public IndicatorBig(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _map = map;
        _enumUnit = enumUnit;
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            LoadIndicatorBig();
        }
        else
        {
            CreateIndicatorBig();
        }
        DataContext = this;
        MainWindow.Widgets.Add(_stateWidget);
    }

    public IndicatorBig(ClassWidget? stateWidge,Rect bounds, ClassMap map, EnumUnit enumUnit):this(map,enumUnit,stateWidge)
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(stateWidge);
    }
    
    /// <summary>
    /// Построение индикатора по сущ. параметрам(вторичная загрузка или загрузка из файла).
    /// </summary>
    private void LoadIndicatorBig()
    {
        //Установка размеров.
        Width = _stateWidget.WidthUnit;
        Height = _stateWidget.HeightUnit;
        _isBlocked=_stateWidget.IsBlocked;
        //Установка позиции.
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
        ContextMenu=CreateContextMenu();
        if (_stateWidget.BindingObjectUnit != null) 
            ParamName = _stateWidget.BindingObjectUnit.NameParam;
    }

    /// <summary>
    /// Первичное создание индикатора.
    /// </summary>
    private void CreateIndicatorBig()
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
        ContextMenu=CreateContextMenu(); 
        _map.Widgets.Add(_stateWidget);
    }

    
    public string ParamName
    {
        get => _paramName;
        set
        {
            if (value == _paramName) return;
            _paramName = value;
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
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit = new IndicatorBig(_stateWidget.Clone(),Bounds, _map,_enumUnit);
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
                WindowPopertyIndicatorBig windowPropertyIndicatorBig = new WindowPopertyIndicatorBig(_stateWidget);
                await windowPropertyIndicatorBig.ShowDialog(MainWindow.currentMainWindow);
                if(windowPropertyIndicatorBig.Tag is not null) 
                    RefreshIndicatorSmall((ClassWidget)windowPropertyIndicatorBig.Tag);
                //TODO Поворот надписи.
                break;
        }
    }

    private void RefreshIndicatorSmall(ClassWidget tag)
    {
        if (tag.BindingObjectUnit != null)
        {
            BindingObject = new BindingObject();
            BindingObject.IdDevice = tag.BindingObjectUnit.IdDevice;
            BindingObject.IdParam = tag.BindingObjectUnit.IdParam;
            BindingObject.NameParam = tag.BindingObjectUnit.NameParam;
            _stateWidget.BindingObjectUnit = BindingObject;
            ParamName = BindingObject.NameParam;
        }
    }

    public override EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }

    public override void SetValue(decimal value)
    {
        ParamValue = value;
    }
}
