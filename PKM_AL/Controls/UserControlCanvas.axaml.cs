using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TestGrathic.ViewModelMap;

namespace PKM_AL.Controls;

public partial class UserControlCanvas : UserControl
{
    public UserControlCanvas()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}