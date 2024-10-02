using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using PKM_AL.Mnemoscheme.AbstractUnit;
using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewModelMap;
using TestGrathic.ViewMap;

namespace PKM_AL.Mnemoscheme.ModelMap;

public class TitleUnit : TextBlock, IUnitService
{
   
   private const EnumUnit _enumUnit = EnumUnit.Title;
   private Point  _pos;
   private bool _isPressed;
   private ClassWidget _stateWidget;
   private bool _isBlocked;
   private ClassMap _map;

   public TitleUnit(ClassMap map, ClassWidget stateWidget=null)
   {
      _map = map;
      if (stateWidget != null)
      {
         _stateWidget = stateWidget;
         CreateTitleLoad();
      }
      else
      {
         _stateWidget = new ClassWidget()
         {
            GuidId = _map.GuidID
         };
         _map.Widgets.Add(_stateWidget);
         CreateTitleNew();
      }
      ContextMenu=CreateContextMenu();
      PointerPressed += PointerPressed_TitleUnit;
      PointerMoved += PointerMoved_TitleUnit;
      PointerReleased += PointerReleased_TitleUnit;
      MainWindow.Widgets.Add(_stateWidget);
   }

   public TitleUnit(ClassWidget? stateWidget, Rect bounds, ClassMap map): this(map)
   {
       Canvas.SetLeft(this, bounds.X+50);
       Canvas.SetTop(this, bounds.Y+50);
      _stateWidget = stateWidget;
      RefreshTitle(stateWidget);
   }

   /// <summary>
   /// Создание надписи с настройками по умолчанию.
   /// </summary>
   private void CreateTitleNew()
   {
      FontFamily = new FontFamily("Arial");
      FontSize = 36;
      FontWeight = FontWeight.Bold;
      Foreground = Brushes.Black;
      Text = "Текст";
      //Настройка класса настроек(ClassWiget).
      CreateWidgetObject();
   }

   /// <summary>
   /// Создать надпись при загрузке из файла.
   /// </summary>
   private void CreateTitleLoad()
   {
      FontFamily = new FontFamily(_stateWidget.FontStyleUnit);
      FontSize = _stateWidget.FontSizeUnit;
      FontWeight = _stateWidget.FontWeightUnit; 
      Text = _stateWidget.TextUnit ;
      _stateWidget.UnitType = _enumUnit;
      Foreground = Brush.Parse(_stateWidget.FontBrushUnit);
      Canvas.SetLeft(this, _stateWidget.PositionX);
      Canvas.SetTop(this, _stateWidget.PositionY);
   }

   private void CreateWidgetObject()
   {
      _stateWidget.FontStyleUnit = FontFamily.ToString();
      _stateWidget.FontSizeUnit = FontSize;
      _stateWidget.FontWeightUnit = FontWeight;
      _stateWidget.TextUnit = Text;
      _stateWidget.PositionX = Bounds.X;
      _stateWidget.PositionY =Bounds.Y;
      _stateWidget.UnitType = _enumUnit;
      _stateWidget.FontBrushUnit = Foreground != null ? Foreground.ToString(): Brushes.Black.ToString();
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
      switch (menuItem.Header)
      {
         case "Копировать" :
            CanvasViewModel.BufferCopiedOneUnit = new TitleUnit(_stateWidget, Bounds,_map);
            break;
         case "Удалить":
            (tt?.ItemsSource as ObservableCollection<object>)?.Remove(this);
            _map.Widgets.Remove(_stateWidget);
            MainWindow.Widgets.Remove(_stateWidget);
            MainWindow.MnemoUnit.Remove(this);
            _map.Widgets.Remove(_stateWidget);
            break;
         case "Закрепить":
            _isBlocked = !_isBlocked;
            ((CheckBox)menuItem.Icon).IsChecked = _isBlocked;
            break;
         case "Изменить":
            WindowPropertyTitle propertyMap=new WindowPropertyTitle(_stateWidget);
            await propertyMap.ShowDialog(MainWindow.currentMainWindow);
            if(propertyMap.Tag is not null) RefreshTitle((ClassWidget)propertyMap.Tag);
            //TODO Поворот надписи.
            break;
      }
   }

   /// <summary>
   /// Обновление параметров надписи.
   /// </summary>
   /// <param name="settings"></param>
   private void RefreshTitle(ClassWidget settings)
   {
      FontFamily= new FontFamily(settings.FontStyleUnit);
      FontSize=double.IsNaN(settings.FontSizeUnit) ? FontSize : settings.FontSizeUnit;
      FontWeight = settings.FontWeightUnit;
      Text = settings.TextUnit;
      Foreground = Brush.Parse(settings.FontBrushUnit.ToString());
      CreateWidgetObject();
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
      //Запись текущего положения.
      _stateWidget.PositionX = Bounds.X;
      _stateWidget.PositionY = Bounds.Y;
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

   public EnumUnit GetTypeUnit()
   {
      return _enumUnit;
   }

   public void SetValue(decimal value)
   {
      Text = value.ToString(CultureInfo.InvariantCulture);
      _stateWidget.TextUnit = Text;
   }
}