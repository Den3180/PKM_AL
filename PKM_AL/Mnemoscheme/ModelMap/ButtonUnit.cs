using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using TestGrathic.Enums;
using TestGrathic.ServiceClasses;
using TestGrathic.ViewMap;
using TestGrathic.ViewModelMap;

namespace TestGrathic.ModelMap;

public class ButtonUnit : Button
{
     private bool _isBlocked;
     private double _buttonSize = 35D;
     private bool _isFlag;
     private EnumTypeTransform _typeTransform = EnumTypeTransform.None;
     private Point _pos;
     private readonly Lazy<SettingsUnitObject> _settingsUnitObject = new Lazy<SettingsUnitObject>();
     
     private Image _image=new Image()
     {
         Source = new Bitmap(AssetLoader.Open(
             new Uri($"avares://{Assembly.GetEntryAssembly()?.GetName().Name}/Assets/knopka_vklyucheniya_65rwem2h45ul_64.png")))
     };
     
    public ButtonUnit()
    {
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        Height = 35;
        Width = 35;
        Click += Click_Button;
        ContextMenu=CreateContextMenu(); 
        PointerMoved += ButtonUnit_PointerMoved;
        DataContext = this;
    }

    public ButtonUnit(Rect bounds):this()
    {
        Canvas.SetLeft(this, bounds.X+50);
        Canvas.SetTop(this, bounds.Y+50);
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
        var grid = tt?.Parent as Grid;
        var wnd=grid?.Parent as Window;
        if (wnd == null) return;
        switch (menuItem.Header)
        {
            case "Копировать":
                MainViewModel.BufferCopiedOneUnit = new ButtonUnit(Bounds);
                break;
            case "Удалить":
                (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
                break;
            case "Закрепить":
                _isBlocked = !_isBlocked;
                ((CheckBox)menuItem.Icon).IsChecked = _isBlocked;
                break;
            case "Свойства":
                WindowPropertyButton windowPropertyButton = new WindowPropertyButton(_settingsUnitObject.Value);
                await windowPropertyButton.ShowDialog(wnd);
                if(windowPropertyButton.Tag is not null) 
                    RefreshButton((SettingsUnitObject)windowPropertyButton.Tag);
                //TODO Поворот надписи.
                break;
        }
    }

    private void RefreshButton(SettingsUnitObject tag)
    {
        Width *= tag.ScaleUnit;
        Height *= tag.ScaleUnit;
    }


    private void Click_Button(object? sender, RoutedEventArgs e)
    {
        if (_typeTransform == EnumTypeTransform.Move)
        {
            _typeTransform = EnumTypeTransform.None;
            SetCursor(sender,IsPressed);
            return;
        }
        if (!_isFlag)
        {
            Image.Source=new Bitmap(AssetLoader.Open(
                new Uri($"avares://{Assembly.GetEntryAssembly()?.GetName().Name}/Assets/knopka_vklyucheniya_green.png")));
        }
        else
        {
            Image.Source = new Bitmap(AssetLoader.Open(new Uri(
                $"avares://{Assembly.GetEntryAssembly()?.GetName().Name}/Assets/knopka_vklyucheniya_65rwem2h45ul_64.png")));
        }
        _isFlag = !_isFlag;
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
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
