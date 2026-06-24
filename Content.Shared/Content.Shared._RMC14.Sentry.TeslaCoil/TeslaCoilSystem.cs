using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Sentry.TeslaCoil;

public sealed class TeslaCoilSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private LineSystem _line;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private RMCDazedSystem _dazed;

	[Dependency]
	private SentrySystem _sentrySystem;

	[Dependency]
	private SharedSentryTargetingSystem _targeting;

	private readonly HashSet<EntityUid> _potentialTargets = new HashSet<EntityUid>();

	private readonly List<EntityUid> _validTargets = new List<EntityUid>();

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCTeslaCoilComponent, SentryComponent, TransformComponent, SentryTargetingComponent> teslaQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCTeslaCoilComponent, SentryComponent, TransformComponent, SentryTargetingComponent>();
		EntityUid uid = default(EntityUid);
		RMCTeslaCoilComponent teslaComp = default(RMCTeslaCoilComponent);
		SentryComponent sentryComp = default(SentryComponent);
		TransformComponent xform = default(TransformComponent);
		SentryTargetingComponent targetingComp = default(SentryTargetingComponent);
		SentryComponent targetSentry = default(SentryComponent);
		MobStateComponent mobState = default(MobStateComponent);
		while (teslaQuery.MoveNext(ref uid, ref teslaComp, ref sentryComp, ref xform, ref targetingComp))
		{
			if (sentryComp.Mode != SentryMode.On || !xform.Anchored || time < teslaComp.LastFired + teslaComp.FireDelay)
			{
				continue;
			}
			_potentialTargets.Clear();
			_validTargets.Clear();
			_entityLookup.GetEntitiesInRange(xform.Coordinates, teslaComp.Range, _potentialTargets, (LookupFlags)78);
			int currentTargets = 0;
			foreach (EntityUid targetUid in _potentialTargets)
			{
				if (currentTargets >= teslaComp.MaxTargets)
				{
					break;
				}
				if (targetUid == uid || !_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(uid), Entity<TransformComponent>.op_Implicit(targetUid), teslaComp.Range) || !_targeting.IsValidTarget(Entity<SentryTargetingComponent>.op_Implicit((uid, targetingComp)), targetUid))
				{
					continue;
				}
				bool isValidTarget = false;
				if (((EntitySystem)this).TryComp<SentryComponent>(targetUid, ref targetSentry))
				{
					if (targetSentry.Mode == SentryMode.On)
					{
						isValidTarget = true;
					}
				}
				else if (((EntitySystem)this).TryComp<MobStateComponent>(targetUid, ref mobState) && _mobState.IsAlive(targetUid, mobState))
				{
					isValidTarget = true;
				}
				if (isValidTarget)
				{
					_validTargets.Add(targetUid);
					currentTargets++;
				}
			}
			teslaComp.LastFired = time;
			if (_validTargets.Count <= 0)
			{
				continue;
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)teslaComp, (MetaDataComponent)null);
			foreach (EntityUid target in _validTargets)
			{
				ApplyTeslaEffects(Entity<RMCTeslaCoilComponent>.op_Implicit((uid, teslaComp)), target);
			}
		}
	}

	private void ApplyTeslaEffects(Entity<RMCTeslaCoilComponent> tesla, EntityUid target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		RMCTeslaCoilComponent teslaComp = tesla.Comp;
		SentryComponent targetSentry = default(SentryComponent);
		if (((EntitySystem)this).TryComp<SentryComponent>(target, ref targetSentry) && targetSentry.Mode == SentryMode.On)
		{
			_sentrySystem.TrySetMode(Entity<SentryComponent>.op_Implicit((target, targetSentry)), SentryMode.Off);
		}
		else
		{
			if (teslaComp.StunDuration > TimeSpan.Zero && _sizeStun.TryGetSize(target, out var size) && (int)size <= 4)
			{
				_stun.TryParalyze(target, teslaComp.StunDuration, refresh: true);
			}
			if (teslaComp.SlowDuration > TimeSpan.Zero)
			{
				_slow.TrySuperSlowdown(target, teslaComp.SlowDuration);
			}
			if (teslaComp.DazeDuration > TimeSpan.Zero)
			{
				_dazed.TryDaze(target, teslaComp.DazeDuration, refresh: true);
			}
		}
		_colorFlash.RaiseEffect(Color.Cyan, new List<EntityUid> { target }, Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null));
		if (!string.IsNullOrEmpty(EntProtoId.op_Implicit(teslaComp.TeslaBeamProto)))
		{
			_line.TryCreateLine(tesla.Owner, target, EntProtoId.op_Implicit(teslaComp.TeslaBeamProto), out List<EntityUid> _);
		}
	}
}
