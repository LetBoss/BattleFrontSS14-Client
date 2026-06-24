using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Tantrum;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Fury;

public sealed class XenoFurySystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private DamageableSystem _damageable;

	private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFuryComponent, MeleeHitEvent>((EntityEventRefHandler<XenoFuryComponent, MeleeHitEvent>)OnFuryHit, (Type[])null, (Type[])null);
	}

	private void OnFuryHit(Entity<XenoFuryComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		if (!_xeno.CanHeal(Entity<XenoFuryComponent>.op_Implicit(xeno)))
		{
			return;
		}
		bool validHit = false;
		foreach (EntityUid ent in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoFuryComponent>.op_Implicit(xeno), ent))
			{
				validHit = true;
				break;
			}
		}
		if (!validHit)
		{
			return;
		}
		int healAmount = (((EntitySystem)this).HasComp<TantrumingComponent>(Entity<XenoFuryComponent>.op_Implicit(xeno)) ? xeno.Comp.BoostedHeal : xeno.Comp.Heal);
		_xenos.Clear();
		_entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.Range, _xenos, (LookupFlags)110);
		foreach (Entity<XenoComponent> otherXeno in _xenos)
		{
			if (_xeno.CanHeal(Entity<XenoComponent>.op_Implicit(otherXeno)) && !_mob.IsDead(Entity<XenoComponent>.op_Implicit(otherXeno)) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(otherXeno.Owner)))
			{
				DamageSpecifier toHeal = -_rmcDamageable.DistributeTypesTotal(Entity<DamageableComponent>.op_Implicit(otherXeno.Owner), healAmount);
				_damageable.TryChangeDamage(otherXeno.Owner, toHeal, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoFuryComponent>.op_Implicit(xeno), Entity<XenoFuryComponent>.op_Implicit(xeno));
				if (_net.IsServer)
				{
					((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), otherXeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
				}
			}
		}
	}
}
