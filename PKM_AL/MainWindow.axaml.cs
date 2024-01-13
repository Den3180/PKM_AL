global using Avalonia.Interactivity;
using Avalonia.Controls;
using AvaloniaTest1.Service;
using MsBox.Avalonia.Enums;
using PKM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
namespace PKM_AL
{
    public partial class MainWindow : Window
    {
        public static List<ClassGroup> Groups;
        public static ClassDB DB;
        //public static ObservableCollection<ClassDevice> Devices;
        //public static ObservableCollection<ClassChannel> Channels;
        public static ClassSettings settings;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        /// <summary>
        /// Загрузка основного окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClassLog.Write("Запуск приложения");
            settings = ClassSettings.Load();
            switch (settings.TypeDB)
            {
                case ClassSettings.EnumTypeDB.SQLite: DB = new ClassDB(); break;
                default:
                break;
            }
            using (var source = new CancellationTokenSource())
            {
                WindowIntro frmIntro = new WindowIntro();
            //Task flag= frmIntro.ShowDialog(this);
                frmIntro.ShowDialog(this).ContinueWith(t=>source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }
            //while (flag.Status==TaskStatus.WaitingForActivation) 
            //{ 
            //}
            ClassMessage.ShowMessage(this, "Смена пользователя...");
        }

        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "Выход":
                ClassMessage.ShowMessage("Завершить работу?", this);
                break;
                case "Смена пользователя...":
                ClassMessage.ShowMessage(this,"Смена пользователя...");
                break;
                case "Устройства...":
                break;
                case "Каналы данных...":
                break;
                case "Графики...":
                break;
                case "Журнал событий...":
                break;
                case "Журнал тревог...":
                break;
                case "Журнал сообщений...":
                break;
                case "Карта...":
                break;
                case "База данных...":
                break;
                case "Параметры...":
                break;
                case "Пользователи...":
                break;
                case "Шаблоны...":
                break;
                case "Создать БД...":
                break;
                case "О программе...":
                break;
                default:
                break;
            }
        }

        private void MenuItemGSM_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_Form(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_OBD(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_Reports(object sender, RoutedEventArgs e)
        {
        }

        private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}