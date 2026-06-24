using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Armor.ThermalCloak;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared._RMC14.Xenonids.Projectile.Spit;
using Content.Shared._RMC14.Xenonids.Projectile.Spit.Charge;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.OnCollide;

public abstract class SharedOnCollideSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private ThermalCloakSystem _cloak;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private XenoSpitSystem _xenoSpit;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private StandingStateSystem _standing;

	private EntityQuery<CollideChainComponent> _collideChainQuery;

	private EntityQuery<DamageOnCollideComponent> _damageOnCollideQuery;

	private readonly List<Entity<DamageOnCollideComponent>> _damageOnCollide = new List<Entity<DamageOnCollideComponent>>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_collideChainQuery = ((EntitySystem)this).GetEntityQuery<CollideChainComponent>();
		_damageOnCollideQuery = ((EntitySystem)this).GetEntityQuery<DamageOnCollideComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DamageOnCollideComponent, StartCollideEvent>((EntityEventRefHandler<DamageOnCollideComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
	}

	private void OnStartCollide(Entity<DamageOnCollideComponent> ent, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnCollide(ent, args.OtherEntity);
	}

	private void OnCollide(Entity<DamageOnCollideComponent> ent, EntityUid other)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Damaged.Contains(other) || !_whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, other) || (!ent.Comp.DamageDead && _mobState.IsDead(other)) || _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(other)) || (ent.Comp.Fire && ((EntitySystem)this).HasComp<RMCImmuneToFireTileDamageComponent>(other)))
		{
			return;
		}
		if (((EntitySystem)this).HasComp<UncloakOnHitComponent>(ent.Owner))
		{
			_cloak.TrySetInvisibility(other, enabling: false, forced: true);
		}
		ent.Comp.Damaged.Add(other);
		((EntitySystem)this).Dirty<DamageOnCollideComponent>(ent, (MetaDataComponent)null);
		bool didEmote = false;
		if (!ent.Comp.Chain.HasValue || AddToChain(Entity<CollideChainComponent>.op_Implicit(ent.Comp.Chain.Value), other))
		{
			DamageSpecifier damage = ent.Comp.Damage;
			if (ent.Comp.Acidic)
			{
				damage = _xeno.TryApplyXenoAcidDamageMultiplier(other, damage);
			}
			_damageable.TryChangeDamage(other, damage, ent.Comp.IgnoreResistances);
			DoEmote(ent, other);
			didEmote = true;
		}
		else
		{
			DamageSpecifier damage2 = ent.Comp.ChainDamage;
			if (ent.Comp.Acidic)
			{
				damage2 = _xeno.TryApplyXenoAcidDamageMultiplier(other, damage2);
			}
			_damageable.TryChangeDamage(other, damage2, ent.Comp.IgnoreResistances);
		}
		_xenoSpit.SetAcidCombo(Entity<UserAcidedComponent>.op_Implicit(other), ent.Comp.AcidComboDuration, ent.Comp.AcidComboDamage, ent.Comp.AcidComboParalyze, ent.Comp.AcidComboResists);
		if (ent.Comp.Paralyze > TimeSpan.Zero && !_standing.IsDown(other) && (!_size.TryGetSize(other, out var size) || (int)size < 5))
		{
			_stun.TryParalyze(other, ent.Comp.Paralyze, refresh: true);
			if (!didEmote)
			{
				DoEmote(ent, other);
			}
		}
		DamageCollideEvent ev = new DamageCollideEvent(other);
		((EntitySystem)this).RaiseLocalEvent<DamageCollideEvent>(Entity<DamageOnCollideComponent>.op_Implicit(ent), ref ev, false);
	}

	protected virtual void DoEmote(Entity<DamageOnCollideComponent> ent, EntityUid other)
	{
	}

	private bool AddToChain(Entity<CollideChainComponent?> chain, EntityUid add)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_collideChainQuery.Resolve(Entity<CollideChainComponent>.op_Implicit(chain), ref chain.Comp, false))
		{
			return true;
		}
		if (chain.Comp.Hit.Add(add))
		{
			((EntitySystem)this).Dirty<CollideChainComponent>(chain, (MetaDataComponent)null);
			return true;
		}
		return false;
	}

	public Entity<CollideChainComponent> SpawnChain()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid ent = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		CollideChainComponent comp = ((EntitySystem)this).EnsureComp<CollideChainComponent>(ent);
		return Entity<CollideChainComponent>.op_Implicit((ent, comp));
	}

	public void SetChain(Entity<DamageOnCollideComponent?> ent, EntityUid chain)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_damageOnCollideQuery.Resolve(Entity<DamageOnCollideComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.Chain = chain;
			((EntitySystem)this).Dirty<DamageOnCollideComponent>(ent, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		_damageOnCollide.Clear();
		try
		{
			EntityQueryEnumerator<DamageOnCollideComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DamageOnCollideComponent>();
			EntityUid uid = default(EntityUid);
			DamageOnCollideComponent comp = default(DamageOnCollideComponent);
			while (query.MoveNext(ref uid, ref comp))
			{
				if (!comp.InitDamaged)
				{
					comp.InitDamaged = true;
					_damageOnCollide.Add(Entity<DamageOnCollideComponent>.op_Implicit((uid, comp)));
				}
			}
			foreach (Entity<DamageOnCollideComponent> entity in _damageOnCollide)
			{
				foreach (EntityUid contact in _physics.GetEntitiesIntersectingBody(Entity<DamageOnCollideComponent>.op_Implicit(entity), (int)entity.Comp.Collision, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
				{
					OnCollide(entity, contact);
				}
			}
		}
		finally
		{
			_damageOnCollide.Clear();
		}
	}
}
