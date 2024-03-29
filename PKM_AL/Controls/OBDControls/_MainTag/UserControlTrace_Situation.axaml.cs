using System.Collections.Generic;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PKM_AL.Classes.ServiceClasses;
using PKM_AL.Controls.OBDControls.Trace_SituationTag;
using PKM_AL.Windows;

namespace PKM_AL.Controls.OBDControls._MainTag;

public partial class UserControlTrace_Situation : UserControl
{
    private TabControl tabControl;
    public List<TextBox> lstTextBox;
    
    public UserControlTrace_Situation()
    {
        InitializeComponent();
        UserControls_Loaded();
    }
    
    public void TagAssembly()
    {
        if (WindowRepOBD.TRACE_SITUATION.HasElements)
        {
            WindowRepOBD.TRACE_SITUATION.RemoveAll();
        }
        foreach (TabItem item in tabC.Items)
        {
            List<TextBox> lst = (new ClassControlManager()).GetTextBox(item);
            XElement elem = new XElement((item.Content as UserControl).Name);
            foreach (var box in lst)
            {
                elem.Add(new XAttribute(box.Name, box.Text));
            }
            WindowRepOBD.TRACE_SITUATION.Add(elem);
        }
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
    
    /// <summary>
    /// Заполнение контроллов.
    /// </summary>
    private void UserControls_Loaded()
    {
        (tabC.Items[0] as TabItem).Content = new UserControl_GROUND() { Name="GROUND"};
        (tabC.Items[1] as TabItem).Content = new UserControl_DISTRICT() { Name = "DISTRICT" };
        (tabC.Items[2] as TabItem).Content = new UserControl_VEGETATION() { Name = "VEGETATION" };
        (tabC.Items[3] as TabItem).Content = new UserControl_CROSSING() { Name = "CROSSING" };
        lstTextBox = (new ClassControlManager()).GetTextBox(this);
    }
}