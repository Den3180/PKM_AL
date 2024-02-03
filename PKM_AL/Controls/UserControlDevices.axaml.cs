using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    /// ��������� ����� � �������.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_LoadingRow(object sender, Avalonia.Controls.DataGridRowEventArgs e)
    {
        e.Row.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    /// <summary>
    /// ��������� ��������� ������� ������������ ����, � ����������� �� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cMenu_Opened(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    /// <summary>
    /// ��������� ����������.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDetails_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// �������� �� �����.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemShowMap_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// �������� ����������.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemAdd_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.DeviceAdd();
    }

    /// <summary>
    /// �������� ����������.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemEdit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// ������� ����������.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemDelete_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// ��������� ������� ���������� � ����.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemSave_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    /// <summary>
    /// ��������� ������� ���������� �� �����.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemLoad_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }


    /// <summary>
    /// ��������� ��������� ��������.����������� ����.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItemCustom_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }



}