using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using MsBox.Avalonia.Enums;
using PKM;

namespace PKM_AL.Classes.ServiceClasses;

public class ExportToCsv
{
    /// <summary>
    /// Экспорт событий в .csv.
    /// </summary>
    /// <param name="GridEvents"></param>
    /// <param name="lstSourceEvents"></param>
     public static void ExportCsvParam(DataGrid GridEvents,List<ClassEvent> lstSourceEvents )
           {
               Dispatcher dispatcher=Dispatcher.UIThread;
               List<ClassTransportEvent> transportEvents = new List<ClassTransportEvent>();
              
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
                          File.Open($"Параметры" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", 
                              FileMode.Create))
                   {
                       using var writer = new StreamWriter(sourceStream, Encoding.UTF8);
                       using (var csv = new CsvWriter(writer, cultureInfo))
                       {
                           csv.Context.RegisterClassMap<ClassEventMap>();
                           csv.WriteRecords(transportEvents);
                       }
                   }
               }
               catch(Exception ex)
               {
                   dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                       MainWindow.currentMainWindow,"Ошибка! Импорт в Excel не завершен!",icon:Icon.Error));
                   return;
               }
               dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                   MainWindow.currentMainWindow,"Экспорт в Excel завершен!", icon:Icon.Success));
           }

            /// <summary>
            /// Экспорт отчетов в .csv
            /// </summary>
            /// <param name="GridEvents"></param>
            /// <param name="lstSourceEvents"></param>
           public static void ExportCsvParam(DataGrid GridEvents,List<(string,string)> lstSourceEvents)
           {
               Dispatcher dispatcher=Dispatcher.UIThread;
               List<ClassReportData> transportReports= dispatcher.Invoke(()=>GridEvents.ItemsSource.Cast<ClassReportData>().ToList());
               
               CultureInfo cultureInfo = Environment.OSVersion.Platform == PlatformID.Win32NT ? 
                   CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
               
               try
               {
                   using (FileStream sourceStream =
                          File.Open($"{lstSourceEvents[0].Item1}" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", 
                              FileMode.Create))
                   {
                       using var writer = new StreamWriter(sourceStream, Encoding.UTF8);
                       StringBuilder str = new StringBuilder();
                       foreach (var item in lstSourceEvents)
                       {
                           if(lstSourceEvents.IndexOf(item) == 0) continue;
                           str.Append(item.Item1+";");
                       }
                       writer.WriteLine(str.Append(" ;"));
                   }
                   using (FileStream sourceStream =
                          File.Open($"{lstSourceEvents[0].Item1}" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv", 
                              FileMode.Append))
                   {
                       using var writer = new StreamWriter(sourceStream, Encoding.UTF8);
                       foreach (var item in transportReports)
                       {
                           writer.WriteLine(item.ToString());
                       }
                   }
               }
               catch(Exception ex)
               {
                   dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                       MainWindow.currentMainWindow,"Ошибка! Импорт в Excel не завершен!",icon:Icon.Error));
                   return;
               }
               dispatcher.Invoke(()=>ClassMessage.ShowMessage(
                   MainWindow.currentMainWindow,"Экспорт в Excel завершен!", icon:Icon.Success));
           }
}