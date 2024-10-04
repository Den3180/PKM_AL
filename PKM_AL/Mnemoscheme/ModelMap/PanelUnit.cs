using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using PKM_AL;
using PKM_AL.Mnemoscheme;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;
using Color = System.Drawing.Color;

namespace TestGrathic.ModelMap;

public class PanelUnit : Rectangle, IUnitService
{
    private Point  _pos;
    private bool _isPressed;
    private bool _isBlocked;
    private EnumTypeTransform _typeTransform;
    private double _startwidth;
    private double _startheight;
    private ClassWidget _stateWidget;
    private ClassMap _map;
    private EnumUnit _enumUnit;


    public PanelUnit(ClassMap map,EnumUnit enumUnit, ClassWidget stateWidget=null)
    {
        _map = map;
        _enumUnit=enumUnit;
        _typeTransform = EnumTypeTransform.None;
        if (stateWidget != null)
        {
            _stateWidget = stateWidget;
            LoadRectangle();
        }
        else
        {
            CreateRectangle();
        }
        StrokeThickness = 3D;
        Stroke= Brushes.Black;
        ContextMenu=CreateContextMenu();
        PointerPressed += rectangle_OnPointerPressed;
        PointerMoved += rectangle_OnPointerMove;
        PointerReleased += rectangle_PointerReleased;
        Effect = new DropShadowEffect()
        {
            Color = Colors.DimGray,
            BlurRadius = 2
        };
        ZIndex = -1;
        MainWindow.Widgets.Add(_stateWidget);
    }

    public PanelUnit(ClassWidget? stateWidge,Rect bounds, ClassMap map, EnumUnit enumUnit):this(map, enumUnit, stateWidge)
    {
        Width = bounds.Width;
        Height = bounds.Height;
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
        _map.Widgets.Add(stateWidge);
    }
    
    private void LoadRectangle()
    {
        //Установка размеров.
        Width = _stateWidget.WidthUnit;
        Height = _stateWidget.HeightUnit;
        //Установка цвета.
        Fill = Brush.Parse(_stateWidget.BackgroundUnit);
        //Установка позиции.
        Canvas.SetLeft(this, _stateWidget.PositionX);
        Canvas.SetTop(this, _stateWidget.PositionY);
    }
    
    /// <summary>
    /// Создать панель.
    /// </summary>
    /// <param name="widht"></param>
    /// <param name="height"></param>
    private void CreateRectangle(double widht = 200, double height = 300D)
    {
        _startwidth=widht; 
        _startheight = height; 
        Width = widht;
        Height = height;
        Fill = Brushes.Azure; //change
        _stateWidget = new ClassWidget()
        {
            GuidId = _map.GuidID,
            UnitType=_enumUnit,
            HeightUnit = Height,
            WidthUnit = Width,
            PositionX = Bounds.X,
            PositionY =Bounds.Y,
            BackgroundUnit = Fill.ToString()
        };
        Canvas.SetLeft(this, 0);
        Canvas.SetTop(this, 0);
        _map.Widgets.Add(_stateWidget);
    }

