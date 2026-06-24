using System;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretMuzzleOffsetSystem : EntitySystem
{
	[Dependency]
	private readonly GunMuzzleOffsetSystem _gunMuzzleOffset;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly VehicleTurretMuzzleSystem _turretMuzzle;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleWeaponsOperatorComponent, BeforeAttemptShootEvent>((EntityEventRefHandler<VehicleWeaponsOperatorComponent, BeforeAttemptShootEvent>)OnBeforeAttemptShoot, (Type[])null, (Type[])null);
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleTurretTrackedMuzzleFlashComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<VehicleTurretTrackedMuzzleFlashComponent, TransformComponent>();
		EntityUid item = default(EntityUid);
		VehicleTurretTrackedMuzzleFlashComponent vehicleTurretTrackedMuzzleFlashComponent = default(VehicleTurretTrackedMuzzleFlashComponent);
		TransformComponent val2 = default(TransformComponent);
		while (val.MoveNext(ref item, ref vehicleTurretTrackedMuzzleFlashComponent, ref val2))
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(vehicleTurretTrackedMuzzleFlashComponent.Weapon, (MetaDataComponent)null) && TryGetGunPose(vehicleTurretTrackedMuzzleFlashComponent.Weapon, null, out var origin, out var rotation))
			{
				MapCoordinates val3 = _transform.ToMapCoordinates(origin, true);
				val2.ActivelyLerping = false;
				Angle val4 = rotation + vehicleTurretTrackedMuzzleFlashComponent.RotationOffset;
				Angle val5 = ((Angle)(ref val4)).Reduced();
				_transform.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit((item, val2)), val5);
				_transform.SetWorldPosition(Entity<TransformComponent>.op_Implicit((item, val2)), val3.Position + ((Angle)(ref val5)).RotateVec(ref vehicleTurretTrackedMuzzleFlashComponent.Offset));
			}
		}
	}

	public bool TryGetGunOrigin(EntityUid weaponUid, EntityCoordinates? target, out EntityCoordinates origin)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Angle rotation;
		return TryGetGunPose(weaponUid, target, out origin, out rotation);
	}

	public bool TryGetGunPose(EntityUid weaponUid, EntityCoordinates? target, out EntityCoordinates origin, out Angle rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		origin = default(EntityCoordinates);
		rotation = Angle.Zero;
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(weaponUid, ref vehicleTurretComponent))
		{
			return false;
		}
		origin = _transform.GetMoverCoordinates(weaponUid);
		rotation = _transform.GetWorldRotation(weaponUid);
		EntityCoordinates? toCoordinates = target;
		GunComponent gunComponent = default(GunComponent);
		if (!toCoordinates.HasValue && ((EntitySystem)this).TryComp<GunComponent>(weaponUid, ref gunComponent))
		{
			EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
			if (shootCoordinates.HasValue)
			{
				EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
				toCoordinates = valueOrDefault;
			}
		}
		GunMuzzleOffsetComponent gunMuzzle = default(GunMuzzleOffsetComponent);
		if (((EntitySystem)this).TryComp<GunMuzzleOffsetComponent>(weaponUid, ref gunMuzzle) && _gunMuzzleOffset.TryGetMuzzleCoordinates(weaponUid, gunMuzzle, toCoordinates, out var muzzleCoords, out var muzzleRotation))
		{
			origin = muzzleCoords;
			rotation = muzzleRotation;
		}
		VehicleTurretMuzzleComponent muzzle = default(VehicleTurretMuzzleComponent);
		if (((EntitySystem)this).TryComp<VehicleTurretMuzzleComponent>(weaponUid, ref muzzle))
		{
			origin = _turretMuzzle.GetMuzzleCoordinates(weaponUid, muzzle, origin);
		}
		return true;
	}

	private void OnBeforeAttemptShoot(Entity<VehicleWeaponsOperatorComponent> ent, ref BeforeAttemptShootEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? selectedWeapon = ent.Comp.SelectedWeapon;
		if (selectedWeapon.HasValue)
		{
			EntityUid valueOrDefault = selectedWeapon.GetValueOrDefault();
			if (TryGetGunOrigin(valueOrDefault, null, out var origin))
			{
				args.Origin = origin;
				args.Handled = true;
			}
		}
	}
}
