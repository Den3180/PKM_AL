using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace PKM
{
    public class ClassLog
    {
        private static readonly string FileLog = "pkm.log";//Расположение лога.
        //private static StreamWriter writer;//Символьный поток.

        /// <summary>
        /// Запись в лог.
        /// </summary>
        /// <param name="Event">Название события</param>
        public static void Write(string Event)
        {
            //Записывает дату и событие.
            StreamWriter writer;
            string s = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "> " + Event;
            using (writer = new StreamWriter(FileLog, true))
            {
                try
                {
                    writer.WriteLine(s);
                //writer.Flush();//Очистка буфера и сброс кодировщика.
                }
                catch(Exception ex) 
                {
                }
            }
        }

        public static void WriteSMSLog(string smsText, bool isWrite)
        {
            if(!isWrite) return;
            string message =DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + " --> " + smsText+"\n";
            using StreamWriter streamWriter = new StreamWriter("sms.log",true);
            try
            {
                streamWriter.WriteAsync(message);
            }
            catch { }

        }
    }
}
