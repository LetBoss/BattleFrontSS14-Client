// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunMuzzleOffsetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunMuzzleOffsetSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunMuzzleOffsetComponent, AttemptShootEvent>(new EntityEventRefHandler<GunMuzzleOffsetComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<GunMuzzleOffsetComponent, RMCBeforeMuzzleFlashEvent>(new EntityEventRefHandler<GunMuzzleOffsetComponent, RMCBeforeMuzzleFlashEvent>(this.OnBeforeMuzzleFlash), after: new Type[1]
    {
      typeof (MountableWeaponSystem)
    });
  }

  private void OnAttemptShoot(Entity<GunMuzzleOffsetComponent> ent, ref AttemptShootEvent args)
  {
    EntityCoordinates muzzleCoords;
    if (args.Cancelled || !this.TryGetMuzzleCoordinates(ent, args.ToCoordinates, out muzzleCoords, out Angle _))
      return;
    args.FromCoordinates = muzzleCoords;
  }

  private void OnBeforeMuzzleFlash(
    Entity<GunMuzzleOffsetComponent> ent,
    ref RMCBeforeMuzzleFlashEvent args)
  {
    if (!ent.Comp.ApplyToMuzzleFlash)
      return;
    EntityCoordinates? toCoordinates = new EntityCoordinates?();
    GunComponent comp;
    if (this.TryComp<GunComponent>((EntityUid) ent, out comp))
      toCoordinates = comp.ShootCoordinates;
    EntityCoordinates muzzleCoords;
    Angle muzzleRotation;
    if (!this.TryGetMuzzleCoordinates(ent, toCoordinates, out muzzleCoords, out muzzleRotation))
      return;
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(muzzleCoords);
    MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(args.Weapon);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return;
    Vector2 vector2_1 = mapCoordinates1.Position - mapCoordinates2.Position;
    Angle worldRotation = this._transform.GetWorldRotation(args.Weapon);
    ref RMCBeforeMuzzleFlashEvent local = ref args;
    muzzleRotation = Angle.op_UnaryNegation(worldRotation);
    Vector2 vector2_2 = ((Angle) ref muzzleRotation).RotateVec(ref vector2_1);
    local.Offset = vector2_2;
  }

  private bool TryGetMuzzleCoordinates(
    Entity<GunMuzzleOffsetComponent> ent,
    EntityCoordinates? toCoordinates,
    out EntityCoordinates muzzleCoords,
    out Angle muzzleRotation)
  {
    muzzleCoords = new EntityCoordinates();
    muzzleRotation = Angle.Zero;
    if (ent.Comp.Offset == Vector2.Zero && ent.Comp.MuzzleOffset == Vector2.Zero && !ent.Comp.UseDirectionalOffsets)
      return false;
    EntityUid owner = ent.Owner;
    BaseContainer container;
    if (ent.Comp.UseContainerOwner && this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (ent.Owner, (TransformComponent) null), out container))
      owner = container.Owner;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(owner);
    Angle baseRotation = this.GetBaseRotation(owner, ent.Comp.AngleOffset);
    muzzleRotation = baseRotation;
    (Vector2 vector2_1, bool Rotate) = this.GetOffset(ent.Comp, owner, baseRotation);
    muzzleCoords = Rotate ? moverCoordinates.Offset(((Angle) ref baseRotation).RotateVec(ref vector2_1)) : moverCoordinates.Offset(vector2_1);
    if (ent.Comp.MuzzleOffset == Vector2.Zero)
      return true;
    if (ent.Comp.UseAimDirection && toCoordinates.HasValue && this._transform.IsValid(muzzleCoords) && this._transform.IsValid(toCoordinates.Value))
    {
      MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(muzzleCoords);
      MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(toCoordinates.Value);
      if (mapCoordinates1.MapId == mapCoordinates2.MapId)
      {
        Vector2 vector2_2 = mapCoordinates2.Position - mapCoordinates1.Position;
        if ((double) vector2_2.LengthSquared() > 9.9999997473787516E-05)
          muzzleRotation = Angle.op_Addition(DirectionExtensions.ToWorldAngle(vector2_2), ent.Comp.AngleOffset);
      }
    }
    muzzleCoords = muzzleCoords.Offset(((Angle) ref muzzleRotation).RotateVec(ref ent.Comp.MuzzleOffset));
    return true;
  }

  public bool TryGetMuzzleCoordinates(
    EntityUid weaponUid,
    GunMuzzleOffsetComponent gunMuzzle,
    EntityCoordinates? toCoordinates,
    out EntityCoordinates muzzleCoords,
    out Angle muzzleRotation)
  {
    return this.TryGetMuzzleCoordinates((Entity<GunMuzzleOffsetComponent>) (weaponUid, gunMuzzle), toCoordinates, out muzzleCoords, out muzzleRotation);
  }

  private Angle GetBaseRotation(EntityUid baseUid, Angle angleOffset)
  {
    Angle angle = this._transform.GetWorldRotation(baseUid);
    GridVehicleMoverComponent comp;
    if (this.TryComp<GridVehicleMoverComponent>(baseUid, out comp) && Vector2i.op_Inequality(comp.CurrentDirection, Vector2i.Zero))
      angle = DirectionExtensions.ToWorldAngle(new Vector2((float) comp.CurrentDirection.X, (float) comp.CurrentDirection.Y));
    return Angle.op_Addition(angle, angleOffset);
  }

  private (Vector2 Offset, bool Rotate) GetOffset(
    GunMuzzleOffsetComponent muzzle,
    EntityUid baseUid,
    Angle baseRotation)
  {
    if (!muzzle.UseDirectionalOffsets)
      return (muzzle.Offset, true);
    Vector2 vector2;
    switch ((int) this.GetBaseDirection(baseUid, baseRotation))
    {
      case 0:
        vector2 = muzzle.OffsetSouth;
        break;
      case 2:
        vector2 = muzzle.OffsetEast;
        break;
      case 4:
        vector2 = muzzle.OffsetNorth;
        break;
      case 6:
        vector2 = muzzle.OffsetWest;
        break;
      default:
        vector2 = muzzle.Offset;
        break;
    }
    return (vector2, muzzle.RotateDirectionalOffsets);
  }

  private Direction GetBaseDirection(EntityUid baseUid, Angle baseRotation)
  {
    GridVehicleMoverComponent comp;
    return this.TryComp<GridVehicleMoverComponent>(baseUid, out comp) && Vector2i.op_Inequality(comp.CurrentDirection, Vector2i.Zero) ? DirectionExtensions.AsDirection(comp.CurrentDirection) : VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation);
  }
}
