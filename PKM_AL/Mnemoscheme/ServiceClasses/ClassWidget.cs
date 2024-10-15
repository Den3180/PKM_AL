using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Avalonia.Media;
using PKM_AL.Mnemoscheme.Enums;

namespace PKM_AL.Mnemoscheme.ServiceClasses;

public class ClassWidget
{
    public Guid GuidId { get; set; } = Guid.Empty;
    public EnumUnit UnitType { get; set; } = EnumUnit.NoneUnit;
    public bool IsDevicePoll { get; set; }=false;
    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; } = 0;
    public string FontStyleUnit { get; set; } = new FontFamily("Arial").Name;
    public FontWeight FontWeightUnit { get; set; }= FontWeight.Normal;
    public double FontSizeUnit { get; set; }
    public string TextUnit { get; set; } = string.Empty;
    public BindingObject? BindingObjectUnit { get; set; }
    public double ScaleUnit { get; set; } = 1D;
    public double HeightUnit { get; set; } = 1D;
    public double WidthUnit { get; set; } = 1D;
    public string FontBrushUnit { get; set; } = Colors.Black.ToString();
    public string BackgroundUnit { get; set; } = Colors.Azure.ToString();
    public bool IsBlocked { get; set; } = false;
    public ClassWidget Clone()
    {
        return new ClassWidget
        {
            GuidId = this.GuidId,
            UnitType = this.UnitType,
            IsDevicePoll = this.IsDevicePoll,
            PositionX = this.PositionX,
            PositionY = this.PositionY,
            FontStyleUnit = this.FontStyleUnit,
            FontWeightUnit = this.FontWeightUnit,
            FontSizeUnit = this.FontSizeUnit,
            TextUnit = this.TextUnit,
            BindingObjectUnit = this.BindingObjectUnit,
            ScaleUnit = this.ScaleUnit,
            HeightUnit = this.HeightUnit,
            WidthUnit = this.WidthUnit,
            FontBrushUnit = this.FontBrushUnit,
            BackgroundUnit = this.BackgroundUnit,
            IsBlocked = this.IsBlocked,
        };
        
    }
}
