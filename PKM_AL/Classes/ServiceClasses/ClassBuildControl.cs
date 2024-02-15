using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using System.Reflection;

namespace PKM_AL.Classes.ServiceClasses
{
    public class ClassBuildControl
    {
        /// <summary>
        /// Формирование содержимого TreeViewItem.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StackPanel MakeContentTreeViewItem(object obj)
        {
            string sourcePath = (obj is ClassGroup) == true ? ((ClassGroup)obj).IconUri : ((ClassItem)obj).IconUri;
            string assembly = Assembly.GetEntryAssembly()?.GetName().Name;

            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal
            };
            TextBlock label = new TextBlock()
            {
                Text = obj.ToString(),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Tag = obj
            };

            Image image = new Image()
            {
                Source = new Bitmap(AssetLoader.Open(new Uri($"avares://{assembly}/Resources/{sourcePath}")))
            };
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(label);
            return stackPanel;
        }
    }
}
