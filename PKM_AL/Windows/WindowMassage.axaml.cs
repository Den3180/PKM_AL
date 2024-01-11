using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL;

public partial class WindowMassage : Window
{
    string text;
    public WindowMassage(string text)
    {
        InitializeComponent();
        this.text = text;
    }

    private void Window_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Message.Text = text;
    }
}