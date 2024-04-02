using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using PKM_AL.Controls.OBDControls._MainTag;

namespace PKM_AL.Classes.ServiceClasses;

public class ClassEventResource
{
        private bool accessButton = false;
        private Brush brushRed;
        private UserControl currentControl, dr_Point;
        private SolidColorBrush brushGreen;
        private readonly ObservableCollection<DataListUserControl> listTemplData;
        List<Button> buttons = new List<Button>();      

        public ClassEventResource()
        {
            listTemplData = new ObservableCollection<DataListUserControl>();
            //buttons = new List<Button>();
        } 

        /// <summary>
        /// Изменение темы поля при потере фокуса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as DatePicker) != null) return;
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text) && !Regex.IsMatch(textBox.Text, ClassControlManager.GetPatternOBDData(textBox.Tag)))
            {
                brushRed = new SolidColorBrush(Color.FromRgb(252, 170, 178));
                textBox.Background = brushRed;
                textBox.Foreground = Brushes.White;
                textBox.FontStyle = FontStyle.Italic;
            }
            else if(!string.IsNullOrEmpty(textBox.Text))
            {
                brushGreen = new SolidColorBrush(Color.FromRgb(217, 250, 205));
                textBox.Background = brushGreen;
            }
        }

        /// <summary>
        /// Изменение темы поля при возвращении фокуса.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Background.ToString() == brushRed?.ToString())
            {
                textBox.Background = Brushes.White;
                textBox.Foreground = Brushes.Black;
                textBox.FontStyle = FontStyle.Normal;
            }
        }

        /// <summary>
        /// Изменения в TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ClassControlManager controlManagement = new ClassControlManager();
            TextBox textBox = sender as TextBox;
            GetParent(textBox);
            List<TextBox> lstTBox = new List<TextBox>();
            List<TextBox> dr_Point_lstTBox = new List<TextBox>();
            buttons = ClassControlManager.GetUIElem<Button>(currentControl, new List<Button>());
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Background = Brushes.White;
            }
            ClassControlManager.CheckPointInValue(textBox);
            //Если окно тега DR_POINT открыто: ищем поля в этой форме.
            if (dr_Point != null)
            {
                 dr_Point_lstTBox = new ClassControlManager().GetTextBox(dr_Point);
                 //dr_Point_lstTBox = ClassControlManager.GetUIElem<TextBox>(dr_Point, new List<TextBox>());
            }
            lstTBox = ClassControlManager.GetUIElem<TextBox>(currentControl, new List<TextBox>());
            //Отключение вкладок для тега DR_POINT(когда выбрана одна, остальные не доступны).
            ClassControlManager.CancelTabsControl(currentControl,lstTBox);
            //Поиск кнопок "Добавить"/"Удалить запись"/"Удалить все".
            _=dr_Point==null? ClassControlManager.CheckFillingTboxes(lstTBox, buttons) : 
                              ClassControlManager.CheckFillingTboxes(dr_Point_lstTBox, buttons);                
        }

        /// <summary>
        /// Обработчик кнопок.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "PART_Button" || button.Name == "PART_NextButton" || button.Name == "PART_PreviousButton") return;
            GetParent(button);
            List<TextBox> lstTBox = ClassControlManager.GetUIElem<TextBox>(currentControl, new List<TextBox>());

            List<CalendarDatePicker> datePickers= ClassControlManager.GetUIElem<CalendarDatePicker>
                                                  (currentControl, new List<CalendarDatePicker>());

            List<ListBox> list = ClassControlManager.GetUIElem<ListBox>(currentControl, new List<ListBox>());
            List<TabControl> lstTabs = ClassControlManager.GetUIElem<TabControl>(currentControl, new List<TabControl>());
            List<XElement> lstElem=new List<XElement>();
            foreach(CalendarDatePicker picker in datePickers)
            {
                lstTBox.Add(new TextBox { Text=picker.Text, Name=picker.Name});
            }            
            list[0].ItemsSource = listTemplData;

            string nameTypeControl = (currentControl.GetType()).Name;
            if (button.Name == "b_Add")
            {
                if (lstTBox.Count == 0) return;
                XElement elem = AddTagToReport(currentControl, lstTBox);
                switch(nameTypeControl)
                {
                    case "UserControlTypeObjs":
                        lstElem = (currentControl as UserControlTypeObjs).lstTypeObj;
                        lstElem.Add(elem);
                        break;
                    case "UserControlDeffects":
                        lstElem = (currentControl as UserControlDeffects).lstTypeObj;
                        lstElem.Add(elem);
                        break;
                    case "UserControl_PM_REG":
                    case "UserControl_KIP_REG":
                    case "UserControl_UKZ_REG":
                    case "UserControl_UDZ_REG":
                    case "UserControl_UPZ_REG":
                    case "UserControl_VST_REG":
                    case "UserControl_AD_REG":
                    case "UserControl_VP_REG":
                    case "UserControl_PM_REG_GR":
                        lstElem = (dr_Point as UserControlDr_Points).lstTypeObj;
                        lstElem.Add(GetDR_POINTElem(dr_Point,elem));
                        List<TextBox> dr_Point_lstTBox = new ClassControlManager().GetTextBox(dr_Point);
                        foreach (TextBox box in dr_Point_lstTBox)
                        {
                            box.Text = string.Empty;
                        } 
                        break;
                    case "UserControlRecommendations":
                        lstElem = (currentControl as UserControlRecommendations).lstTypeObj;
                        lstElem.Add(elem);
                        break;
                }
                listTemplData.Add(new DataListUserControl()
                {
                    Number = (listTemplData.Count + 1).ToString(),
                    FirstName = "Запись добавлена ",
                    SecondName = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
                }
                   );
                buttons[1].IsEnabled=true;
                buttons[2].IsEnabled=true;
            }
            else if(button.Name == "b_Del" && list.Count>0)
            {
                switch (nameTypeControl)
                {
                    case "UserControlTypeObjs":
                        lstElem= (currentControl as UserControlTypeObjs).lstTypeObj;
                        lstElem.RemoveAt((lstElem.Count-1));
                        break;
                    case "UserControlDeffects":
                        lstElem= (currentControl as UserControlDeffects).lstTypeObj;
                        lstElem.RemoveAt(lstElem.Count - 1);
                        break;
                    case "UserControl_PM_REG":
                    case "UserControl_KIP_REG":
                    case "UserControl_UKZ_REG":
                    case "UserControl_UDZ_REG":
                    case "UserControl_UPZ_REG":
                    case "UserControl_VST_REG":
                    case "UserControl_AD_REG":
                    case "UserControl_VP_REG":
                    case "UserControl_PM_REG_GR":                        
                        lstElem= (dr_Point as UserControlDr_Points).lstTypeObj;
                        lstElem.RemoveAt(lstElem.Count-1);                
                        break;
                    case "UserControlRecommendations":
                        lstElem = (currentControl as UserControlRecommendations).lstTypeObj;
                        lstElem.RemoveAt(lstElem.Count - 1);
                        break;
                }
                listTemplData.RemoveAt(listTemplData.Count-1);
                if(lstElem.Count == 0)
                {
                    buttons[1].IsEnabled = false;
                    buttons[2].IsEnabled = false;
                }
            }
            else if(button.Name == "b_DelAll" && list.Count > 0)
            {
                switch (nameTypeControl)
                {
                    case "UserControlTypeObjs":
                        lstElem = (currentControl as UserControlTypeObjs).lstTypeObj;
                        lstElem.Clear();
                        break;
                    case "UserControlDeffects":
                        lstElem = (currentControl as UserControlDeffects).lstTypeObj;
                        lstElem.Clear();
                        break;
                    case "UserControl_PM_REG":
                    case "UserControl_KIP_REG":
                    case "UserControl_UKZ_REG":
                    case "UserControl_UDZ_REG":
                    case "UserControl_UPZ_REG":
                    case "UserControl_VST_REG":
                    case "UserControl_AD_REG":
                    case "UserControl_VP_REG":
                    case "UserControl_PM_REG_GR":
                        lstElem = (dr_Point as UserControlDr_Points).lstTypeObj;
                        lstElem.Clear();
                        break;
                    case "UserControlTrace_Situation":
                        lstElem = (currentControl as UserControlRecommendations).lstTypeObj;
                        lstElem.Clear();
                        break;

                }
                listTemplData.Clear();
                buttons[1].IsEnabled = false;
                buttons[2].IsEnabled = false;
            }
        }

        /// <summary>
        /// Сборка тега DR_POINT.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="elem"></param>
        /// <returns></returns>
        private XElement GetDR_POINTElem(UserControl control, XElement elem)
        {
             List<TextBox> lstTBoxDR_POINT = ClassControlManager.GetTextBoxFromDR_POINTS(dr_Point, new List<TextBox>());
                    var DR_POINT = new XElement("DR_POINT");
                    foreach(var tbox in lstTBoxDR_POINT)
                    {
                        if (tbox.Tag.ToString().Contains("GPS"))
                        {
                            ConvertDegreeToFormatOBD(tbox);
                        }
                        DR_POINT.Add(new XAttribute(tbox.Name,tbox.Text));
                    }                    
                    DR_POINT.Add(new XElement(elem));
            return DR_POINT;
        }

        /// <summary>
        /// Заполнение списка тегов отчета ОБД. 
        /// </summary>
        /// <param name="userControl"></param>
        /// <returns></returns>
        private XElement AddTagToReport(UserControl userControl, List<TextBox> lstTBox)
        {
            XElement elem=null;
            List<TabControl> item = ClassControlManager.GetUIElem<TabControl>(userControl,new List<TabControl>());
            string nameTag = item.Count >0 ? ((item[0].SelectedItem as TabItem).Content as UserControl).Name:userControl.Name;
            if ((userControl as UserControlTypeObjs)!=null)
            {
               elem = new XElement("TYPEOBJ");                
            }
            else
            {
                elem = new XElement(nameTag);                
            }
            foreach (var tbox in lstTBox)
            {
                if (tbox.Tag.ToString().Contains("GPS"))
                {
                    ConvertDegreeToFormatOBD(tbox);
                }
                elem.Add(new XAttribute(tbox.Name,tbox.Text));
            }
            return elem;
        }

        /// <summary>
        /// Поиск UserControl.
        /// </summary>
        /// <param name="current"></param>
        private void GetParent(ILogical current)
        {
            var parent = current.GetLogicalParent();// LogicalTreeHelper.GetParent(current);
            if (parent is UserControlDr_Points dr_Points)
            {
                dr_Point = dr_Points;
                List<TabItem> tabs = ClassControlManager.GetUIElem<TabItem>(dr_Points,new List<TabItem>());
                foreach(var tab in tabs)
                {
                    if (tab.IsSelected)
                    {
                        currentControl = tab.Content as UserControl;
                    }
                }               
            }
            else if ((parent as UserControl) != null)
            {
                currentControl = parent as UserControl;
            }
            if (parent != null)
            {
                GetParent(parent);
            }
        }

        /// <summary>
        /// Преобразование значения координаты к формату ОБД.
        /// </summary>
        /// <param name="textBox"></param>
        private void ConvertDegreeToFormatOBD(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text)) return;
            double coord = Convert.ToDouble(ClassControlManager.CheckCurrentSeparator( textBox.Text, 
                                            NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));
            int deg = (int)coord;
            double min = ((coord - deg) * 60);
            min = Math.Round(min, 4);
            string strMin = min.ToString("0.0000");
            textBox.Text = deg+"°"+strMin;
        }

}