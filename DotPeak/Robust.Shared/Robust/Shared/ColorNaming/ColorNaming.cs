// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ColorNaming.ColorNaming
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Robust.Shared.ColorNaming;

public static class ColorNaming
{
  private static readonly (float Hue, string Loc)[] HueNames = new (float, string)[9]
  {
    (float.DegreesToRadians(0.0f), "color-pink"),
    (float.DegreesToRadians(15f), "color-red"),
    (float.DegreesToRadians(45f), "color-orange"),
    (float.DegreesToRadians(90f), "color-yellow"),
    (float.DegreesToRadians(135f), "color-green"),
    (float.DegreesToRadians(180f), "color-cyan"),
    (float.DegreesToRadians(240f), "color-blue"),
    (float.DegreesToRadians(285f), "color-purple"),
    (float.DegreesToRadians(330f), "color-pink")
  };
  private static readonly (float Hue, string Loc) HueFallback = (float.DegreesToRadians(361f), "color-pink");
  private const float BrownLightnessThreshold = 0.675f;
  private static readonly LocId OrangeString = (LocId) "color-orange";
  private static readonly LocId BrownString = (LocId) "color-brown";
  private const float VeryDarkLightnessThreshold = 0.25f;
  private const float DarkLightnessThreshold = 0.5f;
  private const float NeutralLightnessThreshold = 0.7f;
  private const float LightLightnessThreshold = 0.85f;
  private static readonly LocId VeryDarkString = (LocId) "color-very-dark";
  private static readonly LocId DarkString = (LocId) "color-dark";
  private static readonly LocId LightString = (LocId) "color-light";
  private static readonly LocId VeryLightString = (LocId) "color-very-light";
  private static readonly LocId MixedHueString = (LocId) "color-mixed-hue";
  private static readonly LocId LightLowChromaString = (LocId) "color-pale";
  private static readonly LocId DarkLowChromaString = (LocId) "color-gray-adjective";
  private static readonly LocId HighChromaString = (LocId) "color-strong";
  private static readonly LocId WhiteString = (LocId) "color-white";
  private static readonly LocId GrayString = (LocId) "color-gray";
  private static readonly LocId BlackString = (LocId) "color-black";
  private const float LowChromaThreshold = 0.07f;
  private const float HighChromaThreshold = 0.16f;
  private const float LightLowChromaThreshold = 0.6f;
  private const float WhiteLightnessThreshold = 0.99f;
  private const float BlackLightnessThreshold = 0.01f;
  private const float GrayChromaThreshold = 0.01f;

  private static (string Loc, float AdjustedLightness) DescribeHue(
    Vector4 oklch,
    ILocalizationManager localization)
  {
    float num1;
    float num2;
    float num3;
    float num4;
    VectorHelpers.Deconstruct(oklch, ref num1, ref num2, ref num3, ref num4);
    float num5 = num1;
    float num6 = num3;
    for (int index = 0; index < Robust.Shared.ColorNaming.ColorNaming.HueNames.Length; ++index)
    {
      (float Hue, string Loc) hueName = Robust.Shared.ColorNaming.ColorNaming.HueNames[index];
      (float, string) valueTuple = index + 1 < Robust.Shared.ColorNaming.ColorNaming.HueNames.Length ? Robust.Shared.ColorNaming.ColorNaming.HueNames[index + 1] : Robust.Shared.ColorNaming.ColorNaming.HueFallback;
      if ((double) hueName.Hue <= (double) num6 && (double) num6 < (double) valueTuple.Item1)
      {
        string messageId = hueName.Loc;
        float num7 = num5;
        if ((LocId) hueName.Loc == Robust.Shared.ColorNaming.ColorNaming.OrangeString && (double) num5 <= 0.675000011920929)
          messageId = (string) Robust.Shared.ColorNaming.ColorNaming.BrownString;
        else if ((LocId) hueName.Loc == Robust.Shared.ColorNaming.ColorNaming.OrangeString)
          num7 = (float) ((double) num5 - 0.675000011920929 + 0.5);
        if ((double) num6 < ((double) hueName.Hue + (double) valueTuple.Item1) / 2.0 || !(hueName.Loc != valueTuple.Item2))
          return (localization.GetString(messageId), num7);
        string str;
        return localization.TryGetString($"{messageId}-{valueTuple.Item2}", out str) ? (str, num7) : (localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.MixedHueString, ("a", (object) localization.GetString(messageId)), ("b", (object) localization.GetString(valueTuple.Item2))), num7);
      }
    }
    return (localization.GetString("color-unknown"), num5);
  }

  private static string? DescribeChroma(Vector4 oklch, ILocalizationManager localization)
  {
    float num1;
    float num2;
    float num3;
    float num4;
    VectorHelpers.Deconstruct(oklch, ref num1, ref num2, ref num3, ref num4);
    float num5 = num1;
    float num6 = num2;
    return (double) num6 <= 0.070000000298023224 ? ((double) num5 >= 0.60000002384185791 ? localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.LightLowChromaString) : localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.DarkLowChromaString)) : ((double) num6 >= 0.15999999642372131 ? localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.HighChromaString) : (string) null);
  }

  private static string? DescribeLightness(Vector4 oklch, ILocalizationManager localization)
  {
    float x = oklch.X;
    return (double) x < 0.25 ? localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.VeryDarkString) : ((double) x < 0.5 ? localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.DarkString) : ((double) x < 0.699999988079071 ? (string) null : ((double) x < 0.85000002384185791 ? localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.LightString) : localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.VeryLightString))));
  }

  public static string Describe(Color srgb, ILocalizationManager localization)
  {
    Vector4 lch = Color.ToLch(Color.ToLab(Color.FromSrgb(srgb)));
    if ((double) lch.X >= 0.99000000953674316)
      return localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.WhiteString);
    if ((double) lch.X <= 0.0099999997764825821)
      return localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.BlackString);
    (string Loc, float AdjustedLightness) = Robust.Shared.ColorNaming.ColorNaming.DescribeHue(lch, localization);
    lch.X = AdjustedLightness;
    string str1 = Robust.Shared.ColorNaming.ColorNaming.DescribeChroma(lch, localization);
    string str2 = Robust.Shared.ColorNaming.ColorNaming.DescribeLightness(lch, localization);
    if ((double) lch.Y <= 0.0099999997764825821)
    {
      Loc = localization.GetString((string) Robust.Shared.ColorNaming.ColorNaming.GrayString);
      str1 = (string) null;
    }
    string str3 = Loc;
    string str4 = str1;
    string str5 = str2;
    string str6;
    if (str3 != null)
    {
      if (str4 != null)
      {
        if (str5 != null)
        {
          str6 = localization.GetString("color-hue-chroma-lightness", ("hue", (object) str3), ("chroma", (object) str4), ("lightness", (object) str5));
        }
        else
        {
          string str7 = str3;
          string str8 = str4;
          str6 = localization.GetString("color-hue-chroma", ("hue", (object) str7), ("chroma", (object) str8));
        }
      }
      else if (str5 != null)
      {
        string str9 = str3;
        string str10 = str5;
        str6 = localization.GetString("color-hue-lightness", ("hue", (object) str9), ("lightness", (object) str10));
      }
      else
        str6 = str3;
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      \u003CPrivateImplementationDetails\u003E.ThrowSwitchExpressionException((object) (str3, str4, str5));
    }
    return str6;
  }
}
