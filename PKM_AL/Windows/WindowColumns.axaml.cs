using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM;
using System.Collections.Generic;

namespace PKM_AL;

public partial class WindowColumns : ClassWindowPKM
{
    private DataGrid _Grid;

    public WindowColumns()
    {
        InitializeComponent();
    }

    public WindowColumns(DataGrid grid)
    {
        InitializeComponent();
        _Grid = grid;
        this.lstColumns.Items.Clear();
        foreach (DataGridColumn col in _Grid.Columns)
        {
            CheckBox cb = new CheckBox();
            cb.Content = col.GetType().Name == "DataGridTemplateColumn" ? "Координаты" : col.Header.ToString();
            if (col.IsVisible == false) cb.IsChecked = false;
            else cb.IsChecked = true;
            cb.Tag = col;
            cb.IsCheckedChanged += comboBox_Checked;
            this.lstColumns.Items.Add(cb);
        }
    }

    private void comboBox_Checked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CheckBox cb = (CheckBox)sender;
        DataGridColumn col = (DataGridColumn)cb.Tag;
        col.IsVisible = cb.IsChecked == true;
    }

    /// <summary>
    /// Кнопка 45.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
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