using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using PKM_AL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;

namespace PKM_AL
{
    /// <summary>
    /// Класс создает оконные сообщения.
    /// </summary>
    public class ClassMessage
    {
        public enum EnumType
        {
            Request = 1,
            Answer = 2
        }

        public int ID { get; set; }
        public DateTime DT { get; set; }
        public EnumType Type { get; set; }
        public byte[] Bytes { get; set; }
        public string StrDT { get { return DT.ToString("dd.MM.yyyy hh:mm:ss.fff"); } }
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case EnumType.Request: return "Запрос";
                    case EnumType.Answer: return "Ответ";
                    default: return "Нет данных";
                }
            }
        }
        public string StrBytes
        {
            get
            {
                string s = "";
                foreach (byte b in Bytes)
                    s += "0x" + b.ToString("X2") + " ";
                return s; 
            } 
        }

        public ClassMessage()
        {
            ID = 0;
            DT = DateTime.Now;
            Type = EnumType.Request;
            Bytes = new byte[3];
            for (int i = 0; i < Bytes.Length; i++)
                Bytes[i] = (byte)i;
        }


        public static Task<ButtonResult> ShowMessage (Window owner, string text = "", string title = "", 
            ButtonEnum buttonEnum = ButtonEnum.Ok, Icon icon = Icon.Info, WindowStartupLocation location = WindowStartupLocation.CenterScreen )
        {
            Task<ButtonResult> res;
            using (var source = new CancellationTokenSource())
            {
                IMsBox <ButtonResult> messageWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = title,
                    ContentMessage = text,
                    WindowStartupLocation=location,
                    ButtonDefinitions=buttonEnum,
                    Icon=icon,
                    SystemDecorations = SystemDecorations.Full
                }) ;
                res=messageWindow.ShowWindowDialogAsync(owner);
                res.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }
            return res;
        }
        
        public static Task<string> ShowMessageCustom (Window owner, string text = "", string title = "", 
            ButtonEnum buttonEnum = ButtonEnum.Ok, Icon icon = Icon.Info, WindowStartupLocation location = WindowStartupLocation.CenterScreen )
        {
            Task<string> res;
            using (var source = new CancellationTokenSource())
            {
                var _assembly = Assembly.GetEntryAssembly()?.GetName().Name;
                var messageWindow = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams()
                {
                    ButtonDefinitions = GetButtonDefinition(buttonEnum),
                    ContentTitle = title,
                    ContentMessage =text,
                    Icon = icon,
                    WindowStartupLocation = location,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ShowInCenter = true,
                    Topmost = false,
                    WindowIcon = new WindowIcon(
                        new Bitmap(AssetLoader.Open(
                            new Uri($"avares://{_assembly}/Assets/iconfinder_inventory_categories_44826.ico")))
                        )
                }) ;
                res = messageWindow.ShowWindowDialogAsync(owner);
                res.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }
            return res;
        }

        /// <summary>
        /// Определяет набор кнопок.
        /// </summary>
        /// <param name="buttonEnum"></param>
        /// <returns></returns>
        private static IEnumerable<ButtonDefinition> GetButtonDefinition(ButtonEnum buttonEnum) => buttonEnum switch
        {
            ButtonEnum.YesNo=>new[]{new ButtonDefinition{Name="Да"}, new ButtonDefinition{Name="Нет"}},
            ButtonEnum.YesNoCancel=>new []{
                new ButtonDefinition{Name="Да"}, 
                new ButtonDefinition{Name="Нет"},
                new ButtonDefinition{Name="Отмена"},
            },
            _=>new []{new ButtonDefinition()},
        };
       

        public static void ShowMessage(string text, Window owner)
        {
            WindowMassage w = new(text, owner);
            w.ShowDialog(owner);
        }
        
        public static void SaveNewMessage(ClassMessage mes, byte func, ushort[] data = null)
        {
            if (data != null)
            {
                mes.Bytes = new byte[data.Length * 2 + 1];
                mes.Bytes[0] = func; //Func
                for (int i = 0; i < data.Length; i++)
                {
                    byte[] b = BitConverter.GetBytes(data[i]);
                    mes.Bytes[1 + i * 2] = b[1]; //High
                    mes.Bytes[1 + i * 2 + 1] = b[0]; //Low
                }
            }
            MainWindow.DB.MessageAdd(mes);
            MainWindow.Messages.Add(mes);
        }

    }
}
