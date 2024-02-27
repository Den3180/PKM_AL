global using Avalonia.Interactivity;
using Avalonia.Controls;
using MsBox.Avalonia.Enums;
using PKM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using Avalonia;
using Avalonia.Input;
using Avalonia.LogicalTree;
using PKM_AL.Classes.ServiceClasses;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
using PKM_AL.Classes;
using PKM_AL.Classes.TransferClasses;
using PKM_AL.Controls;
using PKM_AL.Windows;

namespace PKM_AL
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer TimerSec;

        public static ClassDB DB;
        public static MainWindow currentMainWindow;
        public static ClassSlave Slave;
        private static MainWindow thisG;
        private int _TxCount;
        private int _RxCount;
        private int countBackup;//Счетчик сохранение БД.
        private bool _PortErrorMessageShown;
        private static string _assembly;
        public static ClassUSIKP USIKP;
        public static ClassGSM GSM;
        public static ClassModbus modbus;
        public static CancellationTokenSource cts;//Токен отмены.
        public static ClassUser User;

        public static ObservableCollection<ClassGroup> Groups;
        public static ObservableCollection<ClassDevice> Devices;
        public static ObservableCollection<ClassChannel> Channels;
        public static ObservableCollection<ClassEvent> Events;
        public static ObservableCollection<ClassEvent> EventsAlarm;
        public static ObservableCollection<ClassCommand> Commands;
        public static Queue<ClassCommand> QueueCommands;
        public static ObservableCollection<ClassLink> Links;
        public static ObservableCollection<ClassMessage> Messages;


        public static ClassSettings settings;

        public MainWindow()
        {
            InitializeComponent();
            currentMainWindow = this;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            _assembly=Assembly.GetEntryAssembly()?.GetName().Name;
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
                    string path = ClassDialogWindows.CreateDbDialog(this);
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
                var buttonResault = ClassMessage.ShowMessage(this, "Версия базы данных не поддерживается." +
                                                                   "\nВыполнить обновление?", "", ButtonEnum.YesNo, 
                                                                    icon: MsBox.Avalonia.Enums.Icon.Error);
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
            EventsAlarm = new ObservableCollection<ClassEvent>();
            Devices = new ObservableCollection<ClassDevice>(DB.DevicesLoad());
            Channels = new ObservableCollection<ClassChannel>(DB.RegistriesLoad(0));
            Groups = new ObservableCollection<ClassGroup>();
            Slave = new ClassSlave();
            Commands = new ObservableCollection<ClassCommand>(DB.CommandsLoad());
            QueueCommands = new Queue<ClassCommand>();
            Links = new ObservableCollection<ClassLink>(DB.LinksLoad());

            //Заполнение списка каналов для каждого устройства.
            foreach (var item in Channels)
            {
                item.Device = Devices.FirstOrDefault(x => x.ID == item.Device.ID);                
                item.Device?.Channels.Add(item);
            }
            
            foreach (ClassCommand item in Commands)
            {
                item.Device = Devices.FirstOrDefault(x => x.ID == item.Device.ID);
            }

            //Блок генерации дерева.
            BuildContentMainTreeItems();
            MakeTreeViewItem(Groups);

            //Блок настроек и запуска таймера.
            TimerSec = new DispatcherTimer();
            TimerSec.Tick += TimerSec_Tick;
            TimerSec.Interval = new TimeSpan(0, 0, 0, 0, 700);
            TimerSec.Start();
            modbus = new ClassModbus();
            modbus.PortErrorEvent += Modbus_PortErrorEvent;
            modbus.SendRequestEvent += Modbus_SendRequestEvent;
            modbus.ReceivedAnswerEvent += Modbus_ReceivedAnswerEvent;
            
            GSM = new ClassGSM();
            GSM.EventStateChanged += GSM_EventStateChanged;
            if (!string.IsNullOrEmpty(settings.PortModem) && settings.PortModem != "Нет") 
                Task.Run(()=> GSM.Start(settings.PortModem));

            //Создается событие начала работы программы и добавляется архив базы данных.
            DB.EventAdd(new ClassEvent() { Type = ClassEvent.EnumType.Start});
            switch (settings.StartWindow)
            {
                case 0:
                ContentArea.Content = new UserControlDevices();
                StatusMode.Text = "Устройства";
                break;
                case 1:
                ContentArea.Content = new UserControlArchive();
                StatusMode.Text = "Поиск в архиве";
                break;
                //default:
                //if (settings.Interface && Maps.Count > 0)
                //{
                //    this.ContentArea.Content = new UserControlCanvas(Maps[0].ID);
                //    this.StatusMode.Content = "Мнемосхема";
                //}
                //break;
            }
        }

        /// <summary>
        /// Основной таймер.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TimerSec_Tick(object sender, EventArgs e)
        {
            //Запись времени в статусбар.
            StatusTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
            
            //Если в настройках стоит запрос пользователя.
            // if (settings.RequestLogin && (User == null))
            // {
            //     UserLogin();
            // }
            
            //Если не подключено устройство.
            if (modbus.Mode == ClassModbus.eMode.None)
            {
                foreach (var dev in Devices)
                {
                    if (dev.Protocol != ClassDevice.EnumProtocol.SMS &&
                        dev.Protocol != ClassDevice.EnumProtocol.GPRS_SMS &&
                        dev.Protocol != ClassDevice.EnumProtocol.GPRS)
                        dev.PortDisable();
                }

                //Если номер порта не ноль (по умолчанию 1).
                // if (settings.PortModbus != 0)
                if (!string.IsNullOrEmpty(settings.PortModbus))
                {
                    //В этом методе modbus.Mode -> ClassModbus.eMode.PortOpen.
                    //bool res= modbus.PortOpen(settings.PortModbus, settings.BaudRate, settings.DataBits,
                    //    settings.Parity, settings.StopBits);
                    //if (!res) TimerSec.Stop();

                   modbus.PortOpen(settings.PortModbus, settings.BaudRate, settings.DataBits, settings.Parity, settings.StopBits);
                   // await Task.Run(()=>modbus.PortOpen(settings.PortModbus, settings.BaudRate, settings.DataBits,
                   //      settings.Parity, settings.StopBits));
                    //Если включить выход, то все нижние строки не будут доступны.
                    //return;
                }
            }
            if (modbus.Mode == ClassModbus.eMode.PortOpen)
            {
                modbus.RequestServerID();            
            }
            if (modbus.Mode == ClassModbus.eMode.MasterInit && DateTime.Now.Second % 7 == 0)
            {
                _PortErrorMessageShown = !_PortErrorMessageShown;
                modbus.Poll(DateTime.Now.Ticks);
            }
            else
            {
                ImageTx.Source = new Bitmap
                    (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"bullet-grey-32.png")));
                ImageRx.Source = new Bitmap
                    (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"bullet-grey-32.png")));
            }
            if (DateTime.Now.Second % 10 == 0)
            {
                if (settings.PeriodUSIKP > 0) USIKP.Poll();
            }
            
            //Запрос к памяти модема каждую 10-ю секунду.
            if (DateTime.Now.Second % 10 == 0 && settings.PortModem!="Нет")
            {
                if (GSM.StatusPortModem!=ClassGSM.EModePortModem.PortModemOpen)
                {
                   await Task.Run(()=> GSM.Start(settings.PortModem));
                }
                else
                { 
                   // _PortErrorMessageShown = !_PortErrorMessageShown;
                    GSM.SendGetMemory();
                }
            }
            //Сохранение БД каждые 10 часов.
            if (countBackup++>36000)
            {
                DB.Backup(new FileInfo("pkm.exe").DirectoryName);
                countBackup = 0;
            }

        }
        
        /// <summary>
        /// Логгирование исключений.
        /// </summary>
        /// <param name="ex"></param>
        public static void ExeptionLogging(Exception ex)
        {
            string s = Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                       + " " + ex.Message + " " + ex.StackTrace;
            ClassLog.Write(s);
        }
        
        private void Modbus_SendRequestEvent()
        {
            _TxCount++;
            if (_TxCount > 9999) _TxCount = 0;
            StatusFrames.Text = _TxCount.ToString() + "/" + _RxCount.ToString();
            ImageTx.Source = new Bitmap
                (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"bullet-green-32.png")));
        }
        
        private void Modbus_ReceivedAnswerEvent()
        {
            _RxCount++;
            if (_RxCount > 9999) _RxCount = 0;
            StatusFrames.Text = _TxCount.ToString() + "/" + _RxCount.ToString();
            ImageRx.Source = new Bitmap
                (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"bullet-green-32.png")));
        }
        
        /// <summary>
        /// Событие ошибки порта.
        /// </summary>
        /// <param name="ErrorMessage"></param>
        private void Modbus_PortErrorEvent(string ErrorMessage)
        {
            if (_PortErrorMessageShown) return; 
            _PortErrorMessageShown = true;
            ClassEvent eventLostPort = new ClassEvent()
            {
                Type = ClassEvent.EnumType.PortDisabled,
                Param = settings.PortModbus
            };
            DB.EventAdd(eventLostPort);
            Events.Add(eventLostPort);
            if (eventLostPort.Category is ClassEvent.EnumCategory.Fault or ClassEvent.EnumCategory.Alarm)
            {
                EventsAlarm.Add(eventLostPort);
            }
            Dispatcher.UIThread.Invoke(()=> ClassMessage.ShowMessage(currentMainWindow, settings.PortModbus.ToString() + 
                " не доступен" + Environment.NewLine + ErrorMessage + Environment.NewLine + 
                "Проверьте настройки конфигурации","Инициализация",ButtonEnum.Ok,MsBox.Avalonia.Enums.Icon.Error));
        }

        /// <summary>
        /// Загрузка индикаторов подключено/отключено для GSM-соединения.
        /// </summary>
        /// <param name="State"></param>
        private void GSM_EventStateChanged(bool State)
        {
            if (State)
                Dispatcher.UIThread.Invoke(()=> ImageDB.Source = new Bitmap
                    (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"database_green.png"))));
            else 
                Dispatcher.UIThread.Invoke(()=> ImageDB.Source = new Bitmap
                (AssetLoader.Open(new Uri($"avares://{_assembly}/Resources/"+"database_blue.png"))));
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
        /// <param name="Groups"></param>
        /// <returns></returns>
        private void MakeTreeViewItem(ObservableCollection<ClassGroup> Groups)
        {
            foreach (var group in Groups)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header =ClassBuildControl.MakeContentTreeViewItem(group);
                foreach (var subGr in group.SubGroups)
                {
                    item.Items.Add(ClassBuildControl.MakeContentTreeViewItem(subGr));
                }
                treeView.Items.Add(item);
            }
        }
        
        /// <summary>
        /// Происходит в момент закрытия основного окна приложения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            Task<ButtonResult> buttonResult = ClassMessage.ShowMessage(this, "Закрыть программу", "", ButtonEnum.YesNo,
                                                                       icon: MsBox.Avalonia.Enums.Icon.Question);
            if (buttonResult.Result == ButtonResult.No)
            {
                e.Cancel = true;
                return;
            }
            DB.EventAdd(new ClassEvent() { Type = ClassEvent.EnumType.Finish });
            DB.Backup(new FileInfo("pkm.exe").DirectoryName);
            DB.Close();
            ClassLog.Write("Завершение приложения");
            Environment.Exit(0);
        }

        /// <summary>
        /// Кнопки основного меню.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var currentContent = ContentArea.Content;
            MenuItem menuItem = (MenuItem)sender;
        switch (menuItem.Header)
        {
            case "Выход":
            Close();
            break;
            case "Смена пользователя...":
            break;
            case "Устройства...":
                if (currentContent is UserControlDevices) break; 
                ContentArea.Content = new UserControlDevices();
                StatusMode.Text = "Устройства";
            break;
            case "Каналы данных...":
                if (currentContent is UserControlChannels) break; 
                ContentArea.Content = new UserControlChannels();
                StatusMode.Text = "Каналы данных";
            break;
            case "Графики...":
            break;
            case "Журнал событий...":
                if (currentContent is UserControlEvents) break; 
                ContentArea.Content = new UserControlEvents();
                StatusMode.Text = "Журнал событий";
            break;
            case "Журнал тревог...":
                ContentArea.Content = new UserControlAlarms();
                StatusMode.Text = "Журнал тревог";
            break;
            case "Журнал сообщений...":
                ContentArea.Content = new UserControlMessages();
                StatusMode.Text = "Журнал сообщений";
            break;
            case "Карта...":
            break;
            case "База данных...":
            WindowDB windowDb = new WindowDB();
            windowDb.WindowShow(this);
            break;
            case "Параметры...":
                WindowConfig windowConfig = new WindowConfig();
                windowConfig.WindowShow(this);
                if (windowConfig.Tag != null && ((bool)windowConfig.Tag) == true)
                {
                    settings = ClassSettings.Load();
                    modbus.Mode = ClassModbus.eMode.None;
                    GSM.StatusPortModem = ClassGSM.EModePortModem.None;
                }
                break;
            case "Пользователи...":
            break;
            case "Шаблоны...":
            break;
            case "Создать БД...":
            string path = ClassDialogWindows.CreateDbDialog(this);
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
        
        /// <summary>
        /// Открыть окно GSM.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemGSM_Click(object sender, RoutedEventArgs e)
    {
        WindowGSM frm = new WindowGSM(GSM);
        frm.WindowShow(this);
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
    
    /// <summary>
    /// Выбор элемента дерева.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentContent = ContentArea.Content;
        if ((sender as TreeView)?.SelectedItem is StackPanel subStackPanel)
        {
            ClassItem subGroup =new ClassItem();
            IEnumerable<ILogical> devItem = subStackPanel?.GetLogicalChildren();
            foreach (var control in devItem)
            {
                if(control is TextBlock label)
                {
                    subGroup = label.Tag as ClassItem;
                }
            }
            switch (subGroup?.ItemType)
            {
                case ClassItem.eType.Device:
                    ClassDevice obj = Devices.FirstOrDefault(x => x.ID == subGroup.ID);
                    ContentArea.Content = new UserControlChannels(obj);
                    break;
                case ClassItem.eType.Log:
                    if (currentContent is UserControlEvents) break; 
                    ContentArea.Content = new UserControlEvents();
                    StatusMode.Text = "Журнал событий";
                    break;
                case ClassItem.eType.Alarms:
                    if (currentContent is UserControlAlarms) break;
                    ContentArea.Content = new UserControlAlarms();
                    StatusMode.Text = "Журнал тревог";
                    break;
            }
        }
        else if((sender as TreeView)?.SelectedItem is TreeViewItem treeViewItem)
        {
            ClassGroup group = new ClassGroup();
            var devItem= (treeViewItem.Header as StackPanel)?.GetLogicalChildren();
                foreach (var control in devItem)
                {
                    if (control is TextBlock label)
                    {
                        group = label.Tag as ClassGroup;
                    }
                }
                switch (group?.GroupType)
                {
                    case ClassGroup.eType.Devices:
                        if (currentContent is UserControlDevices) break; 
                        ContentArea.Content = new UserControlDevices();
                        break;
                }
        }
        }

        /// <summary>
        /// Открывает окно добавление устройства.
        /// </summary>
        public static void DeviceAdd()
        {
            WindowDevice frm = new WindowDevice(new ClassDevice());
            frm.WindowShow(currentMainWindow);
            bool? res = (bool)frm.Tag;
            //Если была отмена - выход.
            if (!res.Value) return;
            //Добавление в дерево устройств новое устройство.
            ClassItem item = new ClassItem()
            {
                ID = frm.Device.ID,
                NameCh = frm.Device.Name,
                IconUri = "hardware.png",
                Group = Groups[0],
                ItemType = ClassItem.eType.Device
            };
            Groups[0].SubGroups.Add(item);
            (currentMainWindow.treeView.Items[0] as TreeViewItem)?.Items.Add(ClassBuildControl.MakeContentTreeViewItem(item));
        }

        private void TreeView_OnTapped(object sender, TappedEventArgs e)
        {
            
        }
    }
}