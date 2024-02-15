using System;
using System.Collections.Generic;

namespace PKM_AL.Classes;

public class ClassGroupRequest
{
    private ClassChannel.EnumTypeRegistry _TypeRegistry;
    private List<ClassChannel> _Channels;

    public ClassChannel.EnumTypeRegistry TypeRegistry { get { return _TypeRegistry; } }
    public List<ClassChannel> Channels { get { return _Channels; } }

    public int StartAddress
    {
        get
        {
            if (_Channels.Count > 0) return _Channels[0].Address;
            else return 0;
        }
    }

    public ClassGroupRequest(ClassChannel.EnumTypeRegistry typeRegistry)
    {
        _TypeRegistry = typeRegistry;
        _Channels = new List<ClassChannel>();
    }

    public void AddChannel(ClassChannel channel)
    {
        _Channels.Add(channel);
    }
    
    /// <summary>
    /// Получение количества считываемых регистров.
    /// </summary>
    /// <returns></returns>
    public int GetSize()
    {
        if (_Channels.Count == 0) return 0;
        //Адрес последнего регистра минус адрес первого регистра в группе регистров.
        int Size = _Channels[^1].Address - _Channels[0].Address;
        switch (_Channels[^1].Format)
        {
            case ClassChannel.EnumFormat.UINT:
            case ClassChannel.EnumFormat.SINT:
                Size += 1;
                break;
            case ClassChannel.EnumFormat.Float:
            case ClassChannel.EnumFormat.swFloat:
            case ClassChannel.EnumFormat.UInt32:
                Size += 2;
                break;
        }
        return Size;
    }

    /// <summary>
    /// Получить адрес последнего регистра.
    /// </summary>
    /// <returns></returns>
    public int GetLastAddress()
    {
        if (_Channels.Count == 0) return Int32.MaxValue;
        return _Channels[^1].Address;
    }
    
    /// <summary>
    /// Отступ от начального(не всегда нулевого) адреса.
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public int GetOffset(int Index)
    {
        return _Channels[Index].Address - StartAddress;
    }

}