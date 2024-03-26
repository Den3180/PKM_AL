using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
    
    /// <summary>
    /// Построение таблицы.
    /// </summary>
    /// <param name="lstHeaders"></param>
    private void BuildDataGrid(List<(string,string)> lstHeaders)
    {
        for (int i = 1; i < lstHeaders.Count; i++)
        {
            DataGridTextColumn column = new DataGridTextColumn();
            column.IsReadOnly = true;
            column.Binding = new Binding($"DateRec[{i - 1}]");
            column.Header = lstHeaders[i].Item1;
            GridReports.Columns.Add(column);
        }
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(()=>ExportToCsv.ExportCsvParam(GridReports,lstHeaders));
    }
}