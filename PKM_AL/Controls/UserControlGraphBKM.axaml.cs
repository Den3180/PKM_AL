using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CsvHelper;
using PKM;
using ScottPlot;
using ScottPlot.AutoScalers;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using Spire.Xls;
using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;
using MouseButton = Avalonia.Remote.Protocol.Input.MouseButton;

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
    private List<ClassEvent> lstSourceEvents;
    private List<CheckBox> trendCheckList;
    private List<ComboBox> combosChannelsList;
    private List<ClassEvent> eventsOnRequest;
    private readonly ObservableCollection<ClassDevice> devices;
    private DateTime dtBegin;
    private DateTime dtEnd;
    private double nomValueMax;
    private double nomValueMin;
    private readonly Dispatcher dispatcher = Dispatcher.UIThread;

   private ObservableCollection<ClassEvent> testSource = new();


    public UserControlGraphBKM()
    {
        InitializeComponent();
    }

    public UserControlGraphBKM(object obj)
    {
        InitializeComponent();
        devices = MainWindow.Devices;
        allEventsList = new List<ClassEvent>();
        lstSourceEvents = new List<ClassEvent>();
        GetAllEvents();
        SetUserControlGraphBkm();
        DBegin.SelectedDate = new DateTime(2021,2,1);
        GridEvents.ItemsSource = testSource;
        
    }
    
    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {  
       
    }

    /// <summary>
    /// Настройка контролла.
    /// </summary>
    private void SetUserControlGraphBkm()
    {
        //Источник данных для таблицы событий(пока пустое).
        GridEvents.ItemsSource = lstSourceEvents;
        //Присваивание имени объекта.
        AreaTextBlock.Text = MainWindow.DB.InfoArea();
        //Настройка первичных параметров полей графиков.
        SetPlots();
        if (devices.Count > 0)
        {
            KIP.ItemsSource = devices;
            KIP.SelectedIndex = 0;
        }
        combosChannelsList = new List<ComboBox>() { cb1,cb2,cb3};
        trendCheckList = new List<CheckBox>() {tr_cb1,tr_cb2,tr_cb3};
        ClassDevice device = KIP.SelectedItem as ClassDevice;
        foreach(ComboBox box in combosChannelsList)
        {
            box.ItemsSource = GetCurrentDeviceChannels(device);
            box.SelectedIndex = 0;
        }
    }
    
    /// <summary>
    /// Получить все события имеющихся устройств.
    /// </summary>
    /// <returns></returns>
    private async void GetAllEvents()
    {
        //Если есть хоть одно устройство.
        if (devices.Count > 0)
        {
            foreach (var device in devices)
            {
                string nameDevice = ((ClassDevice)device).Name;
                //Выборка идет в отдельном потоке, чтобы не тормозить приложение, так как событий может быть очень много
                //, а обращения к базе не быстро происходят.
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
        //Обход полей графиков(3 шт.) в контейнере UniformGrid.
        for (int i = 0; i < grathic.Children.Count; i++)
        {
            //Выборка одного поля графика.
            AvaPlot  wpfPlot = grathic.Children[i] as AvaPlot ;
            var plt = wpfPlot?.Plot;
            plt?.Style.DarkMode();
            wpfPlot?.Refresh();
        }
    }
    
    /// <summary>
    /// Нумерация строк Datagrid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GridEvents_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        DataGridRow gridRow = e.Row;
        gridRow.Tag = (e.Row.GetIndex() + 1).ToString();
    }

    private void wpfBigData_MouseDown(object sender, TappedEventArgs e)
    {
        
    }
    
    /// <summary>
    /// Смена устройства в KIP.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void KIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (combosChannelsList is null) return;
        ClassDevice device = KIP.SelectedItem as ClassDevice;
        if (device == null) return;
        List<string> channelsLst = GetCurrentDeviceChannels(device);
        foreach (ComboBox box in combosChannelsList)
        {
            //box.ItemsSource = await Task.Run(()=> GetCurrentDeviceChannels(device));
            box.ItemsSource = channelsLst;
            box.SelectedIndex = 0;
        }
    }

    private void DBegin_SelectedDateChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        if (sender is CalendarDatePicker { SelectedDate: null }) return;
        //Включаем кнопку применить.
        bApply.IsEnabled = true;
    }
    
    
    /// <summary>
    /// Рисование графиков (обертка).
    /// </summary>
    private void DrawGraphs()
    {
        DateTime dt = new DateTime();
        //Начало периода.
        DateTime.TryParse(TBegin.Text, out dt);
        if (DBegin.SelectedDate != null) dtBegin = DBegin.SelectedDate.Value.Add(dt.TimeOfDay);
        //Конец периода.
        DateTime.TryParse(this.TEnd.Text, out dt);
        if (DEnd.SelectedDate != null) dtEnd = DEnd.SelectedDate.Value.Add(dt.TimeOfDay);
        if(lstSourceEvents.Count>0) lstSourceEvents.Clear();
        //Запускаем рисование графиков в каждом секторе.  
        foreach (ComboBox combo in combosChannelsList)
        {
            //При условии, что выбранный элемент комбобокса параметров не первый, то есть не "нет".
            if(combo.SelectedIndex>0) DrawSingleGraph(combo);
        }
    }
    
    /// <summary>
    /// Рисование отдельного графика.
    /// </summary>
    private void DrawSingleGraph(ComboBox combo)
    {
        //Номер текущего сектора графиков.
        int indexCombo = combosChannelsList.IndexOf(combo);
        //Название выбранного параметра.
        string selectParam = combo.SelectedValue as string;
        //Заготовки для координат графика. 
        double [] xs;
        double [] ys;
        //Выборка списка событий по фильтру "устройство/параметр".
        eventsOnRequest=GetListEventsDevices(dtBegin,dtEnd,selectParam,KIP.SelectedItem?.ToString());
        //Добавление событий к списку-источнику данных таблицы событий.
        lstSourceEvents.AddRange(eventsOnRequest);
        GridEvents.ItemsSource = new ObservableCollection<ClassEvent>(lstSourceEvents);
        //Очистка сектора графиков, при условии, что для текущего параметра нет событий. 
        if (eventsOnRequest.Count == 0)
        {
            ClassMessage.ShowMessage(MainWindow.currentMainWindow, $"Нет данных для параметра {selectParam}!");
            AvaPlot wpf = (grathic.Children[indexCombo] as AvaPlot);
            wpf?.Plot.Clear();
            wpf?.Refresh();
            return;
        }
        //Выбор диапазона, установленного СТО для выбранного параметра. 
        SetLimitGraph(selectParam);
        //Максимальное и минимальное значение параметра события.
        double maxValEvents = Math.Round(eventsOnRequest.Max(x => Double.Parse(x.Val)), 2);
        double minValEvents = Math.Round(eventsOnRequest.Min(x => Double.Parse(x.Val)), 2);
        //Получаем даты из списка событий.
        DateTime[] dates = eventsOnRequest.Select(x => x.DT).ToArray();
        //Получаем значения параметра как массив строк.
        string[] valueLikeStringArr = eventsOnRequest.Select(x => x.Val).ToArray();
        //Конвертация значений параметра в массив double.
        ys = Array.ConvertAll<string, double>(valueLikeStringArr, new Converter<string, double>(
                (x) => Convert.ToDouble(ClassControlManager.CheckCurrentSeparator(
                    x, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))));
        //Конвертация значений даты событий в формат OADate, тип double.
        xs = dates.Select(x => x.ToOADate()).ToArray();
        //Отрисовка текущего графика.
        DrawPlotCurrent(indexCombo,xs,ys,selectParam);
        trendCheckList[indexCombo].IsEnabled = true;
    }
    
        /// <summary>
        /// Отрисовка текущего графика.
        /// </summary>
        /// <param name="indexCombo">Номер комбобокса параметра</param>
        /// <param name="xs">Координаты по оси х</param>
        /// <param name="ys">Координаты по оси у</param>
        /// <param name="selectParam">Название параметра</param>
        private void DrawPlotCurrent(int indexCombo,double[] xs, double[]ys, string selectParam)
        {
            //Объект сектора графика.
            AvaPlot wpfPlot = grathic.Children[indexCombo] as AvaPlot;
            //Переопределение кнопок мыши.
            //CustomBtnClick(wpfPlot);
            //Объект свойств графика.
            Plot plt = wpfPlot?.Plot;
            if(plt==null) return;
            //Очистка(на всякий случай).
            plt.Clear();
            //Затирание надписи оси х.
            plt.XLabel("");
            //Назначение надписи оси у.
            plt.YLabel(GetLabelY(selectParam));
            //Надпись всего графика.
            plt.Title(selectParam);
            //Добавление графика на холст.
            var lineParam = plt.Add.Scatter(xs, ys);
            lineParam.Color = ScottPlot.Generate.RandomColor();
            lineParam.LineStyle.Width = 3;
            lineParam.MarkerStyle = new MarkerStyle(MarkerShape.FilledDiamond,12,Colors.Brown);
           //Определение типа графика(ток, потенциал или другое) и назначение предельных значений.
            switch (CheckNameParam(selectParam))
            {
                case ParamGraphEnum.PolarPot:
                case ParamGraphEnum.SumPot: PotPlotLimits(plt, xs[0]);
                        break;
                case ParamGraphEnum.Сorrosion: CorrosionPlot(plt, xs[0]);
                        break;
            }
            //Линия текущих данных.
            DrawLineInfo(xs[0], ys[0], plt);
            //Цвет тиков шкалы времени.
           plt.Axes.DateTimeTicksBottom().Color(Color.FromHex("#a0acb5"));
           plt.Axes.AutoScale();
           plt.Axes.Zoom(fracY:0.9);
           plt.Add.Palette=new ScottPlot.Palettes.ColorblindFriendly();
           wpfPlot.Refresh();
        }
        
        private void DrawLineInfo(double x, double y, Plot plt)
        {
             VerticalLine vlUp = plt.Add.VerticalLine(x);
             VerticalLine vlLow = plt.Add.VerticalLine(x);
             vlUp.LineStyle.Color = Colors.Green;
             vlLow.LineStyle.Color = Colors.Green;
             vlUp.LinePattern = LinePattern.Dashed;
             vlLow.LinePattern = LinePattern.Dashed;
             vlUp.Label.Text= $"{y}";
             vlLow.Label.Text= DateTime.FromOADate(x).ToShortDateString();
             vlUp.Label.FontSize = 12;
             vlLow.Label.FontSize = 12;
             vlUp.LabelOppositeAxis = true;
             vlUp.Label.BorderWidth = 1;
             vlLow.Label.BorderWidth = 1;
             vlUp.Label.BorderColor = Colors.LightSeaGreen;
             vlLow.Label.BorderColor = Colors.LightSeaGreen;
             vlUp.Label.BackColor = Colors.DarkGray;
             vlLow.Label.BackColor = Colors.DarkGray;
        }
        
        /// <summary>
        /// Линия ограничений скорости коррозии.
        /// </summary>
        /// <param name="plt"></param>
        /// <param name="x"></param>
        private void CorrosionPlot(Plot plt, double x)
        {
            var ln1 = plt.Add.HorizontalLine(nomValueMax, color: Colors.Red, width: 5);
            var ln2 = plt.Add.HorizontalLine(nomValueMin, color: Colors.Green, width: 5);
            Text limLabelMin = plt.Add.Text($"Скорость коррозии {ln2.Y} мм/год", x, ln2.Y/*, color: System.Drawing.Color.Black*/);
            limLabelMin.Size = 16;
            limLabelMin.Bold = true;
            limLabelMin.Color = Color.FromHex("#a0acb5");
            Text limLabelMax = plt.Add.Text($"Скорость коррозии {ln1.Y} мм/год", x, ln1.Y/*, color: System.Drawing.Color.Black*/);
            limLabelMax.Size = 16;
            limLabelMax.Bold = true;
            limLabelMax.Color = Color.FromHex("#a0acb5");
            limLabelMax.Label.OffsetY = 21;
        }
        
        /// <summary>
        /// Линии ограничения потенциалов.
        /// </summary>
        /// <param name="plt"></param>
        /// <param name="x"></param>
        private void PotPlotLimits(Plot plt,double x)
        {
            var ln1 = plt.Add.HorizontalLine(nomValueMax, color: Colors.Red, width: 5);
            var ln2 = plt.Add.HorizontalLine(nomValueMin, color:Colors.Green, width: 5);
            var limLabelMin = plt.Add.Text($"Минимальный защитный потенциал {ln2.Y} В",x,ln2.Y);
            limLabelMin.Size = 16;
            limLabelMin.Bold = true;
            limLabelMin.Label.OffsetY = -21;
            limLabelMin.Color = Color.FromHex("#a0acb5");
            var limLabelMax = plt.Add.Text($"Максимальный защитный потенциал {ln1.Y} В", x, ln1.Y);
            limLabelMax.Bold = true;
            limLabelMax.Size = 16;
            limLabelMax.Color = Color.FromHex("#a0acb5");
            limLabelMax.Label.OffsetY = 5;
            //plt.YAxis.SetBoundary(ln1.Y*1.1,ln2.Y*0.9);
        }  
        
        /// <summary>
        /// Проверка на потенциал.
        /// </summary>
        /// <param name="selectParam"></param>
        /// <returns></returns>
        private ParamGraphEnum CheckNameParam(string selectParam)
        {
            if (selectParam.ToLower().Contains("потенциал"))
            {
                return ParamGraphEnum.SumPot;
            }
            else if (selectParam.ToLower().Contains("корроз"))
            {
                return ParamGraphEnum.Сorrosion;
            }
            return ParamGraphEnum.OtherParam;
        }
            
        /// <summary>
        /// Надпись оси Y.
        /// </summary>
        /// <param name="selectParam"></param>
        /// <returns></returns>
        private string GetLabelY(string selectParam)
        {
            List<string> checkParam = new List<string>() {"сопро","потенциал","ток","мощнос","напряж","корроз"};
            foreach (string str in checkParam )
            {
                //Проверка на наличие ключевых лексем в выбранном параметре.
                //И возврат едениц измерения для оси Y.
                bool res = selectParam.ToLower().Contains(str);
                if (res)
                {
                    return str switch
                    {
                        "сопро"=> selectParam +",Ом",
                        "потенциал" => selectParam +",В",
                        "ток" => selectParam +",А",
                        "мощнос" => selectParam +",кВт",
                        "напряж" => selectParam +",В",
                        "корроз" => selectParam +",мм/год",
                        _=>string.Empty
                    };
                }
            }
            return string.Empty;
        }
    
        /// <summary>
        /// Настройка граничных значений.
        /// </summary>
        private void SetLimitGraph(string selectParam)
        {
            if (selectParam.ToLower().Contains("суммарн"))
            {
                nomValueMin = -0.9;
                nomValueMax = -3.5;
            }
            else if (selectParam.ToLower().Contains("поляри"))
            {
                nomValueMin = -0.85;
                nomValueMax = -1.15;
            }
            else if (selectParam.ToLower().Contains("корроз"))
            {
                nomValueMin = 0.1;
                nomValueMax = 0.3;
            }

        }
    
        /// <summary>
        /// Получить список событий для определенного устройства и параметра.
        /// </summary>
        /// <param name="dBegin">Начало периода</param>
        /// <param name="dEnd">Конец периода</param>
        /// <param name="selectParam">Выбранный параметр</param>
        /// <param name="nameDevice">Имя устройства</param>
        /// <returns></returns>
        private List<ClassEvent> GetListEventsDevices(DateTime dBegin, DateTime dEnd, string selectParam, string nameDevice)
        {
            //Пустой список.
            List<ClassEvent> lst = new List<ClassEvent>();  
            //Получаем перечислитель.
            var enmr = allEventsList.GetEnumerator();
            //Проход по списку при помощи перечислителя.
            while (enmr.MoveNext())
            {
                ClassEvent ev = enmr.Current;
                //Все события, не соответствующие условиям пропускаются.
                if (ev.DT < dBegin || ev.DT > dEnd || ev.Param != selectParam || ev.NameDevice != nameDevice) continue;
                //Если это потенциал, то добавляем минус перед значением.
                if (ev.Param.ToLower().Contains("потенци") && !ev.Val.Contains("-"))
                {
                    ev.Val = "-" + ev.Val;
                }
                lst.Add(ev);
            }
            enmr.Dispose();
            return lst;
        }
        
        /// <summary>
        /// Проверка каналов на наличие регистров.
        /// </summary>
        /// <returns></returns>
        private bool CheckChannelBoxes()
        {
            if (cb1.Items.Count > 1 || cb2.Items.Count > 1 || cb3.Items.Count > 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Кнопка применить.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            //Проверка, что в комбобоксах выбран парметр отличный от "нет".
            if (!CheckChannelBoxes())
            {
                ClassMessage.ShowMessage(MainWindow.currentMainWindow, "Не найдены регистры устройств!");
                return;
            }
            //Кнопка экспорта в Excel в true.
            bExcel.IsEnabled = true;
            //Кнопка отрисовки графиков в false.
            bGrath.IsEnabled = gTable.IsVisible;
            //Кнопка отображения таблицы событий в true.
            bTable.IsEnabled = !gTable.IsVisible;
            //Установка всех чекбоксов отрисовки тренда в false. 
            foreach(CheckBox chb in trendCheckList)
            {
                chb.IsChecked = false;
            }
            //Рисование графика.
            DrawGraphs();
        }
    
        /// <summary>
        /// Обработчик кнопки включения графиков.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bGrath_Click(object sender, RoutedEventArgs e)
        {
            //Отключение панели с таблицей.
            gTable.IsVisible = false;
            //Включение кнопки таблиц.
            bTable.IsEnabled = true;
            //Включение кнопки графиков.
            bGrath.IsEnabled = false;
            //Выключение кнопки больших графиков, если он отображен.
            if (wpfBigData.Plot.PlottableList.Count > 0)
            {
                gBigData.IsVisible = false;
            }
            //Включение панели графиков.
            grathic.IsVisible = true;
        }

        /// <summary>
        /// Обработчик кнопки включения таблицы с событиями.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bTable_Click(object sender, RoutedEventArgs e)
        {
            //Включение панели с таблицей.
            gTable.IsVisible = true;
            //Отключение панели с графиками.
            grathic.IsVisible = false;
            //Отключение панели с большим графиком.
            gBigData.IsVisible = false;
            //Отключение кнопки таблиц.
            bTable.IsEnabled = false;
            //Включение кнопки графиков.
            bGrath.IsEnabled = true;
        }

        /// <summary>
        /// Обработчик кнопки экспорта в Excel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bExcel_Click(object sender, RoutedEventArgs e)
        {
             //Task.Run(ExportExcelParam);
             Task.Run(ExportCsvParam);
        }
        private void GraphBKM_OnDoubleTapped(object sender, TappedEventArgs e)
        {
            if (e.Source is AvaPlot plot) 
                plot.Plot.Benchmark.IsVisible = false;
            AvaPlot wpfPlot = sender as AvaPlot;
            if (wpfPlot?.Plot.PlottableList.Count == 0) return;
            if(wpfPlot?.Name== "wpfBigData")
            {
                gBigData.IsVisible = false;
                wpfBigData.Plot.Clear();
                // wpfBigData.Configuration.LeftClickDragPan = false;
                wpfBigData.Refresh();
                return;
            }
            Scatter lpar = wpfPlot?.Plot.PlottableList[0] as Scatter;
            int index = grathic.Children.IndexOf(wpfPlot);
            string selectParam = combosChannelsList[index]?.SelectedItem?.ToString();
            Plot plt;
            plt = wpfBigData.Plot;
            plt.Style.DarkMode();
            double[] xs = lpar?.Data.GetScatterPoints().Select(crd => crd.X).ToArray();
            double[] ys = lpar?.Data.GetScatterPoints().Select(crd => crd.Y).ToArray();
            DrawBigData(xs,ys, selectParam);
            gBigData.IsVisible = true;
        }
        
        /// <summary>
        /// Рисование большого графика.
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="selectParam"></param>
        private void DrawBigData(double[] xs, double[] ys, string selectParam)
        {
            Plot plt = wpfBigData.Plot;
            plt?.Clear();
            plt.XLabel("");
            plt.YLabel(GetLabelY(selectParam));
            plt.Title(selectParam);
        
            Scatter lineParam = plt.Add.Scatter(xs, ys);
            lineParam.LineStyle.Color = Generate.RandomColor().WithLightness();
            lineParam.LineWidth = 2.5f;
            lineParam.MarkerStyle = new MarkerStyle(MarkerShape.FilledDiamond,12,Colors.Brown);
            
            switch (CheckNameParam(selectParam))
            {
                case ParamGraphEnum.PolarPot:
                case ParamGraphEnum.SumPot: PotPlotLimits(plt, xs[0]);
                    break;
                case ParamGraphEnum.Сorrosion: CorrosionPlot(plt, xs[0]);
                    break;
            }
            //Линия текущих данных.
            DrawLineInfo(xs[0], ys[0], plt);
            //Цвет тиков шкалы времени.
            plt.Axes.DateTimeTicksBottom().Color(Color.FromHex("#a0acb5"));
            plt.Axes.AutoScale();
            plt.Axes.Zoom(fracY:0.9);
            plt.Add.Palette=new ScottPlot.Palettes.ColorblindFriendly();
            wpfBigData.Refresh();
        }

        private void GraphBKM_1_OnTapped(object sender, TappedEventArgs e)
        {
        }

        /// <summary>
        /// Переопределение кнопок мыши.
        /// </summary>
        /// <param name="wpfPlot"></param>
        private void CustomBtnClick( AvaPlot wpfPlot)
        {
            ScottPlot.Control.InputBindings customInputBindings = new()
            {
                DragPanButton = ScottPlot.Control.MouseButton.Middle,
                // DragZoomRectangleButton = ScottPlot.Control.MouseButton.Right,
                // DragZoomButton = ScottPlot.Control.MouseButton.Right,
                // ZoomInWheelDirection = ScottPlot.Control.MouseWheelDirection.Up,
                // ZoomOutWheelDirection = ScottPlot.Control.MouseWheelDirection.Down,
                // ClickAutoAxisButton = ScottPlot.Control.MouseButton.Right,
                // ClickContextMenuButton = ScottPlot.Control.MouseButton.Left,
            };

            ScottPlot.Control.Interaction interaction = new(wpfPlot)
            {
                Inputs = customInputBindings,
            };

            wpfPlot.Interaction = interaction;
        }

        /// <summary>
        /// Обработчик событий перемещений курсора.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Graph_OnPointerMoved(object sender, PointerEventArgs e)
        {
            if(sender is not AvaPlot wpfPlot) return;
            var list = wpfPlot.Plot.PlottableList;
            if(list.Count==0) return;
            Scatter  line = (Scatter)list[0];
            var vlList = list.Where(ch => ch.GetType() == typeof(VerticalLine)).ToList();
            if(vlList.Count==0) return;
            PointerPoint pp=e.GetCurrentPoint(wpfPlot);
            Pixel mousePixel = new(pp.Position.X, pp.Position.Y);
            Coordinates mouseLocation = wpfPlot.Plot.GetCoordinates(mousePixel);
            DataPoint nearest = line.Data.GetNearest(mouseLocation, wpfPlot.Plot.LastRender);
            if (!nearest.IsReal) return;
            DrawVerticalInfoLine(vlList,nearest.Coordinates);
            wpfPlot.Refresh();
        }

        /// <summary>
        /// Рисование информационных линий текущих значений.
        /// </summary>
        /// <param name="vlList"></param>
        /// <param name="coordinates"></param>
        private void DrawVerticalInfoLine(List<IPlottable> vlList,Coordinates coordinates)
        {
            ((VerticalLine)vlList[0]).Label.Text = coordinates.Y.ToString();
            ((VerticalLine)vlList[0]).X=coordinates.X;
            ((VerticalLine)vlList[1]).X=coordinates.X;
            ((VerticalLine)vlList[1]).Label.Text = DateTime.FromOADate(coordinates.X).ToShortDateString();
            
        }

        /// <summary>
        /// Отрисовка трендов.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            int currentIndexItem = trendCheckList.IndexOf(checkBox);
            AvaPlot wpfPlot = gBigData.IsVisible==false ? grathic.Children[currentIndexItem] as AvaPlot : wpfBigData;
            if(wpfPlot==null) return;
            Plot plt = wpfPlot.Plot;
            var plots = plt.PlottableList;
            //Выход если нет элементов в плоте.
            if (plots.Count == 0) return;
            Scatter lineParam = plots.FirstOrDefault(line=>line.GetType() == typeof(Scatter)) as Scatter;
            if (checkBox?.IsChecked == false)
            {
                if (lineParam != null) lineParam.LineStyle.Color=Generate.RandomColor().WithLightness();
                var regLine = plots.FirstOrDefault(rg => rg.GetType() == typeof(LinePlot));
                if(regLine!=null) plt.Remove(regLine);
                wpfPlot.Refresh();
                return;
            }
            var dataList= lineParam?.Data.GetScatterPoints();
            var xs = dataList?.Select(item => item.X).ToArray();
            var ys = dataList?.Select(item => item.Y).ToArray();
            if(xs==null || ys==null) return;
            ScottPlot.Statistics.LinearRegression reg = new(xs, ys);
            Coordinates pt1 = new(xs.First(), reg.GetValue(xs.First()));
            Coordinates pt2 = new(xs.Last(), reg.GetValue(xs.Last()));
            var line = plt.Add.Line(pt1, pt2);
            line.MarkerSize = 0;
            line.LineWidth = 2;
            line.LinePattern = LinePattern.Dashed;
            line.LineStyle.Color=Colors.LightGreen;
            lineParam.LineStyle.Color=Colors.Transparent;
            wpfPlot.Refresh();
        }

        private void ExportCsvParam()
        {
            List<ClassTransportEvent> transportEvents = new List<ClassTransportEvent>();
            List<string> lstHeader = new List<string>();
            foreach (var header in GridEvents.Columns)
            {
                dispatcher.Invoke(() => lstHeader.Add(header.Header.ToString()));
            }
            var count = 0;
            foreach (var ev in lstSourceEvents)
            {
                count++;
                transportEvents.Add(new ClassTransportEvent()
                {
                   Id = count.ToString(),
                   Date = ev.StrDT,
                   Device = ev.NameDevice,
                   Param = ev.Param,
                   Value = ev.Val,
                   TypeEvent = ev.Name
                });
            }

            CultureInfo cultureInfo = Environment.OSVersion.Platform == PlatformID.Win32NT ? 
                                       CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            using (FileStream sourceStream =
                   File.Open($"Параметры" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", FileMode.OpenOrCreate))
            {
            using var writer = new StreamWriter(sourceStream,Encoding.UTF8);
            using (var csv = new CsvWriter(writer, cultureInfo))
            {
                csv.Context.RegisterClassMap<ClassEventMap>();
                csv.WriteRecords(transportEvents);
            }
            }
        }
        
        /// <summary>
        /// Экспорт в Excel.
        /// </summary>
        private void ExportExcelParam()
        {
            List<string> lstHeader = new List<string>();
            foreach (var header in GridEvents.Columns)
            {
                dispatcher.Invoke(() => lstHeader.Add(header.Header.ToString()));
            }
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            CellRange ranges;
            int totalRow = lstSourceEvents.Count + 1;
            int lstColCount = GridEvents.Columns.Count;
            System.Drawing.Point startPoint = new System.Drawing.Point(1, 1);
            System.Drawing.Point endPoint = new System.Drawing.Point(totalRow, lstColCount);
            for (int j = 0; j < totalRow; j++)
            {
                List<CellRange> list = sheet.Range[startPoint.X + j, startPoint.Y, startPoint.X + j, endPoint.Y].CellList;
                if (j == 0)
                {
                    for (int i = 0; i < lstColCount; i++)
                    {
                        list[i].Text = lstHeader[i].ToString();
                        list[i].IsWrapText = true;
                        list[i].Style.VerticalAlignment = VerticalAlignType.Center;
                        list[i].Style.HorizontalAlignment = HorizontalAlignType.Center;
                        list[i].Style.Font.IsBold = true;
                    }
                }
                else
                {
                    ClassEvent even = lstSourceEvents[j - 1];
                    list[0].Text = even.ID.ToString();
                    list[1].Text = even.StrDT;
                    list[2].Text = even.NameDevice;
                    list[3].Text = even.Name;
                    list[4].Text = even.Param;
                    list[5].Text = even.Val;
                    for (int i = 0; i < lstColCount; i++)
                    {
                        if (i == 0)
                        {
                            list[i].ColumnWidth = 5;
                        }
                        else
                        {
                            list[i].ColumnWidth = 22;
                        }
                    }
                }
            }
            ranges = sheet.Range[startPoint.X, startPoint.Y, endPoint.X, endPoint.Y];
            ranges.BorderInside(LineStyleType.Thin, System.Drawing.Color.Black);
            ranges.BorderAround(LineStyleType.Medium, System.Drawing.Color.Black);
            sheet.AllocatedRange.AutoFitRows();
            try
            {
                book.SaveToFile($"Параметры" + DateTime.Now.ToString("yyyy-MM-dd") + ".xls");
            }
            catch
            {
                //MessageBox.Show("Ошибка! Импорт в Excel не завершен!");
                return;
            }
            //MessageBox.Show("Экспорт в Excel завершен!", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
        }

}
