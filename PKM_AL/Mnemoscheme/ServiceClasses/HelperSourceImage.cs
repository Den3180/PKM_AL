using System.Collections.Generic;
using PKM_AL.Mnemoscheme.Enums;

namespace PKM_AL.Mnemoscheme.ServiceClasses;

public class HelperSourceImage
{
    public static Dictionary<EnumUnit, (string,string)> ListSourceImages = new Dictionary<EnumUnit, (string,string)>()
    {
        { EnumUnit.ButtonOffOn ,("knopka_Off_Red.png","knopka_On_Green.png")}
    }; 
}