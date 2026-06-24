// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretMuzzleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTurretMuzzleSystem : EntitySystem
{
  [Dependency]
  private readonly SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VehicleTurretMuzzleComponent, AttemptShootEvent>(new EntityEventRefHandler<VehicleTurretMuzzleComponent, AttemptShootEvent>(this.OnAttemptShoot), after: new Type[1]
    {
      typeof (GunMuzzleOffsetSystem)
    });
    this.SubscribeLocalEvent<VehicleTurretMuzzleComponent, GunShotEvent>(new EntityEventRefHandler<VehicleTurretMuzzleComponent, GunShotEvent>(this.OnGunShot));
  }

  private void OnAttemptShoot(Entity<VehicleTurretMuzzleComponent> ent, ref AttemptShootEvent args)
  {
    if (args.Cancelled)
      return;
    args.FromCoordinates = this.GetMuzzleCoordinates(ent.Owner, ent.Comp, args.FromCoordinates);
  }

  private void OnGunShot(Entity<VehicleTurretMuzzleComponent> ent, ref GunShotEvent args)
  {
    if (!ent.Comp.Alternate || args.Ammo.Count == 0)
      return;
    for (int index = 0; index < args.Ammo.Count; ++index)
      ent.Comp.UseRightNext = !ent.Comp.UseRightNext;
    this.Dirty<VehicleTurretMuzzleComponent>(ent);
  }

  public EntityCoordinates GetMuzzleCoordinates(
    EntityUid uid,
    VehicleTurretMuzzleComponent muzzle,
    EntityCoordinates baseCoords)
  {
    Vector2 worldOffset = this.GetWorldOffset(uid, muzzle);
    return !(worldOffset == Vector2.Zero) ? baseCoords.Offset(worldOffset) : baseCoords;
  }

  public Vector2 GetWorldOffset(
    EntityUid uid,
    VehicleTurretMuzzleComponent muzzle,
    bool? useRightOverride = null)
  {
    Angle worldRotation = this._transform.GetWorldRotation(uid);
    bool useRight = ((int) useRightOverride ?? (!muzzle.Alternate ? 0 : (muzzle.UseRightNext ? 1 : 0))) != 0;
    Vector2 offset = this.GetOffset(muzzle, worldRotation, useRight);
    return !(offset == Vector2.Zero) ? ((Angle) ref worldRotation).RotateVec(ref offset) : Vector2.Zero;
  }

  private Vector2 GetOffset(VehicleTurretMuzzleComponent muzzle, Angle baseRotation, bool useRight)
  {
    if (!muzzle.UseDirectionalOffsets)
      return !useRight ? muzzle.OffsetLeft : muzzle.OffsetRight;
    Vector2 offset;
    switch ((int) VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation))
    {
      case 0:
        offset = useRight ? muzzle.OffsetRightSouth : muzzle.OffsetLeftSouth;
        break;
      case 2:
        offset = useRight ? muzzle.OffsetRightEast : muzzle.OffsetLeftEast;
        break;
      case 4:
        offset = useRight ? muzzle.OffsetRightNorth : muzzle.OffsetLeftNorth;
        break;
      case 6:
        offset = useRight ? muzzle.OffsetRightWest : muzzle.OffsetLeftWest;
        break;
      default:
        offset = useRight ? muzzle.OffsetRight : muzzle.OffsetLeft;
        break;
    }
    return offset;
  }
}
