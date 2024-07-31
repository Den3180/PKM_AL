using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using MsBox.Avalonia.Enums;
using PKM;
using PKM_AL.Classes.ServiceClasses;
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
        // GridChannels.ItemsSource=MainWindow.Channels;
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
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
    }
    
    /// <summary>
    /// Фильтр отображаемых регистров.
    /// </summary>
    private List<ClassChannel> FilterItems()
    {
        if (_Device == null) 
            return new List<ClassChannel>();
        switch (Filter)
        {
         case   eFilter.All:
             return MainWindow.Channels.Where(ch => ch.Device?.ID == _Device.ID).ToList();
             break;
         case eFilter.AO:
             return MainWindow.Channels.Where(ch => ch.Device?.ID == _Device.ID && 
                                                    ch.TypeRegistry==ClassChannel.EnumTypeRegistry.HoldingRegistry).ToList();
             break;
         case eFilter.DI:
             return MainWindow.Channels.Where(ch => ch.Device?.ID == _Device.ID && 
                                                        ch.TypeRegistry==ClassChannel.EnumTypeRegistry.DiscreteInput).ToList();
             break;
         case eFilter.DO:
             return MainWindow.Channels.Where(ch => ch.Device?.ID == _Device.ID && 
                                                    ch.TypeRegistry==ClassChannel.EnumTypeRegistry.CoilOutput).ToList();
             break;
         case eFilter.AI:
             return MainWindow.Channels.Where(ch => ch.Device?.ID == _Device.ID && 
                                                    ch.TypeRegistry==ClassChannel.EnumTypeRegistry.InputRegistry).ToList();
             break;
         default:
             return new List<ClassChannel>();
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
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
        // var binding = new Binding()
        // {
        //     Source = (gridRow.DataContext as ClassChannel),
        //     Path = nameof(ClassChannel.ColorLineChannel)
        // };
        // gridRow.Bind(DataGridRow.BackgroundProperty, binding);
    }

    /// <summary>
    /// Настройка видимости кнопок меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cMenu_Opened(object sender, RoutedEventArgs e)
    {
        if (_Device == null)
        {
            ((ContextMenu)sender).Close();
            e.Handled=true;
            return;
        }
        bool FlagEnabled = MainWindow.User == null || MainWindow.User.GrantConfig;
        foreach (object item in cMenu.Items)
        {
            if (item is MenuItem)
            {
                ((MenuItem)item).IsEnabled = FlagEnabled;
                if (((MenuItem)item).Name == "Write")
                {
                    ClassChannel obj = this.GridChannels.SelectedItem as ClassChannel;
                    if (obj == null) continue;
                    if (obj.TypeRegistry == ClassChannel.EnumTypeRegistry.HoldingRegistry
                        || obj.TypeRegistry == ClassChannel.EnumTypeRegistry.CoilOutput)
                        ((MenuItem)item).IsEnabled = FlagEnabled;
                    else ((MenuItem)item).IsEnabled = false;
                }
            }
        }
        //Отключение возможности конвертации карты из TikModScan при наличии сущ. карты у устройства.
        this.convertFromTMS.IsEnabled = _Device?.Channels.Count == 0;
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

    /// <summary>
    /// Удалить один или группу регистров.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {
        DeleteOneOrMoreRegistry();
    }
    
    /// <summary>
/// Удалить все регистры.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
    private void MenuItemDeleteAll_Click(object sender, RoutedEventArgs e)
    {
        if (_Device.Channels.Count == 0) return; 
        
        for (int i=0; i< MainWindow.Channels.Count;i++)
        {
            if (MainWindow.Channels[i].Device.ID == _Device.ID)
            {
                MainWindow.Channels.Remove(MainWindow.Channels[i]);
                i--;
            }
        }
        Task.Run(() => MainWindow.DB.RegistryDelDev(_Device.ID));
        Channels.Clear();
        _Device.Channels.Clear();
        GridChannels.Height = _actualHeightUserControl;
    }

    /// <summary>
    /// Открыть диалог конвертации из карт TikModscan.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ConvertToPKM_Click(object sender, RoutedEventArgs e)
    {
        var pathFile = ClassDialogWindows.ChooseDbDialog(MainWindow.currentMainWindow);
        if (string.IsNullOrEmpty(pathFile)) return;
        ConvertToPKM(pathFile);
    }

    /// <summary>
    /// Конвертирование карты регистров из TikModscan.
    /// </summary>
    /// <param name="fileName"></param>
    private async void ConvertToPKM(string fileName)
    {
        SerializableCellsContainer serializableCells = ConverterToPKM.LoadMap(fileName);
        List<CellData> cellsArray = serializableCells.CellsArray;
        for (int i = 0; i < cellsArray.Count; i++)
        {
            if (string.IsNullOrEmpty(cellsArray[i].Name))
            {
                cellsArray[i].Name="Резерв";
            }
            ClassChannel channel = new ClassChannel();
            channel.Device = _Device;
            channel.Name = cellsArray[i].Name;
            channel.Device.ID = _Device.ID;
            channel.TypeRegistry= cellsArray[i].Type switch
            {
                "InputRegister" => ClassChannel.EnumTypeRegistry.InputRegistry,
                "HoldingRegister" => ClassChannel.EnumTypeRegistry.HoldingRegistry,
                _ => ClassChannel.EnumTypeRegistry.CoilOutput
            };
            channel.Address = Int32.Parse(cellsArray[i].Adress);
            channel.Format = ClassChannel.EnumFormat.UINT;
            channel.Koef = 1;
            channel.Min = null;
            channel.Max = null;
            channel.Accuracy = null;
            channel.Ext = null;
            channel.Archive = false;                
            if (channel.ID == 0)
            {
                await Task.Run(()=> MainWindow.DB.RegistryAdd(channel));
                MainWindow.Channels.Add(channel);
                channel.Device.Channels.Add(channel);
                Channels.Add(channel);
            }
        }   

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

    #region [Видимость регистров]
    private void MenuItemDI_Click(object sender, RoutedEventArgs e)
    {
        if(Filter==eFilter.DI) return;
        Filter = eFilter.DI;
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAI_Click(object sender, RoutedEventArgs e)
    {
        if(Filter==eFilter.AI) return;
        Filter = eFilter.AI;
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemDO_Click(object sender, RoutedEventArgs e)
    {
        if(Filter==eFilter.DO) return;
        Filter = eFilter.DO;
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAO_Click(object sender, RoutedEventArgs e)
    {
        if(Filter==eFilter.AO) return;
        Filter = eFilter.AO;
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
        GridChannels.Height = _actualHeightUserControl;
    }

    private void MenuItemAll_Click(object sender, RoutedEventArgs e)
    {
        if(Filter==eFilter.All) return;
        Filter = eFilter.All;
        Channels=new ObservableCollection<ClassChannel>(FilterItems());
        GridChannels.ItemsSource = Channels;
        GridChannels.Height = _actualHeightUserControl;
    }

    #endregion
    
    /// <summary>
    /// Окончательная загрузка контрола.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Удаление одного или более регистров.
    /// </summary>
    private void DeleteOneOrMoreRegistry()
    {
        if (GridChannels.SelectedItems.Count == 0) return;
        ClassChannel ob = this.GridChannels.SelectedItems[0] as ClassChannel;
        if(ob==null) return;
        string mes = GridChannels.SelectedItems.Count > 1 ? "Удалить каналы?" : $"Удалить канал '{ob.Name}'?";
        var res= ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow,mes,buttonEnum:ButtonEnum.OkCancel,
            icon:MsBox.Avalonia.Enums.Icon.Question);
        if (res.Result == "Отмена") return;
        //Создание и заполнение временного списка попавших в выделение элементов.
        List <ClassChannel> listDelChannels = new List<ClassChannel>();
        foreach (var elem in GridChannels.SelectedItems)
        {
          listDelChannels.Add(elem as ClassChannel);   
        }
        foreach (var obj in listDelChannels)
        {
            Task.Run(() => MainWindow.DB.RegistryDel(obj.ID));
            Channels.Remove(obj);
            MainWindow.Channels.Remove(obj);
            _Device.Channels.Remove(obj);
        }
    }

    /// <summary>
    /// Обработка нажатия клавиш над DataGrid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridChannels_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete) return;
            DeleteOneOrMoreRegistry();
    }

    /// <summary>
    /// Редактирование ячеек регистров. Происходит при смене фокуса.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridChannels_OnCellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
    {
        var channel = GridChannels.SelectedItem as ClassChannel;
        if (channel==null) return;
        Task.Run(() => MainWindow.DB.RegistryEdit(channel));
    }
}
