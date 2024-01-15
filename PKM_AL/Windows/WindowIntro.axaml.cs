using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading;

namespace PKM_AL;

public partial class WindowIntro : Window
{
    private string[] listIntro = new []{ "Загрузка приложения...", "Подключение к базе данных...", "Инициализация данных..." };
    private Avalonia.Threading.DispatcherTimer dispatcherTimer;
    int countText=0;
    public WindowIntro()
    {
        InitializeComponent();
        this.Opened += WindowIntro_Opened;
        dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(900) };
        dispatcherTimer.Tick += DispatcherTimer_Tick; ;
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        if (countText > 2)
        {
            dispatcherTimer.Stop();
            Close();
            return;
        }
        ShowInfo(listIntro[countText]);
        countText++;
    }

    private void WindowIntro_Opened(object sender, System.EventArgs e)
    {
        dispatcherTimer.Start();
    }

    public void ShowInfo(string info)
    {
        this.LabelInfo.Text = info;
    }
}