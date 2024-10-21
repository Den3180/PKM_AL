using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using MsBox.Avalonia.Enums;
using PKM_AL.Classes.ServiceClasses;
using PKM;

namespace PKM_AL.Controls;

public partial class UserControlDevices : UserControl
{
    public UserControlDevices()
    {
        InitializeComponent();
        ClassSettings settings = ClassSettings.Load();
        foreach (DataGridColumn col in GridDevices.Columns)
        {
            if (settings.DevicesColumns.FirstOrDefault(x => x == col.Header?.ToString()) != null)
                col.IsVisible = false;
        }
        this.Loaded += Loaded_UserControlDevices;
        GridDevices.ItemsSource = MainWindow.Devices;
    }

    private void Loaded_UserControlDevices(object sender, RoutedEventArgs e)
    {
      
    }

    /// <summary>
    /// Нумерация строк datagrid и привязка к свойству background.
    /// </summary>
    /// <param name="sender">DataGrid</param>
    /// <param name="e">Данные о строке</param>
    private void DataGrid_LoadingRow(object sender, Avalonia.Controls.DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (gridRow.GetIndex() + 1).ToString();
        var tempDev = gridRow.DataContext as ClassDevice;
        if(tempDev==null) return;
        gridRow.DoubleTapped += DataGridRow_DoubleTapped;
        gridRow.Tapped += DataGridRow_Tapped;
    }

    /// <summary>
    /// Еденичный клик по строке DataGrid.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">TappedEventArgs</param>
    /// <returns>void</returns>
    private void DataGridRow_Tapped(object sender, TappedEventArgs e)
    {
        if(sender is not DataGridRow row) return;
        var selectedItem = row.DataContext as ClassDevice;
        if(GridDevices.CurrentColumn is DataGridCheckBoxColumn hh && (hh.Header.ToString()=="Опрос"))
        {
            if (selectedItem != null)
            {
                selectedItem.IsPoll = !selectedItem.IsPoll;
                if (selectedItem.IsPoll == false)
                {
                    //resourseCheckBox.IsDevAll = false;
                }
                // foreach (ClassDevice dev in MainWindow.Devices)
                // {
                //     if (dev.ID == selectedItem.ID)
                //     {
                //         dev.IsPoll=selectedItem.IsPoll;
                //     }
                // }
            }
        }
    }
    
    private void DataGridRow_DoubleTapped(object sender, TappedEventArgs e)
    {
        if(sender is not DataGridRow row) return;
        var selectedItem = row.DataContext as ClassDevice;
        ClassDevice obj = MainWindow.Devices.FirstOrDefault(x => x.ID == selectedItem?.ID);
        if (this.Parent is ContentControl control) control.Content = new UserControlChannels(obj);
        MainWindow.currentMainWindow.StatusMode.Text=@"Устройство '" + obj?.Name + "'";
    }

