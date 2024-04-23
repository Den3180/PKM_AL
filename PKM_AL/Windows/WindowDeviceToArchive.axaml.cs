using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL.Windows;

public partial class WindowDeviceToArchive : ClassWindowPKM
{
    public WindowDeviceToArchive()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.Tag = devCombo.SelectedItem as ClassDevice;
        Close();
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        devCombo.ItemsSource = MainWindow.Devices;
        devCombo.SelectedIndex=0;
    }
}