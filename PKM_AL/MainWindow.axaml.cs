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
using System.IO;
using System.Windows;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using Avalonia;

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

            WindowIntro frmIntro = new WindowIntro();
            frmIntro.WindowShow(this);

            if (!File.Exists(settings.DB))
            {
                Task<ButtonResult> buttonResult= ClassMessage.ShowMessage(this, "База данных не доступна.\nСоздать базу данных?"
                      ,"",ButtonEnum.YesNo,icon:MsBox.Avalonia.Enums.Icon.Question); 

                if (buttonResult.Result == ButtonResult.Yes)
                {
                    using (var source = new CancellationTokenSource())
                    {
                        var topLevel = TopLevel.GetTopLevel(this);
                        var files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                        {
                            Title = "Выбор БД",
                            DefaultExtension = "db",
                            ShowOverwritePrompt = true,
                            SuggestedFileName = "pkm.db",
                            FileTypeChoices = new List<FilePickerFileType>()
                            {
                                new FilePickerFileType("") { Patterns=new[] { "*.db" } }
                            }
                        });
                        files.ContinueWith(t=>source.Cancel(),TaskScheduler.FromCurrentSynchronizationContext());
                        Dispatcher.UIThread.MainLoop(source.Token);
                        var dialogResult = files.Result;
                        if (!string.IsNullOrEmpty(dialogResult?.Name))
                        {
                            ClassDB.Create(dialogResult?.Path.AbsolutePath);
                        }
                        else
                        {
                            ClassMessage.ShowMessage(this,"База данных не создана.\nПрограмма будет закрыта.", 
                                                     icon: MsBox.Avalonia.Enums.Icon.Stop);
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    ClassMessage.ShowMessage(this, "База данных не создана.\nПрограмма будет закрыта.",
                         icon: MsBox.Avalonia.Enums.Icon.Stop);
                    Environment.Exit(0);

                }
            }
        }

        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            Task<ButtonResult> buttonResult = ClassMessage.ShowMessage(this, "Закрыть программу","",ButtonEnum.YesNo, 
                                                                       icon: MsBox.Avalonia.Enums.Icon.Question);
            if (buttonResult.Result == ButtonResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "Выход":
                Close();
                break;
                case "Смена пользователя...":                
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
                    WindowDB windowDb = new WindowDB();
                    
                    windowDb.WindowShow(this);
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