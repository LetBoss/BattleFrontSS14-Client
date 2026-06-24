// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.GunSpreadModifierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class GunSpreadModifierSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GunSpreadModifierComponent, GunGetAmmoSpreadEvent>(new ComponentEventRefHandler<GunSpreadModifierComponent, GunGetAmmoSpreadEvent>(this.OnGunGetAmmoSpread));
    this.SubscribeLocalEvent<GunSpreadModifierComponent, ExaminedEvent>(new ComponentEventHandler<GunSpreadModifierComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnGunGetAmmoSpread(
    EntityUid uid,
    GunSpreadModifierComponent comp,
    ref GunGetAmmoSpreadEvent args)
  {
    ref GunGetAmmoSpreadEvent local = ref args;
    local.Spread = Angle.op_Implicit(Angle.op_Implicit(local.Spread) * (double) comp.Spread);
  }

  private void OnExamine(EntityUid uid, GunSpreadModifierComponent comp, ExaminedEvent args)
  {
    double num = Math.Round((double) comp.Spread * 100.0);
    string markup = this.Loc.GetString(num < 100.0 ? "examine-gun-spread-modifier-reduction" : "examine-gun-spread-modifier-increase", ("percentage", (object) (num < 100.0 ? 100.0 - num : num - 100.0)));
    args.PushMarkup(markup);
  }
}
