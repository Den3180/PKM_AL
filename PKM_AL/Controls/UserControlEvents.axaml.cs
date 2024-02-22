using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace PKM_AL.Controls;

public partial class UserControlEvents : UserControl
{
    public UserControlEvents()
    {
        InitializeComponent();
        GridEvents.ItemsSource = MainWindow.Events;
    }
    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
        ClassEvent ev = gridRow.DataContext as ClassEvent;
        gridRow.Background = ev?.Type switch
        {
            ClassEvent.EnumType.Connect=>Brushes.GreenYellow,
            ClassEvent.EnumType.Disconnect=>Brushes.Red,
            ClassEvent.EnumType.Less=>Brushes.Red,
            ClassEvent.EnumType.Over=>Brushes.Red,
            ClassEvent.EnumType.Start=>Brushes.DeepSkyBlue,
            ClassEvent.EnumType.Finish=>Brushes.DeepSkyBlue,
            _=>null
        };
    }
}