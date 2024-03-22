using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MsBox.Avalonia.Enums;
using PKM;

namespace PKM_AL.Controls;

public partial class UserControlArchive : UserControl
{
    private List<ClassEvent> Events;
    private readonly Dispatcher dispatcher = Dispatcher.UIThread;
    public UserControlArchive()
    {
        InitializeComponent();
        AreaTextBlock.Text = ClassDB.AreaName;
        SelectNameKIP();
        DTBegin.SelectedDate = DateTime.Now;
        DTEnd.SelectedDate = DateTime.Now;
        AreaTextBlock.Text = MainWindow.DB.InfoArea();
        Search.IsEnabled = false;
        Excel.IsEnabled = false;
        TxtSearch.SelectedIndex = 0;            
        RefreshTable();
    }

    private void RefreshTable()
    {
        //Список для выбранных параметров поиска.
        List<int> types = new List<int>();
        //Название устройства.
        string nameDev = KIP?.SelectedValue?.ToString();
        //ListEvents - это листбокс с чекбоксами выбора параметров поиска.
        for (int i = 0; i < ListEvents.Items.Count; i++)
        {
            if (((CheckBox)ListEvents.Items[i]).IsChecked.Value) types.Add(i + 1);
        }
        //Если не выбрано устройство и не выбраны параметры поиска.
        if (KIP.SelectedIndex == 0 && types.Count==0) return;            
        Events = MainWindow.DB.EventsLoad(DTBegin.SelectedDate.Value,DTEnd.SelectedDate.Value.AddDays(1), types,
            TxtSearch.SelectedItem.ToString(),nameDev);
        if (Events.Count > 0)
        {
            ClassEvent.EventSortByID(Events);
            Excel.IsEnabled = true; 
        }
        else
        {
            Excel.IsEnabled = false;
            dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                MainWindow.currentMainWindow,"Нет данных!",icon:Icon.Error));
        } 
        GridEvents.ItemsSource = Events;
    }

    /// <summary>
    /// Заполнение комбо КИП.
    /// </summary>
    private void SelectNameKIP()
    {
        KIP.Items.Add("Нет");
        foreach (ClassDevice item in MainWindow.Devices)
        {               
            KIP.Items.Add(item.Name);
        }
        KIP.SelectedIndex = 0;
    }

    private void MenuItemExport_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void KIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var combox = sender as ComboBox;
        CheksEnabled(combox.SelectedIndex);
        if (combox.SelectedIndex == 0)
        {
            Search.IsEnabled = false;
            Excel.IsEnabled = false;
            TxtSearch.Items.Clear();
            TxtSearch.Items.Add("Нет");
            TxtSearch.SelectedIndex = 0;
            return;            
        }
        ClassDevice device = MainWindow.Devices.FirstOrDefault<ClassDevice>(x => x.Name == combox.SelectedItem.ToString());
        if (device == null) return;
        TxtSearch.Items?.Clear();
        TxtSearch.Items.Add("Нет");
        TxtSearch.SelectedIndex = 0;
        foreach (var ch in device.Channels)
        {
            if(!string.IsNullOrEmpty(ch.Name)) TxtSearch.Items.Add(ch.Name);
        }            
    }

    private void CheksEnabled(int index)
    {
        foreach(var chBox in ListEvents.Items)
        {
            if(chBox is CheckBox item && item.IsChecked == true)
            {
                item.IsChecked = false;
            }
        }
        // Снять выделение со всех чекбоксов, когда не выбрано устройство. 
        if (index == 0)
        {
            ((CheckBox)ListEvents.Items[2]).IsEnabled = false;
            ((CheckBox)ListEvents.Items[4]).IsEnabled = false;
            ((CheckBox)ListEvents.Items[5]).IsEnabled = false;
            ((CheckBox)ListEvents.Items[6]).IsEnabled = false;
            ((CheckBox)ListEvents.Items[7]).IsEnabled = false;
            ((CheckBox)ListEvents.Items[0]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[1]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[3]).IsEnabled = true;
            TxtSearch.IsEnabled = false;
        }
        else
        {
            ((CheckBox)ListEvents.Items[2]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[4]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[5]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[6]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[7]).IsEnabled = true;
            ((CheckBox)ListEvents.Items[0]).IsEnabled = false;//?
            ((CheckBox)ListEvents.Items[1]).IsEnabled = false;//?
            ((CheckBox)ListEvents.Items[3]).IsEnabled = false;//?
            TxtSearch.IsEnabled = true;
        }
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        foreach (var chBox in ListEvents.Items)
        {
            if (chBox is CheckBox item && item.IsChecked == true)
            {
                Search.IsEnabled = true;
                return;
            }
        }
        Search.IsEnabled = false; 
    }

    private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in this.ListEvents.Items)
        {
            ((CheckBox)item).IsChecked = true;
        }
    }

    private void ButtonClearAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in this.ListEvents.Items)
        {
            ((CheckBox)item).IsChecked = false;
        }
    }

    private void ButtonFind_Click(object sender, RoutedEventArgs e)
    {
        RefreshTable(); 
    }

    private void Button_Excel(object sender, RoutedEventArgs e)
    {
        
    }

    private void ButtonPrint_Click(object sender, RoutedEventArgs e)
    {
        
    }
    
    /// <summary>
    /// Нумерация строк таблицы.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridEvents_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
    }

}