using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Avalonia.Threading;
using MsBox.Avalonia.Enums;

namespace PKM_AL.Classes;

public class ClassDeviceArchive
{
     public ClassDeviceArchive()
 {
 }

 /// <summary>
 /// Сохранение архива в базу данных.
 /// </summary>
 /// <param name="archive"></param>
 /// <param name="nmDev"></param>
 public static void SaveArchiveToDB(List<int[]> archive, object nmDev)
 {
     //Устройство, которому архив принадлежит.
     string devName = (nmDev as ClassDevice).Name;
     //Список регистров устройства.
     List<ClassChannel> lstDevCh = MainWindow.Devices.FirstOrDefault(x => x.Name == devName).Channels.
             Where(x => x.TypeRegistry == ClassChannel.EnumTypeRegistry.InputRegistry).ToList();
     try
     {
         foreach (var note in archive)
         {
             int centuryNow = (DateTime.Now.Year / 100) * 100;
             DateTime date = new DateTime(centuryNow + note[58], note[59], note[60], note[61], note[62], note[63]);
             string dateTime = date.ToString("yyyy-MM-dd HH:mm:ss");
             //количество записей ячеек в записи.
             int countNoteMax = note[15];
             //Временно всегда бкм5.
             ClassEvent ev = new ClassEvent() { Type = ClassEvent.EnumType.Measure, NameDevice = devName };
             int countList = 0;
             //Позиция начала данных.
             int positionStartData = 39;
             for (int i = positionStartData; i < note.Length; i++)
             {
                 if (countList >= countNoteMax) break;
                 //ClassChannel ch = lstDevCh[countList];
                 ClassChannel ch = lstDevCh.FirstOrDefault(c=>c.Address==countList);
                 if(ch==null) continue;
                 ev.Param = ch.Name;
                 ev.Val = note[i].ToString();
                 ev.DT = date;
                 MainWindow.DB.EventAddArchive(ev);
                 countList++;
             }
         }
     }
     catch(Exception ex)
     {
         Dispatcher.UIThread.Invoke(()=>ClassMessage.ShowMessageCustom
         (MainWindow.currentMainWindow, ex.Message, "Сохранение архива",
             icon: Icon.Error));
     }
     SaveArchiveToFile(archive);
     Dispatcher.UIThread.Invoke(()=>ClassMessage.ShowMessageCustom
     (MainWindow.currentMainWindow, "Архив сохранен в БД!", "Сохранение архива",
         icon: Icon.Success));
 }

 /// <summary>
 /// Сохранение архива в файл.
 /// </summary>
 /// <param name="archive"></param>
 private static void SaveArchiveToFile(List<int[]> archive)
 {
     try
     {
         byte[] buffer;
         var formatter = new BinaryFormatter();
         using (var stream = new MemoryStream())
         {
             formatter.Serialize(stream, archive);
             buffer = stream.ToArray();
         }

         using FileStream fileStream = new FileStream($"archive.pka", FileMode.OpenOrCreate);
         fileStream.Write(buffer);
     }
     catch (Exception ex)
     {
         Dispatcher.UIThread.Invoke(()=>ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Ошибка!Архив не сохранен в файл!", "Сохранение архива",
             icon: Icon.Error));
     }
     Dispatcher.UIThread.Invoke(()=>ClassMessage.ShowMessageCustom(MainWindow.currentMainWindow, "Архив сохранен в файл!",
         "Сохранение архива",
         icon: Icon.Success));
 }
}