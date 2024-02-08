using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using PKM;
using PKM_AL.Windows;

namespace PKM_AL.Controls;

public partial class UserControlChannels : UserControl
{
    private enum eFilter
    {
        All = 0,
        DI = 1,
        AI = 2, 
        DO = 3,
        AO = 4
    }

    private ObservableCollection<ClassChannel> Channels;
    private ClassDevice _Device;
    private eFilter Filter=eFilter.All;
    private double _actualHeightUserControl; //Актуальная высота UserControl.
    
    public UserControlChannels()
    {
        InitializeComponent();
    }
    
    public UserControlChannels(ClassDevice newDevice)
    {
        InitializeComponent();
        _Device = newDevice;
        ClassSettings settings = ClassSettings.Load();
        //GridChannels- объявлен в XAML.
        foreach (DataGridColumn col in GridChannels.Columns)
        {
            if (settings.ChannelsColumns.FirstOrDefault(x => x == col.Header?.ToString()) != null)
                col.IsVisible = false;
        }
        FilterItems();
    }
    
    /// <summary>
    /// Фильтр отображаемых регистров.
    /// </summary>
    private void FilterItems()
    {
        if (_Device == null) return;
        switch (Filter)
        {
         case   eFilter.All:
             Channels=new ObservableCollection<ClassChannel>(MainWindow.Channels.Where(ch => 
                                          ch.Device.ID == _Device.ID));
             GridChannels.ItemsSource = Channels;
             break;
         case eFilter.AO:
             Channels=new ObservableCollection<ClassChannel>(MainWindow.Channels.Where(ch => (
                                       ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.HoldingRegistry)));
             GridChannels.ItemsSource = Channels;
             break;
         case eFilter.DI:
             Channels=new ObservableCollection<ClassChannel>(MainWindow.Channels.Where(ch => (
                                       ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.DiscreteInput)));
             GridChannels.ItemsSource = Channels;
             break;
         case eFilter.DO:
             Channels=new ObservableCollection<ClassChannel>( MainWindow.Channels.Where(ch => (
                                       ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.CoilOutput)));
             GridChannels.ItemsSource = Channels;
             break;
         case eFilter.AI:
             Channels=new ObservableCollection<ClassChannel>( MainWindow.Channels.Where(ch => (
                                       ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.InputRegistry)));
             GridChannels.ItemsSource = Channels;
             break;
         default:
             break;
        }
    }
    
    /// <summary>
    /// Нумерация строк в GridChannels.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_LoadingRow(object sender, Avalonia.Controls.DataGridRowEventArgs e)
    {
        e.Row.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    /// <summary>
    /// Настройка видимости кнопок меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cMenu_Opened(object sender, RoutedEventArgs e)
    {
        
    }

    /// <summary>
    /// Добавить регистр.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
    {
        ClassChannel obj = new ClassChannel();
        obj.Device = _Device;
        WindowChannel frm = new WindowChannel(obj, isEditChannel: true);
        frm.WindowShow(MainWindow.currentMainWindow);
        if(frm.Tag ==null) return;
        Channels.Add(obj);
        MainWindow.Channels.Add(obj);
        obj.Device.Channels.Add(obj);
    }

    /// <summary>
    /// Редактировать регистр.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        ClassChannel obj = this.GridChannels.SelectedItem as ClassChannel;
        if (obj == null) return;
        WindowChannel frm = new WindowChannel(obj,isEditChannel:true,GridChannels.CurrentColumn.Header.ToString());
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemDeleteAll_Click(object sender, RoutedEventArgs e)
    {
        if (_Device.Channels.Count == 0) return;
        foreach (ClassChannel ch in _Device.Channels)
        {
            ClassChannel channel = Channels.FirstOrDefault(c => c.Address == ch.Address && c.Name == ch.Name
                && c.Device.ID == ch.Device.ID);
            Channels.Remove(channel);
            MainWindow.Channels.Remove(channel);
        }
        Task.Run(() => MainWindow.DB.RegistryDelDev(_Device.ID));
        _Device.Channels.Clear();
       GridChannels.Height = _actualHeightUserControl;
    }

    private void ConvertToPKM_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Запись значения в канал.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MenuItemWrite_Click(object sender, RoutedEventArgs e)
    {
        ClassChannel obj = this.GridChannels.SelectedItem as ClassChannel;
        if (obj == null) return;
        WindowValue frm = new WindowValue(obj);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    /// <summary>
    /// Видимость столбцов.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemCustom_Click(object sender, RoutedEventArgs e)
    {
        WindowColumns frm = new WindowColumns(this.GridChannels);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private void MenuItemDI_Click(object sender, RoutedEventArgs e)
    {
        Filter = eFilter.DI;
        FilterItems();
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAI_Click(object sender, RoutedEventArgs e)
    {
        Filter = eFilter.AI;
        FilterItems();
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemDO_Click(object sender, RoutedEventArgs e)
    {
        Filter = eFilter.DO;
        FilterItems();
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAO_Click(object sender, RoutedEventArgs e)
    {
        Filter = eFilter.AO;
        FilterItems();
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAll_Click(object sender, RoutedEventArgs e)
    {
        Filter = eFilter.All;
        FilterItems();
        GridChannels.Height = _actualHeightUserControl;
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _actualHeightUserControl = this.GetTransformedBounds().Value.Bounds.Height;
            GridChannels.Height = _actualHeightUserControl;
        }
        catch
        {
          _actualHeightUserControl=double.NaN;  
        }
    }
}
