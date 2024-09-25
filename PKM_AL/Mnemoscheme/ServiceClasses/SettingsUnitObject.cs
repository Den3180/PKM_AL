using Avalonia.Media;

namespace TestGrathic.ServiceClasses;

public class SettingsUnitObject
{
    public FontFamily? FontStyleUnit { get; set; }
    public FontWeight FontWeightUnit { get; set; }
    public double FontSizeUnit { get; set; }
    public string? TextUnit { get; set; }
    public Color FontBrushUnit { get; set; }
    public BindingObject? BindingObjectUnit { get; set; }
    public double ScaleUnit { get; set; } = 1D;
    
    public double HeightUnit { get; set; } = 1D;
    
    public double WidthUnit { get; set; } = 1D;
}
