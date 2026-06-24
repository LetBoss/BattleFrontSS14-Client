using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.CCVar;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Events;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Content.Shared.Climbing.Systems;

public sealed class ClimbSystem : VirtualController
{
	[Serializable]
	[NetSerializable]
	private sealed class ClimbDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<ClimbDoAfterEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref ClimbDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (ClimbDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<ClimbDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref ClimbDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ClimbDoAfterEvent cast = (ClimbDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			ClimbDoAfterEvent cast = (ClimbDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override ClimbDoAfterEvent Instantiate()
		{
			return new ClimbDoAfterEvent();
		}
	}

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private FixtureSystem _fixtureSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	private SharedTransformSystem _xformSystem;

	[Dependency]
	private RMCMovementSystem _rmcMovement;

	[Dependency]
	private INetConfigurationManager _netConfig;

	private const string ClimbingFixtureName = "climb";

	private const int ClimbingCollisionGroup = 67108884;

	private EntityQuery<ClimbableComponent> _climbableQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).Initialize();
		_climbableQuery = ((EntitySystem)this).GetEntityQuery<ClimbableComponent>();
		_fixturesQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, UpdateCanMoveEvent>((ComponentEventHandler<ClimbingComponent, UpdateCanMoveEvent>)OnMoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, EntParentChangedMessage>((ComponentEventRefHandler<ClimbingComponent, EntParentChangedMessage>)OnParentChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, ClimbDoAfterEvent>((ComponentEventHandler<ClimbingComponent, ClimbDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, EndCollideEvent>((ComponentEventRefHandler<ClimbingComponent, EndCollideEvent>)OnClimbEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, BuckledEvent>((ComponentEventRefHandler<ClimbingComponent, BuckledEvent>)OnBuckled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, EntGotInsertedIntoContainerMessage>((ComponentEventRefHandler<ClimbingComponent, EntGotInsertedIntoContainerMessage>)OnStored, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbingComponent, RMCCombatModeInteractOverrideUserEvent>((ComponentEventRefHandler<ClimbingComponent, RMCCombatModeInteractOverrideUserEvent>)OnCombatModeInteractOverride, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbableComponent, CanDropTargetEvent>((ComponentEventRefHandler<ClimbableComponent, CanDropTargetEvent>)OnCanDragDropOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<ClimbableComponent, GetVerbsEvent<AlternativeVerb>>)AddClimbableVerb, (Type[])null, (Type[])null);
		ComponentEventRefHandler<ClimbableComponent, InteractHandEvent> obj = OnClimbableInteractHand;
		Type[] array = new Type[1] { typeof(SharedBuckleSystem) };
		((EntitySystem)this).SubscribeLocalEvent<ClimbableComponent, InteractHandEvent>(obj, new Type[1] { typeof(InteractionPopupSystem) }, array);
		((EntitySystem)this).SubscribeLocalEvent<ClimbableComponent, DragDropTargetEvent>((ComponentEventRefHandler<ClimbableComponent, DragDropTargetEvent>)OnClimbableDragDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClimbableComponent, StartCollideEvent>((ComponentEventRefHandler<ClimbableComponent, StartCollideEvent>)OnClimbableStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GlassTableComponent, ClimbedOnEvent>((ComponentEventRefHandler<GlassTableComponent, ClimbedOnEvent>)OnGlassClimbed, (Type[])null, (Type[])null);
	}

	public override void UpdateBeforeSolve(bool prediction, float frameTime)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).UpdateBeforeSolve(prediction, frameTime);
		EntityQueryEnumerator<ClimbingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ClimbingComponent>();
		TimeSpan curTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		ClimbingComponent comp = default(ClimbingComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.NextTransition.HasValue)
			{
				if (comp.NextTransition < curTime)
				{
					FinishTransition(uid, comp);
					continue;
				}
				TransformComponent xform = _xformQuery.GetComponent(uid);
				_xformSystem.SetLocalPosition(uid, xform.LocalPosition + comp.Direction * frameTime, xform);
			}
		}
	}

	private void FinishTransition(EntityUid uid, ClimbingComponent comp)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		comp.NextTransition = null;
		_actionBlockerSystem.UpdateCanMove(uid);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		FixturesComponent fixtures = default(FixturesComponent);
		if (!_fixturesQuery.TryGetComponent(uid, ref fixtures) || !IsClimbing(uid, fixtures))
		{
			StopClimb(uid, comp);
		}
	}

	private bool IsClimbing(EntityUid uid, FixturesComponent? fixturesComp = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!_fixturesQuery.Resolve(uid, ref fixturesComp, true) || !fixturesComp.Fixtures.TryGetValue("climb", out var climbFixture))
		{
			return false;
		}
		foreach (Contact contact in climbFixture.Contacts.Values)
		{
			EntityUid other = ((uid == contact.EntityA) ? contact.EntityB : contact.EntityA);
			if (((EntitySystem)this).HasComp<ClimbableComponent>(other))
			{
				return true;
			}
		}
		return false;
	}

	private void OnMoveAttempt(EntityUid uid, ClimbingComponent component, UpdateCanMoveEvent args)
	{
		if (component.NextTransition.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnParentChange(EntityUid uid, ClimbingComponent component, ref EntParentChangedMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (component.NextTransition.HasValue)
		{
			FinishTransition(uid, component);
		}
	}

	private void OnCanDragDropOn(EntityUid uid, ClimbableComponent component, ref CanDropTargetEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ClimbingComponent climbing = default(ClimbingComponent);
		if (!args.Handled && component.Vaultable && (!((EntitySystem)this).TryComp<ClimbingComponent>(args.Dragged, ref climbing) || !climbing.IsClimbing))
		{
			string reason;
			bool canVault = ((args.User == args.Dragged) ? CanVault(component, args.User, uid, out reason) : CanVault(component, args.User, args.Dragged, uid, out reason));
			args.CanDrop = canVault;
			if (!((EntitySystem)this).HasComp<HandsComponent>(args.User))
			{
				args.CanDrop = false;
			}
			args.Handled = true;
		}
	}

	private void AddClimbableVerb(EntityUid uid, ClimbableComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		ClimbingComponent climbingComponent = default(ClimbingComponent);
		if (args.CanAccess && args.CanInteract && _actionBlockerSystem.CanMove(args.User) && component.Vaultable && ((EntitySystem)this).TryComp<ClimbingComponent>(args.User, ref climbingComponent) && !climbingComponent.IsClimbing && climbingComponent.CanClimb)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryClimb(args.User, args.User, args.Target, out var _, component);
				},
				Text = ((EntitySystem)this).Loc.GetString("comp-climbable-verb-climb")
			});
		}
	}

	private void OnCombatModeInteractOverride(EntityUid uid, ClimbingComponent component, ref RMCCombatModeInteractOverrideUserEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ClimbableComponent climbable = default(ClimbableComponent);
		if (args.Target.HasValue && _climbableQuery.TryComp(args.Target.Value, ref climbable) && CanTryManualClimb(uid, args.Target.Value, climbable, component, ignoreCombatMode: true))
		{
			args.CanInteract = true;
			args.Handled = true;
		}
	}

	private void OnClimbableInteractHand(EntityUid uid, ClimbableComponent component, ref InteractHandEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && CanTryManualClimb(args.User, uid, component))
		{
			((HandledEntityEventArgs)args).Handled = TryClimb(args.User, args.User, uid, out var _, component);
		}
	}

	private void OnClimbableDragDrop(EntityUid uid, ClimbableComponent component, ref DragDropTargetEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			TryClimb(args.User, args.Dragged, uid, out var _, component);
		}
	}

