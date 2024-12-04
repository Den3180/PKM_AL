using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PKM_AL;

public partial class WindowIntro : ClassWindowPKM
{
    private string[] listIntro = new []{ "Загрузка приложения...", "Подключение к базе данных...", "Инициализация данных..." };
    private Avalonia.Threading.DispatcherTimer dispatcherTimer;
    int countText=0;
    private string _inputString = string.Empty;
    public WindowIntro()
    {
        InitializeComponent();
        Opened += WindowIntro_Opened;
        dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(900) };
        dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    public WindowIntro(string str)
    {
        InitializeComponent();
        _inputString = str;
        LabelInfo.Text = _inputString;
        Opened += WindowIntro_Opened;
        dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(900) };
        dispatcherTimer.Tick += DispatcherTimer_Tick;
    }
    
    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        //Если сторка не передана в конструктор.
       if(string.IsNullOrEmpty(_inputString))
       {
           if (countText > 2)
           {
               dispatcherTimer.Stop();
               Close();
               return;
           }
           ShowInfo(listIntro[countText]);
       }
       //Если в конструктор передана строка.
       else
       {
           if (countText > 0)
           {
               dispatcherTimer.Stop();
               Close();
               return;
           }
       } 
       countText++;
    }

    private void WindowIntro_Opened(object sender, System.EventArgs e)
    {
        dispatcherTimer.Start();
    }

    
   /// <summary>
   /// Отображение текста в окне.
   /// </summary>
   /// <param name="info">Строка для отображения</param>
    public void ShowInfo(string info)
    {
        LabelInfo.Text = info;
    }
}