using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL;

public partial class WindowMassage : Window
{
    private Window owner;
    public WindowMassage()
    {
        InitializeComponent();
    }

    public WindowMassage(string text, Window owner)
    {
        InitializeComponent();
        this.owner = owner;
        this.Message.Text = text;
    }

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Button button = (Button)sender;
        if (button.Content.ToString() == "��")
        {
            owner.Close();
            Close();
        }
        else
        {
            Close();
        }

    }
}