	private void OnClimbableStartCollide(EntityUid uid, ClimbableComponent component, ref StartCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		EntityUid climber = args.OtherEntity;
		ActorComponent actor = default(ActorComponent);
		ClimbingComponent climbing = default(ClimbingComponent);
		CombatModeComponent combat = default(CombatModeComponent);
		PullerComponent puller = default(PullerComponent);
		PullableComponent pullable = default(PullableComponent);
		InputMoverComponent mover = default(InputMoverComponent);
		if (climber == uid || !((EntitySystem)this).TryComp<ActorComponent>(climber, ref actor) || !_netConfig.GetClientCVar<bool>(actor.PlayerSession.Channel, CCVars.PubgAutoClimbEnabled) || !((EntitySystem)this).TryComp<ClimbingComponent>(climber, ref climbing) || !climbing.CanClimb || climbing.IsClimbing || climbing.DoAfter.HasValue || (((EntitySystem)this).TryComp<CombatModeComponent>(climber, ref combat) && combat.IsInCombatMode) || (((EntitySystem)this).TryComp<PullerComponent>(climber, ref puller) && puller.Pulling.HasValue) || (((EntitySystem)this).TryComp<PullableComponent>(climber, ref pullable) && pullable.BeingPulled) || !((EntitySystem)this).TryComp<InputMoverComponent>(climber, ref mover) || !mover.HasDirectionalMovement)
		{
			return;
		}
		Vector2 wishDir = mover.WishDir;
		if (!(wishDir.LengthSquared() < 0.001f))
		{
			MapCoordinates climberCoords = _xformSystem.GetMapCoordinates(climber, (TransformComponent)null);
			Vector2 toClimbable = _xformSystem.GetMapCoordinates(uid, (TransformComponent)null).Position - climberCoords.Position;
			if (!(toClimbable.LengthSquared() < 0.0001f) && !(Vector2.Dot(Vector2.Normalize(wishDir), Vector2.Normalize(toClimbable)) < 0.6f) && CanVault(component, climber, uid, out string _) && _rmcMovement.CanClimbOver(climber, climber, uid, includeTarget: true, popup: false))
			{
				TryClimb(climber, climber, uid, out var _, component, climbing);
			}
		}
	}

