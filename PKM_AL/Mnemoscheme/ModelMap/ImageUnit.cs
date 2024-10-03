using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Image = Avalonia.Controls.Image;
using WindowImageProperty = PKM_AL.Mnemoscheme.ViewMap.WindowImageProperty;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class ImageUnit : Image,IUnitService
{
    private Point  _pos;
    private bool _isPressed;
    private EnumTypeTransform _typeTransform;
    private double _startwidth;
    private double _startheight;
    private double _koef;
    private EnumUnit _enumUnit;
    private bool _isBlocked;
    private ClassWidget _stateWidget;
    private ClassMap _map;
    private readonly Dictionary<EnumUnit,string> _unitsPath = new Dictionary<EnumUnit, string>
    {
        { EnumUnit.ImageKip ,"kip3.png"},
        { EnumUnit.ImagePipe ,"Pipe Yellow Horz.png"}
    };
    public ImageUnit(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _enumUnit = enumUnit;
        _map = map;
        Source = new Bitmap(AssetLoader.Open
            (new Uri($"avares://{Assembly.GetEntryAssembly()?.GetName().Name}/Assets/{_unitsPath[enumUnit]}")));
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            CreateLoadImageUnit();
        }
        else
        {
            CreateImageUnit();
            _map.Widgets.Add(_stateWidget);
        }
        Stretch = Stretch.Uniform;
        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        Effect = new DropShadowEffect()
        {
            BlurRadius = 10,
            Color = Colors.DimGray,
            OffsetY=10
        };
        ContextMenu=CreateContextMenu();
        PointerPressed += ImageUnit_OnPointerPressed;
        PointerMoved += ImageUnit_OnPointerMove;
        PointerReleased += ImageUnit_PointerReleased;
        Loaded += ImageUnit_Loaded;
        MainWindow.Widgets.Add(_stateWidget);
    }

    /// <summary>
    /// Создание юнита из файла.
    /// </summary>
    private void CreateLoadImageUnit()
    {
        //Установка размеров.
        Width = _stateWidget.WidthUnit;
        Height = _stateWidget.HeightUnit;
        //Установка позиции.
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
    }

    public ImageUnit(Rect bounds, EnumUnit enumUnit, ClassMap map, ClassWidget stateWidge): this(map,enumUnit,stateWidge)
    {
        Width = bounds.Width;
        Height = bounds.Height;
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(stateWidge);
    }

    /// <summary>
    /// Создание юнита. 
    /// </summary>
    private void CreateImageUnit()
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
         Canvas.SetLeft(this, 0);
         Canvas.SetTop(this, 0);
    }

    private void ImageUnit_Loaded(object? sender, RoutedEventArgs e)
    {
        _startwidth = Width = Bounds.Width;
        _startheight = Height = Bounds.Height;
    }

    /// <summary>
    /// Настройка значка курсора. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="pressed"></param>
    private void SetCursor(object? sender, PointerEventArgs e, bool pressed=false)
    {
        if(pressed) return;
        Point position = e.GetPosition(this);
        if (position.X>=Bounds.Width-5 && position.X<=Bounds.Width+5)
        {
            Cursor = new Cursor(StandardCursorType.SizeWestEast);
            _typeTransform = EnumTypeTransform.ResizeX;
        }
        else if (position.Y >= Bounds.Height-5 && position.Y <= Bounds.Height+5)
        {
            Cursor = new Cursor(StandardCursorType.SizeNorthSouth);
            _typeTransform = EnumTypeTransform.ResizeY;
        }
        else
        {
            Cursor = Cursor.Default;
            _typeTransform = EnumTypeTransform.Move;
        }
    }

    /// <summary>
    /// Создать контекстное меню.
    /// </summary>
    /// <returns></returns>
    private ContextMenu CreateContextMenu()
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
        
        #region [Сепаратор]
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
    
    /// <summary>
    /// Обработчик контекстного меню панели.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if(sender is not MenuItem menuItem) return;
        var tt = Parent as ItemsControl;
        switch (menuItem.Header)
        {
            case "Копировать" :
                CanvasViewModel.BufferCopiedOneUnit=new ImageUnit(Bounds,_enumUnit,_map,_stateWidget.Clone());
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
                _stateWidget.WidthUnit = Width;
                _stateWidget.HeightUnit = Height;
                WindowImageProperty windowImageProperty = new WindowImageProperty(_stateWidget);
                await windowImageProperty.ShowDialog(MainWindow.currentMainWindow);
                if(windowImageProperty.Tag is not null) 
                    RefreshImage((ClassWidget)windowImageProperty.Tag);
                break;
        }
    }

    private void RefreshImage(ClassWidget tag)
    {
        if (!tag.ScaleUnit.Equals(1))
        {
            Width *= tag.ScaleUnit;
            Height *= tag.ScaleUnit;
        }
        else
        {
            Width = tag.WidthUnit;
            Height = tag.HeightUnit;
        }
        _stateWidget.WidthUnit = Width;
        _stateWidget.HeightUnit = Height;
    }

    private void ImageUnit_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isPressed = false;
        _typeTransform = EnumTypeTransform.None;
        _startwidth = Bounds.Width;
        _startheight = Bounds.Height;
        ZIndex=0;
    }

    private void ImageUnit_OnPointerMove(object? sender, PointerEventArgs e)
    {
        SetCursor(sender,e,_isPressed);
        if(_isPressed==false || _isBlocked) return;
        var curPointPos = e.GetPosition(this);
        var deltaPos = curPointPos -_pos;
        var leftPosPanel= Canvas.GetLeft(this);
        var topPosPanel= Canvas.GetTop(this);
        if ( _typeTransform == EnumTypeTransform.Move)
        {
            MovePanel(deltaPos,leftPosPanel,topPosPanel);
        }
        else if(_typeTransform == EnumTypeTransform.ResizeX)
        {
            PanelResizeX(deltaPos);
        }
        else if(_typeTransform == EnumTypeTransform.ResizeY)
        {
            PanelResizeY(deltaPos);
        }
    }

    private void PanelResizeY(Point deltaPos)
    {
        _koef = (_startheight+deltaPos.Y) / Height;
        Height =_startheight + deltaPos.Y;
        Width*=_koef;
        _stateWidget.HeightUnit = Height;
    }

    private void PanelResizeX(Point deltaPos)
    {
        _koef = (_startwidth+deltaPos.X) / Width;
        Width =_startwidth + deltaPos.X;
        Height *= _koef;
        _stateWidget.WidthUnit = Width;
    }

    private void MovePanel(Point deltaPos, double leftPosPanel, double topPosPanel)
    {
        Canvas.SetLeft(this, leftPosPanel+ deltaPos.X);
        Canvas.SetTop(this, topPosPanel+deltaPos.Y);
        _stateWidget.PositionX = Bounds.X;
        _stateWidget.PositionY = Bounds.Y;
    }

    private void ImageUnit_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var currentPoint= e.GetCurrentPoint(this);
        if (currentPoint.Properties.IsLeftButtonPressed)
        {
            _isPressed = true;
            _pos=e.GetPosition(this);
            ZIndex = 10;
            if(Cursor != null && Cursor.Equals(Cursor.Default))
                Cursor = new Cursor(StandardCursorType.SizeAll);
        }
    }

    public EnumUnit GetTypeUnit()
    {
        return _enumUnit;
    }

    public void SetValue(decimal value)
    {
        throw new NotImplementedException();
    }
}