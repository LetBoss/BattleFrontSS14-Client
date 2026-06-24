using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Alert;
using Content.Shared.Coordinates;
using Content.Shared.Damage.Components;
using Content.Shared.Fluids.Components;
using Content.Shared.Gravity;
using Content.Shared.Mobs;
using Content.Shared.Movement.Systems;
using Content.Shared.Slippery;
using Content.Shared.Toggleable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Rootable;

public abstract class SharedRootableSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedGravitySystem _gravity;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeedModifier;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private SharedAudioSystem _audio;

	protected EntityQuery<PuddleComponent> PuddleQuery;

	protected EntityQuery<PhysicsComponent> PhysicsQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		PuddleQuery = ((EntitySystem)this).GetEntityQuery<PuddleComponent>();
		PhysicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, MapInitEvent>((EntityEventRefHandler<RootableComponent, MapInitEvent>)OnRootableMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, ComponentShutdown>((EntityEventRefHandler<RootableComponent, ComponentShutdown>)OnRootableShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, StartCollideEvent>((EntityEventRefHandler<RootableComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, EndCollideEvent>((EntityEventRefHandler<RootableComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, ToggleActionEvent>((EntityEventRefHandler<RootableComponent, ToggleActionEvent>)OnRootableToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, MobStateChangedEvent>((EntityEventRefHandler<RootableComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, IsWeightlessEvent>((EntityEventRefHandler<RootableComponent, IsWeightlessEvent>)OnIsWeightless, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, SlipAttemptEvent>((EntityEventRefHandler<RootableComponent, SlipAttemptEvent>)OnSlipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RootableComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<RootableComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovementSpeed, (Type[])null, (Type[])null);
	}

	private void OnRootableMapInit(Entity<RootableComponent> entity, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		ActionsComponent comp = default(ActionsComponent);
		if (((EntitySystem)this).TryComp<ActionsComponent>(Entity<RootableComponent>.op_Implicit(entity), ref comp))
		{
			entity.Comp.NextUpdate = _timing.CurTime;
			SharedActionsSystem actions = _actions;
			EntityUid performer = Entity<RootableComponent>.op_Implicit(entity);
			ref EntityUid? actionEntity = ref entity.Comp.ActionEntity;
			string actionPrototypeId = EntProtoId.op_Implicit(entity.Comp.Action);
			ActionsComponent component = comp;
			actions.AddAction(performer, ref actionEntity, actionPrototypeId, default(EntityUid), component);
		}
	}

	private void OnRootableShutdown(Entity<RootableComponent> entity, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ActionsComponent comp = default(ActionsComponent);
		if (((EntitySystem)this).TryComp<ActionsComponent>(Entity<RootableComponent>.op_Implicit(entity), ref comp))
		{
			Entity<ActionsComponent> actions = default(Entity<ActionsComponent>);
			actions._002Ector(Entity<RootableComponent>.op_Implicit(entity), comp);
			SharedActionsSystem actions2 = _actions;
			Entity<ActionsComponent> performer = actions;
			EntityUid? actionEntity = entity.Comp.ActionEntity;
			actions2.RemoveAction(performer, actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		}
	}

	private void OnRootableToggle(Entity<RootableComponent> entity, ref ToggleActionEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = TryToggleRooting(Entity<RootableComponent>.op_Implicit((Entity<RootableComponent>.op_Implicit(entity), Entity<RootableComponent>.op_Implicit(entity))));
	}

	private void OnMobStateChanged(Entity<RootableComponent> entity, ref MobStateChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Rooted)
		{
			TryToggleRooting(Entity<RootableComponent>.op_Implicit((Entity<RootableComponent>.op_Implicit(entity), Entity<RootableComponent>.op_Implicit(entity))));
		}
	}

	public bool TryToggleRooting(Entity<RootableComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RootableComponent>(Entity<RootableComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		entity.Comp.Rooted = !entity.Comp.Rooted;
		_movementSpeedModifier.RefreshMovementSpeedModifiers(Entity<RootableComponent>.op_Implicit(entity));
		((EntitySystem)this).Dirty<RootableComponent>(entity, (MetaDataComponent)null);
		if (entity.Comp.Rooted)
		{
			_alerts.ShowAlert(Entity<RootableComponent>.op_Implicit(entity), entity.Comp.RootedAlert);
			TimeSpan curTime = _timing.CurTime;
			if (curTime > entity.Comp.NextUpdate)
			{
				entity.Comp.NextUpdate = curTime;
			}
		}
		else
		{
			_alerts.ClearAlert(Entity<RootableComponent>.op_Implicit(entity), entity.Comp.RootedAlert);
		}
		_audio.PlayPredicted(entity.Comp.RootSound, entity.Owner.ToCoordinates(), (EntityUid?)Entity<RootableComponent>.op_Implicit(entity), (AudioParams?)null);
		return true;
	}

	private void OnIsWeightless(Entity<RootableComponent> ent, ref IsWeightlessEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && ent.Comp.Rooted && _gravity.EntityOnGravitySupportingGridOrMap(Entity<TransformComponent>.op_Implicit(ent.Owner)))
		{
			args.IsWeightless = false;
			args.Handled = true;
		}
	}

	private void OnSlipAttempt(Entity<RootableComponent> ent, ref SlipAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Rooted && (!args.SlipCausingEntity.HasValue || !((EntitySystem)this).HasComp<DamageUserOnTriggerComponent>(args.SlipCausingEntity)))
		{
			args.NoSlip = true;
		}
	}

	private void OnStartCollide(Entity<RootableComponent> entity, ref StartCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (PuddleQuery.HasComp(args.OtherEntity))
		{
			entity.Comp.PuddleEntity = args.OtherEntity;
			if (entity.Comp.NextUpdate < _timing.CurTime)
			{
				entity.Comp.NextUpdate = _timing.CurTime;
			}
		}
	}

	private void OnEndCollide(Entity<RootableComponent> entity, ref EndCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? puddleEntity = entity.Comp.PuddleEntity;
		EntityUid otherEntity = args.OtherEntity;
		if (!puddleEntity.HasValue || puddleEntity.GetValueOrDefault() != otherEntity)
		{
			return;
		}
		bool exists = ((EntitySystem)this).Exists(args.OtherEntity);
		PhysicsComponent body = default(PhysicsComponent);
		if (!PhysicsQuery.TryComp(Entity<RootableComponent>.op_Implicit(entity), ref body))
		{
			return;
		}
		foreach (EntityUid ent in _physics.GetContactingEntities(Entity<RootableComponent>.op_Implicit(entity), body, false))
		{
			if ((!exists || !(ent == args.OtherEntity)) && PuddleQuery.HasComponent(ent))
			{
				entity.Comp.PuddleEntity = ent;
				return;
			}
		}
		entity.Comp.PuddleEntity = null;
	}

	private void OnRefreshMovementSpeed(Entity<RootableComponent> entity, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Rooted)
		{
			args.ModifySpeed(entity.Comp.SpeedModifier);
		}
	}
}
