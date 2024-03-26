using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using MsBox.Avalonia.Enums;
using PKM;
using Icon = System.Drawing.Icon;

namespace PKM_AL.Controls;

public partial class UserControlAddReport : UserControl
{
    ClassReportSettings reportSettings;
    private readonly List<(string,string)> lstHeaders;
    private readonly EnumTypeReport typeReport;
    private ObservableCollection<ClassReportData> reportDatas;
    private Regex regex;
    public UserControlAddReport()
    {
        InitializeComponent();
    }
    public UserControlAddReport(EnumTypeReport typeReport)
    {
        InitializeComponent();
        reportDatas = new ObservableCollection<ClassReportData>();
        dtReport.SelectedDate = DateTime.Now;
        this.typeReport = typeReport;
        reportSettings = new ClassReportSettings();
        //Выбор списка заголовков выбранного отчета.
        lstHeaders = reportSettings.GetReportHaders(typeReport);            
        MainTitle.Text = lstHeaders[0].Item1;
        GridReports.ItemsSource = reportDatas;
        BuildDataGrid(lstHeaders);           
    }

    private void BuildDataGrid(List<(string, string)> lstHeaders)
    {
        for (int i = 1; i < lstHeaders.Count; i++)
        { 
            //Объект одной колонки.
            DataGridTextColumn column = new DataGridTextColumn();
            column.Header = $"{i}";
            //Редактирование возможно только с 4-ой колонки.
            column.IsReadOnly = i<3;
            //Ширина колонки(при)
            column.MaxWidth = 200;
            //Привяка данных к ячейкам.
            column.Binding = new Binding($"DateRec[{i - 1}]");
            column.Header = lstHeaders[i].Item1;
            //Добавление в источник данных.
            GridReports.Columns.Add(column);
        }
    }

    private void Add_NewNote_Btn(object sender, RoutedEventArgs e)
    {
        var temp = new ClassReportData(typeReport);            
        temp.DateRec = new ObservableCollection<string>();
        if (!ClearBtn.IsEnabled && !DeleteBtn.IsEnabled)
        {
            ClearBtn.IsEnabled = true;
            DeleteBtn.IsEnabled = true;
        }
        for (int i=1;i<lstHeaders.Count;i++)
        {
            //Первый столбец, номера по порядку.
            if (i == 1)
            {
                temp.DateRec.Add((reportDatas.Count+1).ToString());
            }
            //Второй столбец, даты.
            else if (i==2) 
            {
                temp.DateRec.Add(dtReport.SelectedDate.Value.ToString("yyyy-MM-dd"));
            }
            //остальные столбцы.
            else
            {
                temp.DateRec.Add(string.Empty);
            }
        }
        reportDatas.Add(temp);            
    }

    private void GridReports_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        string pattern = string.Empty;
        string nameColumn = e.Column.Header.ToString();
        var cellText = ((TextBox)(e.Column.GetCellContent(e.Row)));
        foreach (var head in lstHeaders)
        {
            if (head.Item1 == nameColumn)
            {
                pattern = reportSettings.GetPattern(head.Item2);
            }
        }
        if (!string.IsNullOrEmpty(cellText.Text) && !Regex.IsMatch(cellText.Text,pattern))
        {
            ClassMessage.ShowMessage(MainWindow.currentMainWindow, "Неверный ввод!",buttonEnum:ButtonEnum.Ok,
                icon: MsBox.Avalonia.Enums.Icon.Error);
            cellText.Text = "";
        }
    }

    private void ClearBtn_Click(object sender, RoutedEventArgs e)
    {
        reportDatas.Clear();
        ClearBtn.IsEnabled = false;
        DeleteBtn.IsEnabled = false;
    }

    private void DeleteBtn_Click(object sender, RoutedEventArgs e)
    {
        reportDatas.Remove(reportDatas[^1]);
        if (reportDatas.Count == 0)
        {
            ClearBtn.IsEnabled = false;
            DeleteBtn.IsEnabled = false;
        }
    }

    private void SaveRepBtn_Click(object sender, RoutedEventArgs e)
    {
        var dispatch = Dispatcher.UIThread;
        Task.Run(()=>dispatch.Invoke(()=>MainWindow.DB.SaveReport(GridReports.ItemsSource,typeReport)));
    }
}