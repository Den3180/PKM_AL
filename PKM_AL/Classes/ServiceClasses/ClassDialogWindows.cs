using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PKM_AL.Classes.ServiceClasses
{
    public class ClassDialogWindows
    {
        /// <summary>
        /// Диалог создать базу данных.
        /// </summary>
        public static string CreateDBDialog(Window owner)
        {
            IStorageFile dialogResult;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                var files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Выбор БД",
                    DefaultExtension = "db",
                    ShowOverwritePrompt = true,
                    SuggestedFileName = "pkm.db",
                    FileTypeChoices = new List<FilePickerFileType>()
                            {
                                new FilePickerFileType("База данных") { Patterns=new[] { "*.db" } },
                                new FilePickerFileType("Все файлы") { Patterns=new[] { "*.*"} }
                            }
                });
                files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
                dialogResult = files.Result;
            }
            if (!string.IsNullOrEmpty(dialogResult?.Name))
            {
                return dialogResult?.Path.LocalPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// Диалог выбрать файл базы данных.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static string ChooseDBDialog(Window owner)
        {
            IStorageFile dialogResult;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                var files = topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Выбор БД",
                    AllowMultiple=false,
                    FileTypeFilter = new List<FilePickerFileType>()
                    {
                        new FilePickerFileType("База данных") { Patterns=new[] { "*.db" } }
                    }
                }); 
            files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
            dialogResult = files.Result[0];
            }
            if (!string.IsNullOrEmpty(dialogResult?.Name))
            {
                return dialogResult?.Path.LocalPath;
            }
            return string.Empty;
        }
    }
}
