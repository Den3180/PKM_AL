using CsvHelper.Configuration;

namespace PKM_AL.Classes.ServiceClasses;

public class ClassEventMap: ClassMap<ClassTransportEvent>
{
    public ClassEventMap()
    {
        Map(m => m.Id).Name("№");
        Map(m => m.Date).Name("Дата");
        Map(m => m.Device).Name("Устройство");
        Map(m => m.TypeEvent).Name("Тип события");
        Map(m => m.Param).Name("Параметр");
        Map(m => m.Value).Name("Значение");
    }
}