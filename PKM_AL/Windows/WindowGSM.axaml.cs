using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PKM_AL.Classes.TransferClasses;

namespace PKM_AL.Windows;

public partial class WindowGSM : ClassWindowPKM
{
    private ClassGSM GSM;
    
    public WindowGSM()
    {
        InitializeComponent();
    }
    
    public WindowGSM(ClassGSM GSM)
    {
        InitializeComponent();
        this.GSM = GSM;
        this.GSM.EventSendCommand += GSM_EventSendCommand;
        this.GSM.EventReceivedMessage += GSM_EventReceivedMessage;
    }
    
    private void GSM_EventSendCommand(string Command)
    {
        Dispatcher.UIThread.Invoke(()=> this.txtInfo.Text = DateTime.Now.ToString("HH:mm:ss.fff") + " > " + Command
                                                       + Environment.NewLine + this.txtInfo.Text);
    }
    /// <summary>
    /// Запись полученного сообщения от модема в окно GSM.
    /// </summary>
    /// <param name="Message"></param>
    private void GSM_EventReceivedMessage(string Message)
    {
           Dispatcher.UIThread.Invoke(()=> this.txtInfo.Text = DateTime.Now.ToString("HH:mm:ss.fff") + " < " + Message
                                + Environment.NewLine + this.txtInfo.Text);
    }
    //Обработчик кнопки опрос.
    private void ButtonPoll_Click(object sender, RoutedEventArgs e)
    {
        GSM.SendPoll();
    }
    //Обработчик кнопки инициализация.
    private void ButtonInit_Click(object sender, RoutedEventArgs e)
    {
        GSM.SendInit();
    }
    //Обработчик кнопки уровень сигнала.
    private void ButtonSignal_Click(object sender, RoutedEventArgs e)
    {
        GSM.SendSignal();
    }
    //Обработчик кнопки состояния памяти/наличие смс.
    private void ButtonMemory_Click(object sender, RoutedEventArgs e)
    {
        GSM.SendGetMemory();
    }
    //Обработчик кнопки считать сообщение.
    private void ButtonSMS_Click(object sender, RoutedEventArgs e)
    {
        GSM.SendReadSMS(1);
    }

    /// <summary>
    /// Закрыть окно GSM
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Close(object sender, RoutedEventArgs e)
    {
        this.GSM.EventSendCommand -= GSM_EventSendCommand;
        this.GSM.EventReceivedMessage -= GSM_EventReceivedMessage;
        Close();
    }
}