using System;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.CriticalGrace;

public sealed class CriticalGraceSystem : EntitySystem
{
	[Dependency]
	private MobThresholdSystem _mobThresholds;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<CriticalGraceTimeComponent, UpdateMobStateEvent>((EntityEventRefHandler<CriticalGraceTimeComponent, UpdateMobStateEvent>)OnCriticalGraceMobState, (Type[])null, new Type[2]
		{
			typeof(MobThresholdSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<InCriticalGraceComponent, ComponentShutdown>((EntityEventRefHandler<InCriticalGraceComponent, ComponentShutdown>)OnInCriticalGraceRemove, (Type[])null, (Type[])null);
	}

	private void OnCriticalGraceMobState(Entity<CriticalGraceTimeComponent> ent, ref UpdateMobStateEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (args.State != MobState.Critical || !_mobState.HasState(Entity<CriticalGraceTimeComponent>.op_Implicit(ent), MobState.Critical))
		{
			return;
		}
		InCriticalGraceComponent crit = default(InCriticalGraceComponent);
		if (((EntitySystem)this).TryComp<InCriticalGraceComponent>(Entity<CriticalGraceTimeComponent>.op_Implicit(ent), ref crit))
		{
			if (!crit.Over)
			{
				args.State = MobState.Alive;
			}
		}
		else if (_mobState.IsAlive(Entity<CriticalGraceTimeComponent>.op_Implicit(ent)))
		{
			InCriticalGraceComponent inCriticalGraceComponent = ((EntitySystem)this).EnsureComp<InCriticalGraceComponent>(Entity<CriticalGraceTimeComponent>.op_Implicit(ent));
			GetCriticalGraceTimeEvent ev = new GetCriticalGraceTimeEvent(ent.Comp.GraceDuration);
			((EntitySystem)this).RaiseLocalEvent<GetCriticalGraceTimeEvent>(Entity<CriticalGraceTimeComponent>.op_Implicit(ent), ref ev, false);
			inCriticalGraceComponent.GraceEndsAt = _timing.CurTime + ev.Time;
			args.State = MobState.Alive;
		}
	}

	private void OnInCriticalGraceRemove(Entity<InCriticalGraceComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<InCriticalGraceComponent>.op_Implicit(ent), (MetaDataComponent)null) && ((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<InCriticalGraceComponent>.op_Implicit(ent), ref thresholds))
		{
			_mobThresholds.VerifyThresholds(Entity<InCriticalGraceComponent>.op_Implicit(ent), thresholds);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<InCriticalGraceComponent> graceQuery = ((EntitySystem)this).EntityQueryEnumerator<InCriticalGraceComponent>();
		EntityUid uid = default(EntityUid);
		InCriticalGraceComponent grace = default(InCriticalGraceComponent);
		while (graceQuery.MoveNext(ref uid, ref grace))
		{
			if (!(time < grace.GraceEndsAt))
			{
				grace.Over = true;
				((EntitySystem)this).RemCompDeferred<InCriticalGraceComponent>(uid);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)grace, (MetaDataComponent)null);
			}
		}
	}
}