	private bool CanTryManualClimb(EntityUid user, EntityUid climbable, ClimbableComponent? climbableComponent = null, ClimbingComponent? climbingComponent = null, bool ignoreCombatMode = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ClimbableComponent>(climbable, ref climbableComponent, false) || !((EntitySystem)this).Resolve<ClimbingComponent>(user, ref climbingComponent, false))
		{
			return false;
		}
		if (!climbableComponent.Vaultable || climbingComponent.IsClimbing || climbingComponent.DoAfter.HasValue || !climbingComponent.CanClimb)
		{
			return false;
		}
		if (!_actionBlockerSystem.CanMove(user))
		{
			return false;
		}
		if (_interactionSystem.TryGetUsedEntity(user, out var _, checkCanUse: false))
		{
			return false;
		}
		CombatModeComponent combat = default(CombatModeComponent);
		if (!ignoreCombatMode && ((EntitySystem)this).TryComp<CombatModeComponent>(user, ref combat) && combat.IsInCombatMode)
		{
			return false;
		}
		PullerComponent puller = default(PullerComponent);
		if (((EntitySystem)this).TryComp<PullerComponent>(user, ref puller) && puller.Pulling.HasValue)
		{
			return false;
		}
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(user, ref pullable) && pullable.BeingPulled)
		{
			return false;
		}
		return true;
	}

	public bool TryClimb(EntityUid user, EntityUid entityToMove, EntityUid climbable, out DoAfterId? id, ClimbableComponent? comp = null, ClimbingComponent? climbing = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		id = null;
		if (!((EntitySystem)this).Resolve<ClimbableComponent>(climbable, ref comp, true) || !((EntitySystem)this).Resolve<ClimbingComponent>(entityToMove, ref climbing, false))
		{
			return false;
		}
		if (!((user == entityToMove) ? CanVault(comp, user, climbable, out string reason) : CanVault(comp, user, entityToMove, climbable, out reason)))
		{
			_popupSystem.PopupClient(reason, user, user);
			return false;
		}
		if (climbing.IsClimbing)
		{
			return true;
		}
		if (!_rmcMovement.CanClimbOver(user, entityToMove, climbable))
		{
			return false;
		}
		DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)((EntitySystem)this).EntityManager, user, comp.ClimbDelay, new ClimbDoAfterEvent(), entityToMove, climbable, entityToMove)
		{
			BreakOnMove = true,
			DuplicateCondition = (DuplicateConditions.SameTool | DuplicateConditions.SameTarget)
		};
		_audio.PlayPredicted(comp.StartClimbSound, climbable, (EntityUid?)user, (AudioParams?)null);
		bool num = _doAfterSystem.TryStartDoAfter(args, out id);
		if (num)
		{
			climbing.DoAfter = id;
		}
		return num;
	}

	private void OnDoAfter(EntityUid uid, ClimbingComponent component, ClimbDoAfterEvent args)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		component.DoAfter = null;
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Args.Target.HasValue && args.Args.Used.HasValue)
		{
			if (_containers.IsEntityInContainer(uid, (MetaDataComponent)null))
			{
				((HandledEntityEventArgs)args).Handled = true;
				return;
			}
			Climb(uid, args.Args.User, args.Args.Target.Value, silent: false, component);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public void Climb(EntityUid uid, EntityUid user, EntityUid climbable, bool silent = false, ClimbingComponent? climbing = null, PhysicsComponent? physics = null, FixturesComponent? fixtures = null, ClimbableComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ClimbingComponent, PhysicsComponent, FixturesComponent>(uid, ref climbing, ref physics, ref fixtures, false) || !((EntitySystem)this).Resolve<ClimbableComponent>(climbable, ref comp, false))
		{
			return;
		}
		SelfBeforeClimbEvent selfEvent = new SelfBeforeClimbEvent(uid, user, Entity<ClimbableComponent>.op_Implicit((climbable, comp)));
		((EntitySystem)this).RaiseLocalEvent<SelfBeforeClimbEvent>(uid, selfEvent, false);
		if (((CancellableEntityEventArgs)selfEvent).Cancelled)
		{
			return;
		}
		TargetBeforeClimbEvent targetEvent = new TargetBeforeClimbEvent(uid, user, Entity<ClimbableComponent>.op_Implicit((climbable, comp)));
		((EntitySystem)this).RaiseLocalEvent<TargetBeforeClimbEvent>(climbable, targetEvent, false);
		if (((CancellableEntityEventArgs)targetEvent).Cancelled || !ReplaceFixtures(uid, climbing, fixtures))
		{
			return;
		}
		TransformComponent xform = _xformQuery.GetComponent(uid);
		ValueTuple<Vector2, Angle> worldPositionRotation = _xformSystem.GetWorldPositionRotation(xform);
		Vector2 worldPos = worldPositionRotation.Item1;
		Angle item = worldPositionRotation.Item2;
		Vector2 worldDirection = _xformSystem.GetWorldPosition(climbable) - worldPos;
		float distance = worldDirection.Length();
		Angle val = -(item - xform.LocalRotation);
		Vector2 localDirection = ((Angle)(ref val)).RotateVec(ref worldDirection);
		if (localDirection.LengthSquared() < 0.01f)
		{
			climbing.NextTransition = null;
		}
		else
		{
			TimeSpan climbDuration = TimeSpan.FromSeconds((distance + comp.VaultPastDistance) / climbing.TransitionRate);
			climbing.NextTransition = _timing.CurTime + climbDuration;
			climbing.Direction = Vector2Helpers.Normalized(localDirection) * climbing.TransitionRate;
			_actionBlockerSystem.UpdateCanMove(uid);
		}
		climbing.IsClimbing = true;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)climbing, (MetaDataComponent)null);
		_audio.PlayPredicted(comp.FinishClimbSound, climbable, (EntityUid?)user, (AudioParams?)null);
		StartClimbEvent startEv = new StartClimbEvent(climbable);
		ClimbedOnEvent climbedEv = new ClimbedOnEvent(uid, user);
		((EntitySystem)this).RaiseLocalEvent<StartClimbEvent>(uid, ref startEv, false);
		((EntitySystem)this).RaiseLocalEvent<ClimbedOnEvent>(climbable, ref climbedEv, false);
		if (!silent)
		{
			string othersMessage;
			string selfMessage;
			if (user == uid)
			{
				othersMessage = ((EntitySystem)this).Loc.GetString("comp-climbable-user-climbs-other", (ValueTuple<string, object>)("user", Identity.Entity(uid, (IEntityManager)(object)((EntitySystem)this).EntityManager)), (ValueTuple<string, object>)("climbable", climbable));
				selfMessage = ((EntitySystem)this).Loc.GetString("comp-climbable-user-climbs", (ValueTuple<string, object>)("climbable", climbable));
			}
			else
			{
				othersMessage = ((EntitySystem)this).Loc.GetString("comp-climbable-user-climbs-force-other", new(string, object)[3]
				{
					("user", Identity.Entity(user, (IEntityManager)(object)((EntitySystem)this).EntityManager)),
					("moved-user", Identity.Entity(uid, (IEntityManager)(object)((EntitySystem)this).EntityManager)),
					("climbable", climbable)
				});
				selfMessage = ((EntitySystem)this).Loc.GetString("comp-climbable-user-climbs-force", (ValueTuple<string, object>)("moved-user", Identity.Entity(uid, (IEntityManager)(object)((EntitySystem)this).EntityManager)), (ValueTuple<string, object>)("climbable", climbable));
			}
			_popupSystem.PopupPredicted(selfMessage, othersMessage, uid, user);
		}
	}

	private bool ReplaceFixtures(EntityUid uid, ClimbingComponent climbingComp, FixturesComponent fixturesComp)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (name, fixture) in fixturesComp.Fixtures)
		{
			if (!climbingComp.DisabledFixtureMasks.ContainsKey(name) && fixture.Hard && (fixture.CollisionMask & 0x4000014) != 0)
			{
				climbingComp.DisabledFixtureMasks.Add(name, fixture.CollisionMask & 0x4000014);
				_physics.SetCollisionMask(uid, name, fixture, fixture.CollisionMask & -67108885, fixturesComp, (PhysicsComponent)null);
			}
		}
		if (!_fixtureSystem.TryCreateFixture(uid, (IPhysShape)new PhysShapeCircle(0.35f), "climb", 1f, false, 0, 67108884, 0.4f, 0f, true, fixturesComp, (PhysicsComponent)null, (TransformComponent)null))
		{
			return false;
		}
		return true;
	}

	private void OnClimbEndCollide(EntityUid uid, ClimbingComponent component, ref EndCollideEvent args)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (args.OurFixtureId != "climb" || !component.IsClimbing || component.NextTransition.HasValue)
		{
			return;
		}
		foreach (Contact contact in args.OurFixture.Contacts.Values)
		{
			if (contact.IsTouching)
			{
				EntityUid otherEnt = contact.OtherEnt(uid);
				var (otherFixtureId, otherFixture) = contact.OtherFixture(uid);
				if ((!(args.OtherEntity == otherEnt) || !(args.OtherFixtureId == otherFixtureId)) && otherFixture != null && otherFixture.Hard && _climbableQuery.HasComp(otherEnt))
				{
					return;
				}
			}
		}
		foreach (Fixture otherFixture2 in args.OurFixture.Contacts.Keys)
		{
			if (otherFixture2 != args.OtherFixture && ((EntitySystem)this).HasComp<ClimbableComponent>(otherFixture2.Owner))
			{
				return;
			}
		}
		StopClimb(uid, component);
	}

	private void StopClimb(EntityUid uid, ClimbingComponent? climbing = null, FixturesComponent? fixtures = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ClimbingComponent, FixturesComponent>(uid, ref climbing, ref fixtures, false))
		{
			return;
		}
		foreach (var (name, fixtureMask) in climbing.DisabledFixtureMasks)
		{
			if (fixtures.Fixtures.TryGetValue(name, out var fixture))
			{
				_physics.SetCollisionMask(uid, name, fixture, fixture.CollisionMask | fixtureMask, fixtures, (PhysicsComponent)null);
			}
		}
		climbing.DisabledFixtureMasks.Clear();
		_fixtureSystem.DestroyFixture(uid, "climb", true, (PhysicsComponent)null, fixtures, (TransformComponent)null);
		climbing.IsClimbing = false;
		climbing.NextTransition = null;
		EndClimbEvent ev = default(EndClimbEvent);
		((EntitySystem)this).RaiseLocalEvent<EndClimbEvent>(uid, ref ev, false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)climbing, (MetaDataComponent)null);
	}

	public bool CanVault(ClimbableComponent component, EntityUid user, EntityUid target, out string reason)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Vaultable)
		{
			reason = string.Empty;
			return false;
		}
		if (!_actionBlockerSystem.CanInteract(user, target))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-interact");
			return false;
		}
		ClimbingComponent climbingComp = default(ClimbingComponent);
		if (!((EntitySystem)this).TryComp<ClimbingComponent>(user, ref climbingComp) || !climbingComp.CanClimb)
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-climb");
			return false;
		}
		if (!_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), component.Range))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-reach");
			return false;
		}
		if (_containers.IsEntityInContainer(user, (MetaDataComponent)null))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-reach");
			return false;
		}
		reason = string.Empty;
		return true;
	}

	public bool CanVault(ClimbableComponent component, EntityUid user, EntityUid dragged, EntityUid target, out string reason)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		if (!_actionBlockerSystem.CanInteract(user, dragged) || !_actionBlockerSystem.CanInteract(user, target))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-interact");
			return false;
		}
		if (!((EntitySystem)this).HasComp<ClimbingComponent>(dragged))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-target-cant-climb", (ValueTuple<string, object>)("moved-user", Identity.Entity(dragged, (IEntityManager)(object)((EntitySystem)this).EntityManager)));
			return false;
		}
		if (!_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored) || !_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(dragged), component.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-reach");
			return false;
		}
		if (_containers.IsEntityInContainer(user, (MetaDataComponent)null) || _containers.IsEntityInContainer(dragged, (MetaDataComponent)null))
		{
			reason = ((EntitySystem)this).Loc.GetString("comp-climbable-cant-reach");
			return false;
		}
		reason = string.Empty;
		return true;
		bool Ignored(EntityUid entity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (!(entity == target) && !(entity == user))
			{
				return entity == dragged;
			}
			return true;
		}
	}

	public void ForciblySetClimbing(EntityUid uid, EntityUid climbable, ClimbingComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Climb(uid, uid, climbable, silent: true, component);
	}

	private void OnBuckled(EntityUid uid, ClimbingComponent component, ref BuckledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopOrCancelClimb(uid, component);
	}

	private void OnStored(EntityUid uid, ClimbingComponent component, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopOrCancelClimb(uid, component);
	}

	private void StopOrCancelClimb(EntityUid uid, ClimbingComponent component)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsClimbing)
		{
			StopClimb(uid, component);
		}
		else if (component.DoAfter.HasValue)
		{
			_doAfterSystem.Cancel(component.DoAfter);
			component.DoAfter = null;
		}
	}

	private void OnGlassClimbed(EntityUid uid, GlassTableComponent component, ref ClimbedOnEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(args.Climber, ref physics) || !(physics.Mass <= component.MassLimit))
		{
			_damageableSystem.TryChangeDamage(args.Climber, component.ClimberDamage, ignoreResistances: false, interruptsDoAfters: true, null, args.Climber);
			_damageableSystem.TryChangeDamage(uid, component.TableDamage, ignoreResistances: false, interruptsDoAfters: true, null, args.Climber);
			_stunSystem.TryParalyze(args.Climber, TimeSpan.FromSeconds(component.StunTime), refresh: true);
			_popupSystem.PopupEntity(((EntitySystem)this).Loc.GetString("glass-table-shattered-others", (ValueTuple<string, object>)("table", uid), (ValueTuple<string, object>)("climber", Identity.Entity(args.Climber, (IEntityManager)(object)((EntitySystem)this).EntityManager))), args.Climber, Filter.PvsExcept(args.Climber, 2f, (IEntityManager)null), recordReplay: true);
		}
	}
}
