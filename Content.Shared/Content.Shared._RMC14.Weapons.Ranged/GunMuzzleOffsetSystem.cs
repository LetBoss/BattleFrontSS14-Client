using System;
using System.Numerics;
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

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class GunMuzzleOffsetSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunMuzzleOffsetComponent, AttemptShootEvent>((EntityEventRefHandler<GunMuzzleOffsetComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunMuzzleOffsetComponent, RMCBeforeMuzzleFlashEvent>((EntityEventRefHandler<GunMuzzleOffsetComponent, RMCBeforeMuzzleFlashEvent>)OnBeforeMuzzleFlash, (Type[])null, new Type[1] { typeof(MountableWeaponSystem) });
	}

	private void OnAttemptShoot(Entity<GunMuzzleOffsetComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && TryGetMuzzleCoordinates(ent, args.ToCoordinates, out var fromCoords, out var _))
		{
			args.FromCoordinates = fromCoords;
		}
	}

	private void OnBeforeMuzzleFlash(Entity<GunMuzzleOffsetComponent> ent, ref RMCBeforeMuzzleFlashEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.ApplyToMuzzleFlash)
		{
			return;
		}
		EntityCoordinates? target = null;
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(Entity<GunMuzzleOffsetComponent>.op_Implicit(ent), ref gun))
		{
			target = gun.ShootCoordinates;
		}
		if (TryGetMuzzleCoordinates(ent, target, out var muzzleCoords, out var muzzleRotation))
		{
			MapCoordinates muzzleMap = _transform.ToMapCoordinates(muzzleCoords, true);
			MapCoordinates weaponMap = _transform.GetMapCoordinates(args.Weapon, (TransformComponent)null);
			if (!(muzzleMap.MapId != weaponMap.MapId))
			{
				Vector2 worldOffset = muzzleMap.Position - weaponMap.Position;
				Angle weaponRotation = _transform.GetWorldRotation(args.Weapon);
				muzzleRotation = -weaponRotation;
				args.Offset = ((Angle)(ref muzzleRotation)).RotateVec(ref worldOffset);
			}
		}
	}

	private bool TryGetMuzzleCoordinates(Entity<GunMuzzleOffsetComponent> ent, EntityCoordinates? toCoordinates, out EntityCoordinates muzzleCoords, out Angle muzzleRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		muzzleCoords = default(EntityCoordinates);
		muzzleRotation = Angle.Zero;
		if (ent.Comp.Offset == Vector2.Zero && ent.Comp.MuzzleOffset == Vector2.Zero && !ent.Comp.UseDirectionalOffsets)
		{
			return false;
		}
		EntityUid baseUid = ent.Owner;
		BaseContainer container = default(BaseContainer);
		if (ent.Comp.UseContainerOwner && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(ent.Owner, null)), ref container))
		{
			baseUid = container.Owner;
		}
		EntityCoordinates baseCoords = _transform.GetMoverCoordinates(baseUid);
		Angle baseRotation = (muzzleRotation = GetBaseRotation(baseUid, ent.Comp.AngleOffset));
		(Vector2 Offset, bool Rotate) offset = GetOffset(ent.Comp, baseUid, baseRotation);
		Vector2 offset2 = offset.Offset;
		bool rotateOffset = offset.Rotate;
		muzzleCoords = (rotateOffset ? ((EntityCoordinates)(ref baseCoords)).Offset(((Angle)(ref baseRotation)).RotateVec(ref offset2)) : ((EntityCoordinates)(ref baseCoords)).Offset(offset2));
		if (ent.Comp.MuzzleOffset == Vector2.Zero)
		{
			return true;
		}
		if (ent.Comp.UseAimDirection && toCoordinates.HasValue && _transform.IsValid(muzzleCoords) && _transform.IsValid(toCoordinates.Value))
		{
			MapCoordinates pivotMap = _transform.ToMapCoordinates(muzzleCoords, true);
			MapCoordinates targetMap = _transform.ToMapCoordinates(toCoordinates.Value, true);
			if (pivotMap.MapId == targetMap.MapId)
			{
				Vector2 direction = targetMap.Position - pivotMap.Position;
				if (direction.LengthSquared() > 0.0001f)
				{
					muzzleRotation = DirectionExtensions.ToWorldAngle(direction) + ent.Comp.AngleOffset;
				}
			}
		}
		muzzleCoords = ((EntityCoordinates)(ref muzzleCoords)).Offset(((Angle)(ref muzzleRotation)).RotateVec(ref ent.Comp.MuzzleOffset));
		return true;
	}

	public bool TryGetMuzzleCoordinates(EntityUid weaponUid, GunMuzzleOffsetComponent gunMuzzle, EntityCoordinates? toCoordinates, out EntityCoordinates muzzleCoords, out Angle muzzleRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TryGetMuzzleCoordinates(Entity<GunMuzzleOffsetComponent>.op_Implicit((weaponUid, gunMuzzle)), toCoordinates, out muzzleCoords, out muzzleRotation);
	}

	private Angle GetBaseRotation(EntityUid baseUid, Angle angleOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Angle rotation = _transform.GetWorldRotation(baseUid);
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(baseUid, ref mover) && mover.CurrentDirection != Vector2i.Zero)
		{
			rotation = DirectionExtensions.ToWorldAngle(new Vector2(mover.CurrentDirection.X, mover.CurrentDirection.Y));
		}
		return rotation + angleOffset;
	}

	private (Vector2 Offset, bool Rotate) GetOffset(GunMuzzleOffsetComponent muzzle, EntityUid baseUid, Angle baseRotation)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected I4, but got Unknown
		if (!muzzle.UseDirectionalOffsets)
		{
			return (Offset: muzzle.Offset, Rotate: true);
		}
		Direction dir = GetBaseDirection(baseUid, baseRotation);
		return (Offset: (int)dir switch
		{
			4 => muzzle.OffsetNorth, 
			2 => muzzle.OffsetEast, 
			0 => muzzle.OffsetSouth, 
			6 => muzzle.OffsetWest, 
			_ => muzzle.Offset, 
		}, Rotate: muzzle.RotateDirectionalOffsets);
	}

	private Direction GetBaseDirection(EntityUid baseUid, Angle baseRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(baseUid, ref mover) && mover.CurrentDirection != Vector2i.Zero)
		{
			return DirectionExtensions.AsDirection(mover.CurrentDirection);
		}
		return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation);
	}
}
