using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    public List<TextBox> AllTextBox { get; set; }  // Все TextBox окна отчета ОБД.

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
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void SaveSend_Click(object sender, RoutedEventArgs e)
    {
        
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
        
    }
}