    /// <summary>
    /// Доступность пунктов меню, в зависимости от роли.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cMenu_Opened(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        bool FlagEnabled = MainWindow.User == null || MainWindow.User.GrantConfig;
        foreach (object item in cMenu.Items)
            if (item is MenuItem) ((MenuItem)item).IsEnabled = FlagEnabled;
    }

    /// <summary>
    /// Вывод окна состояния устройства.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDetails_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice device = this.GridDevices.SelectedItem as ClassDevice;
        if (device == null) return;
        string s = $"Состояние: {device.LinkStateName}";
        //Только для СКЗ-ИП или СКЗ.
        if (device.Model == ClassDevice.EnumModel.SKZ_IP || device.Model == ClassDevice.EnumModel.SKZ)
        {
            s += Environment.NewLine + "Режим работы: " + device.GetModeName();
            s += Environment.NewLine + "Идентификационные данные: " + device.ServerID;
            //if (String.IsNullOrEmpty(device.NominalU)) return;
            s += Environment.NewLine + "Номинальное напряжение: " + device.NominalU + " В";
            s += Environment.NewLine + "Номинальный ток: " + device.NominalI + " A";
            s += Environment.NewLine + "Код предприятия: " + device.FactoryCode;
            s += Environment.NewLine + "Число модулей: " + device.ModulesCount.ToString();
            s += Environment.NewLine + "Год выпуска: " + device.FactoryYear.ToString();
            s += Environment.NewLine + "Заводской номер: " + device.FactoryNumber.ToString();
        }
        ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, s, "Состояние");
    }

    /// <summary>
    /// Переход на карту.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemShowMap_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Добавить устройство.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemAdd_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.DeviceAdd();
    }

    /// <summary>
    /// Редактировать устройство.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemEdit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice obj = this.GridDevices.SelectedItem as ClassDevice;
        if (obj == null) return;
        WindowDevice frmDevice = new WindowDevice(obj);
        frmDevice.WindowShow(MainWindow.currentMainWindow);
    }

    /// <summary>
    ///Удалить устройство.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDelete_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice obj = this.GridDevices.SelectedItem as ClassDevice;
        if (obj == null) return;
        Task<string> res = ClassMessage.ShowMessageCustom(text: $"Удалить устройство {obj.Name}?", owner: MainWindow.currentMainWindow,
                                 buttonEnum: ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Question);
        if (res.Result == "Нет") return;
        //Удаление регистров из БД.
        Task.Run(() => MainWindow.DB.RegistryDelDev(obj.ID));
        for (int i = MainWindow.Channels.Count - 1; i >= 0; i--)
        {
            if (MainWindow.Channels[i].Device.ID == obj.ID)
                MainWindow.Channels.RemoveAt(i);
        }
        MainWindow.DB.DeviceDel(obj);
        MainWindow.Devices.Remove(obj);
        for (int i = MainWindow.Groups[0].SubGroups.Count - 1; i >= 0; i--)
        {
            if (MainWindow.Groups[0].SubGroups[i].ID == obj.ID)
            {
                MainWindow.Groups[0].SubGroups.RemoveAt(i);
                (MainWindow.currentMainWindow.treeView.Items[0] as TreeViewItem)?.Items.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Сохранить профиль устройства в файл.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemSave_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice obj = this.GridDevices.SelectedItem as ClassDevice;
        if (obj == null) return;
        var path= ClassDialogWindows.SaveDialog(MainWindow.currentMainWindow);
        if (string.IsNullOrEmpty(path)) return;
        bool ret = obj.SaveProfile(path);
        ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Сохранение завершено", "Сохранить", 
            ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Success);
    }

    /// <summary>
    /// Загрузка из XML файла.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemLoad_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var path= ClassDialogWindows.ChooseDialog(MainWindow.currentMainWindow);
        if (string.IsNullOrEmpty(path)) return;
       //Загрузка устройства из файла.
        ClassDevice device = ClassDevice.Load(path);           
        if (device == null)
        {
            ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Недопустимый формат файла", "Загрузить", 
                ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Error);
            return;
        }
        bool resD = MainWindow.DB.DeviceAdd(device);
        MainWindow.Devices.Add(device);
        device.CountNumber = MainWindow.Devices[^1].CountNumber + 1;
        //Занесение в дерево устройств.
        ClassItem item = new ClassItem()
        {
            ID = device.ID,
            NameCh = device.Name,
            IconUri = "hardware.png",
            Group = MainWindow.Groups[0],
            ItemType = ClassItem.eType.Device
        };
        MainWindow.Groups[0].SubGroups.Add(item);
        (MainWindow.currentMainWindow.treeView.Items[0] as TreeViewItem)?.Items.Add(ClassBuildControl.MakeContentTreeViewItem(item));
       //Загрузка канала в базу данных(асинхронно) и в список каналов.
        foreach (ClassChannel channel in device.Channels)
        {
            channel.Device = device;
            Task.Run(() => MainWindow.DB.RegistryAdd(channel));
            MainWindow.Channels.Add(channel);
        }           
    }

    /// <summary>
    /// Настройка видимости столбцов.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemCustom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowColumns frm = new WindowColumns(this.GridDevices);
        frm.WindowShow(MainWindow.currentMainWindow);
    }

    private ClassStaticResoursUserControlDevice resourseCheckBox;
    private void GridDevices_OnLoaded(object sender, RoutedEventArgs e)
    {
        var found2 = this.TryFindResource("ResKey", this.ActualThemeVariant, out var result2);
        if (result2!=null) resourseCheckBox = result2 as ClassStaticResoursUserControlDevice;
    }
}