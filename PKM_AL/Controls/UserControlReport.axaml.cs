using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CsvHelper;
using MsBox.Avalonia.Enums;
using PKM;
using PKM_AL.Classes.ServiceClasses;

namespace PKM_AL.Controls;
public partial class UserControlReport : UserControl
{
    ClassReportSettings reportSettings;
    private readonly List<(string,string)> lstHeaders;
    private readonly EnumTypeReport typeReport;
    
    public UserControlReport()
    {
        InitializeComponent();
    }
    public UserControlReport(EnumTypeReport typeReport)
    {
        InitializeComponent();
        this.typeReport = typeReport;
        reportSettings = new ClassReportSettings();
        //Выбор списка заголовков выбранного отчета.
        lstHeaders = reportSettings.GetReportHaders(typeReport);
        GridReports.ItemsSource = MainWindow.Reports;
        MainTitle.Text = lstHeaders[0].Item1;
        BuildDataGrid(lstHeaders);
        //Отключение кнопки, если нет отчетов.
        if (MainWindow.Reports.Count == 0)
        {
            btExcel.IsEnabled = false;
        }
    }
    
    private void BuildDataGrid(List<(string,string)> lstHeaders)
    {
        for (int i = 1; i < lstHeaders.Count; i++)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            // DataTemplate dataTemplate = new DataTemplate();
            // dataTemplate.DataType = typeof(DataGridColumnHeader);
            // FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(TextBlock));
            // spFactory.Name = "myComboFactory";
            // spFactory.SetValue(TextBlock.TextProperty, lstHeaders[i].Item1);
            // spFactory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            // spFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            // dataTemplate.VisualTree = spFactory;
            // column.HeaderTemplate = dataTemplate;
            // column.ElementStyle = new System.Windows.Style();
            // //i-1 потому, что заголовков на 1 больше, чем колонок таблицы. DateRec[i]-открытое св-во класса, привязанного списком
            // //к ItemSource DataGrid.
            column.Binding = new Binding($"DateRec[{i - 1}]");
            column.Header = lstHeaders[i].Item1;
            // column.ElementStyle.TargetType = typeof(TextBlock);
            // column.ElementStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
            // column.ElementStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            GridReports.Columns.Add(column);
        }
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void ExportExcelReports(DataGrid GridEvents,List<(string,string)> lstHeaders)
    {
               // Dispatcher dispatcher=Dispatcher.UIThread;
               // List<ClassTransportEvent> transportEvents = new List<ClassTransportEvent>();
               // List<string> lstHeader = new List<string>();
               // foreach (var header in GridEvents.Columns)
               // {
               //     dispatcher.Invoke(() => lstHeader.Add(header.Header.ToString()));
               // }
               // var count = 0;
               // foreach (var ev in lstSourceEvents)
               // {
               //     count++;
               //     transportEvents.Add(new ClassTransportEvent()
               //     {
               //        Id = count.ToString(),
               //        Date = ev.StrDT,
               //        Device = ev.NameDevice,
               //        Param = ev.Param,
               //        Value = ev.Val,
               //        TypeEvent = ev.Name
               //     });
               // }
               // CultureInfo cultureInfo = Environment.OSVersion.Platform == PlatformID.Win32NT ? 
               //                            CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
               // try
               // {
               //
               //     using (FileStream sourceStream =
               //            File.Open($"Параметры" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", FileMode.OpenOrCreate | FileMode.Truncate))
               //     {
               //         using var writer = new StreamWriter(sourceStream, Encoding.UTF8);
               //         using (var csv = new CsvWriter(writer, cultureInfo))
               //         {
               //             csv.Context.RegisterClassMap<ClassEventMap>();
               //             csv.WriteRecords(transportEvents);
               //         }
               //     }
               // }
               // catch
               // {
               //     dispatcher.Invoke(()=>ClassMessage.ShowMessage(
               //         MainWindow.currentMainWindow,"Ошибка! Импорт в Excel не завершен!",icon:Icon.Error));
               //     return;
               // }
               // dispatcher.Invoke(()=>ClassMessage.ShowMessage(
               //     MainWindow.currentMainWindow,"Экспорт в Excel завершен!", icon:Icon.Success));
    }
}