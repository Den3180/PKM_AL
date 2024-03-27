using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia.Media;
using PKM_AL.Controls.OBDControls.DeffectsTag;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlDeffects : UserControl
{
    private TabControl tabControl;
    public List<TextBox> lstTextBox;
    public List<XElement> lstTypeObj;

    public UserControlDeffects()
    {
        InitializeComponent();
        UserControls_Loaded();
        lstTypeObj = new List<XElement>();
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
    }

    private void UserControls_Loaded()
    {
        (tabC.Items[0] as TabItem).Content = new UserControl_PZ() { Name="PZ"};
        (tabC.Items[1] as TabItem).Content = new UserControl_NZ() { Name = "NZ" };
        (tabC.Items[2] as TabItem).Content = new UserControl_IP_I() { Name = "IP_I" };
        (tabC.Items[3] as TabItem).Content = new UserControl_IP_IPI() { Name = "IP_IPI" };
        (tabC.Items[4] as TabItem).Content = new UserControl_ZKO() { Name = "ZKO" };
        (tabC.Items[5] as TabItem).Content = new UserControl_ZBT() { Name = "ZBT" };
        (tabC.Items[6] as TabItem).Content = new UserControl_ZVV() { Name = "ZVV" };
        (tabC.Items[7] as TabItem).Content = new UserControl_ZNU() { Name = "ZNU" };
        (tabC.Items[8] as TabItem).Content = new UserControl_SHR() { Name = "SHR" };
    }

    /// <summary>
    /// Выделение заголовка вкладки табконтрола цветом и толщиной.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TabControl_SelectionChanged_Deffects(object sender, SelectionChangedEventArgs e)
    {
        tabControl = e.Source as TabControl;
        if (tabControl == null) return;
        TextBlock selectedText = (tabControl.SelectedItem as TabItem).Header as TextBlock;
        selectedText.FontWeight = FontWeight.ExtraBold;
        selectedText.Foreground = Brushes.Green;
        foreach (var item in tabControl.Items)
        {
            if (!item.Equals(tabControl.SelectedItem))
            {
                ((item as TabItem).Header as TextBlock).FontWeight = FontWeight.SemiBold;
                ((item as TabItem).Header as TextBlock).Foreground = Brushes.Black;
            }
        }
        e.Handled = true;
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.DEFECTS.HasElements)
        {
            WindowRepOBD.DEFECTS.RemoveAll();
        }
        foreach (var elem in lstTypeObj)
        {
            WindowRepOBD.DEFECTS.Add(elem);
        }
    }

}