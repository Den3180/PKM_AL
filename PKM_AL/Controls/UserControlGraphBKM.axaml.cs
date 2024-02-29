using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PKM;
using ScottPlot;
using ScottPlot.Avalonia;

namespace PKM_AL.Controls;

public partial class UserControlGraphBKM : UserControl
{
    public enum ParamGraphEnum
    {
        SumPot=1,
        PolarPot=2,
        Сorrosion=3,
        OtherParam
    }
    
    private readonly object locker=new object();
    private List<ClassEvent> allEventsList;
    private List<ClassEvent> lstSource;
    private List<CheckBox> trendCheckList;
    private List<ComboBox> combosChannelsList;
    private List<ClassEvent> eventsOnRequest;
    private readonly ObservableCollection<ClassDevice> devices;
    private DateTime dtBegin;
    private DateTime dtEnd;
    private double nomValueMax;
    private double nomValueMin;
    private readonly Dispatcher dispatcher = Dispatcher.UIThread;


    public UserControlGraphBKM()
    {
        InitializeComponent();
    }

    public UserControlGraphBKM(object obj)
    {
        InitializeComponent();
        devices = MainWindow.Devices;
        allEventsList = new List<ClassEvent>();
        lstSource = new List<ClassEvent>();
        SetUserControlGraphBkm();
        // wpfBigData.Configuration.DoubleClickBenchmark = false;
        // wpfBigData.Configuration.LeftClickDragPan = false;
    }
    
    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
       // SetUserControlGraphBkm();
        GridEvents.ItemsSource = lstSource;
    }

    /// <summary>
    /// Настройка контролла.
    /// </summary>
    private void SetUserControlGraphBkm()
    {
        AreaTextBlock.Text = MainWindow.DB.InfoArea();
        GetAllEvents();
        SetPlots();
        if (devices.Count > 0)
        {
            KIP.ItemsSource = devices;
        }
        combosChannelsList = new List<ComboBox>() { cb1,cb2,cb3};
        trendCheckList = new List<CheckBox>() {tr_cb1,tr_cb2,tr_cb3};
        ClassDevice device = KIP.SelectedItem as ClassDevice;
        foreach(ComboBox box in combosChannelsList)
        {
            box.ItemsSource = GetCurrentDeviceChannels(device);
        }
    }
    
    /// <summary>
    /// Получить все события имеющихся устройств.
    /// </summary>
    /// <returns></returns>
    private async void GetAllEvents()
    {
        if (devices.Count > 0)
        {
            foreach (var device in devices)
            {
                string nameDevice = ((ClassDevice)device).Name;

                await Task.Run(() =>
                    {
                        lock (locker)
                        {
                            allEventsList.AddRange(ClassDB.AllEventsDeviceLoad(nameDevice, (int)ClassEvent.EnumType.Measure));
                        }
                    }
                );
            }
        }
    }

    /// <summary>
    /// Получить имена регистров для текущего устройства.
    /// </summary>
    /// <returns></returns>
    private List<string> GetCurrentDeviceChannels(ClassDevice device)
    {
        List<string> lstChans = new List<string>();
        lstChans = device?.Channels.Select(x => x.Name).ToList();
        lstChans?.Sort();
        lstChans?.Insert(0,"нет");
        return lstChans;
    }

    /// <summary>
    /// Настроить общие параметры поля графиков.
    /// </summary>
    private void SetPlots()
    {
        for (int i = 0; i < grathic.Children.Count; i++)
        {
            AvaPlot  wpfPlot = grathic.Children[i] as AvaPlot ;
            var plt = wpfPlot?.Plot;
            plt?.Style.Background(figure: Color.FromHex("#07263b"), data: Color.FromHex("#0b3049"));
            plt?.Style.ColorAxes(Color.FromHex("#a0acb5"));
            plt?.Style.ColorGrids(Color.FromHex("#0e3d54"));
            // wpfPlot.Configuration.DoubleClickBenchmark = false;
            // wpfBigData.Configuration.LeftClickDragPan = false;
            wpfPlot?.Refresh();
        }
    }

    private void GridEvents_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    private void wpfBigData_MouseDown(object sender, TappedEventArgs e)
    {
        
    }

    private void KIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }

    private void DBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        
    }

    private void ButtonFind_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void bGrath_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void bTable_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void bExcel_Click(object sender, RoutedEventArgs e)
    {
        
    }
}