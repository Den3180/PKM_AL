using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.AbstractUnit;
public abstract class AbstractControl : ContentControl, INotifyPropertyChanged,IUnitService
{
    protected bool _isBlocked;
    private bool _isPressed;
    private Point _pos=new Point();
    private EnumTypeTransform _typeTransform = EnumTypeTransform.None;
    protected ClassWidget _stateWidget;

    protected AbstractControl()
    {
        PointerReleased += ContentControl_PointerReleased;
        PointerPressed += ContentControl_PointerPressed;
        PointerMoved += ContentControl_PointerMoved;
    }
    protected virtual ContextMenu? CreateContextMenu()
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
    
    protected virtual void ContentControl_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isPressed = false;
        _typeTransform = EnumTypeTransform.None;
        ZIndex=0;
    }
    
    protected virtual void ContentControl_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var yy= e.GetCurrentPoint(this);
        if (yy.Properties.IsLeftButtonPressed)
        {
            _isPressed = true;
            _pos=e.GetPosition(this);
            ZIndex = 10;
            if(Cursor != null && Cursor.Equals(Cursor.Default))
                Cursor = new Cursor(StandardCursorType.SizeAll);
        }
    }
    
    protected virtual void ContentControl_PointerMoved(object? sender, PointerEventArgs e)
    {
        ContentControl_SetCursor(sender,e,_isPressed);
        if(!_isPressed || _isBlocked) return;
        var curPointPos = e.GetPosition(this);
        var deltaPos = curPointPos -_pos;
        var leftPosPanel= Bounds.X;
        var topPosPanel = Bounds.Y;
        if ( _typeTransform == EnumTypeTransform.Move)
        {
            ContentControl_Move(deltaPos,leftPosPanel,topPosPanel);
        }
    }
    
    protected virtual void ContentControl_Move(Point deltaPos, double leftPosPanel, double topPosPanel)
    {
        Canvas.SetLeft(this, leftPosPanel+ deltaPos.X);
        Canvas.SetTop(this, topPosPanel+deltaPos.Y);
        _stateWidget.PositionX = Canvas.GetLeft(this);
        _stateWidget.PositionY = Canvas.GetTop(this);
    }
    
    protected virtual void ContentControl_SetCursor(object? sender, PointerEventArgs e, bool pressed=false)
    {
        if(pressed) return;
        Cursor = Cursor.Default;
        _typeTransform = EnumTypeTransform.Move;
    }

    protected abstract void MenuItem_Click(object? sender, RoutedEventArgs e);
    
    public new event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    
    public virtual EnumUnit GetTypeUnit()
    {
        throw new System.NotImplementedException();
    }

    public BindingObject GetBindingObject()
    {
        return _stateWidget.BindingObjectUnit;
    }

    public virtual void SetValue(decimal value, object spare)
    {
        throw new System.NotImplementedException();
    }

    public void SetFixUnit(bool fix)
    {
        _isBlocked = fix;
        var mItem= ContextMenu?.Items.FirstOrDefault(t => ((MenuItem)t).Header!.ToString()!.Equals("Закрепить"));
        if(mItem==null) return;
        (((mItem as MenuItem)?.Icon as CheckBox)!).IsChecked = fix;
    }
}