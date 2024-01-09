using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml.Serialization;

namespace PKM
{     

    public class ClassSettings
    {

        public enum EnumTypeDB
        {
            SQLite = 0,
            MSSQL = 1,
            MySQL = 2,
            PostgreSQL = 3
        }
        
        private const string FileName = "pkm.xml";
        private const string PathDB = "./pkm.db";
        public EnumTypeDB TypeDB { get; set; }
        public string DB { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int PortModbus { get; set; }
        public int PortModem { get; set; }
        public int Timeout { get; set; }
        public bool RequestLogin { get; set; }
        public bool RecordLog { get; set; }
        public bool Interpol { get; set; }
        public List<string> DevicesColumns { get; set; }
        public List<string> ChannelsColumns { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public int Parity { get; set; }
        public int StopBits { get; set; }
        public bool Interface { get; set; }
        public int StartWindow { get; set; }
        public bool ModbusSlave { get; set; }
        public int PeriodUSIKP { get; set; }

        public ClassSettings()
        {
            TypeDB = EnumTypeDB.SQLite;
            DB = PathDB;
            Server = "localhost";
            PortModbus = 1;
            PortModem = 0;
            Timeout = 300;
            RequestLogin = false;
            RecordLog = false;
            Interpol = false;
            BaudRate = 9600;
            DataBits = 8;
            Parity = 0;
            StopBits = 1;
            Interface = false;
            StartWindow = 0;
            ModbusSlave = false;
            DevicesColumns = new List<string>();
            ChannelsColumns = new List<string> ();            
        }

        public void Save()
        {
            TextWriter writer = new StreamWriter(FileName);
            XmlSerializer serializer = new XmlSerializer(typeof(ClassSettings));
            serializer.Serialize(writer, this);
            writer.Close();
        }
        /// <summary>
        /// Загрузка настроек из файла.
        /// </summary>
        /// <returns></returns>
        public static ClassSettings Load()
        {
            ClassSettings settings = new ClassSettings();
            if (!File.Exists(FileName)) return settings;
            TextReader reader = new StreamReader(FileName);
            XmlSerializer serializer = new XmlSerializer(typeof(ClassSettings));
            try
            {
                settings = (ClassSettings)serializer.Deserialize(reader);
            }
            catch { }
            finally
            {
                reader.Close();
            }
            return settings;
        }
    }
}
