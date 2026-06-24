// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Settings.PubgGeneralAutoSettings
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.Settings;

public static class PubgGeneralAutoSettings
{
  public const int Version = 3;
  private static readonly Vector2i ReferenceResolution = new Vector2i(1920, 1080);
  private static readonly Vector2i FullHdResolution = new Vector2i(1920, 1080);
  private static readonly Vector2i QhdResolution = new Vector2i(2560 /*0x0A00*/, 1440);
  private const int ReferenceMinimapWidgetSize = 200;
  private const int ReferenceMinimapOffsetX = 0;
  private const int ReferenceMinimapOffsetY = 0;
  private const int ReferenceMinimapMarkerScale = 100;

  public static void Apply(IConfigurationManager cfg, Vector2i screenSize, int appliedVersion)
  {
    if (appliedVersion < 1)
      PubgGeneralAutoSettings.ApplyMinimapSettings(cfg, screenSize);
    if (appliedVersion < 2)
      PubgGeneralAutoSettings.ApplyHealthHudSettings(cfg, screenSize);
    if (appliedVersion >= 3)
      return;
    PubgGeneralAutoSettings.ApplyBattlefrontLayout(cfg);
  }

  private static void ApplyBattlefrontLayout(IConfigurationManager cfg)
  {
    cfg.SetCVar<string>(CCVars.UILayout, "Battlefront", false);
  }

  private static void ApplyMinimapSettings(IConfigurationManager cfg, Vector2i screenSize)
  {
    float x = (float) screenSize.X / (float) PubgGeneralAutoSettings.ReferenceResolution.X;
    float y = (float) screenSize.Y / (float) PubgGeneralAutoSettings.ReferenceResolution.Y;
    float num = MathF.Min(x, y);
    cfg.SetCVar<int>(CCVars.PubgMinimapWidgetSize, Math.Clamp((int) MathF.Round(200f * num), 120, 500), false);
    cfg.SetCVar<int>(CCVars.PubgMinimapOffsetX, Math.Clamp((int) MathF.Round(0.0f * x), -600, 600), false);
    cfg.SetCVar<int>(CCVars.PubgMinimapOffsetY, Math.Clamp((int) MathF.Round(0.0f * y), -600, 600), false);
    cfg.SetCVar<int>(CCVars.PubgMinimapMarkerScale, Math.Clamp((int) MathF.Round(100f * num), 50, 250), false);
  }

  private static void ApplyHealthHudSettings(IConfigurationManager cfg, Vector2i screenSize)
  {
    if (screenSize.X >= PubgGeneralAutoSettings.QhdResolution.X && screenSize.Y >= PubgGeneralAutoSettings.QhdResolution.Y)
    {
      cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetY, 112 /*0x70*/, false);
      cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetX, -1000, false);
    }
    else
    {
      if (screenSize.X < PubgGeneralAutoSettings.FullHdResolution.X || screenSize.Y < PubgGeneralAutoSettings.FullHdResolution.Y)
        return;
      cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetY, -14, false);
      cfg.SetCVar<int>(CCVars.PubgHealthHudOffsetX, -492, false);
    }
  }
}
