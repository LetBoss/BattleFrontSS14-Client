// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Weapon.DropshipAmmoVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dropship.Weapon;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipAmmoVisualizerSystem : VisualizerSystem<DropshipAmmoComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    DropshipAmmoComponent component,
    ref AppearanceChangeEvent args)
  {
    base.OnAppearanceChange(uid, component, ref args);
    SpriteComponent sprite = args.Sprite;
    int num1;
    int num2;
    if (sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) DropshipAmmoVisuals.Fill, ref num1, args.Component) || !this.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) DropshipAmmoVisuals.Fill, ref num2, false) || component.AmmoType == null)
      return;
    int num3 = Math.Clamp(num1 / component.RoundsPerShot, 0, component.MaxRounds / component.RoundsPerShot);
    string str = $"{component.AmmoType}_{num3.ToString()}";
    this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, RSI.StateId.op_Implicit(str));
  }
}
