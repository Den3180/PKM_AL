using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using PKM_AL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaTest1.Service
{
    public class ClassMessage
    {
        //static IMsBox <ButtonResult> mesageWindow;

        public static Task<ButtonResult> ShowMessage(Window owner, string text = "", string title = "", ButtonEnum buttonEnum = ButtonEnum.Ok,
                                       Icon icon = Icon.Info, WindowStartupLocation location = WindowStartupLocation.CenterScreen )
        {
            Task<ButtonResult> res;
            using (var source = new CancellationTokenSource())
            {
                IMsBox <ButtonResult> messageWindow = MessageBoxManager.GetMessageBoxStandard(new MsBox.Avalonia.Dto.MessageBoxStandardParams
                {
                    ContentTitle = title,
                    ContentMessage = text,
                    WindowStartupLocation=location,
                    ButtonDefinitions=buttonEnum,
                    Icon=icon,
                    SystemDecorations = SystemDecorations.None                    
                }) ;
                res=messageWindow.ShowWindowDialogAsync(owner);
                res.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }
            return res;
        }

        public static void ShowMessage(string text, Window owner)
        {
            WindowMassage w = new(text, owner);
            w.ShowDialog(owner);
        }
    }
}
