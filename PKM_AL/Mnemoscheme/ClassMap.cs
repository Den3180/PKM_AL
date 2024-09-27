using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM;
using TestGrathic.ServiceClasses;

namespace PKM_AL.Mnemoscheme;

public class ClassMap
{
    [XmlIgnore] 
    public int ID { get; set; }
    
    public Guid GuidID { get; set; }
    
    public string Name { get; set; }
 
    public Color ForeColor { get; set; }
    
    public string ForeColorString { get; set; }

   // [JsonIgnore]
    public List<ClassWidget> Widgets { get; set; }
    private int count = 0;

    public ClassMap()
    {
        ID = 0;
        GuidID=Guid.NewGuid();
        Name = "";
        ForeColor = Colors.Blue;
        ForeColorString = ForeColor.ToString();
        Widgets = new List<ClassWidget>();
        count++;
    }

    public string GetJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            IgnoreReadOnlyProperties = true
        };
        //options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        //options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        string s = JsonSerializer.Serialize<ClassMap>(this, options);
        ClassMap obj = JsonSerializer.Deserialize<ClassMap>(s);
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