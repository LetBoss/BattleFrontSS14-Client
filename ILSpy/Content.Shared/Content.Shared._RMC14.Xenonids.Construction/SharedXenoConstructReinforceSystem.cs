using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Construction;

public sealed class SharedXenoConstructReinforceSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructReinforceComponent, DamageModifyEvent>((EntityEventRefHandler<XenoConstructReinforceComponent, DamageModifyEvent>)OnReinforceDamageModify, (Type[])null, new Type[1] { typeof(SharedRMCMeleeWeaponSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructReinforceComponent, BeforeExplodeEvent>((EntityEventRefHandler<XenoConstructReinforceComponent, BeforeExplodeEvent>)OnReinforceBeforeExplode, (Type[])null, (Type[])null);
	}

	public void Reinforce(EntityUid uid, FixedPoint2 amount, TimeSpan duration)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructReinforceComponent xenoConstructReinforceComponent = ((EntitySystem)this).EnsureComp<XenoConstructReinforceComponent>(uid);
		xenoConstructReinforceComponent.ReinforceAmount = amount;
		xenoConstructReinforceComponent.Duration = duration;
	}

	private void ReduceDamage(Entity<XenoConstructReinforceComponent> ent, ref DamageSpecifier damage)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!damage.AnyPositive())
		{
			return;
		}
		damage = new DamageSpecifier(damage);
		foreach (KeyValuePair<string, FixedPoint2> type in damage.DamageDict)
		{
			if (!(damage.DamageDict[type.Key] <= 0))
			{
				FixedPoint2 modifyStep = FixedPoint2.New(Math.Min(ent.Comp.ReinforceAmount.Double(), damage.DamageDict[type.Key].Double()));
				damage.DamageDict[type.Key] -= modifyStep;
				ent.Comp.ReinforceAmount -= modifyStep;
				if (ent.Comp.ReinforceAmount <= 0)
				{
					ReinforceRemoved(ent);
					break;
				}
			}
		}
	}

	private void OnReinforceBeforeExplode(Entity<XenoConstructReinforceComponent> ent, ref BeforeExplodeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ReduceDamage(ent, ref args.Damage);
	}

	private void OnReinforceDamageModify(Entity<XenoConstructReinforceComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ReduceDamage(ent, ref args.Damage);
	}

	private void ReinforceRemoved(Entity<XenoConstructReinforceComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<XenoConstructReinforceComponent>(ent.Owner);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoConstructReinforceComponent> reinforceQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoConstructReinforceComponent>();
		EntityUid uid = default(EntityUid);
		XenoConstructReinforceComponent comp = default(XenoConstructReinforceComponent);
		while (reinforceQuery.MoveNext(ref uid, ref comp))
		{
			XenoConstructReinforceComponent xenoConstructReinforceComponent = comp;
			TimeSpan valueOrDefault = xenoConstructReinforceComponent.EndAt.GetValueOrDefault();
			if (!xenoConstructReinforceComponent.EndAt.HasValue)
			{
				valueOrDefault = time + comp.Duration;
				xenoConstructReinforceComponent.EndAt = valueOrDefault;
			}
			valueOrDefault = time;
			TimeSpan? endAt = comp.EndAt;
			if (!(valueOrDefault < endAt))
			{
				ReinforceRemoved(Entity<XenoConstructReinforceComponent>.op_Implicit((uid, comp)));
			}
		}
	}
}
