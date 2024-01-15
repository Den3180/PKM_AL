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
        /// �������� ��������� ����.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ClassLog.Write("������ ����������");
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
                    buttonResult= ClassMessage.ShowMessage(this, "���� �� ��������.\n������� ���� ������?"
                        ,"",ButtonEnum.YesNo,icon:MsBox.Avalonia.Enums.Icon.Question);
                    buttonResult.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                    Dispatcher.UIThread.MainLoop(source.Token);
                }
                if (buttonResult.Result == ButtonResult.Yes)
                {
                    using (var source = new CancellationTokenSource())
                    {
                        var topLevel = TopLevel.GetTopLevel(this);
                        var files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                        {
                            Title = "����� ��"                            
                        });
                        IStorageFile lis = files.Result;
                        files.ContinueWith(t=>source.Cancel(),TaskScheduler.FromCurrentSynchronizationContext());                    
                        Dispatcher.UIThread.MainLoop(source.Token);
                    }
                }

                //    //if (System.Windows.MessageBox.Show("���� �� �� ��������" + Environment.NewLine
                //    //                    + "������� ��?", "����", MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
                //    //                    == MessageBoxResult.Yes)

                //    ClassMessage.ShowMessage(this, "���� �� �� ��������");
                //    //if(ClassMessage.ShowMessage(this, "���� �� �� ��������"))
                //    //{
                //    //    //System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                //    //    //dlg.Filter = "����� DB (*.db)|*.db|��� ����� (*.*)|*.*";
                //    //    //dlg.FileName = "pkm.db";
                //    //    //if ((dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) &&
                //    //    //    (dlg.FileName != ""))
                //    //    //{
                //    //    //    ClassDB.Create(dlg.FileName);
                //    //    //}
                //    //}
            }
            //ClassMessage.ShowMessage(this, "����� ������������...");
        }

        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            Task<ButtonResult> buttonResult;
            using (var source = new CancellationTokenSource())
            {
                buttonResult = ClassMessage.ShowMessage(this, "������� ���������","",ButtonEnum.YesNo);
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
                case "�����":
                //ClassMessage.ShowMessage("��������� ������?", this);
                Close();
                break;
                case "����� ������������...":
                // ClassMessage.ShowMessage(this,"����� ������������...");
                break;
                case "����������...":
                break;
                case "������ ������...":
                break;
                case "�������...":
                break;
                case "������ �������...":
                break;
                case "������ ������...":
                break;
                case "������ ���������...":
                break;
                case "�����...":
                break;
                case "���� ������...":
                break;
                case "���������...":
                break;
                case "������������...":
                break;
                case "�������...":
                break;
                case "������� ��...":
                break;
                case "� ���������...":
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