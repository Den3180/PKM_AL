using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewMap;
using PKM_AL.Mnemoscheme.ViewModelMap;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class ListParamDevice : AbstractControl
{
    private EnumUnit _enumUnit;
    private ClassMap _map;

    public ObservableCollection<BindingObject> BindingObjects { get; set; } = new ObservableCollection<BindingObject>();

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
        ContextMenu=CreateContextMenu(); 
        _map.Widgets.Add(_stateWidget);
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
            //TODO Объект привязки юнита сделать.
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
                // WindowPopertyIndicatorBig windowPropertyIndicatorBig = new WindowPopertyIndicatorBig(_stateWidget);
                // await windowPropertyIndicatorBig.ShowDialog(MainWindow.currentMainWindow);
                // if(windowPropertyIndicatorBig.Tag is not null) 
                //     RefreshListParamDevice((ClassWidget)windowPropertyIndicatorBig.Tag);
                break;
        }
    }

    private void RefreshListParamDevice(ClassWidget tag)
    {
        throw new System.NotImplementedException();
    }
    
    public override EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }
    
    public override void SetValue(decimal value)
    {
        //TODO Закончить с изменением величины.
    }
}