using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using PKM;
using ScottPlot;
using ScottPlot.AutoScalers;
using ScottPlot.Avalonia;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using Color = ScottPlot.Color;
using Colors = ScottPlot.Colors;

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
       //SetUserControlGraphBkm();
    }

    /// <summary>
    /// Настройка контролла.
    /// </summary>
    private void SetUserControlGraphBkm()
    {
        //Источник данных для таблицы событий(пока пустое).
        GridEvents.ItemsSource = lstSource;
        //Присваивание имени объекта.
        AreaTextBlock.Text = MainWindow.DB.InfoArea();
        //Выборка всех событий из базы для всех устройств.
        GetAllEvents();
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
        lstSource.AddRange(eventsOnRequest);
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
            //Диапазон значений.
            var rangeDate = ys.Max() - ys.Min();
            //Назначение границы отображения графика.
            plt.Axes.SetLimits(xs[0], xs[^1], ys.Min()-rangeDate*0.1 ,ys.Max()+rangeDate*0.1);
            //Добавление графика на холст.
            var lineParam = plt.Add.Scatter(xs, ys);
            lineParam.Color = ScottPlot.Generate.RandomColor();
            lineParam.LineWidth = 2f;
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
            //Перекрестие.
            Crosshair ch = plt.Add.Crosshair(xs[0], ys[0]);
            ch.LineStyle.Color = Colors.Green;
            ch.LineStyle.Pattern = LinePattern.Dashed;
            
            //ch.HorizontalLine.Color = System.Drawing.Color.Green;    //LabelFont.Color = ;
            //ch.VerticalLine.PositionLabelBackground = System.Drawing.Color.DarkCyan;
            //ch.HorizontalLine.PositionLabelBackground = System.Drawing.Color.DarkCyan;
            //ch.VerticalLine.PositionFormatter = pos => DateTime.FromOADate(pos).ToString("dd.MM.yyyy\nhh:mm:ss");
            //Цвет тиков шкалы времени.
           plt.Axes.DateTimeTicksBottom().Color(Color.FromHex("#a0acb5"));
           wpfPlot.Refresh();
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
            bGrath.IsEnabled = false;
            //Кнопка отображения таблицы событий в true.
            bTable.IsEnabled = true;
            //Установка всех чекбоксов отрисовки тренда в false. 
            foreach(CheckBox chb in trendCheckList)
            {
                chb.IsChecked = false;
            }
            //Рисование графика.
            DrawGraphs();
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
