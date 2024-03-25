using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM;

namespace PKM_AL.Controls;

public partial class UserControlAddReport : UserControl
{
    public UserControlAddReport()
    {
        InitializeComponent();
    }
    public UserControlAddReport(EnumTypeReport typeReport)
    {
        InitializeComponent();
    }

    private void Add_NewNote_Btn(object sender, RoutedEventArgs e)
    {
        
    }
}