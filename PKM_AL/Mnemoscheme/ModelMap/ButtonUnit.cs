using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewMap;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class ButtonUnit : Button, IUnitService
{
     private readonly EnumUnit _enumUnit;
     private bool _isBlocked;
     private double _buttonSize = 35D;
     private bool _isFlag;
     private EnumTypeTransform _typeTransform = EnumTypeTransform.None;
     private Point _pos;
     private ClassWidget _stateWidget;
     private ClassMap _map;
     private Image _image;
     
    public ButtonUnit(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _map = map;
        _enumUnit=enumUnit;
        Height = 35;
        Width = 35;
        _image=new Image()
        {
            Source = new Bitmap(AssetLoader.Open(
                new Uri($"avares://{Assembly.GetEntryAssembly()?.GetName().Name}/Assets/" +
                        $"{HelperSourceImage.ListSourceImages[_enumUnit].Item1}")))
        };
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            CreateButtonLoad();
        }
        else
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
            _map.Widgets.Add(_stateWidget);
        }
        
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        Click += Click_Button;
        ContextMenu=CreateContextMenu(); 
        PointerMoved += ButtonUnit_PointerMoved;
        DataContext = this;
        MainWindow.Widgets.Add(_stateWidget);
    }
    public ButtonUnit(ClassWidget? stateWidge,Rect bounds, ClassMap map, EnumUnit enumUnit):this(map, enumUnit, stateWidge)
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(stateWidge);
    }

    /// <summary>
    /// Настрйка параметров загружаемой кнопки.
    /// </summary>
    private void CreateButtonLoad()
    {
        Width = _stateWidget.WidthUnit;
        Height = _stateWidget.HeightUnit;
        var dev = MainWindow.Devices.FirstOrDefault(
            d => _stateWidget.BindingObjectUnit != null && d.ID == _stateWidget.BindingObjectUnit.IdDevice
            );
        IsFlag = dev?.IsPoll ?? false;
        _isBlocked=_stateWidget.IsBlocked;
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var yy= e.GetCurrentPoint(this);
        if (yy.Properties.IsLeftButtonPressed)
        {
            _pos=e.GetPosition(this);
            ZIndex = 10;
        }
    }
    
    private void MoveButton(Point deltaPos, double leftPosPanel, double topPosPanel)
    {
        Canvas.SetLeft(this, leftPosPanel+ deltaPos.X);
        Canvas.SetTop(this, topPosPanel+deltaPos.Y);
        //Запись текущего положения.
        _stateWidget.PositionX = Bounds.X;
        _stateWidget.PositionY = Bounds.Y;
    }
    
    private void SetCursor(object? sender, bool pressed=false)
    {
        if(pressed) return;
        Cursor = Cursor.Default;
        _typeTransform = EnumTypeTransform.None;
    }

    private void ButtonUnit_PointerMoved(object? sender, PointerEventArgs e)
    {
        SetCursor(sender,IsPressed);
        if(!IsPressed || _isBlocked) return;
        _typeTransform = EnumTypeTransform.Move;
        var curPointPos = e.GetPosition(this);
        var deltaPos = curPointPos -_pos;
        var leftPosPanel= Bounds.X;
        var topPosPanel = Bounds.Y;
        if ( _typeTransform == EnumTypeTransform.Move)
        {
            MoveButton(deltaPos,leftPosPanel,topPosPanel);
        }
    }

    private ContextMenu? CreateContextMenu()
    {
        ContextMenu contextMenu = new ContextMenu();

        #region [Копировать]
        MenuItem menuItem = new MenuItem
        {
            Header = "Копировать"
        };
        menuItem.Click += MenuItem_Click;
        contextMenu.Items.Add(menuItem);
        #endregion

        #region [Удалить]

        menuItem = new MenuItem
        {
            Header = "Удалить"

        };
        menuItem.Click += MenuItem_Click;
        contextMenu.Items.Add(menuItem);

            #endregion

        #region [Закрепить]

            menuItem = new MenuItem()
            {
                Header = "Закрепить"
            };
            menuItem.Icon = new CheckBox()
            {
                IsChecked = _isBlocked,
            }; 
            var item=menuItem;
            ((CheckBox)menuItem.Icon).Click+= (s, e) =>
            {
                MenuItem_Click(item, e);
                contextMenu.Close();
            };
            menuItem.Click += MenuItem_Click;
            contextMenu.Items.Add(menuItem);

        #endregion

        #region Сепаратор

        var separator = new Separator()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.Black,
        };
        contextMenu.Items.Add(separator);

            #endregion

        #region [Свойства]

                menuItem = new MenuItem
                {
                    Header = "Свойства"

                };
                menuItem.Click += MenuItem_Click;
                contextMenu.Items.Add(menuItem);

                #endregion
                
        return contextMenu;
    }

    private async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
         var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать":
                CanvasViewModel.BufferCopiedOneUnit = new ButtonUnit(_stateWidget.Clone(),Bounds,_map, _enumUnit);
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
                WindowPropertyButton windowPropertyButton = new WindowPropertyButton(_stateWidget);
                await windowPropertyButton.ShowDialog(MainWindow.currentMainWindow);
                if(windowPropertyButton.Tag is not null) 
                    RefreshButton((ClassWidget)windowPropertyButton.Tag);
                //TODO Поворот надписи.
                break;
        }
    }

    private void RefreshButton(ClassWidget tag)
    {
        Width *= tag.ScaleUnit;
        Height *= tag.ScaleUnit;
        _stateWidget.WidthUnit = Width;
        _stateWidget.HeightUnit = Height;
        _stateWidget.BindingObjectUnit = tag.BindingObjectUnit;
        _stateWidget.IsDevicePoll=tag.IsDevicePoll;
        IsFlag=tag.IsDevicePoll;
    }


    /// <summary>
    /// Обработчик кнопки.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Click_Button(object? sender, RoutedEventArgs e)
    {
        if (_typeTransform == EnumTypeTransform.Move)
        {
            _typeTransform = EnumTypeTransform.None;
            SetCursor(sender,IsPressed);
            return;
        }
        IsFlag = !IsFlag;
        ButtonStatusHandler(_enumUnit);
    }

    /// <summary>
    /// Обработчик статуса кнопки.
    /// </summary>
    /// <param name="enumUnit"></param>
    private void ButtonStatusHandler(EnumUnit enumUnit)
    {
        if (enumUnit == EnumUnit.ButtonOffOn)
        {
            if(_stateWidget.BindingObjectUnit is { IdDevice: 0 }) return;
            var device=MainWindow.Devices.FirstOrDefault(dev=>dev.ID==_stateWidget.BindingObjectUnit?.IdDevice);
            if (device != null) device.IsPoll = _isFlag;
        }
    }

    /// <summary>
    /// Настройка источника изображения кнопки.
    /// </summary>
    /// <param name="flag"></param>
    private void SetImageRecource(bool flag)
    {
        if (flag)
        {
            Image.Source=new Bitmap(AssetLoader.Open(
                new Uri($"avares://{Assembly.GetEntryAssembly()?.GetName().Name}" +
                        $"/Assets/{HelperSourceImage.ListSourceImages[_enumUnit].Item2}")));
        }
        else
        {
            Image.Source = new Bitmap(AssetLoader.Open(new Uri(
                $"avares://{Assembly.GetEntryAssembly()?.GetName().Name}" +
                $"/Assets/{HelperSourceImage.ListSourceImages[_enumUnit].Item1}")));
        }
    }

    public bool IsFlag
    {
        get => _isFlag;
        set
        {
           _isFlag = value;
           SetImageRecource(_isFlag);
           ButtonStatusHandler(_enumUnit);
           _stateWidget.IsDevicePoll = _isFlag;
        }
    }

    public double ButtonSize
    {
        get=> _buttonSize;
        set
        {
            _buttonSize = value;
            OnPropertyChanged();
        }
    }
    
     public Image Image
     {
         get => _image;
         set
         {
             _image = value;
             OnPropertyChanged();
         }
     } 

    public EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }

    public BindingObject GetBindingObject()
    {
        return _stateWidget.BindingObjectUnit;
    }

    public void SetValue(decimal value, object spare)
    {
        throw new NotImplementedException();
    }

    public void SetFixUnit(bool fix)
    {
        _isBlocked = fix;
        var mItem= ContextMenu?.Items.FirstOrDefault(t => ((MenuItem)t).Header!.ToString()!.Equals("Закрепить"));
        if(mItem==null) return;
        (((mItem as MenuItem)?.Icon as CheckBox)!).IsChecked = fix;
    }


    #region [PropertyChanged]
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
