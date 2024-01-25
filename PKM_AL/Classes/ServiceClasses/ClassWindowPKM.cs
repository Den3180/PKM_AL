using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PKM_AL
{
    public class ClassWindowPKM : Window
    {
        public void WindowShow(Window owner)
        {
            using (var source = new CancellationTokenSource())
            {
                this.ShowDialog(owner).ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
                Dispatcher.UIThread.MainLoop(source.Token);
            }

        }
    }
}
