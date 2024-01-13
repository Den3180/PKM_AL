using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using PKM_AL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest1.Service
{
    public class ClassMessage
    {
        static IMsBox <ButtonResult> mesageWindow;

        public static void ShowMessage(Window owner, string text = "", string title = "", ButtonEnum buttonEnum = ButtonEnum.Ok,
                                       Icon icon = Icon.None, WindowStartupLocation location = WindowStartupLocation.CenterScreen )
        {
            // mesageWindow = MessageBoxManager.GetMessageBoxStandard(title, text, buttonEnum, icon, location);
            mesageWindow = MessageBoxManager.GetMessageBoxStandard(new MsBox.Avalonia.Dto.MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = text,
                WindowStartupLocation=WindowStartupLocation.CenterScreen,
                ButtonDefinitions=ButtonEnum.OkCancel,
                Icon=Icon.Success,
                SystemDecorations = SystemDecorations.BorderOnly
            }) ;
            mesageWindow.ShowWindowDialogAsync(owner);
        }

        public static void ShowMessage(string text, Window owner)
        {
            WindowMassage w = new WindowMassage(text, owner);
            w.ShowDialog(owner);
        }
    }
}
