using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Explosion;

public sealed class CMClusterGrenadeSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<UserLimitHitsComponent> _userLimits;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_userLimits = ((EntitySystem)this).GetEntityQuery<UserLimitHitsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ClusterLimitHitsComponent, CMClusterSpawnedEvent>((EntityEventRefHandler<ClusterLimitHitsComponent, CMClusterSpawnedEvent>)OnClusterLimitHitsSpawned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileLimitHitsComponent, ProjectileHitEvent>((EntityEventRefHandler<ProjectileLimitHitsComponent, ProjectileHitEvent>)OnProjectileLimitHitsHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileLimitHitsComponent, PreventCollideEvent>((EntityEventRefHandler<ProjectileLimitHitsComponent, PreventCollideEvent>)OnProjectileLimitHitsPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnClusterLimitHitsSpawned(Entity<ClusterLimitHitsComponent> ent, ref CMClusterSpawnedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid spawned in args.Spawned)
		{
			ProjectileLimitHitsComponent projectile = ((EntitySystem)this).EnsureComp<ProjectileLimitHitsComponent>(spawned);
			if (ent.Comp.IgnoreFirstHit)
			{
				projectile.IgnoredEntities = args.HitEntities;
			}
			projectile.Limit = ent.Comp.Limit;
			projectile.OriginEntity = args.OriginEntity;
			projectile.ExtraId = args.ExtraId;
			((EntitySystem)this).Dirty(spawned, (IComponent)(object)projectile, (MetaDataComponent)null);
		}
	}

	private void OnProjectileLimitHitsHit(Entity<ProjectileLimitHitsComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		UserLimitHitsComponent limit = default(UserLimitHitsComponent);
		if (_userLimits.TryComp(args.Target, ref limit))
		{
			if (!CanHit(Entity<UserLimitHitsComponent>.op_Implicit((args.Target, limit)), ent))
			{
				args.Handled = true;
				return;
			}
			limit.HitBy.Add(new Hit(((EntitySystem)this).GetNetEntity(ent.Comp.OriginEntity, (MetaDataComponent)null), _timing.CurTime + limit.Expire, ent.Comp.ExtraId));
			((EntitySystem)this).Dirty(args.Target, (IComponent)(object)limit, (MetaDataComponent)null);
		}
	}

	private void OnProjectileLimitHitsPreventCollide(Entity<ProjectileLimitHitsComponent> ent, ref PreventCollideEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		UserLimitHitsComponent limit = default(UserLimitHitsComponent);
		if (!args.Cancelled && _userLimits.TryComp(args.OtherEntity, ref limit) && !CanHit(Entity<UserLimitHitsComponent>.op_Implicit((args.OtherEntity, limit)), ent))
		{
			args.Cancelled = true;
		}
	}

	public bool CanHit(Entity<UserLimitHitsComponent> user, Entity<ProjectileLimitHitsComponent> projectile)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		Span<Hit> span = CollectionsMarshal.AsSpan(user.Comp.HitBy);
		int count = 0;
		Span<Hit> span2 = span;
		for (int i = 0; i < span2.Length; i++)
		{
			ref Hit hit = ref span2[i];
			if (projectile.Comp.Limit != 0 || projectile.Comp.IgnoredEntities.Contains(Entity<UserLimitHitsComponent>.op_Implicit(user)))
			{
				if ((hit.Id.Id == projectile.Comp.OriginEntity.Id && (!hit.ExtraId.HasValue || hit.ExtraId == projectile.Comp.ExtraId)) || (hit.Id == ((EntitySystem)this).GetNetEntity(projectile.Owner, (MetaDataComponent)null) && (!hit.ExtraId.HasValue || hit.ExtraId == projectile.Comp.ExtraId) && hit.ExpireAt > time))
				{
					count++;
				}
				if (count >= projectile.Comp.Limit && count != 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<UserLimitHitsComponent> query = ((EntitySystem)this).EntityQueryEnumerator<UserLimitHitsComponent>();
		EntityUid uid = default(EntityUid);
		UserLimitHitsComponent comp = default(UserLimitHitsComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			bool removed = false;
			for (int i = comp.HitBy.Count - 1; i >= 0; i--)
			{
				if (!(time <= comp.HitBy[i].ExpireAt))
				{
					Extensions.RemoveSwap<Hit>((IList<Hit>)comp.HitBy, i);
					removed = true;
				}
			}
			if (removed)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}
}
