// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.ProgressColorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems;

public sealed class ProgressColorSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _configuration;
  private bool _colorBlindFriendly;
  private static readonly Color[] Plasma = new Color[5]
  {
    new Color((byte) 240 /*0xF0*/, (byte) 249, (byte) 33, byte.MaxValue),
    new Color((byte) 248, (byte) 149, (byte) 64 /*0x40*/, byte.MaxValue),
    new Color((byte) 204, (byte) 71, (byte) 120, byte.MaxValue),
    new Color((byte) 126, (byte) 3, (byte) 168, byte.MaxValue),
    new Color((byte) 13, (byte) 8, (byte) 135, byte.MaxValue)
  };

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configuration, CCVars.AccessibilityColorblindFriendly, new CVarChanged<bool>((object) this, __methodptr(OnColorBlindFriendlyChanged)), true);
  }

  private void OnColorBlindFriendlyChanged(bool value, in CVarChangeInfo info)
  {
    this._colorBlindFriendly = value;
  }

  public Color GetProgressColor(float progress)
  {
    if (this._colorBlindFriendly)
      return ProgressColorSystem.InterpolateColorGaussian(ProgressColorSystem.Plasma, (double) progress);
    return (double) progress >= 1.0 ? new Color(0.0f, 1f, 0.0f, 1f) : Color.FromHsv(new Vector4(0.2777778f * progress, 1f, 0.75f, 1f));
  }

  public static Color InterpolateColorGaussian(Color[] colors, double x)
  {
    double num1 = 0.0;
    double num2 = 0.0;
    double num3 = 0.0;
    float num4 = 0.0f;
    double num5 = 1.0 / (double) (colors.Length - 1);
    double num6 = 0.0;
    foreach (Color color in colors)
    {
      double num7 = Math.Exp(-(x - num6) * (x - num6) / 0.07) / Math.Sqrt(7.0 * Math.PI / 100.0);
      num4 += (float) num7;
      num6 += num5;
      num1 += (double) color.R * num7;
      num2 += (double) color.G * num7;
      num3 += (double) color.B * num7;
    }
    return new Color((float) num1 / num4, (float) num2 / num4, (float) num3 / num4, 1f);
  }
}
