using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PKM_AL.Controls.OBDControls.RecommendationTag;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlRecommendations : UserControl
{
    private TabControl tabControl;
    public List<TextBox> lstTextBox;
    public List<XElement> lstTypeObj;

    public UserControlRecommendations()
    {
        InitializeComponent();
        UserControls_Loaded();
        lstTypeObj = new List<XElement>();
        lstTextBox = (new ClassControlManager()).GetTextBox(this);

    }
    
            /// <summary>
        /// Заполнение контроллов.
        /// </summary>
        private void UserControls_Loaded()
        {
            (tabC.Items[0] as TabItem).Content = new UserControl_RECOM_UKZ() { Name= "RECOM_UKZ" };
            (tabC.Items[1] as TabItem).Content = new UserControl_RECOM_UDZ() { Name = "RECOM_UDZ" };
            (tabC.Items[2] as TabItem).Content = new UserControl_RECOM_UPZ() { Name = "RECOM_UPZ" };
            (tabC.Items[3] as TabItem).Content = new UserControl_RECOM_KIP() { Name = "RECOM_KIP" };
            (tabC.Items[4] as TabItem).Content = new UserControl_RECOM_VST() { Name = "RECOM_VST" };
            (tabC.Items[5] as TabItem).Content = new UserControl_RECOM_AJD() { Name = "RECOM_AJD" };
            (tabC.Items[6] as TabItem).Content = new UserControl_RECOM_SHURF() { Name = "RECOM_SHURF" };
            (tabC.Items[7] as TabItem).Content = new UserControl_RECOM_REM_IP() { Name = "RECOM_REM_IP" };
            (tabC.Items[8] as TabItem).Content = new UserControl_RECOM_ZAS() { Name = "RECOM_ZAS" };
            (tabC.Items[9] as TabItem).Content = new UserControl_RECOM_PROS() { Name = "RECOM_PROS" };
            (tabC.Items[10] as TabItem).Content = new UserControl_RECOM_REM_TP() { Name = "RECOM_REM_TP" };
            (tabC.Items[11] as TabItem).Content = new UserControl_RECOM_OBSL() { Name = "RECOM_OBSL" };
            (tabC.Items[12] as TabItem).Content = new UserControl_RECOM_OTHER() { Name = "RECOM_OTHER" };
            (tabC.Items[13] as TabItem).Content = new UserControl_NEW_UKZ() { Name = "RECOM_NEW_UKZ" };
            (tabC.Items[14] as TabItem).Content = new UserControl_NEW_UDZ() { Name = "RECOM_NEW_UDZ" };
            (tabC.Items[15] as TabItem).Content = new UserControl_NEW_UPZ() { Name = "RECOM_NEW_UPZ" };
            (tabC.Items[16] as TabItem).Content = new UserControl_NEW_KIP() { Name = "RECOM_NEW_KIP" };
            (tabC.Items[17] as TabItem).Content = new UserControl_NEW_VST() { Name = "RECOM_NEW_VST" };
        }

    private void tabC_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.RECOMMENDATIONS.HasElements)
        {
            WindowRepOBD.RECOMMENDATIONS.RemoveAll();
        }
        foreach (var elem in lstTypeObj)
        {
            WindowRepOBD.RECOMMENDATIONS.Add(elem);
        }
    }

}