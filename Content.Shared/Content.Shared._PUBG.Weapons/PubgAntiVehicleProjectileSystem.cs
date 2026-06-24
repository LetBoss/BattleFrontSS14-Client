using System;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Projectiles;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._PUBG.Weapons;

public sealed class PubgAntiVehicleProjectileSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private HardpointSystem _hardpoints;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PubgAntiVehicleProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<PubgAntiVehicleProjectileComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, new Type[1] { typeof(RMCProjectileSystem) });
	}

	private void OnProjectileHit(Entity<PubgAntiVehicleProjectileComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetVehicle(args.Target, out EntityUid vehicle, out HardpointIntegrityComponent frame))
		{
			return;
		}
		if (ent.Comp.VehicleDamageMultiplier > 0f && MathF.Abs(ent.Comp.VehicleDamageMultiplier - 1f) > 0.0001f)
		{
			args.Damage *= ent.Comp.VehicleDamageMultiplier;
		}
		if (!_net.IsClient && !(ent.Comp.BonusFrameDamageFraction <= 0f))
		{
			float bonusDamage = args.Damage.GetTotal().Float() * ent.Comp.BonusFrameDamageFraction;
			if (!(bonusDamage <= 0f))
			{
				_hardpoints.DamageHardpoint(vehicle, vehicle, bonusDamage, frame);
			}
		}
	}

	private bool TryGetVehicle(EntityUid uid, out EntityUid vehicle, out HardpointIntegrityComponent frame)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		vehicle = default(EntityUid);
		frame = null;
		EntityUid current = uid;
		HardpointIntegrityComponent currentFrame = default(HardpointIntegrityComponent);
		BaseContainer container = default(BaseContainer);
		while (true)
		{
			if (((EntitySystem)this).HasComp<HardpointSlotsComponent>(current) && ((EntitySystem)this).TryComp<HardpointIntegrityComponent>(current, ref currentFrame))
			{
				vehicle = current;
				frame = currentFrame;
				return true;
			}
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(current, null)), ref container))
			{
				break;
			}
			current = container.Owner;
		}
		return false;
	}
}
