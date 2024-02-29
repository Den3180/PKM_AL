using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.ComponentModel;
using System.Reactive.Linq;

namespace PKM_AL.Controls;

public partial class UserControlAlarms : UserControl
{
    private readonly List<DataGridRow> _dataGridRows=new List<DataGridRow>();
   
    public UserControlAlarms()
    {
        InitializeComponent();
        GridAlarms.ItemsSource = MainWindow.EventsAlarm;
        
    }
    
    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        ClassEvent ev = gridRow.DataContext as ClassEvent;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
        gridRow.Background = ev?.Ack switch
        {
            0=>Brushes.Red,
            _=>Brushes.LightGray
        };
        _dataGridRows.Add(gridRow);
    }

    private void MenuItemAck_Click(object sender, RoutedEventArgs e)
    {
        ClassEvent obj = this.GridAlarms.SelectedItem as ClassEvent;
        if (obj == null) return;
        obj.Ack = 1;
        var indexRow= GridAlarms.SelectedIndex;
        _dataGridRows[indexRow].Background = Brushes.LightGray;
        //Подтверждение тревоги в БД по конкректной тревоге.
        Task.Run(()=>MainWindow.DB.EventAck(obj.ID));
    }

    private void MenuItemAckAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (ClassEvent obj in MainWindow.Events)
        {
            obj.Ack = 1;
        }
        foreach (var row in _dataGridRows )
        {
            row.Background = Brushes.LightGray;
        }
        //Подтверждение всех тревог в БД.
        Task.Run(()=>MainWindow.DB.EventAckAll());
    }
}
