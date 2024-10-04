using PKM_AL.Mnemoscheme.Enums;
using PKM_AL.Mnemoscheme.ServiceClasses;

namespace PKM_AL.Mnemoscheme.AbstractUnit;

public interface IUnitService
{
    public EnumUnit GetTypeUnit();
   // public BindingObject GetBindingObject();
    public void SetValue(decimal value);
    public void SetFixUnit(bool fix);
}