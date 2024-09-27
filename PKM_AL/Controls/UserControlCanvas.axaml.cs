using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using TestGrathic.ViewModelMap;

namespace PKM_AL.Controls;

public partial class UserControlCanvas : UserControl
{
    public UserControlCanvas()
    {
        InitializeComponent();
        DataContext = Dispatcher.UIThread.Invoke(()=> new CanvasViewModel());
        DetachedFromVisualTree += UserControl_Detached;
    }

    private void UserControl_Detached(object sender, VisualTreeAttachmentEventArgs e)
    {
       CanvasViewModel.GraphicUnitObjects.Clear();
       var tt= CanvasViewModel.Map;
    }
}