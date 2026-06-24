// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Systems.SharedGeigerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Radiation.Systems;

public abstract class SharedGeigerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GeigerComponent, ExaminedEvent>(new ComponentEventHandler<GeigerComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnExamine(EntityUid uid, GeigerComponent component, ExaminedEvent args)
  {
    if (!component.ShowExamine || !component.IsEnabled || !args.IsInDetailsRange)
      return;
    string markup = this.Loc.GetString("geiger-component-examine", ("rads", (object) component.CurrentRadiation.ToString("N1")), ("color", (object) SharedGeigerSystem.LevelToColor(component.DangerLevel)));
    args.PushMarkup(markup);
  }

  public static Color LevelToColor(GeigerDangerLevel level)
  {
    switch (level)
    {
      case GeigerDangerLevel.None:
        return Color.Green;
      case GeigerDangerLevel.Low:
        return Color.Yellow;
      case GeigerDangerLevel.Med:
        return Color.DarkOrange;
      case GeigerDangerLevel.High:
      case GeigerDangerLevel.Extreme:
        return Color.Red;
      default:
        return Color.White;
    }
  }
}
