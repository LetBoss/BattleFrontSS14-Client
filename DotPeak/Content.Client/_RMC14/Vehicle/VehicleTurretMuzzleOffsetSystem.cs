// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleTurretMuzzleOffsetSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretMuzzleOffsetSystem : EntitySystem
{
  [Dependency]
  private readonly GunMuzzleOffsetSystem _gunMuzzleOffset;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly VehicleTurretMuzzleSystem _turretMuzzle;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleWeaponsOperatorComponent, BeforeAttemptShootEvent>(new EntityEventRefHandler<VehicleWeaponsOperatorComponent, BeforeAttemptShootEvent>((object) this, __methodptr(OnBeforeAttemptShoot)), (Type[]) null, (Type[]) null);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    EntityQueryEnumerator<VehicleTurretTrackedMuzzleFlashComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleTurretTrackedMuzzleFlashComponent, TransformComponent>();
    EntityUid entityUid;
    VehicleTurretTrackedMuzzleFlashComponent muzzleFlashComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref muzzleFlashComponent, ref transformComponent))
    {
      EntityCoordinates origin;
      Angle rotation;
      if (!this.TerminatingOrDeleted(muzzleFlashComponent.Weapon, (MetaDataComponent) null) && this.TryGetGunPose(muzzleFlashComponent.Weapon, new EntityCoordinates?(), out origin, out rotation))
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin, true);
        transformComponent.ActivelyLerping = false;
        Angle angle1 = Angle.op_Addition(rotation, muzzleFlashComponent.RotationOffset);
        Angle angle2 = ((Angle) ref angle1).Reduced();
        this._transform.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit((entityUid, transformComponent)), angle2);
        this._transform.SetWorldPosition(Entity<TransformComponent>.op_Implicit((entityUid, transformComponent)), mapCoordinates.Position + ((Angle) ref angle2).RotateVec(ref muzzleFlashComponent.Offset));
      }
    }
  }

  public bool TryGetGunOrigin(
    EntityUid weaponUid,
    EntityCoordinates? target,
    out EntityCoordinates origin)
  {
    return this.TryGetGunPose(weaponUid, target, out origin, out Angle _);
  }

  public bool TryGetGunPose(
    EntityUid weaponUid,
    EntityCoordinates? target,
    out EntityCoordinates origin,
    out Angle rotation)
  {
    origin = new EntityCoordinates();
    rotation = Angle.Zero;
    VehicleTurretComponent vehicleTurretComponent;
    if (!this.TryComp<VehicleTurretComponent>(weaponUid, ref vehicleTurretComponent))
      return false;
    origin = this._transform.GetMoverCoordinates(weaponUid);
    rotation = this._transform.GetWorldRotation(weaponUid);
    EntityCoordinates? toCoordinates = target;
    GunComponent gunComponent;
    if (!toCoordinates.HasValue && this.TryComp<GunComponent>(weaponUid, ref gunComponent))
    {
      EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
      if (shootCoordinates.HasValue)
        toCoordinates = new EntityCoordinates?(shootCoordinates.GetValueOrDefault());
    }
    GunMuzzleOffsetComponent gunMuzzle;
    EntityCoordinates muzzleCoords;
    Angle muzzleRotation;
    if (this.TryComp<GunMuzzleOffsetComponent>(weaponUid, ref gunMuzzle) && this._gunMuzzleOffset.TryGetMuzzleCoordinates(weaponUid, gunMuzzle, toCoordinates, out muzzleCoords, out muzzleRotation))
    {
      origin = muzzleCoords;
      rotation = muzzleRotation;
    }
    VehicleTurretMuzzleComponent muzzle;
    if (this.TryComp<VehicleTurretMuzzleComponent>(weaponUid, ref muzzle))
      origin = this._turretMuzzle.GetMuzzleCoordinates(weaponUid, muzzle, origin);
    return true;
  }

  private void OnBeforeAttemptShoot(
    Entity<VehicleWeaponsOperatorComponent> ent,
    ref BeforeAttemptShootEvent args)
  {
    EntityUid? selectedWeapon = ent.Comp.SelectedWeapon;
    EntityCoordinates origin;
    if (!selectedWeapon.HasValue || !this.TryGetGunOrigin(selectedWeapon.GetValueOrDefault(), new EntityCoordinates?(), out origin))
      return;
    args.Origin = origin;
    args.Handled = true;
  }
}
