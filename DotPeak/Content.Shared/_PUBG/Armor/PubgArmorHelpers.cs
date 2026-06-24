// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Armor.PubgArmorHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared._PUBG.Armor;

public static class PubgArmorHelpers
{
  public static float GetDurabilityRatio(PubgArmorComponent component)
  {
    return (double) component.MaxDurability <= 0.0 ? 0.0f : Math.Clamp(component.Durability / component.MaxDurability, 0.0f, 1f);
  }

  public static Color GetDurabilityColor(float ratio)
  {
    Color color1 = new Color(1f, 0.0f, 0.0f, 1f);
    Color color2;
    // ISSUE: explicit constructor call
    ((Color) ref color2).\u002Ector(0.0f, 1f, 0.0f, 1f);
    Color color3 = color2;
    double num = (double) ratio;
    return Color.InterpolateBetween(color1, color3, (float) num);
  }
}
