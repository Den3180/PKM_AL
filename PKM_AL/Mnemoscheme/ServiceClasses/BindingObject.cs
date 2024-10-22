namespace PKM_AL.Mnemoscheme.ServiceClasses;

public class BindingObject
{
    public string NameParam { get; set; }=string.Empty;
    public int IdParam { get; set; } = 0;
    
    public decimal? Max { get; set; }
    
    public decimal? Min { get; set; }
    public int IdDevice { get; set; } = 0;
}