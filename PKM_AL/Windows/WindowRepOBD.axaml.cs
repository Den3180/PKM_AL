using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MsBox.Avalonia.Enums;
using PKM_AL.Controls.OBDControls._MainTag;

namespace PKM_AL.Windows;

public partial class WindowRepOBD : Window
{
    public static XDocument ReportOBD { get; set; }//Документ XML.
    public static XElement EHZ_EXHG { get; set; }  //Тег EHZ_EXHG.
    public static XElement TYPEOBJS { get; set; }  //Тег TYPEOBJS.
    public static XElement DOGOVOR { get; set; }   //Тег DOGOVOR.
    public static XElement UCH { get; set; }       //Тег UCH.
    public static XElement DEFECTS { get; set; }   //Тег DEFECTS.
    public static XElement DR_POINTS { get; set; } //Тег DR_POINTS.
    public static XElement TRACE_SITUATION { get; set; }  //Тег TRACE_SITUATION.
    public static XElement FINDINGS { get; set; }  //Тег FINDINGS.
    public static XElement RECOMMENDATIONS { get; set; }  //Тег RECOMMENDATIONS.
    private List<TextBox> AllTextBox { get; set; }  // Все TextBox окна отчета ОБД.

    public WindowRepOBD()
    {
        InitializeComponent();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ReportOBD = new XDocument();
        ReportOBD.Declaration = new XDeclaration("1.0", Encoding.GetEncoding("windows-1251").WebName, "");
        EHZ_EXHG = new XElement(nameof(EHZ_EXHG));
        TYPEOBJS = new XElement(nameof(TYPEOBJS));
        DOGOVOR = new XElement(nameof(DOGOVOR));
        UCH = new XElement(nameof(UCH));
        DEFECTS = new XElement(nameof(DEFECTS));
        DR_POINTS = new XElement(nameof(DR_POINTS));
        TRACE_SITUATION = new XElement(nameof(TRACE_SITUATION));
        FINDINGS = new XElement(nameof(FINDINGS));
        RECOMMENDATIONS = new XElement(nameof(RECOMMENDATIONS));
        statusBarOBD.Text = (((TabItem)TabC.SelectedItem).Header as TextBlock).Text;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // if (!CheckFillingFields())
        // {
        //     ClassMessage.ShowMessage(MainWindow.currentMainWindow,"Заполните все обязательные поля!", 
        //         buttonEnum:ButtonEnum.Ok, icon:MsBox.Avalonia.Enums.Icon.Error);
        //     return;
        // }
        if (ReportOBD.Root != null)
        {
            ReportOBD.Elements().Remove();
        }            
        foreach (TabItem tab in TabC.Items)
        {
            var t= (tab as TabItem).Content as UserControl;
            switch (t.Name)
            {
                case "EHZ_EXHG":
                    (t as UserControlBegin).TagAssembly();
                    break;
                case "TYPEOBJS":
                    (t as UserControlTypeObjs).TagAssembly();
                    break;
                case "DOGOVOR":
                    (t as UserControlDogovor).TagAssembly();
                    break;
                case "USH":
                    (t as UserControlUSH).TagAssembly();
                    break;
                case "DEFECTS":
                    (t as UserControlDeffects).TagAssembly();
                    break;
                case "DR_POINTS":
                    (t as UserControlDr_Points).TagAssembly();
                    break;
                case "TRACE_SITUATION":
                    (t as UserControlTrace_Situation).TagAssembly();
                    break;
                case "FINDINGS":
                    (t as UserControlFindings).TagAssembly();
                    break;
                case "RECOMMENDATIONS":
                    (t as UserControlRecommendations).TagAssembly();
                    break;
            }
        }
        UCH.Add(DEFECTS);
        UCH.Add(DR_POINTS);
        UCH.Add(TRACE_SITUATION);
        UCH.Add(FINDINGS);
        UCH.Add(RECOMMENDATIONS);
        DOGOVOR.Add(UCH);
        EHZ_EXHG.Add(TYPEOBJS);
        EHZ_EXHG.Add(DOGOVOR);
        ReportOBD.Add(EHZ_EXHG);
        ReportOBD.Save("ReportOBD.xml");
    }

    private void SaveSend_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckFillingFields())
        {
                ClassMessage.ShowMessage(MainWindow.currentMainWindow,"Отчет не отправлен!\nЗаполните все обязательные поля!", 
                    buttonEnum:ButtonEnum.Ok, icon:MsBox.Avalonia.Enums.Icon.Error);
                return;
        }
    }

    private bool CheckFillingFields()
    {
        foreach (TextBox box in AllTextBox)
        {
            if (box.Tag.ToString().Contains("*") && string.IsNullOrEmpty(box.Text))
            {
                return false;
            }
        }
        return true;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Load_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TabControl tabControl = e.Source as TabControl;
        if (tabControl == null) return;
        TextBlock selectedText = (tabControl.SelectedItem as TabItem).Header as TextBlock;
        selectedText.FontWeight = FontWeight.ExtraBold;
        selectedText.Foreground = Brushes.IndianRed;
        foreach (var item in tabControl.Items)
        {
            if (item.Equals(tabControl.SelectedItem)) continue;
            ((item as TabItem).Header as TextBlock).FontWeight = FontWeight.SemiBold;
            ((item as TabItem).Header as TextBlock).Foreground = Brushes.Black;
        }
        if(statusBarOBD!=null) statusBarOBD.Text = selectedText.Text;   
    }

    private void Control_OnLoaded(object sender, RoutedEventArgs e)
    {
        (TabC.Items[0] as TabItem).Content = new UserControlBegin() { Name="EHZ_EXHG" };
        (TabC.Items[1] as TabItem).Content = new UserControlTypeObjs() { Name = "TYPEOBJS" };
        (TabC.Items[2] as TabItem).Content = new UserControlDogovor() { Name = "DOGOVOR" };
        (TabC.Items[3] as TabItem).Content = new UserControlUSH() { Name = "USH" };
        (TabC.Items[4] as TabItem).Content = new UserControlDeffects() { Name = "DEFECTS" };
        (TabC.Items[5] as TabItem).Content = new UserControlDr_Points() { Name = "DR_POINTS" };
        (TabC.Items[6] as TabItem).Content = new UserControlTrace_Situation() { Name = "TRACE_SITUATION" };
        (TabC.Items[7] as TabItem).Content = new UserControlFindings() { Name = "FINDINGS" };
        (TabC.Items[8] as TabItem).Content = new UserControlRecommendations() { Name = "RECOMMENDATIONS" };
        AllTextBox = (new ClassControlManager()).GetAllTextBox(TabC);
    }
}