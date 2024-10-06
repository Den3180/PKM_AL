using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.ServiceClasses;
using PKM_AL.Mnemoscheme.ViewMap;
using PKM;

namespace PKM_AL.Mnemoscheme;

public class ClassMap :INotifyPropertyChanged
{
    
    private IBrush _backgroundColor;
    private string _mapColorString;
    
    [XmlIgnore] 
    public int ID { get; set; }
    
    public Guid GuidID { get; set; }
    
    public string Name { get; set; }
 
    [XmlIgnore] 
    public Color ForeColor { get; set; }

    [XmlIgnore] 
    public string CurrentPathFile { get; set; } = string.Empty;
    public List<ClassWidget> Widgets { get; set; }
    

    [XmlIgnore] 
    public IBrush BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (value.Equals(_backgroundColor)) return;
            _backgroundColor = value;
            MapColorString = _backgroundColor.ToString();
            OnPropertyChanged();
        }
    }

    public string MapColorString
    {
        get=>_mapColorString;
        set
        {
            _mapColorString = value;
            BackgroundColor = Brush.Parse(value);
        }
    }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ClassMap()
    {
        ID = 0;
        GuidID=Guid.NewGuid();
        Name = "Мнемосхема";
        _backgroundColor = Brush.Parse("#ff10aee2");
        MapColorString = _backgroundColor.ToString();
        Widgets = new List<ClassWidget>();
       
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
        try
        {
            using TextWriter writer = new StreamWriter(FileName, false);
            XmlSerializer serializer = new XmlSerializer(typeof(ClassMap));
            serializer.Serialize(writer, this);
        }
        catch (Exception e)
        {
            //TODO Окно ошибки.
            return false;
        }
        return true;
    }

    public static ClassMap Load(string FileName)
    {
        ClassMap map = null;
        using TextReader reader = new StreamReader(FileName);
        XmlSerializer serializer = new XmlSerializer(typeof(ClassMap));
        try
        {
            map = (ClassMap)serializer.Deserialize(reader);
        }
        catch (Exception Ex)
        {
            //TODO Окно ошибки.
            ClassLog.Write(Ex.Message);
        }
        if(map!=null) map.CurrentPathFile = FileName;
        return map;
    }

    public static List<ClassMap> LoadMapsFromFile(string dirName)
    {
        List<ClassMap> maps = new List<ClassMap>();
        if (Directory.Exists(dirName))
        {
            var files = Directory.GetFiles(dirName);
            if (files.Length == 0) return maps;
            foreach (var file in files)
            {
                maps.Add(Load(file));
            }
        }
        return maps;
    }

    public bool DeleteFileMap()
    {
        if (string.IsNullOrEmpty(CurrentPathFile)) return false;
        File.Delete(CurrentPathFile);
        return true;
    }

    /// <summary>
    /// Клонировать карту.
    /// </summary>
    /// <param name="map"></param>
    public void MapClone(ClassMap map)
    {
        GuidID = map.GuidID;
        ID = map.ID;
        MapColorString = map.MapColorString;
        Widgets.Clear();
        Widgets.AddRange(map.Widgets);
        Name = map.Name;
    }

    #region [PropertyChanged]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    #endregion
}