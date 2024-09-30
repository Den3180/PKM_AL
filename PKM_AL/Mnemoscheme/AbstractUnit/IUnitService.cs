using PKM_AL.Mnemoscheme.Enums;

namespace PKM_AL.Mnemoscheme.AbstractUnit;

public interface IUnitService
{
    public EnumUnit GetTypeUnit();
    public void SetValue(decimal value);
}