// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emplacements.MountableWeaponSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Overheat;
using Content.Shared.Foldable;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Emplacements;

public sealed class MountableWeaponSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedWeaponMountSystem _weaponMount;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MountableWeaponComponent, AttemptShootEvent>(new EntityEventRefHandler<MountableWeaponComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<MountableWeaponComponent, TakeAmmoEvent>(new EntityEventRefHandler<MountableWeaponComponent, TakeAmmoEvent>(this.OnTakeAmmo));
    this.SubscribeLocalEvent<MountableWeaponComponent, OverheatedEvent>(new EntityEventRefHandler<MountableWeaponComponent, OverheatedEvent>(this.OnOverheated));
    this.SubscribeLocalEvent<MountableWeaponComponent, HeatGainedEvent>(new EntityEventRefHandler<MountableWeaponComponent, HeatGainedEvent>(this.OnHeatGained));
    this.SubscribeLocalEvent<MountableWeaponComponent, RMCBeforeMuzzleFlashEvent>(new EntityEventRefHandler<MountableWeaponComponent, RMCBeforeMuzzleFlashEvent>(this.OnBeforeMuzzleFlash));
  }

  private void OnAttemptShoot(Entity<MountableWeaponComponent> ent, ref AttemptShootEvent args)
  {
    if (!args.ToCoordinates.HasValue)
      return;
    if (ent.Comp.RequiresMount && !ent.Comp.MountedTo.HasValue)
    {
      args.Cancelled = true;
    }
    else
    {
      if (!ent.Comp.MountedTo.HasValue)
        return;
      EntityUid entity = this.GetEntity(ent.Comp.MountedTo.Value);
      Vector2 worldPosition = this._transform.GetWorldPosition((EntityUid) ent);
      Angle angle1 = Angle.FromWorldVec(this._transform.ToWorldPosition(args.ToCoordinates.Value) - worldPosition);
      Angle worldRotation = this._transform.GetWorldRotation(entity);
      Angle angle2 = Angle.ShortestDistance(ref worldRotation, ref angle1);
      if (Math.Abs(((Angle) ref angle2).Degrees) <= (double) ent.Comp.ShootArc / 2.0)
        return;
      args.Cancelled = true;
    }
  }

  private void OnTakeAmmo(Entity<MountableWeaponComponent> ent, ref TakeAmmoEvent args)
  {
    int? ammoCount;
    int? ammoCapacity;
    if (!ent.Comp.MountedTo.HasValue || !this._weaponMount.TryGetWeaponAmmo((EntityUid) ent, out ammoCount, out ammoCapacity))
      return;
    WeaponMountComponentVisualLayers componentVisualLayers = WeaponMountComponentVisualLayers.MountedAmmo;
    FoldableComponent comp;
    if (this.TryComp<FoldableComponent>((EntityUid) ent, out comp) && comp.IsFolded)
      componentVisualLayers = WeaponMountComponentVisualLayers.FoldedAmmo;
    SharedAppearanceSystem appearance = this._appearance;
    EntityUid entity = this.GetEntity(ent.Comp.MountedTo.Value);
    // ISSUE: variable of a boxed type
    __Boxed<WeaponMountComponentVisualLayers> key = (Enum) componentVisualLayers;
    int? nullable = ammoCount;
    ammoCapacity = nullable.HasValue ? new int?(nullable.GetValueOrDefault() - 1) : new int?();
    int num = 0;
    // ISSUE: variable of a boxed type
    __Boxed<bool> local = (ValueType) (ammoCapacity.GetValueOrDefault() > num & ammoCapacity.HasValue);
    appearance.SetData(entity, (Enum) key, (object) local);
  }

  private void OnOverheated(Entity<MountableWeaponComponent> ent, ref OverheatedEvent args)
  {
    if (!ent.Comp.MountedTo.HasValue)
      return;
    MountableWeaponRelayedEvent<OverheatedEvent> args1 = new MountableWeaponRelayedEvent<OverheatedEvent>(args);
    this.RaiseLocalEvent<MountableWeaponRelayedEvent<OverheatedEvent>>(this.GetEntity(ent.Comp.MountedTo.Value), ref args1);
  }

  private void OnHeatGained(Entity<MountableWeaponComponent> ent, ref HeatGainedEvent args)
  {
    if (!ent.Comp.MountedTo.HasValue)
      return;
    MountableWeaponRelayedEvent<HeatGainedEvent> args1 = new MountableWeaponRelayedEvent<HeatGainedEvent>(args);
    this.RaiseLocalEvent<MountableWeaponRelayedEvent<HeatGainedEvent>>(this.GetEntity(ent.Comp.MountedTo.Value), ref args1);
  }

  private void OnBeforeMuzzleFlash(
    Entity<MountableWeaponComponent> ent,
    ref RMCBeforeMuzzleFlashEvent args)
  {
    if (!ent.Comp.MountedTo.HasValue)
      return;
    args.Weapon = this.GetEntity(ent.Comp.MountedTo.Value);
  }

  public bool TryGetWeaponMount(
    EntityUid weapon,
    [NotNullWhen(true)] out EntityUid? mountEntity,
    MountableWeaponComponent? mountable = null)
  {
    mountEntity = new EntityUid?();
    if (!this.Resolve<MountableWeaponComponent>(weapon, ref mountable, false) || !mountable.MountedTo.HasValue)
      return false;
    mountEntity = new EntityUid?(this.GetEntity(mountable.MountedTo.Value));
    return true;
  }
}
