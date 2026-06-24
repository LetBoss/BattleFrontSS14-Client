// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.SkinColor
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable disable
namespace Content.Shared.Humanoid;

public static class SkinColor
{
  public const float MaxTintedHuesSaturation = 0.1f;
  public const float MinTintedHuesLightness = 0.85f;
  public const float MinHuesLightness = 0.175f;
  public const float MinFeathersHue = 0.08055556f;
  public const float MaxFeathersHue = 0.483333319f;
  public const float MinFeathersSaturation = 0.2f;
  public const float MaxFeathersSaturation = 0.88f;
  public const float MinFeathersValue = 0.36f;
  public const float MaxFeathersValue = 0.55f;

  public static Color ValidHumanSkinTone => Color.FromHsv(new Vector4(0.07f, 0.2f, 1f, 1f));

  public static Color ValidTintedHuesSkinTone(Color color) => SkinColor.TintedHues(color);

  public static Color HumanSkinTone(int tone)
  {
    tone = Math.Clamp(tone, 0, 100);
    int num1 = tone - 20;
    float num2 = 25f;
    float num3 = 20f;
    float num4 = 100f;
    if (num1 <= 0)
    {
      num2 += (float) Math.Abs(num1);
    }
    else
    {
      num3 += (float) num1;
      num4 -= (float) num1;
    }
    return Color.FromHsv(new Vector4(num2 / 360f, num3 / 100f, num4 / 100f, 1f));
  }

  public static float HumanSkinToneFromColor(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    return (double) Math.Clamp(hsv.X, 0.06944445f, 1f) > 0.0694444477558136 && (double) hsv.Z == 1.0 ? Math.Abs((float) (45.0 - (double) hsv.X * 360.0)) : hsv.Y * 100f;
  }

  public static bool VerifyHumanSkinTone(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    double num1 = Math.Round((double) hsv.X * 360.0);
    double num2 = Math.Round((double) hsv.Y * 100.0);
    double num3 = Math.Round((double) hsv.Z * 100.0);
    return num1 >= 25.0 && num1 <= 45.0 && num2 >= 20.0 && num3 >= 20.0;
  }

  public static Color TintedHues(Color color)
  {
    Vector4 hsl = Color.ToHsl(color);
    hsl.Y *= 0.1f;
    hsl.Z = MathHelper.Lerp(0.85f, 1f, hsl.Z);
    return Color.FromHsv(hsl);
  }

  public static bool VerifyTintedHues(Color color)
  {
    return (double) Color.ToHsl(color).Y <= 0.10000000149011612 && (double) Color.ToHsl(color).Z >= 0.85000002384185791;
  }

  public static Color ProportionalVoxColor(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    hsv.X = (float) ((double) hsv.X * 0.402777761220932 + 0.080555558204650879);
    hsv.Y = (float) ((double) hsv.Y * 0.68000000715255737 + 0.20000000298023224);
    hsv.Z = (float) ((double) hsv.Z * 0.18999999761581421 + 0.36000001430511475);
    return Color.FromHsv(hsv);
  }

  public static Color ClosestVoxColor(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    hsv.X = Math.Clamp(hsv.X, 0.08055556f, 0.483333319f);
    hsv.Y = Math.Clamp(hsv.Y, 0.2f, 0.88f);
    hsv.Z = Math.Clamp(hsv.Z, 0.36f, 0.55f);
    return Color.FromHsv(hsv);
  }

  public static bool VerifyVoxFeathers(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    return (double) hsv.X >= 0.080555558204650879 && (double) hsv.X <= 0.48333331942558289 && (double) hsv.Y >= 0.20000000298023224 && (double) hsv.Y <= 0.87999999523162842 && (double) hsv.Z >= 0.36000001430511475 && (double) hsv.Z <= 0.550000011920929;
  }

  public static Color MakeHueValid(Color color)
  {
    Vector4 hsv = Color.ToHsv(color);
    hsv.Z = Math.Max(hsv.Z, 0.175f);
    return Color.FromHsv(hsv);
  }

  public static bool VerifyHues(Color color)
  {
    return (double) Color.ToHsv(color).Z >= 0.17499999701976776;
  }

  public static bool VerifySkinColor(HumanoidSkinColor type, Color color)
  {
    bool flag;
    switch (type)
    {
      case HumanoidSkinColor.HumanToned:
        flag = SkinColor.VerifyHumanSkinTone(color);
        break;
      case HumanoidSkinColor.Hues:
        flag = SkinColor.VerifyHues(color);
        break;
      case HumanoidSkinColor.VoxFeathers:
        flag = SkinColor.VerifyVoxFeathers(color);
        break;
      case HumanoidSkinColor.TintedHues:
        flag = SkinColor.VerifyTintedHues(color);
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public static Color ValidSkinTone(HumanoidSkinColor type, Color color)
  {
    Color color1;
    switch (type)
    {
      case HumanoidSkinColor.HumanToned:
        color1 = SkinColor.ValidHumanSkinTone;
        break;
      case HumanoidSkinColor.Hues:
        color1 = SkinColor.MakeHueValid(color);
        break;
      case HumanoidSkinColor.VoxFeathers:
        color1 = SkinColor.ClosestVoxColor(color);
        break;
      case HumanoidSkinColor.TintedHues:
        color1 = SkinColor.ValidTintedHuesSkinTone(color);
        break;
      default:
        color1 = color;
        break;
    }
    return color1;
  }
}
