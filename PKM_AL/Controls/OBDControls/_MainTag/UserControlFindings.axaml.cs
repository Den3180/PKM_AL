using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PKM_AL.Controls.OBDControls.FindingsTag;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlFindings : UserControl
{
    private TabControl tabControl;
    public UserControlFindings()
    {
        InitializeComponent();
        UserControls_Loaded();
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    
    /// <summary>
    /// Заполнение контроллов.
    /// </summary>
    private void UserControls_Loaded()
    {
        (tabC.Items[0] as TabItem).Content = new UserControl_RESUME_GLOBAL() { Name="RESUME_GLOBAL"};
        (tabC.Items[1] as TabItem).Content = new UserControl_SIP_RP() { Name = "SIP_RP" };
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.FINDINGS.HasElements)
        {
            WindowRepOBD.FINDINGS.RemoveAll();
        }
        foreach (TabItem item in tabC.Items)
        {
            List<TextBox> lst = (new ClassControlManager()).GetTextBox(item);
            XElement elem = new XElement((item.Content as UserControl).Name);
            foreach (var box in lst)
            {
                elem.Add(new XAttribute(box.Name, box.Text));
            }
            WindowRepOBD.FINDINGS.Add(elem);
        }
    }
}