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
    public static class ClassDialogWindows
    {
        /// <summary>
        /// Диалог создать базу данных.
        /// </summary>
        public static string CreateDbDialog(Window owner)
        {
            Task<IStorageFile> files;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Выбор БД",
                    DefaultExtension = "db",
                    ShowOverwritePrompt = true,
                    SuggestedFileName = "pkm.db",
                    FileTypeChoices = new List<FilePickerFileType>()
                            {
                                new FilePickerFileType("База данных") { Patterns=new[] { "*.db" } },
                                new FilePickerFileType("Все файлы") { Patterns=new[] { "*.*" } }
                            }
                });
                files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }
            var dialogResult = files.Result;
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
        public static string ChooseDbDialog(Window owner)
        {
            Task < IReadOnlyList < IStorageFile >> files;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                files = topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Выбор БД",
                    AllowMultiple=false,
                    FileTypeFilter = new List<FilePickerFileType>()
                    {
                        new FilePickerFileType("Все файлы") { Patterns=new[] { "*.*" } },
                        new FilePickerFileType("База данных") { Patterns=new[] { "*.db" } }
                    }
                });
            files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
            }
            if (files.Result.Count > 0)
            {
                return files.Result[0].Path.LocalPath;
            }
            return string.Empty;
        }
        
        /// <summary>
        /// Общий диалог выбора файлов.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static string SaveDialog(Window owner)
        {
            Task<IStorageFile> files;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                files = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Сохранить",
                    DefaultExtension = "xml",
                    ShowOverwritePrompt = true,
                    SuggestedFileName = "device",
                    FileTypeChoices = new List<FilePickerFileType>()
                    {
                        
                        new FilePickerFileType("XML") { Patterns=new[] { "*.xml" } },
                        new FilePickerFileType("Все файлы") { Patterns=new[] { "*.*" } }
                    }
                });
                // files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                // Dispatcher.UIThread.MainLoop(source.Token);
            }
            var dialogResult = files.Result;
            if (!string.IsNullOrEmpty(dialogResult?.Name))
            {
                return dialogResult?.Path.LocalPath;
            }
            return string.Empty;
        }
        
        /// <summary>
        /// Общий диалог выбора.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static string ChooseDialog(Window owner)
        {
            Task < IReadOnlyList < IStorageFile >> files;
            using (var source = new CancellationTokenSource())
            {
                var topLevel = TopLevel.GetTopLevel(owner);
                files = topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Выбор файла",
                    AllowMultiple=false,
                    FileTypeFilter = new List<FilePickerFileType>()
                    {
                        new FilePickerFileType("База данных") { Patterns=new[] { "*.xml" } },
                        new FilePickerFileType("Все файлы") { Patterns=new[] { "*.*" } }
                    }
                });
                // files.ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                // Dispatcher.UIThread.MainLoop(source.Token);
            }
            if (files.Result.Count > 0)
            {
                return files.Result[0].Path.LocalPath;
            }
            return string.Empty;
        }
    }
}
