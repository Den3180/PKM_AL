using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PKM_AL.Classes.ServiceClasses;
using PKM_AL.Controls.OBDControls.Dr_PointTag;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlDr_Points : UserControl
{
    private readonly string title = "Точечные измерения";
    private readonly ObservableCollection<DataListUserControl> listTemplData =new ObservableCollection<DataListUserControl>();
    private TabControl tabControl;
    private List<Button> buttons;
    public List<XElement> lstTypeObj;
    public List<TextBox> lstTextBox;

    public UserControlDr_Points()
    {
        InitializeComponent();
        lstTypeObj = new List<XElement>();
        UserControls_Loaded(buttons);
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
        ClassControlManager.SetToolTipText(lstTextBox);
    }
    public IEnumerable<DataListUserControl> DataSourceList => listTemplData;

    /// <summary>
    /// Заполнение контроллов.
    /// </summary>
    private void UserControls_Loaded(List<Button> buttons)
    {
        (tabC.Items[0] as TabItem).Content = new UserControl_PM_REG() {Name= "PM_REG" };
        (tabC.Items[1] as TabItem).Content = new UserControl_KIP_REG() { Name = "KIP_REG" };
        (tabC.Items[2] as TabItem).Content = new UserControl_UKZ_REG() { Name = "UKZ_REG" };
        (tabC.Items[3] as TabItem).Content = new UserControl_UDZ_REG() { Name = "UDZ_REG" };
        (tabC.Items[4] as TabItem).Content = new UserControl_UPZ_REG() { Name = "UPZ_REG" };
        (tabC.Items[5] as TabItem).Content = new UserControl_VST_REG() { Name = "VST_REG" };
        (tabC.Items[6] as TabItem).Content = new UserControl_AD_REG() { Name = "AD_REG" };
        (tabC.Items[7] as TabItem).Content = new UserControl_VP_REG() { Name = "VP_REG" };
        (tabC.Items[8] as TabItem).Content = new UserControl_PM_REG_GR() { Name = "PM_REG_GR" };
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.DR_POINTS.HasElements)
        {
            WindowRepOBD.DR_POINTS.RemoveAll();
        }
        foreach (var elem in lstTypeObj)
        {
            WindowRepOBD.DR_POINTS.Add(elem);
        }
    }


    private void tabC_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
}