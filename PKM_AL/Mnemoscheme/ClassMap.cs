using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Avalonia.Media;
using PKM;

namespace PKM_AL.Mnemoscheme;

public class ClassMap
{
    [XmlIgnore] 
    public int ID { get; set; }
 
    public string Name { get; set; }
 
    public Color ForeColor { get; set; }

    //public List<ClassWidget> Widgets { get; set; }

    public ClassMap()
    {
        ID = 0;
        Name = "";
        ForeColor = Colors.Blue;
        //Widgets = new List<ClassWidget>();
    }

    public string GetJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.IgnoreReadOnlyProperties = true;
        string s = JsonSerializer.Serialize<ClassMap>(this, options);
        return s;
    }

    public static ClassMap GetObject(string json)
    {
        ClassMap obj = JsonSerializer.Deserialize<ClassMap>(json);
        return obj;
    }

    public bool SaveProfile(string FileName)
    {
        TextWriter writer = new StreamWriter(FileName);
        XmlSerializer serializer = new XmlSerializer(typeof(ClassMap));
        serializer.Serialize(writer, this);
        writer.Close();
        return true;
    }

    public static ClassMap Load(string FileName)
    {
        ClassMap map = null;
        TextReader reader = new StreamReader(FileName);
        XmlSerializer serializer = new XmlSerializer(typeof(ClassMap));
        try
        {
            map = (ClassMap)serializer.Deserialize(reader);
        }
        catch (Exception Ex)
        {
            ClassLog.Write(Ex.Message);
        }
        return map;
    }
}