using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PKM_AL.Mnemoscheme;
using PKM_AL.Mnemoscheme.ViewModelMap;

namespace PKM_AL.Controls;

public partial class UserControlCanvas : UserControl
{
    public UserControlCanvas()
    {
        InitializeComponent();
        DataContext = new CanvasViewModel();
    }

    public UserControlCanvas(ClassMap map)
    {
        InitializeComponent();
        DataContext = new CanvasViewModel(map);
    }
}