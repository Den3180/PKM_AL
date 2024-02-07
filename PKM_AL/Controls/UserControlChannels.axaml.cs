using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using PKM;

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
        Channels = MainWindow.Channels;
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
                GridChannels.ItemsSource = Channels.Where(ch => ch.Device.ID == _Device.ID);
             break;
         case eFilter.AO:
             GridChannels.ItemsSource = Channels.Where(ch => (
                 ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.HoldingRegistry));
             break;
         case eFilter.DI:
             GridChannels.ItemsSource = Channels.Where(ch => (
                 ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.DiscreteInput));
             break;
         case eFilter.DO:
             GridChannels.ItemsSource = Channels.Where(ch => (
                 ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.CoilOutput));
             break;
         case eFilter.AI:
             GridChannels.ItemsSource = Channels.Where(ch => (
                 ch.Device.ID == _Device.ID && ch.TypeRegistry==ClassChannel.EnumTypeRegistry.InputRegistry));
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

    private void cMenu_Opened(object sender, RoutedEventArgs e)
    {
        
    }

    private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemDeleteAll_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ConvertToPKM_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemWrite_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

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
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _actualHeightUserControl = this.GetTransformedBounds().Value.Bounds.Height;
        }
        catch
        {
          _actualHeightUserControl=double.NaN;  
        }
    }
}
