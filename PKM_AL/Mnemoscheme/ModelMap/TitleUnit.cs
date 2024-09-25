using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Skia;
using Avalonia.Styling;
using PKM_AL;
using TestGrathic.ServiceClasses;
using TestGrathic.ViewMap;
using TestGrathic.ViewModelMap;

namespace TestGrathic.ModelMap;

public class TitleUnit : TextBlock
{
   
   private Point  _pos;
   private bool _isPressed;
   private readonly SettingsUnitObject? _settings;
   private bool _isBlocked;

   public TitleUnit()
   {
      _settings = new SettingsUnitObject();
      CreateTitle();
   }

   public TitleUnit(SettingsUnitObject? settings, Rect bounds): this()
   {
       Canvas.SetLeft(this, bounds.X+50);
       Canvas.SetTop(this, bounds.Y+50);
      _settings = settings;
      RefreshTitle(settings);
   }

   /// <summary>
   /// Создание надписи с настройками по умолчанию.
   /// </summary>
   private void CreateTitle()
   {
      FontFamily = new FontFamily("Arial");
      FontSize = 36;
      FontWeight = FontWeight.Bold;
      Foreground = Brushes.ForestGreen;
      Text = "Текст";
      _settings.FontStyleUnit = FontFamily;
      _settings.FontSizeUnit = FontSize;
      _settings.FontWeightUnit = FontWeight;
      _settings.TextUnit = Text;
      try
      {
         _settings.FontBrushUnit = Color.Parse(Foreground.ToString());
      }
      catch (FormatException e)
      {
         _settings.FontBrushUnit=Colors.Black;
      }
      ContextMenu=CreateContextMenu();
      PointerPressed += PointerPressed_TitleUnit;
      PointerMoved += PointerMoved_TitleUnit;
      PointerReleased += PointerReleased_TitleUnit;
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
      
      menuItem = new MenuItem()
      {
         Header = "Закрепить"
      };
      menuItem.Icon = new CheckBox()
      {
         IsChecked = _isBlocked,
      };
      var item = menuItem;
      ((CheckBox)menuItem.Icon).Click+= (s, e) =>
      {
         MenuItem_Click(item, e);
         contextMenu.Close();
      };
      menuItem.Click += MenuItem_Click;
      contextMenu.Items.Add(menuItem);
      
      var separator = new Separator()
      {
         VerticalAlignment = VerticalAlignment.Center,
         Foreground = Brushes.Black,
      };
      contextMenu.Items.Add(separator);
      
      menuItem = new MenuItem
      {
         Header = "Изменить"

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
      // var grid = tt?.Parent as Grid;
      // var wnd=grid?.Parent as Window;
      // if (wnd == null) return;
      switch (menuItem.Header)
      {
         case "Копировать" :
            CanvasViewModel.BufferCopiedOneUnit = new TitleUnit(_settings, Bounds);
            break;
         case "Удалить":
            (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
            break;
         case "Закрепить":
            _isBlocked = !_isBlocked;
            ((CheckBox)menuItem.Icon).IsChecked = _isBlocked;
            break;
         case "Изменить":
            WindowPropertyTitle propertyMap=new WindowPropertyTitle(_settings);
            await propertyMap.ShowDialog(MainWindow.currentMainWindow);
            if(propertyMap.Tag is not null) RefreshTitle((SettingsUnitObject)propertyMap.Tag);
            //TODO Поворот надписи.
            break;
      }
   }

   private void RefreshTitle(SettingsUnitObject? settings)
   {
      FontFamily=settings.FontStyleUnit == null ? FontFamily : settings.FontStyleUnit;
      FontSize=double.IsNaN(settings.FontSizeUnit) ? FontSize : settings.FontSizeUnit;
      FontWeight = settings.FontWeightUnit;
      Text = settings.TextUnit;
      Foreground = Brush.Parse(settings.FontBrushUnit.ToString());
   }

   /// <summary>
   /// Отпускаем кнопку мыши.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void PointerReleased_TitleUnit(object? sender, PointerReleasedEventArgs e)
   {
      _isPressed = false;
   }

   /// <summary>
   /// Перемещение указателя мыши.
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void PointerMoved_TitleUnit(object? sender, PointerEventArgs e)
   {
      SetCursor(_isPressed);
      if(!_isPressed || _isBlocked) return;
      TitleUnit title=sender as TitleUnit ?? throw new InvalidOperationException();
      var pp = e.GetPosition(this);
      var deltaPos = e.GetPosition(this) - (Vector)_pos;
      var leftPosPanel = Bounds.X;
      var topPosPanel = Bounds.Y; 
      Canvas.SetLeft(title, leftPosPanel+ deltaPos.X);
      Canvas.SetTop(title, topPosPanel+deltaPos.Y);
   }

   /// <summary>
   /// Нажата клавиша мыши. 
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
   private void PointerPressed_TitleUnit(object? sender, PointerPressedEventArgs e)
   {
      var yy= e.GetCurrentPoint(this);
      if (yy.Properties.IsLeftButtonPressed)
      {
         TitleUnit title=sender as TitleUnit ?? throw new InvalidOperationException();
         _isPressed = true;
         _pos=e.GetPosition(title);
         SetCursor(_isPressed);
         ZIndex = 10;
      }
   }
   
   /// <summary>
   /// Настройка значка курсора. 
   /// </summary>
   /// <param name="pressed"></param>
   private void SetCursor(bool pressed=false)
   {
      if (pressed)
      {
         Cursor  = new Cursor(StandardCursorType.SizeAll);
      }
      else
      {
         Cursor = Cursor.Default;
      }
   }

}