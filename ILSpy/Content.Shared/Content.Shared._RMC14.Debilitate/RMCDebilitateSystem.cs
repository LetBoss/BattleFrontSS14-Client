using System;
using Content.Shared.Projectiles;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Debilitate;

public sealed class RMCDebilitateSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedStunSystem _stun;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCDebilitateComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCDebilitateComponent, ProjectileHitEvent>)OnDebilitateProjectileHit, (Type[])null, (Type[])null);
	}

	private void OnDebilitateProjectileHit(Entity<RMCDebilitateComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (_entityWhitelist.CheckBoth(args.Target, ent.Comp.Blacklist, ent.Comp.Whitelist))
		{
			_stun.TryParalyze(args.Target, ent.Comp.Knockdown, refresh: true);
		}
	}
}
