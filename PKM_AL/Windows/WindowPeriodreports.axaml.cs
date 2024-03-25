using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PKM_AL.Windows;

public partial class WindowPeriodreports : ClassWindowPKM
{
    DateTime dtBegin;
    DateTime dtEnd;
    public WindowPeriodreports()
    {
        InitializeComponent();
        dtBegin = DateTime.MinValue;
        dtEnd = DateTime.MaxValue;
        dtB.SelectedDate = DateTime.Now;
        dtE.SelectedDate = DateTime.Now;
    }
    
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (chBox.IsChecked == false)
        {
            dtBegin = (DateTime)dtB.SelectedDate;
            dtEnd = (DateTime)dtE.SelectedDate;
            Content = new List<DateTime>() { dtBegin, dtEnd };
        }
        else
        {
            Content = new List<DateTime>() { dtBegin, dtEnd };
        }
        Close();
    }

    private void ChBox_OnIsCheckedChanged(object sender, RoutedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        if(checkBox==null) return;
        if (checkBox.IsChecked == true)
        {
            if(dtB!=null && dtE != null)
            {
                dtB.IsEnabled = false;
                dtE.IsEnabled = false;
            }
        }
        else
        {
            dtB.IsEnabled = true;
            dtE.IsEnabled = true;
        }
    }
}