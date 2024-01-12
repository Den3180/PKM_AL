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
namespace PKM_AL
{
    public partial class MainWindow : Window
    {
        public static List<ClassGroup> Groups;
        //public static ObservableCollection<ClassDevice> Devices;
        //public static ObservableCollection<ClassChannel> Channels;
        public static ClassSettings settings;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //WindowMassage mes = new WindowMassage("Завершить работу программы?", this);
            //flag = mes.ShowDialog<bool>(this).Result;
            //if (flag)
            //{
            //    Close();
            //}
        }

        /// <summary>
        /// Загрузка основного окна.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            settings = ClassSettings.Load();
        }

        bool flag = false;
        private void MainWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            //ClassMessage.ShowMessage("Завершить работу программы?",this);
            //if(buttonResult== MsBox.Avalonia.Enums.ButtonResult.No)
            //{
            //    e.Cancel = true;
            //    return;
            //}

            //WindowMassage mes = new WindowMassage("Завершить работу программы?", this);
            //mes.ShowDialog<bool>(this);
            //if (!flag)
            //{
            //    e.Cancel = true;
            //}
            Environment.Exit(0);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "Выход":
                WindowMassage mes = new WindowMassage("Завершить работу программы?", this);
                var yy= mes.ShowDialog<bool>(this);
                //Close();
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