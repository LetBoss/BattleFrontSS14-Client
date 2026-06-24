// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Shields.XenoShieldVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Shields;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Shields;

public sealed class XenoShieldVisualizerSystem : VisualizerSystem<XenoShieldComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    XenoShieldComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent spriteComponent;
    int num1;
    bool flag;
    if (!((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) RMCShieldVisuals.Base, ref num1, true) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) RMCShieldVisuals.Active, ref flag, (AppearanceComponent) null))
      return;
    if (!flag)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num1, false);
    }
    else
    {
      string str1;
      FixedPoint2 fixedPoint2_1;
      double num2;
      if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) RMCShieldVisuals.Prefix, ref str1, (AppearanceComponent) null) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<FixedPoint2>(uid, (Enum) RMCShieldVisuals.Current, ref fixedPoint2_1, (AppearanceComponent) null) || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<double>(uid, (Enum) RMCShieldVisuals.Max, ref num2, (AppearanceComponent) null))
        return;
      FixedPoint2 fixedPoint2_2 = fixedPoint2_1 / (FixedPoint2) num2;
      string str2 = str1 + "-";
      string str3 = !(fixedPoint2_2 > (FixedPoint2) 0.5) ? (!(fixedPoint2_2 > (FixedPoint2) 0.25) ? str2 + "quarter" : str2 + "half") : str2 + "full";
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num1, RSI.StateId.op_Implicit(str3));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num1, true);
    }
  }
}
