using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL;

public partial class WindowMassage : Window
{
    string text;
    Window owner;
    public WindowMassage(string text, Window owner)
    {
        InitializeComponent();
        this.text = text;
        this.owner = owner;
        this.Message.Text = text;

        this.Opened += WindowMassage_Opened;
        this.Closed += WindowMassage_Closed;
    }

    private void WindowMassage_Closed(object sender, System.EventArgs e)
    {
        owner.Close();
    }

    private void WindowMassage_Opened(object sender, System.EventArgs e)
    {
         //this.Message.Text = text;
    }

    private void Window_Loaded(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //this.Message.Text = text;
    }

    private void Window_Closing(object sender, Avalonia.Controls.WindowClosingEventArgs e)
    {
        //owner.Close();
    }
}