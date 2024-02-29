using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot;
using ScottPlot.Avalonia;

namespace PKM_AL.Controls;

public partial class UserControlGraphBKM : UserControl
{
    public UserControlGraphBKM()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        double[] dataX = { 1, 2, 3, 4, 5 };
        double[] dataY = { 1, 4, 9, 16, 25 };

        AvaPlot avaPlot1 = this.Find<AvaPlot>("graphBKM_1");
        avaPlot1.Plot.Add.Scatter(dataX, dataY);
        avaPlot1.Refresh();
    }
}