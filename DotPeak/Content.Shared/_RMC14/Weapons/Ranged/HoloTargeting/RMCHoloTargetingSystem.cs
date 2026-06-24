// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.HoloTargeting.RMCHoloTargetingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

public sealed class RMCHoloTargetingSystem : EntitySystem
{
  [Dependency]
  private RMCHoloTargetedSystem _holoTargeted;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HoloTargetingComponent, ProjectileHitEvent>(new ComponentEventRefHandler<HoloTargetingComponent, ProjectileHitEvent>(this.OnProjectileHit));
  }

  private void OnProjectileHit(
    EntityUid uid,
    HoloTargetingComponent component,
    ref ProjectileHitEvent args)
  {
    this._holoTargeted.ApplyHoloStacks(args.Target, component.Decay, component.Stacks, component.MaxStacks);
  }
}
