using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM;

namespace PKM_AL.Windows;

public partial class WindowColomns : ClassWindowPKM
{
    private DataGrid _Grid;
    public WindowColomns(DataGrid grid)
    {
        InitializeComponent();
        _Grid = grid;
        this.lstColumns.Items.Clear();
        foreach (DataGridColumn col in _Grid.Columns)
        {
            CheckBox cb = new CheckBox();
            cb.Content = col.GetType().Name == "DataGridTemplateColumn"?"Координаты": col.Header.ToString();
            if (col.IsVisible == false) cb.IsChecked = false;
            else cb.IsChecked = true;
            cb.Tag = col;
            cb.Checked += comboBox_Checked;
            cb.Unchecked += comboBox_Unchecked;
            this.lstColumns.Items.Add(cb);
        }
    }

    private void comboBox_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox cb = (CheckBox)sender;
        DataGridColumn col = (DataGridColumn)cb.Tag;
        col.IsVisible = true;

    }

    private void comboBox_Unchecked(object sender, RoutedEventArgs e)
    {
        CheckBox cb = (CheckBox)sender;
        DataGridColumn col = (DataGridColumn)cb.Tag;
        col.IsVisible = false;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ClassSettings settings = ClassSettings.Load();
        List<string> lst = null;
        if (_Grid.Name == "GridDevices") lst = settings.DevicesColumns;
        if (_Grid.Name == "GridChannels") lst = settings.ChannelsColumns;
        lst.Clear();
        foreach (DataGridColumn col in _Grid.Columns)
        {
            if (col.IsVisible == false)
                lst.Add(col.Header.ToString());
        }
        settings.Save();
        this.Tag = true;
        this.Close();
    }
}