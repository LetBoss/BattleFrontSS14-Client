using System;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Weapons.Ranged.HoloTargeting;

public sealed class RMCHoloTargetingSystem : EntitySystem
{
	[Dependency]
	private RMCHoloTargetedSystem _holoTargeted;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HoloTargetingComponent, ProjectileHitEvent>((ComponentEventRefHandler<HoloTargetingComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
	}

	private void OnProjectileHit(EntityUid uid, HoloTargetingComponent component, ref ProjectileHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_holoTargeted.ApplyHoloStacks(args.Target, component.Decay, component.Stacks, component.MaxStacks);
	}
}
