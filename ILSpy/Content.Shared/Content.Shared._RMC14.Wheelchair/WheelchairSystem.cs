using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Wheelchair;

public sealed class WheelchairSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedMoverController _mover;

	private readonly HashSet<EntityUid> _processingUnbuckle = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<WheelchairComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<WheelchairComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WheelchairComponent, StrappedEvent>((EntityEventRefHandler<WheelchairComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WheelchairComponent, UnstrappedEvent>((EntityEventRefHandler<WheelchairComponent, UnstrappedEvent>)OnUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveWheelchairPilotComponent, RingBellActionEvent>((EntityEventRefHandler<ActiveWheelchairPilotComponent, RingBellActionEvent>)OnRingBell, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveWheelchairPilotComponent, PreventCollideEvent>((EntityEventRefHandler<ActiveWheelchairPilotComponent, PreventCollideEvent>)OnActivePilotPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveWheelchairPilotComponent, KnockedDownEvent>((EntityEventRefHandler<ActiveWheelchairPilotComponent, KnockedDownEvent>)OnActivePilotStunned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveWheelchairPilotComponent, StunnedEvent>((EntityEventRefHandler<ActiveWheelchairPilotComponent, StunnedEvent>)OnActivePilotStunned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveWheelchairPilotComponent, MobStateChangedEvent>((EntityEventRefHandler<ActiveWheelchairPilotComponent, MobStateChangedEvent>)OnActivePilotMobStateChanged, (Type[])null, (Type[])null);
	}

	private void OnRefreshSpeed(Entity<WheelchairComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(Entity<WheelchairComponent>.op_Implicit(ent), ref strap) && strap.BuckledEntities.Count != 0)
		{
			float speed = ent.Comp.SpeedMultiplier;
			args.ModifySpeed(speed, speed);
		}
	}

	private void OnStrapped(Entity<WheelchairComponent> ent, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Entity<BuckleComponent> buckle = args.Buckle;
		ActiveWheelchairPilotComponent pilot = ((EntitySystem)this).EnsureComp<ActiveWheelchairPilotComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		_mover.SetRelay(Entity<BuckleComponent>.op_Implicit(buckle), Entity<WheelchairComponent>.op_Implicit(ent));
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<WheelchairComponent>.op_Implicit(ent));
		if (ent.Comp.BellAction.HasValue)
		{
			SharedActionsSystem actions = _actions;
			EntityUid performer = Entity<BuckleComponent>.op_Implicit(buckle);
			EntProtoId? bellAction = ent.Comp.BellAction;
			pilot.BellActionEntity = actions.AddAction(performer, bellAction.HasValue ? EntProtoId.op_Implicit(bellAction.GetValueOrDefault()) : null);
		}
	}

	private void OnUnstrapped(Entity<WheelchairComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Entity<BuckleComponent> buckle = args.Buckle;
		if (_processingUnbuckle.Contains(buckle.Owner))
		{
			return;
		}
		_processingUnbuckle.Add(buckle.Owner);
		try
		{
			ActiveWheelchairPilotComponent pilot = default(ActiveWheelchairPilotComponent);
			if (((EntitySystem)this).TryComp<ActiveWheelchairPilotComponent>(Entity<BuckleComponent>.op_Implicit(buckle), ref pilot) && pilot.BellActionEntity.HasValue)
			{
				_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(buckle.Owner), Entity<ActionComponent>.op_Implicit(pilot.BellActionEntity.Value));
			}
			((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
			((EntitySystem)this).RemCompDeferred<ActiveWheelchairPilotComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<WheelchairComponent>.op_Implicit(ent));
		}
		finally
		{
			_processingUnbuckle.Remove(buckle.Owner);
		}
	}

	private void OnActivePilotPreventCollide(Entity<ActiveWheelchairPilotComponent> ent, ref PreventCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnActivePilotStunned<T>(Entity<ActiveWheelchairPilotComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemovePilot(ent);
	}

	private void OnActivePilotMobStateChanged(Entity<ActiveWheelchairPilotComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Critical || args.NewMobState == MobState.Dead)
		{
			OnActivePilotStunned(ent, ref args);
		}
	}

	private void RemovePilot(Entity<ActiveWheelchairPilotComponent> active)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!_processingUnbuckle.Contains(active.Owner))
		{
			((EntitySystem)this).RemCompDeferred<ActiveWheelchairPilotComponent>(Entity<ActiveWheelchairPilotComponent>.op_Implicit(active));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		List<Entity<ActiveWheelchairPilotComponent>> toRemove = new List<Entity<ActiveWheelchairPilotComponent>>();
		EntityQueryEnumerator<ActiveWheelchairPilotComponent> pilots = ((EntitySystem)this).EntityQueryEnumerator<ActiveWheelchairPilotComponent>();
		EntityUid uid = default(EntityUid);
		ActiveWheelchairPilotComponent active = default(ActiveWheelchairPilotComponent);
		BuckleComponent buckle = default(BuckleComponent);
		while (pilots.MoveNext(ref uid, ref active))
		{
			if (!((EntitySystem)this).TryComp<BuckleComponent>(uid, ref buckle) || !buckle.BuckledTo.HasValue || !((EntitySystem)this).HasComp<WheelchairComponent>(buckle.BuckledTo))
			{
				toRemove.Add(Entity<ActiveWheelchairPilotComponent>.op_Implicit((uid, active)));
			}
		}
		foreach (Entity<ActiveWheelchairPilotComponent> pilot in toRemove)
		{
			((EntitySystem)this).RemCompDeferred<ActiveWheelchairPilotComponent>(Entity<ActiveWheelchairPilotComponent>.op_Implicit(pilot));
		}
	}

	private void OnRingBell(Entity<ActiveWheelchairPilotComponent> ent, ref RingBellActionEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		WheelchairComponent wheelchair = default(WheelchairComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<BuckleComponent>(Entity<ActiveWheelchairPilotComponent>.op_Implicit(ent), ref buckle) && ((EntitySystem)this).TryComp<WheelchairComponent>(buckle.BuckledTo, ref wheelchair))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_audio.PlayPredicted(wheelchair.BellSound, args.Performer, (EntityUid?)args.Performer, (AudioParams?)null);
		}
	}
}
