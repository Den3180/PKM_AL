using System.Collections.Generic;

namespace PKM_AL.Classes.ServiceClasses;

/// <summary>
/// Класс хранения данных для csv. События.
/// </summary>
public class ClassTransportEvent
{
    public string Id { get; set; }
    public string Date { get; set; }
    public string Device { get; set; }
    public string TypeEvent { get; set; }
    public string Param { get; set; }
    public string Value  { get; set; }
}