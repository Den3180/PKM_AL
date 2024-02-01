global using Avalonia.Interactivity;
using Avalonia.Controls;
using AvaloniaTest1.Service;
using MsBox.Avalonia.Enums;
using PKM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PKM_AL.Classes.ServiceClasses;
using Avalonia.Media.Imaging;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PKM_AL
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer TimerSec;

        public static ClassDB DB;
        public static MainWindow currentMainWindow;
        public static ClassSlave Slave;

        public static ObservableCollection<ClassGroup> Groups;
        public static ObservableCollection<ClassDevice> Devices;
        public static ObservableCollection<ClassChannel> Channels;
        public static ObservableCollection<ClassEvent> Events;

        public static ClassSettings settings;

        public MainWindow()
        {
            InitializeComponent();
            currentMainWindow = this;
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
                Task<ButtonResult> buttonResult = ClassMessage.ShowMessage(this, "База данных не доступна.\nСоздать базу данных?"
                      , "", ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Question);
                if (buttonResult.Result == ButtonResult.Yes)
                {
                    string path = ClassDialogWindows.CreateDBDialog(this);
                    if (!string.IsNullOrEmpty(path))
                    {
                        ClassDB.Create(path);
                        settings.DB = path;
                        settings.Save();
                    }
                    else
                    {
                        ClassMessage.ShowMessage(this, "База данных не создана.\nПриложение будет закрыто.",
                                                icon: MsBox.Avalonia.Enums.Icon.Stop);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    ClassMessage.ShowMessage(this, "База данных не создана.\nПриложение будет закрыто.",
                         icon: MsBox.Avalonia.Enums.Icon.Stop);
                    Environment.Exit(0);
                }
            }
            if (!DB.Open(settings))
            {
                ClassMessage.ShowMessage(this, "База данных не доступна.\nПроверьте конфигурацию!"
                                         , "", ButtonEnum.Ok, icon: MsBox.Avalonia.Enums.Icon.Error);
                WindowDB windowDb = new WindowDB();
                windowDb.WindowShow(this);
                Environment.Exit(0);
            }
            else
            {
                ClassMessage.ShowMessage(this, "База данных подключена.", "", ButtonEnum.Ok,
                                         icon: MsBox.Avalonia.Enums.Icon.Success);
            }
            int VersionDB = DB.InfoLoad();
            if (VersionDB != ClassDB.Version)
            {
                var buttonResault = ClassMessage.ShowMessage(this, "Версия базы данных не поддерживается.\nВыполнить обновление?", "",
                    ButtonEnum.YesNo, icon: MsBox.Avalonia.Enums.Icon.Error);
                if (buttonResault.Result == ButtonResult.No)
                {
                    ClassMessage.ShowMessage(this, "База данных не обновлена.\nПриложение будет закрыто.",
                         icon: MsBox.Avalonia.Enums.Icon.Stop);
                    Environment.Exit(0);
                }
                else
                {
                    if (!DB.Update(VersionDB))
                    {
                        ClassMessage.ShowMessage(this, "Ошибка обновления базы данных.\nПриложение будет закрыто.", "",
                                                 ButtonEnum.Ok, icon: MsBox.Avalonia.Enums.Icon.Error);
                        Environment.Exit(0);
                    }
                    else
                    {
                        ClassMessage.ShowMessage(this, "База данных обновлена!", "", ButtonEnum.Ok,
                                        icon: MsBox.Avalonia.Enums.Icon.Success);
                    }
                }
            }

            Events = new ObservableCollection<ClassEvent>();
            Devices = new ObservableCollection<ClassDevice>(DB.DevicesLoad());
            Channels = new ObservableCollection<ClassChannel>(DB.RegistriesLoad(0));
            Groups = new ObservableCollection<ClassGroup>();
            Slave = new ClassSlave();


            //Блок генерации дерева.
            BuildContentMainTreeItems();
            MakeTreeViewItem();

            //Блок настроек и запуска таймера.
            TimerSec = new DispatcherTimer();
            TimerSec.Tick += TimerSec_Tick;
            TimerSec.Interval = new TimeSpan(0, 0, 0, 0, 700);
            //TimerSec.Start();

            switch (settings.StartWindow)
            {
                case 0:
                this.ContentArea.Content = new UserControlDevices();
                this.StatusMode.Text = "Устройства";
                break;
                //case 1:
                //this.ContentArea.Content = new UserControlArchive();
                //this.StatusMode.Content = "Поиск в архиве";
                //break;
                //default:
                //if (settings.Interface && Maps.Count > 0)
                //{
                //    this.ContentArea.Content = new UserControlCanvas(Maps[0].ID);
                //    this.StatusMode.Content = "Мнемосхема";
                //}
                //break;
            }


        }

        private void TimerSec_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Формирование контента основного дерева приложения.
        /// </summary>
        private void BuildContentMainTreeItems()
        {
            ClassGroup d;
            //Добаваить узел устройств.
            d = new ClassGroup();
            d.ID = Groups.Count + 1;
            d.Name = "Устройства";
            d.IconUri = "folders_explorer.png";
            d.GroupType = ClassGroup.eType.Devices;
            foreach (ClassDevice item in Devices)
            {
                //Добавляем их в подузлы.
                d.SubGroups.Add(new ClassItem()
                {
                    ID = item.ID,
                    NameCh = item.Name,
                    IconUri = "hardware.png",
                    Group = d,
                    ItemType = ClassItem.eType.Device
                });
            }
            Groups.Add(d);

            //Добавить узел журналов.
            d = new ClassGroup();
            d.ID = Groups.Count + 1;
            d.Name = "Журналы";
            d.IconUri = "folders_explorer.png";
            //Подузел журнала событий.
            d.SubGroups.Add(new ClassItem()
            {
                ID = 1,
                NameCh = "Журнал событий",
                IconUri = "book.png",
                ItemType = ClassItem.eType.Log
            });
            //Подузел журнала тревог.
            d.SubGroups.Add(new ClassItem()
            {
                ID = 2,
                NameCh = "Журнал тревог",
                IconUri = "book_error.png",
                ItemType = ClassItem.eType.Alarms
            });
            Groups.Add(d);

            //Добавить узел сценариев.
            if (settings.Interface)
            {
                d = new ClassGroup();
                d.ID = Groups.Count + 1;
                d.Name = "Сценарии";
                d.IconUri = "folders_explorer.png";
                //Подузел команды.
                d.SubGroups.Add(new ClassItem()
                {
                    ID = 1,
                    NameCh = "Команды",
                    IconUri = "tags.png",
                    ItemType = ClassItem.eType.Command
                });
                //Подузел связи событий.
                d.SubGroups.Add(new ClassItem()
                {
                    ID = 2,
                    NameCh = "Связи событий",
                    IconUri = "connect.png",
                    ItemType = ClassItem.eType.Links
                });
                Groups.Add(d);
            }
                //Узел архив.
                d = new ClassGroup();
                d.ID = Groups.Count + 1;
                d.Name = "Архив";
                d.IconUri = "folders_explorer.png";
                //Подузел график трендов.
                d.SubGroups.Add(new ClassItem()
                {
                    ID = 1,
                    NameCh = "Графики трендов",
                    IconUri = "wave.png",
                    ItemType = ClassItem.eType.Graph,
                    Group = d
                });
                //Подузел поиск в архиве.
                d.SubGroups.Add(new ClassItem()
                {
                    ID = 2,
                    NameCh = "Поиск в архиве",
                    IconUri = "magnify_16.png",
                    ItemType = ClassItem.eType.Archive,
                    Group = d
                });
                Groups.Add(d);
        }

        /// <summary>
        /// Построение элементов основного дерева приложения.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private void MakeTreeViewItem()
        {
            foreach (var group in Groups)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header =ClassBuildControl.MakeContentTreeViewItem(group);
                foreach (var subGr in group.SubGroups)
                {
                    item.Items.Add(ClassBuildControl.MakeContentTreeViewItem(subGr));
                }
                this.treeView.Items.Add(item);
            }
        }


    private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
    {
        Task<ButtonResult> buttonResult = ClassMessage.ShowMessage(this, "Закрыть программу", "", ButtonEnum.YesNo,
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
            WindowDB windowDb = new WindowDB();
            windowDb.WindowShow(this);
            break;
            case "Параметры...":
            break;
            case "Пользователи...":
            break;
            case "Шаблоны...":
            break;
            case "Создать БД...":
            string path = ClassDialogWindows.CreateDBDialog(this);
            if (!string.IsNullOrEmpty(path))
            {
                ClassDB.Create(path);
            }
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