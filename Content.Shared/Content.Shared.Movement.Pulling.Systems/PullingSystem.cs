using System;
using System.Numerics;
using Content.Shared._RMC14.Fireman;
using Content.Shared._RMC14.Pulling;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Pulling.Systems;

public sealed class PullingSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private AlertsSystem _alertsSystem;

	[Dependency]
	private MovementSpeedModifierSystem _modifierSystem;

	[Dependency]
	private SharedJointSystem _joints;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private HeldSpeedModifierSystem _clothingMoveSpeed;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedVirtualItemSystem _virtual;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	public override void Initialize()
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedPhysicsSystem));
		((EntitySystem)this).UpdatesOutsidePrediction = true;
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, MoveInputEvent>((ComponentEventRefHandler<PullableComponent, MoveInputEvent>)OnPullableMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, CollisionChangeEvent>((ComponentEventRefHandler<PullableComponent, CollisionChangeEvent>)OnPullableCollisionChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, JointRemovedEvent>((ComponentEventHandler<PullableComponent, JointRemovedEvent>)OnJointRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<PullableComponent, GetVerbsEvent<Verb>>)AddPullVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<PullableComponent, EntGotInsertedIntoContainerMessage>)OnPullableContainerInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, ModifyUncuffDurationEvent>((EntityEventRefHandler<PullableComponent, ModifyUncuffDurationEvent>)OnModifyUncuffDuration, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, StopBeingPulledAlertEvent>((EntityEventRefHandler<PullableComponent, StopBeingPulledAlertEvent>)OnStopBeingPulledAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, UpdateMobStateEvent>((ComponentEventRefHandler<PullerComponent, UpdateMobStateEvent>)OnStateChanged, (Type[])null, new Type[1] { typeof(MobThresholdSystem) });
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<PullerComponent, AfterAutoHandleStateEvent>)OnAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<PullerComponent, EntGotInsertedIntoContainerMessage>)OnPullerContainerInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, EntityUnpausedEvent>((ComponentEventRefHandler<PullerComponent, EntityUnpausedEvent>)OnPullerUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, VirtualItemDeletedEvent>((ComponentEventHandler<PullerComponent, VirtualItemDeletedEvent>)OnVirtualItemDeleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<PullerComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovespeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, DropHandItemsEvent>((ComponentEventHandler<PullerComponent, DropHandItemsEvent>)OnDropHandItems, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullerComponent, StopPullingAlertEvent>((EntityEventRefHandler<PullerComponent, StopPullingAlertEvent>)OnStopPullingAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, PullStartedMessage>((ComponentEventHandler<HandsComponent, PullStartedMessage>)HandlePullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, PullStoppedMessage>((ComponentEventHandler<HandsComponent, PullStoppedMessage>)HandlePullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, StrappedEvent>((EntityEventRefHandler<PullableComponent, StrappedEvent>)OnBuckled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, BuckledEvent>((EntityEventRefHandler<PullableComponent, BuckledEvent>)OnGotBuckled, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.ReleasePulledObject, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(OnReleasePulledObject), (StateInputCmdDelegate)null, false, true)).Register<PullingSystem>();
	}

	private void HandlePullStarted(EntityUid uid, HandsComponent component, PullStartedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PullerComponent pullerComp = default(PullerComponent);
		if (!(args.PullerUid != uid) && (!((EntitySystem)this).TryComp<PullerComponent>(args.PullerUid, ref pullerComp) || pullerComp.NeedsHands))
		{
			_virtual.TrySpawnVirtualItemInHand(args.PulledUid, uid);
		}
	}

	private void HandlePullStopped(EntityUid uid, HandsComponent component, PullStoppedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (args.PullerUid != uid)
		{
			return;
		}
		VirtualItemComponent virtualItem = default(VirtualItemComponent);
		foreach (EntityUid held in _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit((uid, component))))
		{
			if (((EntitySystem)this).TryComp<VirtualItemComponent>(held, ref virtualItem) && !(virtualItem.BlockingEntity != args.PulledUid))
			{
				_handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit((args.PullerUid, component)), held);
				break;
			}
		}
	}

	private void OnStateChanged(EntityUid uid, PullerComponent component, ref UpdateMobStateEvent args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent comp = default(PullableComponent);
		if (component.Pulling.HasValue && ((EntitySystem)this).TryComp<PullableComponent>(component.Pulling, ref comp) && (args.State == MobState.Critical || args.State == MobState.Dead))
		{
			TryStopPull(component.Pulling.Value, comp);
		}
	}

	private void OnBuckled(Entity<PullableComponent> ent, ref StrappedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? puller = ent.Comp.Puller;
		EntityUid owner = args.Buckle.Owner;
		if (puller.HasValue && puller.GetValueOrDefault() == owner && !args.Buckle.Comp.PullStrap)
		{
			StopPulling(Entity<PullableComponent>.op_Implicit(ent), Entity<PullableComponent>.op_Implicit(ent));
		}
	}

	private void OnGotBuckled(Entity<PullableComponent> ent, ref BuckledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? puller = ent.Comp.Puller;
		if (puller.HasValue)
		{
			EntityUid puller2 = puller.GetValueOrDefault();
			puller = _rmcPulling.TryRetargetPull(puller2, Entity<PullableComponent>.op_Implicit(ent));
			if (puller.HasValue)
			{
				EntityUid retarget = puller.GetValueOrDefault();
				TryStartPull(puller2, retarget);
				return;
			}
		}
		StopPulling(Entity<PullableComponent>.op_Implicit(ent), Entity<PullableComponent>.op_Implicit(ent));
	}

	private void OnAfterState(Entity<PullerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Pulling.HasValue)
		{
			((EntitySystem)this).RemComp<ActivePullerComponent>(ent.Owner);
		}
		else
		{
			((EntitySystem)this).EnsureComp<ActivePullerComponent>(ent.Owner);
		}
	}

	private void OnDropHandItems(EntityUid uid, PullerComponent pullerComp, DropHandItemsEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullableComp = default(PullableComponent);
		if (pullerComp.Pulling.HasValue && !pullerComp.NeedsHands && ((EntitySystem)this).TryComp<PullableComponent>(pullerComp.Pulling, ref pullableComp))
		{
			TryStopPull(pullerComp.Pulling.Value, pullableComp, uid);
		}
	}

	private void OnStopPullingAlert(Entity<PullerComponent> ent, ref StopPullingAlertEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<PullableComponent>(ent.Comp.Pulling, ref pullable))
		{
			((HandledEntityEventArgs)args).Handled = TryStopPull(ent.Comp.Pulling.Value, pullable, Entity<PullerComponent>.op_Implicit(ent));
		}
	}

	private void OnPullerContainerInsert(Entity<PullerComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pulling = default(PullableComponent);
		if (ent.Comp.Pulling.HasValue && ((EntitySystem)this).TryComp<PullableComponent>(ent.Comp.Pulling.Value, ref pulling))
		{
			TryStopPull(ent.Comp.Pulling.Value, pulling, ent.Owner);
		}
	}

	private void OnPullableContainerInsert(Entity<PullableComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryStopPull(ent.Owner, ent.Comp);
	}

	private void OnModifyUncuffDuration(Entity<PullableComponent> ent, ref ModifyUncuffDurationEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BeingPulled && !(args.User != args.Target))
		{
			args.Duration *= 2f;
		}
	}

	private void OnStopBeingPulledAlert(Entity<PullableComponent> ent, ref StopBeingPulledAlertEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryStopPull(Entity<PullableComponent>.op_Implicit(ent), Entity<PullableComponent>.op_Implicit(ent), Entity<PullableComponent>.op_Implicit(ent));
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<PullingSystem>();
	}

	private void OnPullerUnpaused(EntityUid uid, PullerComponent component, ref EntityUnpausedEvent args)
	{
		component.NextThrow += args.PausedTime;
	}

	private void OnVirtualItemDeleted(EntityUid uid, PullerComponent component, VirtualItemDeletedEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && component.Pulling.HasValue)
		{
			EntityUid? pulling = component.Pulling;
			EntityUid blockingEntity = args.BlockingEntity;
			PullableComponent comp = default(PullableComponent);
			if (pulling.HasValue && !(pulling.GetValueOrDefault() != blockingEntity) && ((EntitySystem)this).TryComp<PullableComponent>(args.BlockingEntity, ref comp))
			{
				TryStopPull(args.BlockingEntity, comp);
			}
		}
	}

	private void AddPullVerbs(EntityUid uid, PullableComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.User == args.Target)
		{
			return;
		}
		EntityUid? puller = component.Puller;
		EntityUid user = args.User;
		if (puller.HasValue && puller.GetValueOrDefault() == user)
		{
			Verb verb = new Verb
			{
				Text = base.Loc.GetString("pulling-verb-get-data-text-stop-pulling"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					TryStopPull(uid, component, args.User);
				},
				DoContactInteraction = false
			};
			args.Verbs.Add(verb);
		}
		else if (CanPull(args.User, args.Target))
		{
			Verb verb2 = new Verb
			{
				Text = base.Loc.GetString("pulling-verb-get-data-text"),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					TryStartPull(args.User, args.Target);
				},
				DoContactInteraction = false
			};
			args.Verbs.Add(verb2);
		}
	}

	private void OnRefreshMovespeed(EntityUid uid, PullerComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		HeldSpeedModifierComponent heldMoveSpeed = default(HeldSpeedModifierComponent);
		if (((EntitySystem)this).TryComp<HeldSpeedModifierComponent>(component.Pulling, ref heldMoveSpeed) && component.Pulling.HasValue)
		{
			var (walkMod, sprintMod) = _clothingMoveSpeed.GetHeldMovementSpeedModifiers(component.Pulling.Value, heldMoveSpeed);
			args.ModifySpeed(walkMod, sprintMod);
		}
		else
		{
			args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
		}
	}

	private void OnPullableMoveInput(EntityUid uid, PullableComponent component, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (component.BeingPulled)
		{
			Entity<InputMoverComponent> entity = args.Entity;
			if (_blocker.CanMove(Entity<InputMoverComponent>.op_Implicit(entity)))
			{
				TryStopPull(uid, component, uid);
			}
		}
	}

	private void OnPullableCollisionChange(EntityUid uid, PullableComponent component, ref CollisionChangeEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && component.PullJointId != null && !args.CanCollide)
		{
			_joints.RemoveJoint(uid, component.PullJointId);
		}
	}

	private void OnJointRemoved(EntityUid uid, PullableComponent component, JointRemovedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? puller = component.Puller;
		EntityUid otherEntity = args.OtherEntity;
		if (puller.HasValue && !(puller.GetValueOrDefault() != otherEntity) && !(args.Joint.ID != component.PullJointId) && !_timing.ApplyingState && !(args.Joint.ID != component.PullJointId) && component.Puller.HasValue)
		{
			StopPulling(uid, component);
		}
	}

	private void StopPulling(EntityUid pullableUid, PullableComponent pullableComp)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!pullableComp.Puller.HasValue)
		{
			return;
		}
		if (!_timing.ApplyingState)
		{
			if (pullableComp.PullJointId != null)
			{
				_joints.RemoveJoint(pullableUid, pullableComp.PullJointId);
				pullableComp.PullJointId = null;
			}
			PhysicsComponent pullablePhysics = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(pullableUid, ref pullablePhysics))
			{
				_physics.SetFixedRotation(pullableUid, pullableComp.PrevFixedRotation, true, (FixturesComponent)null, pullablePhysics);
			}
		}
		EntityUid? oldPuller = pullableComp.Puller;
		if (oldPuller.HasValue)
		{
			((EntitySystem)this).RemComp<ActivePullerComponent>(oldPuller.Value);
		}
		pullableComp.PullJointId = null;
		pullableComp.Puller = null;
		((EntitySystem)this).Dirty(pullableUid, (IComponent)(object)pullableComp, (MetaDataComponent)null);
		PullerComponent pullerComp = default(PullerComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(oldPuller, ref pullerComp))
		{
			EntityUid pullerUid = oldPuller.Value;
			_alertsSystem.ClearAlert(pullerUid, pullerComp.PullingAlert);
			pullerComp.Pulling = null;
			((EntitySystem)this).Dirty(oldPuller.Value, (IComponent)(object)pullerComp, (MetaDataComponent)null);
			PullStoppedMessage message = new PullStoppedMessage(pullerUid, pullableUid);
			_modifierSystem.RefreshMovementSpeedModifiers(pullerUid);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(17, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(pullerUid)), "user", "ToPrettyString(pullerUid)");
			handler.AppendLiteral(" stopped pulling ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(pullableUid)), "target", "ToPrettyString(pullableUid)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
			((EntitySystem)this).RaiseLocalEvent<PullStoppedMessage>(pullerUid, message, false);
			((EntitySystem)this).RaiseLocalEvent<PullStoppedMessage>(pullableUid, message, false);
		}
		_alertsSystem.ClearAlert(pullableUid, pullableComp.PulledAlert);
	}

	public bool IsPulled(EntityUid uid, PullableComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PullableComponent>(uid, ref component, false))
		{
			return component.BeingPulled;
		}
		return false;
	}

	public bool IsPulling(EntityUid puller, PullerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PullerComponent>(puller, ref component, false))
		{
			return component.Pulling.HasValue;
		}
		return false;
	}

	public EntityUid? GetPuller(EntityUid puller, PullableComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PullableComponent>(puller, ref component, false))
		{
			return component.Puller;
		}
		return null;
	}

	public EntityUid? GetPulling(EntityUid puller, PullerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PullerComponent>(puller, ref component, false))
		{
			return component.Pulling;
		}
		return null;
	}

	private void OnReleasePulledObject(ICommonSession? session)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			EntityUid player = val.GetValueOrDefault();
			PullerComponent pullerComp = default(PullerComponent);
			PullableComponent pullableComp = default(PullableComponent);
			if (((EntityUid)(ref player)).Valid && ((EntitySystem)this).TryComp<PullerComponent>(player, ref pullerComp) && ((EntitySystem)this).TryComp<PullableComponent>(pullerComp.Pulling, ref pullableComp))
			{
				TryStopPull(pullerComp.Pulling.Value, pullableComp, player);
			}
		}
	}

	public bool CanPull(EntityUid puller, EntityUid pullableUid, PullerComponent? pullerComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PullerComponent>(puller, ref pullerComp, false))
		{
			return false;
		}
		if (pullerComp.NeedsHands && !_handsSystem.TryGetEmptyHand(Entity<HandsComponent>.op_Implicit(puller), out string _) && !pullerComp.Pulling.HasValue)
		{
			return false;
		}
		if (!_blocker.CanInteract(puller, pullableUid))
		{
			return false;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(pullableUid, ref physics))
		{
			return false;
		}
		if ((int)physics.BodyType == 4)
		{
			return false;
		}
		if (puller == pullableUid)
		{
			return false;
		}
		if (!_containerSystem.IsInSameOrNoContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(puller), Entity<TransformComponent, MetaDataComponent>.op_Implicit(pullableUid)))
		{
			return false;
		}
		BeingPulledAttemptEvent getPulled = new BeingPulledAttemptEvent(puller, pullableUid);
		((EntitySystem)this).RaiseLocalEvent<BeingPulledAttemptEvent>(pullableUid, getPulled, true);
		StartPullAttemptEvent startPull = new StartPullAttemptEvent(puller, pullableUid);
		((EntitySystem)this).RaiseLocalEvent<StartPullAttemptEvent>(puller, startPull, true);
		if (!((CancellableEntityEventArgs)startPull).Cancelled)
		{
			return !((CancellableEntityEventArgs)getPulled).Cancelled;
		}
		return false;
	}

	public bool TogglePull(Entity<PullableComponent?> pullable, EntityUid pullerUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PullableComponent>(Entity<PullableComponent>.op_Implicit(pullable), ref pullable.Comp, false))
		{
			return false;
		}
		EntityUid? puller = pullable.Comp.Puller;
		if (puller.HasValue && puller.GetValueOrDefault() == pullerUid)
		{
			RMCPullToggleEvent ev = default(RMCPullToggleEvent);
			((EntitySystem)this).RaiseLocalEvent<RMCPullToggleEvent>(pullerUid, ref ev, false);
			if (ev.Handled)
			{
				return true;
			}
			return TryStopPull(Entity<PullableComponent>.op_Implicit(pullable), pullable.Comp);
		}
		return TryStartPull(pullerUid, Entity<PullableComponent>.op_Implicit(pullable), null, Entity<PullableComponent>.op_Implicit(pullable));
	}

	public bool TogglePull(EntityUid pullerUid, PullerComponent puller)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (!((EntitySystem)this).TryComp<PullableComponent>(puller.Pulling, ref pullable))
		{
			return false;
		}
		return TogglePull(Entity<PullableComponent>.op_Implicit((puller.Pulling.Value, pullable)), pullerUid);
	}

	public bool TryStartPull(EntityUid pullerUid, EntityUid pullableUid, PullerComponent? pullerComp = null, PullableComponent? pullableComp = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		RMCGetPullTargetEvent ev = new RMCGetPullTargetEvent(pullerUid, pullableUid);
		((EntitySystem)this).RaiseLocalEvent<RMCGetPullTargetEvent>(pullableUid, ref ev, false);
		if (pullableUid != ev.Target)
		{
			pullableUid = ev.Target;
			pullableComp = ((EntitySystem)this).CompOrNull<PullableComponent>(pullableUid);
		}
		if (!((EntitySystem)this).Resolve<PullerComponent>(pullerUid, ref pullerComp, false) || !((EntitySystem)this).Resolve<PullableComponent>(pullableUid, ref pullableComp, false))
		{
			return false;
		}
		EntityUid? pulling = pullerComp.Pulling;
		EntityUid val = pullableUid;
		if (pulling.HasValue && pulling.GetValueOrDefault() == val)
		{
			return true;
		}
		if (!CanPull(pullerUid, pullableUid))
		{
			return false;
		}
		PhysicsComponent pullerPhysics = default(PhysicsComponent);
		PhysicsComponent pullablePhysics = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(pullerUid, ref pullerPhysics) || !((EntitySystem)this).TryComp<PhysicsComponent>(pullableUid, ref pullablePhysics))
		{
			return false;
		}
		PullableComponent oldPullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(pullerComp.Pulling, ref oldPullable) && !TryStopPull(pullerComp.Pulling.Value, oldPullable, pullerUid))
		{
			return false;
		}
		if (pullableComp.Puller.HasValue)
		{
			pulling = pullableComp.Puller;
			val = pullerUid;
			if (pulling.HasValue && pulling.GetValueOrDefault() == val)
			{
				return false;
			}
			if (!TryStopPull(pullableUid, pullableComp, pullableComp.Puller))
			{
				return false;
			}
		}
		PullAttemptEvent pullAttempt = new PullAttemptEvent(pullerUid, pullableUid);
		((EntitySystem)this).RaiseLocalEvent<PullAttemptEvent>(pullerUid, pullAttempt, false);
		if (pullAttempt.Cancelled)
		{
			return false;
		}
		((EntitySystem)this).RaiseLocalEvent<PullAttemptEvent>(pullableUid, pullAttempt, false);
		if (pullAttempt.Cancelled)
		{
			return false;
		}
		_interaction.DoContactInteraction(pullableUid, pullerUid);
		pullableComp.PullJointId = $"pull-joint-{((EntitySystem)this).GetNetEntity(pullableUid, (MetaDataComponent)null)}";
		((EntitySystem)this).EnsureComp<ActivePullerComponent>(pullerUid);
		pullerComp.Pulling = pullableUid;
		pullableComp.Puller = pullerUid;
		pullableComp.PrevFixedRotation = pullablePhysics.FixedRotation;
		if (!_timing.ApplyingState)
		{
			DistanceJoint obj = _joints.CreateDistanceJoint(pullableUid, pullerUid, (Vector2?)pullablePhysics.LocalCenter, (Vector2?)pullerPhysics.LocalCenter, pullableComp.PullJointId, (TransformComponent)null, (TransformComponent)null, (float?)1f);
			((Joint)obj).CollideConnected = false;
			obj.MaxLength = obj.Length + 0.15f;
			obj.MinLength = 0f;
			obj.Stiffness = 0f;
			_physics.SetFixedRotation(pullableUid, pullableComp.FixedRotationOnPull, true, (FixturesComponent)null, pullablePhysics);
		}
		PullStartedMessage message = new PullStartedMessage(pullerUid, pullableUid);
		_modifierSystem.RefreshMovementSpeedModifiers(pullerUid);
		_alertsSystem.ShowAlert(pullerUid, pullerComp.PullingAlert);
		_alertsSystem.ShowAlert(pullableUid, pullableComp.PulledAlert);
		((EntitySystem)this).RaiseLocalEvent<PullStartedMessage>(pullerUid, message, false);
		((EntitySystem)this).RaiseLocalEvent<PullStartedMessage>(pullableUid, message, false);
		((EntitySystem)this).Dirty(pullerUid, (IComponent)(object)pullerComp, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(pullableUid, (IComponent)(object)pullableComp, (MetaDataComponent)null);
		string pullingMessage = base.Loc.GetString("getting-pulled-popup", (ValueTuple<string, object>)("puller", Identity.Entity(pullerUid, (IEntityManager)(object)base.EntityManager)));
		_popup.PopupEntity(pullingMessage, pullableUid, pullableUid);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(17, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(pullerUid)), "user", "ToPrettyString(pullerUid)");
		handler.AppendLiteral(" started pulling ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(pullableUid)), "target", "ToPrettyString(pullableUid)");
		adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		return true;
	}

	public bool TryStopPull(EntityUid pullableUid, PullableComponent pullable, EntityUid? user = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? pullerUidNull = pullable.Puller;
		if (!pullerUidNull.HasValue)
		{
			return true;
		}
		if (user.HasValue && !_blocker.CanInteract(user.Value, pullableUid))
		{
			return false;
		}
		AttemptStopPullingEvent msg = new AttemptStopPullingEvent(user);
		((EntitySystem)this).RaiseLocalEvent<AttemptStopPullingEvent>(pullableUid, msg, true);
		if (msg.Cancelled)
		{
			return false;
		}
		StopPulling(pullableUid, pullable);
		return true;
	}
}
