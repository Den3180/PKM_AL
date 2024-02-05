using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Enums;
using PKM;
using PKM_AL.Classes.ServiceClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PKM_AL;

public partial class UserControlDevices : UserControl
{
    public UserControlDevices()
    {
        InitializeComponent();
        ClassSettings settings = ClassSettings.Load();
        foreach (DataGridColumn col in GridDevises.Columns)
        {
            if (settings.DevicesColumns.FirstOrDefault(x => x == col.Header?.ToString()) != null)
                col.IsVisible = false;
        }
        GridDevises.ItemsSource = MainWindow.Devices;
    }

    private void UserControl_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    /// <summary>
    /// Нумерация строк в таблице.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_LoadingRow(object sender, Avalonia.Controls.DataGridRowEventArgs e)
    {
        e.Row.Tag = (e.Row.GetIndex() + 1).ToString();//fff
    }

    /// <summary>
    /// Настройка видимости пунктов контекстного меню, в зависимости от роли.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cMenu_Opened(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    /// <summary>
    /// Состояние устройства.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDetails_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Показать на карте.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemShowMap_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Добавить устройство.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemAdd_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.DeviceAdd();
    }

    /// <summary>
    /// Изменить устройство.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemEdit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice obj = this.GridDevises.SelectedItem as ClassDevice;
        if (obj == null) return;
        WindowDevice frmDevice = new WindowDevice(obj);
        frmDevice.WindowShow(MainWindow.currentMainWindow);
    }

    /// <summary>
    /// Удалить устройство.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDelete_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ClassDevice obj = this.GridDevises.SelectedItem as ClassDevice;
        if (obj == null) return;
        Task <ButtonResult> res = ClassMessage.ShowMessage(text: $"Удалить устройство {obj.Name}?", owner: MainWindow.currentMainWindow,
                                 buttonEnum:ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Question);
        if (res.Result == ButtonResult.No) return;
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
                (MainWindow.currentMainWindow.treeView.Items[0] as TreeViewItem).Items.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Сохранить профиль устройства в файл.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemSave_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// Загрузить профиль устройства из файла.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemLoad_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }


    /// <summary>
    /// Настроить видимость столбцов.Контекстное меню.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemCustom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }



}