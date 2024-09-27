using System;
using System.Text.Json.Serialization;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.Enums;
using TestGrathic.ServiceClasses;

namespace PKM_AL.Mnemoscheme.ServiceClasses;

public class ClassWidget
{
    public Guid Id { get; set; } = Guid.Empty;

    public EnumUnit UnitType { get; set; } = EnumUnit.NoneUnit;
    
    public string ImageUnitPath { get; set; }=string.Empty;
    
    public double PositionX { get; set; } = 0;
    
    public double PositionY { get; set; } = 0;
    
    [JsonIgnore]
    public FontFamily FontStyleUnit { get; set; }
    public FontWeight FontWeightUnit { get; set; }
    public double FontSizeUnit { get; set; }
    public string TextUnit { get; set; } = string.Empty;
    public Color FontBrushUnit { get; set; }
    public BindingObject? BindingObjectUnit { get; set; }
    public double ScaleUnit { get; set; } = 1D;
    
    public double HeightUnit { get; set; } = 1D;
    
    public double WidthUnit { get; set; } = 1D;
}
