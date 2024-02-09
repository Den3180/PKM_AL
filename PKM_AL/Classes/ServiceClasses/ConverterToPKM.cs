using System;
using System.IO;
using System.Xml.Serialization;

namespace PKM_AL.Classes.ServiceClasses;

public class ConverterToPKM
{
    public string FilePath { get; set; } = string.Empty;
    public static SerializableCellsContainer LoadMap(string FileName)
    {
        SerializableCellsContainer deviceTic = new SerializableCellsContainer(); //Объект устройства.
        using (TextReader reader = new StreamReader(FileName))  //Открытие потока чтения данных.
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableCellsContainer)); //Объект сериализации.
            try
            {
                return serializer.Deserialize(reader) as SerializableCellsContainer;//Считывание данных с объекта сериализации.              
            }
            catch (Exception Ex)
            {
            }
            throw new Exception();
        }
    }
}