using System;

namespace PKM_AL.Classes;

public class ClassLink : MyPropertyChanged
{
    private ClassEvent.EnumType _EventType;
    private int _SourceID;
    private string _SourceName;
    private ClassCommand _Command;
    private DateTime _DT;

    public int ID { get; set; }

    public ClassEvent.EnumType EventType
    {
        get { return _EventType; }
        set
        {
            _EventType = value;
            OnPropertyChanged("EventType");
            OnPropertyChanged("EventTypeName");
        }
    }

    public int SourceID
    {
        get { return _SourceID; }
        set
        {
            _SourceID = value;
            OnPropertyChanged("SourceID");
        }
    }

    public string SourceName
    {
        get { return _SourceName; }
        set
        {
            _SourceName = value;
            OnPropertyChanged("SourceName");
        }
    }

    public ClassCommand Command
    {
        get { return _Command; }
        set
        {
            _Command = value;
            OnPropertyChanged("CommandName");
        }
    }

    public DateTime DT
    {
        get { return _DT; }
        set
        {
            _DT = value;
            OnPropertyChanged("DT");
            OnPropertyChanged("StrDT");
        }
    }

    public string EventTypeName { get { return ClassEvent.GetEventName(_EventType); } }
    public string CommandName { get { return _Command.Name; } }
    public string StrDT
    {
        get
        {
            if (_DT == DateTime.MinValue) return "";
            else return DT.ToString("dd.MM.yyyy HH:mm:ss.fff");
        }
    }

    public ClassLink()
    {
        _EventType = ClassEvent.EnumType.Over;
        _SourceID = 0;
        _SourceName = "";
        _Command = new ClassCommand();
        _DT = DateTime.MinValue;
    }
}