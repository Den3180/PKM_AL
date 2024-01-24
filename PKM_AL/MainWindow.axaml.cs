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
            using (var source = new CancellationTokenSource())
            {
                WindowIntro frmIntro = new WindowIntro();
                frmIntro.ShowDialog(this).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());                
                Dispatcher.UIThread.MainLoop(source.Token);
            }

            if (!File.Exists(settings.DB))
            {
                Task<ButtonResult> buttonResult;
                using (var source = new CancellationTokenSource())
                {
                    buttonResult= ClassMessage.ShowMessage(this, "База данных не доступна.\nСоздать базу данных?"
                        ,"",ButtonEnum.YesNo,icon:MsBox.Avalonia.Enums.Icon.Question);
                    buttonResult.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                    Dispatcher.UIThread.MainLoop(source.Token);
                }
                if (buttonResult.Result == ButtonResult.Yes)
                {
                    //IStorageFile dialogResult;
                    using (var source = new CancellationTokenSource())
                    {
                        var topLevel = TopLevel.GetTopLevel(this);
                        var files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                        {
                            Title = "Выбор БД",
                            DefaultExtension="db",
                            ShowOverwritePrompt=true,
                            SuggestedFileName="pkm.db",
                            FileTypeChoices=new List<FilePickerFileType>() 
                            { 
                                new FilePickerFileType("") { Patterns=new[] { "*.db" } } 
                            }
                        });
                        files.ContinueWith(t=>source.Cancel(),TaskScheduler.FromCurrentSynchronizationContext());
                        Dispatcher.UIThread.MainLoop(source.Token);
                        var dialogResult = files.Result;
                        if (dialogResult?.Name!=string.Empty)
                        {
                            ClassDB.Create(dialogResult?.Path.AbsolutePath);
                            //ClassDB.MySQLCreate(files.Result.Path.AbsolutePath);
                            //ClassDB.PostgresCreate();
                        }
                    }
                }

                //    //if (System.Windows.MessageBox.Show("Файл БД не доступен" + Environment.NewLine
                //    //                    + "Создать БД?", "СУБД", MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
                //    //                    == MessageBoxResult.Yes)

                //    ClassMessage.ShowMessage(this, "Файл БД не доступен");
                //    //if(ClassMessage.ShowMessage(this, "Файл БД не доступен"))
                //    //{
                //    //    //System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                //    //    //dlg.Filter = "Файлы DB (*.db)|*.db|Все файлы (*.*)|*.*";
                //    //    //dlg.FileName = "pkm.db";
                //    //    //if ((dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) &&
                //    //    //    (dlg.FileName != ""))
                //    //    //{
                //    //    //    ClassDB.Create(dlg.FileName);
                //    //    //}
                //    //}
            }
            //ClassMessage.ShowMessage(this, "Смена пользователя...");
        }

        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            Task<ButtonResult> buttonResult;
            using (var source = new CancellationTokenSource())
            {
                buttonResult = ClassMessage.ShowMessage(this, "Закрыть программу","",ButtonEnum.YesNo);
                buttonResult.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }            
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
                //ClassMessage.ShowMessage("Завершить работу?", this);
                Close();
                break;
                case "Смена пользователя...":
                // ClassMessage.ShowMessage(this,"Смена пользователя...");
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