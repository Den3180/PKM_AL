using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Avalonia.Controls;
using Avalonia.Threading;
using CsvHelper;
using MsBox.Avalonia.Enums;

namespace PKM_AL.Classes.ServiceClasses;

public class ExportToCsv
{
     public static void ExportCsvParam(DataGrid GridEvents,List<ClassEvent> lstSourceEvents )
           {
               Dispatcher dispatcher=Dispatcher.UIThread;
               List<ClassTransportEvent> transportEvents = new List<ClassTransportEvent>();
               List<string> lstHeader = new List<string>();
               foreach (var header in GridEvents.Columns)
               {
                   dispatcher.Invoke(() => lstHeader.Add(header.Header.ToString()));
               }
               var count = 0;
               foreach (var ev in lstSourceEvents)
               {
                   count++;
                   transportEvents.Add(new ClassTransportEvent()
                   {
                      Id = count.ToString(),
                      Date = ev.StrDT,
                      Device = ev.NameDevice,
                      Param = ev.Param,
                      Value = ev.Val,
                      TypeEvent = ev.Name
                   });
               }
               CultureInfo cultureInfo = Environment.OSVersion.Platform == PlatformID.Win32NT ? 
                                          CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
               try
               {
   
                   using (FileStream sourceStream =
                          File.Open($"Параметры" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", FileMode.OpenOrCreate | FileMode.Truncate))
                   {
                       using var writer = new StreamWriter(sourceStream, Encoding.UTF8);
                       using (var csv = new CsvWriter(writer, cultureInfo))
                       {
                           csv.Context.RegisterClassMap<ClassEventMap>();
                           csv.WriteRecords(transportEvents);
                       }
                   }
               }
               catch
               {
                   dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                       MainWindow.currentMainWindow,"Ошибка! Импорт в Excel не завершен!",icon:Icon.Error));
                   return;
               }
               dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                   MainWindow.currentMainWindow,"Экспорт в Excel завершен!", icon:Icon.Success));
           } 
}