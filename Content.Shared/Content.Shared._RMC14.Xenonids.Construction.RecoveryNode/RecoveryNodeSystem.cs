using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Construction.RecoveryNode;

public sealed class RecoveryNodeSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _time;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedXenoHealSystem _heal;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedDoAfterSystem _doafter;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RecoveryNodeComponent, RecoveryNodeRecoverDoAfterEvent>((EntityEventRefHandler<RecoveryNodeComponent, RecoveryNodeRecoverDoAfterEvent>)OnRecoveryDoAfter, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan curTime = _time.CurTime;
		EntityQueryEnumerator<RecoveryNodeComponent> recoverNodes = ((EntitySystem)this).EntityQueryEnumerator<RecoveryNodeComponent>();
		EntityUid ent = default(EntityUid);
		RecoveryNodeComponent comp = default(RecoveryNodeComponent);
		while (recoverNodes.MoveNext(ref ent, ref comp))
		{
			if (comp.NextHealAt < curTime && !comp.HealDoAfter.HasValue)
			{
				TryHealRandomXeno(Entity<RecoveryNodeComponent>.op_Implicit((ent, comp)));
			}
		}
	}

	private void TryHealRandomXeno(Entity<RecoveryNodeComponent> recoveryNode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Entity<RecoveryNodeComponent> val = recoveryNode;
		EntityUid val2 = default(EntityUid);
		RecoveryNodeComponent recoveryNodeComponent = default(RecoveryNodeComponent);
		val.Deconstruct(ref val2, ref recoveryNodeComponent);
		EntityUid ent = val2;
		RecoveryNodeComponent comp = recoveryNodeComponent;
		HashSet<EntityUid> entitiesInRange = _lookup.GetEntitiesInRange(ent, comp.HealRange, (LookupFlags)110);
		List<EntityUid> possibleTargets = new List<EntityUid>();
		DamageableComponent damageComp = default(DamageableComponent);
		foreach (EntityUid nearbyEntity in entitiesInRange)
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent), Entity<HiveMemberComponent>.op_Implicit(nearbyEntity)) && ((EntitySystem)this).HasComp<XenoComponent>(nearbyEntity) && ((EntitySystem)this).HasComp<XenoRestingComponent>(nearbyEntity) && ((EntitySystem)this).TryComp<DamageableComponent>(nearbyEntity, ref damageComp) && !(damageComp.TotalDamage <= 0) && ((EntitySystem)this).HasComp<MobStateComponent>(nearbyEntity) && !_mob.IsDead(nearbyEntity))
			{
				possibleTargets.Add(nearbyEntity);
			}
		}
		recoveryNode.Comp.NextHealAt = _time.CurTime + recoveryNode.Comp.HealCooldown;
		if (possibleTargets.Count != 0)
		{
			EntityUid selectedTarget = RandomExtensions.Pick<EntityUid>(_random, (IReadOnlyList<EntityUid>)possibleTargets);
			DoAfterArgs recover = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<RecoveryNodeComponent>.op_Implicit(recoveryNode), recoveryNode.Comp.HealCooldown, new RecoveryNodeRecoverDoAfterEvent(), Entity<RecoveryNodeComponent>.op_Implicit(recoveryNode), selectedTarget)
			{
				BreakOnMove = true,
				MovementThreshold = 0.5f,
				DuplicateCondition = DuplicateConditions.SameEvent,
				TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy")
			};
			if (_doafter.TryStartDoAfter(recover, out var id))
			{
				recoveryNode.Comp.HealDoAfter = id;
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-recovery-node-heal-target"), selectedTarget, selectedTarget);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-recovery-node-heal-other", (ValueTuple<string, object>)("target", selectedTarget)), selectedTarget, Filter.PvsExcept(selectedTarget, 2f, (IEntityManager)null), recordReplay: true);
			}
		}
	}

	private void OnRecoveryDoAfter(Entity<RecoveryNodeComponent> recoveryNode, ref RecoveryNodeRecoverDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		recoveryNode.Comp.HealDoAfter = null;
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Target.HasValue)
		{
			_heal.Heal(args.Target.Value, recoveryNode.Comp.HealAmount);
		}
	}
}
