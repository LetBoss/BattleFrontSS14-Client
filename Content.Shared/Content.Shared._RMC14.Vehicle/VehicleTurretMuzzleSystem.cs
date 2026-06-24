using System;
using System.Numerics;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTurretMuzzleSystem : EntitySystem
{
	[Dependency]
	private readonly SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretMuzzleComponent, AttemptShootEvent>((EntityEventRefHandler<VehicleTurretMuzzleComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, new Type[1] { typeof(GunMuzzleOffsetSystem) });
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretMuzzleComponent, GunShotEvent>((EntityEventRefHandler<VehicleTurretMuzzleComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
	}

	private void OnAttemptShoot(Entity<VehicleTurretMuzzleComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			args.FromCoordinates = GetMuzzleCoordinates(ent.Owner, ent.Comp, args.FromCoordinates);
		}
	}

	private void OnGunShot(Entity<VehicleTurretMuzzleComponent> ent, ref GunShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Alternate && args.Ammo.Count != 0)
		{
			for (int i = 0; i < args.Ammo.Count; i++)
			{
				ent.Comp.UseRightNext = !ent.Comp.UseRightNext;
			}
			((EntitySystem)this).Dirty<VehicleTurretMuzzleComponent>(ent, (MetaDataComponent)null);
		}
	}

	public EntityCoordinates GetMuzzleCoordinates(EntityUid uid, VehicleTurretMuzzleComponent muzzle, EntityCoordinates baseCoords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Vector2 offset = GetWorldOffset(uid, muzzle);
		if (!(offset == Vector2.Zero))
		{
			return ((EntityCoordinates)(ref baseCoords)).Offset(offset);
		}
		return baseCoords;
	}

	public Vector2 GetWorldOffset(EntityUid uid, VehicleTurretMuzzleComponent muzzle, bool? useRightOverride = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Angle baseRotation = _transform.GetWorldRotation(uid);
		bool useRight = useRightOverride ?? (muzzle.Alternate && muzzle.UseRightNext);
		Vector2 offset = GetOffset(muzzle, baseRotation, useRight);
		if (!(offset == Vector2.Zero))
		{
			return ((Angle)(ref baseRotation)).RotateVec(ref offset);
		}
		return Vector2.Zero;
	}

	private Vector2 GetOffset(VehicleTurretMuzzleComponent muzzle, Angle baseRotation, bool useRight)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected I4, but got Unknown
		if (!muzzle.UseDirectionalOffsets)
		{
			if (!useRight)
			{
				return muzzle.OffsetLeft;
			}
			return muzzle.OffsetRight;
		}
		Direction renderAlignedCardinalDir = VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation);
		return (int)renderAlignedCardinalDir switch
		{
			4 => useRight ? muzzle.OffsetRightNorth : muzzle.OffsetLeftNorth, 
			2 => useRight ? muzzle.OffsetRightEast : muzzle.OffsetLeftEast, 
			0 => useRight ? muzzle.OffsetRightSouth : muzzle.OffsetLeftSouth, 
			6 => useRight ? muzzle.OffsetRightWest : muzzle.OffsetLeftWest, 
			_ => useRight ? muzzle.OffsetRight : muzzle.OffsetLeft, 
		};
	}
}
