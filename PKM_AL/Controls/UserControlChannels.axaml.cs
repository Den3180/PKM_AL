using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    private ICollectionView ItemsView;
    private ClassDevice _Device;
    private eFilter Filter=eFilter.All;
    
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
        GridChannels.ItemsSource = Channels;
        //FilterItems();
    }
    
    // /// <summary>
    // /// Фильтр отображаемых регистров.
    // /// </summary>
    // private void FilterItems()
    // {
    //     if (_Device != null)
    //     {
    //         ItemsView.Filter = item =>
    //         {
    //             ClassChannel x = item as ClassChannel;
    //             switch (Filter)
    //             {
    //                 case eFilter.DI:
    //                     return x.Device.ID == _Device.ID && x.TypeRegistry == ClassChannel.EnumTypeRegistry.DiscreteInput;
    //                 case eFilter.AI:
    //                     return x.Device.ID == _Device.ID && x.TypeRegistry == ClassChannel.EnumTypeRegistry.InputRegistry;
    //                 case eFilter.DO:
    //                     return x.Device.ID == _Device.ID && x.TypeRegistry == ClassChannel.EnumTypeRegistry.CoilOutput;
    //                 case eFilter.AO:
    //                     return x.Device.ID == _Device.ID && x.TypeRegistry == ClassChannel.EnumTypeRegistry.HoldingRegistry;
    //                 default:
    //                     return x.Device.ID == _Device.ID;
    //             }
    //         };
    //     }
    //     else ItemsView.Filter = null;
    // }
    
    

    
    /// <summary>
    /// Нумерация строк datagrid.
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
        throw new System.NotImplementedException();
    }

    private void MenuItemAI_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemDO_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemAO_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MenuItemAll_Click(object sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}

internal interface ICollectionView
{
}