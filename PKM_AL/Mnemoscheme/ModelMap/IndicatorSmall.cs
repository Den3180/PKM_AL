using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;
using WindowPropertyIndicatorSmall = PKM_AL.Mnemoscheme.ViewMap.WindowPropertyIndicatorSmall;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class IndicatorSmall : AbstractControl
{
    private double _sizeUnit;
    private EnumUnit _enumUnit;
    private ClassMap _map;
    private decimal _paramValue = 0.00M;
    public BindingObject BindingObject { get; set; }


    /// <summary>
    /// Конструктор создания.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="enumUnit"></param>
    /// <param name="stateWidget"></param>
    public IndicatorSmall(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _map = map;
        _enumUnit = enumUnit;
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            LoadIndicatorSmall();
        }
        else
        {
            CreateIndicatorSmall();
        }
        DataContext = this;
        MainWindow.Widgets.Add(_stateWidget);
    }


    /// <summary>
    /// Конструктор копирования.
    /// </summary>
    /// <param name="stateWidge"></param>
    /// <param name="bounds"></param>
    /// <param name="map"></param>
    /// <param name="enumUnit"></param>
    /// <param name="sizeUnit"></param>
    public IndicatorSmall(ClassWidget? stateWidge,Rect bounds, ClassMap map, EnumUnit enumUnit, double sizeUnit):this(map,enumUnit,stateWidge)
    {
        SizeUnit = sizeUnit;
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(_stateWidget);
    }
    
    private void LoadIndicatorSmall()
    {
        SizeUnit = _stateWidget.FontSizeUnit;
        //Установка позиции.
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
        ContextMenu=CreateContextMenu(); 
    }

    private void CreateIndicatorSmall()
    {
        _sizeUnit = 24;
        _stateWidget = new ClassWidget()
        {
            GuidId = _map.GuidID,
            UnitType=_enumUnit,
            FontSizeUnit = SizeUnit,
            PositionX = Bounds.X,
            PositionY =Bounds.Y
        };
        ContextMenu=CreateContextMenu();
        _map.Widgets.Add(_stateWidget);
    }

    protected override async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit = new IndicatorSmall(_stateWidget.Clone(),Bounds, _map,_enumUnit,SizeUnit);
                break;
            case "Удалить":
                _map.Widgets.Remove(_stateWidget);
                MainWindow.Widgets.Remove(_stateWidget);
                MainWindow.MnemoUnit.Remove(this);
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                _isBlocked = !_isBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = _isBlocked;
                break;
            case "Свойства":
                WindowPropertyIndicatorSmall windowPropertyIndicatorSmall = new WindowPropertyIndicatorSmall(_stateWidget);
                await windowPropertyIndicatorSmall.ShowDialog(MainWindow.currentMainWindow);
                if(windowPropertyIndicatorSmall.Tag is not null) 
                    RefreshIndicatorSmall((ClassWidget)windowPropertyIndicatorSmall.Tag);
                break;
        }
    }

    private void RefreshIndicatorSmall(ClassWidget tag)
    {
        SizeUnit = tag.FontSizeUnit;
        _stateWidget.FontSizeUnit=SizeUnit;
        if (tag.BindingObjectUnit != null)
        {
            BindingObject = tag.BindingObjectUnit;
            _stateWidget.BindingObjectUnit = BindingObject;
        }
    }
  
    /// <summary>
    /// Свойство-привязка к размеру шрифта.
    /// </summary>
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
    
    public decimal ParamValue
    {
        get => _paramValue;
        set
        {
            _paramValue = value;
            OnPropertyChanged();
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