    /// <summary>
    /// Контекстное меню панели.
    /// </summary>
    /// <returns>ContextMenu</returns>
    private ContextMenu CreateContextMenu()
    {
        ContextMenu contextMenu = new ContextMenu();
        
        MenuItem menuItem = new MenuItem
        {
            Header = "Копировать"
        };
        menuItem.Click += MenuItem_Click;
        contextMenu.Items.Add(menuItem);
        menuItem = new MenuItem
        {
            Header = "Удалить"

        };
        menuItem.Click += MenuItem_Click;
        contextMenu.Items.Add(menuItem);
        
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
        
        
        menuItem = new MenuItem
        {
            Header = "Свойства"

        };
        menuItem.Click += MenuItem_Click;
        contextMenu.Items.Add(menuItem);
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
                CanvasViewModel.BufferCopiedOneUnit = new PanelUnit(_stateWidget.Clone(),Bounds,_map, _enumUnit);
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
                _stateWidget.HeightUnit = Height;
                _stateWidget.WidthUnit = Width;
                if (Fill != null) 
                    _stateWidget.FontBrushUnit = Fill.ToString() ?? Brushes.Black.ToString();
                WindowPropertyPanel propertyPanel = new WindowPropertyPanel(_stateWidget);
                await propertyPanel.ShowDialog(MainWindow.currentMainWindow);
                if(propertyPanel.Tag is not null) RefreshTitle((ClassWidget)propertyPanel.Tag);
                break;
        }
    }

    private void RefreshTitle(ClassWidget propertyPanelTag)
    {
        Width = propertyPanelTag.WidthUnit;
        Height = propertyPanelTag.HeightUnit;
        Fill = Brush.Parse(propertyPanelTag.FontBrushUnit.ToString());
        _stateWidget.HeightUnit = Height;
        _stateWidget.WidthUnit = Width;
        _stateWidget.BackgroundUnit = Fill.ToString() ?? Brushes.Azure.ToString();
    }

    /// <summary>
    /// Кнопка мыши отпущена.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void rectangle_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        PanelUnit panel=sender as PanelUnit ?? throw new InvalidOperationException();
        _isPressed = false;
        _typeTransform = EnumTypeTransform.None;
        _startwidth = panel.Width;
        _startheight = panel.Height;
        panel.ZIndex=-1;
    }
  
    /// <summary>
    /// Движение мыши.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void rectangle_OnPointerMove(object? sender, PointerEventArgs e)
    {
        PanelUnit panel=sender as PanelUnit ?? throw new InvalidOperationException();
        SetCursor(sender, e, _isPressed);
        if(!_isPressed  || _isBlocked) return;
        var pp = e.GetPosition(this);
        var deltaPos = e.GetPosition(this) - (Vector)_pos;
        var leftPosPanel= Canvas.GetLeft(panel);
        var topPosPanel= Canvas.GetTop(panel);
        if ( _typeTransform == EnumTypeTransform.Move)
        {
            MovePanel(panel,deltaPos,leftPosPanel,topPosPanel);
        }
        else if(_typeTransform == EnumTypeTransform.ResizeX)
        {
            PanelResizeX(panel, leftPosPanel,deltaPos,e);
        }
        else if(_typeTransform == EnumTypeTransform.ResizeY)
        {
            PanelResizeY(panel,topPosPanel, deltaPos, e);
        }
    }

    /// <summary>
    /// Перемещение границ панели по оси X.
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="leftPosPanel"></param>
    /// <param name="deltaPos"></param>
    /// <param name="e"></param>
    private void PanelResizeX(PanelUnit panel, double leftPosPanel, Point deltaPos,PointerEventArgs e)
    {
        //Движение левой границы.
        if (e.GetPosition(panel).X >= 0-20 && e.GetPosition(panel).X <= 0+20)
        {
            var f = e.GetPosition(panel).X;
            if(Width-deltaPos.X<50) return; 
            leftPosPanel= Canvas.GetLeft(panel);
            Canvas.SetLeft(panel, leftPosPanel+ deltaPos.X); 
            panel.Width = Width-deltaPos.X;
        }
        //Движение правой границы.
        else if(e.GetPosition(panel).X >= panel.Width-20 && e.GetPosition(panel).X <= panel.Width+20)
        {
            if(Width + deltaPos.X<50) return;
            panel.Width =_startwidth + deltaPos.X;
        }
        //Выход из области захвата мыши границы панели.
        else
        {
            _isPressed = false;
        }
        _stateWidget.WidthUnit = Width;
    }

    /// <summary>
    /// Перемещение границ панели по Y.
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="topPosPanel"></param>
    /// <param name="deltaPos"></param>
    /// <param name="e"></param>
    private void PanelResizeY(PanelUnit panel, double topPosPanel, Point deltaPos, PointerEventArgs e)
    {
        
        if (e.GetPosition(panel).Y >= 0-20 && e.GetPosition(panel).Y <= 0+20)
        {
            if(Height-deltaPos.Y<50) return; 
            topPosPanel= Canvas.GetTop(panel);
            Canvas.SetTop(panel, topPosPanel+ deltaPos.Y);
            panel.Height = Height-deltaPos.Y;
        }
        else if(e.GetPosition(panel).Y >= panel.Height-20 && e.GetPosition(panel).Y <= panel.Height+20)
        {
            if(Height + deltaPos.Y<50) return;
            panel.Height =_startheight + deltaPos.Y;
        }
        else
        {
            _isPressed = false;
        }

        _stateWidget.HeightUnit = Height;
    }

    /// <summary>
    /// Перемещение панели.
    /// </summary>
    /// <param name="panel">панель</param>
    /// <param name="deltaPos">смещение</param>
    /// <param name="leftPosPanel"></param>
    /// <param name="topPosPanel"></param>
    private void MovePanel(PanelUnit panel,Point deltaPos,double leftPosPanel,double topPosPanel)
    {
        Canvas.SetLeft(panel, leftPosPanel+ deltaPos.X);
        Canvas.SetTop(panel, topPosPanel+deltaPos.Y);
        _stateWidget.PositionX = Bounds.X;
        _stateWidget.PositionY = Bounds.Y;
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
        if ((position.X>=Width-15 && position.X<=Width+15) || (position.X>=-15 && position.X<=+15))
        {
            Cursor = new Cursor(StandardCursorType.SizeWestEast);
            _typeTransform = EnumTypeTransform.ResizeX;
        }
        else if ((position.Y >= Height-15 && position.Y <= Height+15) || (position.Y >=-15 && position.Y<=15))
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
    /// Кнопка мыши нажата.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void rectangle_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var yy= e.GetCurrentPoint(this);
        if (yy.Properties.IsLeftButtonPressed)
        {
            PanelUnit panel=sender as PanelUnit ?? throw new InvalidOperationException();
            _isPressed = true;
            _pos=e.GetPosition(panel);
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

    public void SetFixUnit(bool fix)
    {
        _isBlocked = fix;
        var mItem= ContextMenu?.Items.FirstOrDefault(t => ((MenuItem)t).Header!.ToString()!.Equals("Закрепить"));
        if(mItem==null) return;
        (((mItem as MenuItem)?.Icon as CheckBox)!).IsChecked = fix;
    }
}