using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Storage;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Players.RateLimiting;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Strip;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Wall;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Interaction;

public abstract class SharedInteractionSystem : EntitySystem
{
	public delegate bool Ignored(EntityUid entity);

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private ISharedChatManager _chat;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private PullingSystem _pullSystem;

	[Dependency]
	private RotateToFaceSystem _rotateToFaceSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedPhysicsSystem _broadphase;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedVerbSystem _verbSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedStrippableSystem _strippable;

	[Dependency]
	private SharedPlayerRateLimitManager _rateLimit;

	[Dependency]
	private TagSystem _tagSystem;

	[Dependency]
	private UseDelaySystem _useDelay;

	private EntityQuery<IgnoreUIRangeComponent> _ignoreUiRangeQuery;

	private EntityQuery<FixturesComponent> _fixtureQuery;

	private EntityQuery<ItemComponent> _itemQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<HandsComponent> _handsQuery;

	private EntityQuery<InteractionRelayComponent> _relayQuery;

	private EntityQuery<CombatModeComponent> _combatQuery;

	private EntityQuery<WallMountComponent> _wallMountQuery;

	private EntityQuery<UseDelayComponent> _delayQuery;

	private EntityQuery<ActivatableUIComponent> _uiQuery;

	private const CollisionGroup InRangeUnobstructedMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable;

	public const float InteractionRange = 1.5f;

	public const float InteractionRangeSquared = 2.25f;

	public const float MaxRaycastRange = 100f;

	public const string RateLimitKey = "Interaction";

	private static readonly ProtoId<TagPrototype> BypassInteractionRangeChecksTag = ProtoId<TagPrototype>.op_Implicit("BypassInteractionRangeChecks");

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private INetManager _net;

	private void InitializeBlocking()
	{
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, UpdateCanMoveEvent>((ComponentEventHandler<BlockMovementComponent, UpdateCanMoveEvent>)OnMoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, UseAttemptEvent>((ComponentEventHandler<BlockMovementComponent, UseAttemptEvent>)CancelEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, InteractionAttemptEvent>((EntityEventRefHandler<BlockMovementComponent, InteractionAttemptEvent>)CancelInteractEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, DropAttemptEvent>((ComponentEventHandler<BlockMovementComponent, DropAttemptEvent>)CancelEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, PickupAttemptEvent>((ComponentEventHandler<BlockMovementComponent, PickupAttemptEvent>)CancelEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, ChangeDirectionAttemptEvent>((ComponentEventHandler<BlockMovementComponent, ChangeDirectionAttemptEvent>)CancelEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, ComponentStartup>((ComponentEventHandler<BlockMovementComponent, ComponentStartup>)OnBlockingStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BlockMovementComponent, ComponentShutdown>((ComponentEventHandler<BlockMovementComponent, ComponentShutdown>)OnBlockingShutdown, (Type[])null, (Type[])null);
	}

	private void CancelInteractEvent(Entity<BlockMovementComponent> ent, ref InteractionAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BlockInteraction)
		{
			args.Cancelled = true;
		}
	}

	private void OnMoveAttempt(EntityUid uid, BlockMovementComponent component, UpdateCanMoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<RelayInputMoverComponent>(uid))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void CancelEvent(EntityUid uid, BlockMovementComponent component, CancellableEntityEventArgs args)
	{
		args.Cancel();
	}

	private void OnBlockingStartup(EntityUid uid, BlockMovementComponent component, ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_actionBlockerSystem.UpdateCanMove(uid);
	}

	private void OnBlockingShutdown(EntityUid uid, BlockMovementComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_actionBlockerSystem.UpdateCanMove(uid);
	}

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		_ignoreUiRangeQuery = ((EntitySystem)this).GetEntityQuery<IgnoreUIRangeComponent>();
		_fixtureQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		_itemQuery = ((EntitySystem)this).GetEntityQuery<ItemComponent>();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_handsQuery = ((EntitySystem)this).GetEntityQuery<HandsComponent>();
		_relayQuery = ((EntitySystem)this).GetEntityQuery<InteractionRelayComponent>();
		_combatQuery = ((EntitySystem)this).GetEntityQuery<CombatModeComponent>();
		_wallMountQuery = ((EntitySystem)this).GetEntityQuery<WallMountComponent>();
		_delayQuery = ((EntitySystem)this).GetEntityQuery<UseDelayComponent>();
		_uiQuery = ((EntitySystem)this).GetEntityQuery<ActivatableUIComponent>();
		((EntitySystem)this).SubscribeLocalEvent<BoundUserInterfaceCheckRangeEvent>((EntityEventRefHandler<BoundUserInterfaceCheckRangeEvent>)HandleUserInterfaceRangeCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserInterfaceComponent, BoundUserInterfaceMessageAttempt>((EntityEventRefHandler<UserInterfaceComponent, BoundUserInterfaceMessageAttempt>)OnBoundInterfaceInteractAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<InteractInventorySlotEvent>((EntitySessionEventHandler<InteractInventorySlotEvent>)HandleInteractInventorySlotEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>((ComponentEventHandler<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>)OnRemoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnremoveableComponent, GotUnequippedEvent>((ComponentEventHandler<UnremoveableComponent, GotUnequippedEvent>)OnUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnremoveableComponent, GotUnequippedHandEvent>((ComponentEventHandler<UnremoveableComponent, GotUnequippedHandEvent>)OnUnequipHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnremoveableComponent, DroppedEvent>((ComponentEventHandler<UnremoveableComponent, DroppedEvent>)OnDropped, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(ContentKeyFunctions.AltActivateItemInWorld, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleAltUseInteraction), true, false)).Bind(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleUseInteraction), true, false)).Bind(ContentKeyFunctions.ActivateItemInWorld, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleActivateItemInWorld), true, false))
			.Bind(ContentKeyFunctions.TryPullObject, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(HandleTryPullObject), true, false))
			.Register<SharedInteractionSystem>();
		_rateLimit.Register("Interaction", new RateLimitRegistration(CCVars.InteractionRateLimitPeriod, CCVars.InteractionRateLimitCount, null, CCVars.InteractionRateLimitAnnounceAdminsDelay, RateLimitAlertAdmins));
		InitializeBlocking();
	}

	private void RateLimitAlertAdmins(ICommonSession session)
	{
		_chat.SendAdminAlert(base.Loc.GetString("interaction-rate-limit-admin-announcement", (ValueTuple<string, object>)("player", session.Name)));
	}

	public override void Shutdown()
	{
		CommandBinds.Unregister<SharedInteractionSystem>();
		((EntitySystem)this).Shutdown();
	}

	private void OnBoundInterfaceInteractAttempt(Entity<UserInterfaceComponent> ent, ref BoundUserInterfaceMessageAttempt ev)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		ActivatableUIComponent aUiComp = default(ActivatableUIComponent);
		_uiQuery.TryComp(ev.Target, ref aUiComp);
		if (!_actionBlockerSystem.CanInteract(ev.Actor, ev.Target) && (!(ev.Message is OpenBoundInterfaceMessage) || !((EntitySystem)this).HasComp<GhostComponent>(ev.Actor) || (aUiComp != null && aUiComp.BlockSpectators)))
		{
			((CancellableEntityEventArgs)ev).Cancel();
		}
		else if (_ui.GetUiRange(Entity<UserInterfaceComponent>.op_Implicit(ev.Target), ev.UiKey) <= 0f && !IsAccessible(Entity<TransformComponent>.op_Implicit(ev.Actor), Entity<TransformComponent>.op_Implicit(ev.Target)))
		{
			((CancellableEntityEventArgs)ev).Cancel();
		}
		else
		{
			if (aUiComp == null)
			{
				return;
			}
			if (aUiComp.SingleUser && aUiComp.CurrentSingleUser.HasValue)
			{
				EntityUid? currentSingleUser = aUiComp.CurrentSingleUser;
				EntityUid actor = ev.Actor;
				if (!currentSingleUser.HasValue || currentSingleUser.GetValueOrDefault() != actor)
				{
					((CancellableEntityEventArgs)ev).Cancel();
					return;
				}
			}
			if (aUiComp.RequiresComplex && !_actionBlockerSystem.CanComplexInteract(ev.Actor))
			{
				((CancellableEntityEventArgs)ev).Cancel();
			}
		}
	}

	private bool UiRangeCheck(Entity<TransformComponent?> user, Entity<TransformComponent?> target, float range)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(target), ref target.Comp, true))
		{
			return false;
		}
		if (user.Owner == target.Owner)
		{
			return true;
		}
		if (target.Comp.ParentUid == user.Owner)
		{
			return true;
		}
		if (!InRangeAndAccessible(user, target, range))
		{
			return _ignoreUiRangeQuery.HasComp(Entity<TransformComponent>.op_Implicit(user));
		}
		return true;
	}

	private void OnRemoveAttempt(EntityUid uid, UnremoveableComponent item, ContainerGettingRemovedAttemptEvent args)
	{
		if (!_gameTiming.ApplyingState)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUnequip(EntityUid uid, UnremoveableComponent item, GotUnequippedEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!item.DeleteOnDrop)
		{
			((EntitySystem)this).RemCompDeferred<UnremoveableComponent>(uid);
		}
		else
		{
			((EntitySystem)this).PredictedQueueDel(uid);
		}
	}

	private void OnUnequipHand(EntityUid uid, UnremoveableComponent item, GotUnequippedHandEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!item.DeleteOnDrop)
		{
			((EntitySystem)this).RemCompDeferred<UnremoveableComponent>(uid);
		}
		else
		{
			((EntitySystem)this).PredictedQueueDel(uid);
		}
	}

	private void OnDropped(EntityUid uid, UnremoveableComponent item, DroppedEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!item.DeleteOnDrop)
		{
			((EntitySystem)this).RemCompDeferred<UnremoveableComponent>(uid);
		}
		else
		{
			((EntitySystem)this).PredictedQueueDel(uid);
		}
	}

	private bool HandleTryPullObject(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		_rmcLagCompensation.SendLastRealTick();
		if (!ValidateClientInput(session, coords, uid, out var userEntity))
		{
			((EntitySystem)this).Log.Info("TryPullObject input validation failed");
			return true;
		}
		if (userEntity.Value == uid)
		{
			return false;
		}
		if (((EntitySystem)this).Deleted(uid, (MetaDataComponent)null))
		{
			return false;
		}
		if (!InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(userEntity.Value), Entity<TransformComponent>.op_Implicit(uid), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return false;
		}
		_pullSystem.TogglePull(Entity<PullableComponent>.op_Implicit(uid), userEntity.Value);
		return false;
	}

	private void HandleInteractInventorySlotEvent(InteractInventorySlotEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid item = ((EntitySystem)this).GetEntity(msg.ItemUid);
		TransformComponent itemXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(item, ref itemXform) || !ValidateClientInput(((EntitySessionEventArgs)(ref args)).SenderSession, itemXform.Coordinates, item, out var user))
		{
			((EntitySystem)this).Log.Info($"Inventory interaction validation failed.  Session={((EntitySessionEventArgs)(ref args)).SenderSession}");
		}
		else if (msg.AltInteract)
		{
			UserInteraction(user.Value, itemXform.Coordinates, item, msg.AltInteract);
		}
		else
		{
			InteractionActivate(user.Value, item);
		}
	}

	public bool HandleAltUseInteraction(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_rmcLagCompensation.SendLastRealTick();
		if (!ValidateClientInput(session, coords, uid, out var user))
		{
			((EntitySystem)this).Log.Info("Alt-use input validation failed");
			return true;
		}
		UserInteraction(user.Value, coords, uid, altInteract: true, checkCanInteract: true, ShouldCheckAccess(user.Value));
		return false;
	}

	public bool HandleUseInteraction(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		_rmcLagCompensation.SendLastRealTick();
		if (!ValidateClientInput(session, coords, uid, out var userEntity))
		{
			((EntitySystem)this).Log.Info("Use input validation failed");
			return true;
		}
		UserInteraction(userEntity.Value, coords, (!((EntitySystem)this).Deleted(uid, (MetaDataComponent)null)) ? new EntityUid?(uid) : ((EntityUid?)null), altInteract: false, checkCanInteract: true, ShouldCheckAccess(userEntity.Value));
		return false;
	}

	private bool ShouldCheckAccess(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return !_tagSystem.HasTag(user, BypassInteractionRangeChecksTag);
	}

	public bool CombatModeCanHandInteract(EntityUid user, EntityUid? target)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		RMCCombatModeInteractOverrideUserEvent ev = new RMCCombatModeInteractOverrideUserEvent(target);
		((EntitySystem)this).RaiseLocalEvent<RMCCombatModeInteractOverrideUserEvent>(user, ref ev, false);
		if (ev.Handled)
		{
			return ev.CanInteract;
		}
		HandsComponent hands = default(HandsComponent);
		if (!target.HasValue || !_handsQuery.TryComp(user, ref hands) || _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit((user, hands))).HasValue)
		{
			return false;
		}
		if (!_itemQuery.HasComp(target))
		{
			return false;
		}
		CombatModeShouldHandInteractEvent combatEv = new CombatModeShouldHandInteractEvent(user);
		((EntitySystem)this).RaiseLocalEvent<CombatModeShouldHandInteractEvent>(target.Value, ref combatEv, false);
		if (combatEv.Cancelled)
		{
			return false;
		}
		return true;
	}

	public void UserInteraction(EntityUid user, EntityCoordinates coordinates, EntityUid? target, bool altInteract = false, bool checkCanInteract = true, bool checkAccess = true, bool checkCanUse = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		InteractionRelayComponent relay = default(InteractionRelayComponent);
		if (_relayQuery.TryComp(user, ref relay))
		{
			EntityUid? relayEntity = relay.RelayEntity;
			if (relayEntity.HasValue && _actionBlockerSystem.CanInteract(user, target))
			{
				UserInteraction(relay.RelayEntity.Value, coordinates, target, altInteract, checkCanInteract, checkAccess, checkCanUse);
				return;
			}
		}
		CombatModeComponent combatMode = default(CombatModeComponent);
		if ((target.HasValue && ((EntitySystem)this).Deleted(target.Value, (MetaDataComponent)null)) || (!altInteract && _combatQuery.TryComp(user, ref combatMode) && combatMode.IsInCombatMode && !CombatModeCanHandInteract(user, target)) || !ValidateInteractAndFace(user, coordinates))
		{
			return;
		}
		if (altInteract && target.HasValue)
		{
			AltInteract(user, target.Value);
		}
		else
		{
			if ((checkCanInteract && !_actionBlockerSystem.CanInteract(user, target)) || (checkAccess && target.HasValue && !IsAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target.Value))))
			{
				return;
			}
			bool inRangeUnobstructed = (target.HasValue ? (!checkAccess || InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target.Value))) : (!checkAccess || InRangeUnobstructed(user, coordinates)));
			if (!TryGetUsedEntity(user, out var used, checkCanUse))
			{
				if (inRangeUnobstructed && target.HasValue)
				{
					InteractHand(user, target.Value);
				}
				return;
			}
			EntityUid? relayEntity = target;
			EntityUid? val = used;
			if (relayEntity.HasValue == val.HasValue && (!relayEntity.HasValue || relayEntity.GetValueOrDefault() == val.GetValueOrDefault()))
			{
				UseInHandInteraction(user, target.Value, checkCanUse: false, checkCanInteract: false);
			}
			else if (inRangeUnobstructed && target.HasValue)
			{
				InteractUsing(user, used.Value, target.Value, coordinates, checkCanInteract: false, checkCanUse: false);
			}
			else
			{
				InteractUsingRanged(user, used.Value, target, coordinates, inRangeUnobstructed);
			}
		}
	}

	private bool IsDeleted(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
		{
			return base.EntityManager.IsQueuedForDeletion(uid);
		}
		return true;
	}

	private bool IsDeleted(EntityUid? uid)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (uid.HasValue)
		{
			return IsDeleted(uid.Value);
		}
		return false;
	}

	public void InteractHand(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (IsDeleted(user) || IsDeleted(target))
		{
			return;
		}
		bool complexInteractions = _actionBlockerSystem.CanComplexInteract(user);
		if (!complexInteractions)
		{
			InteractionActivate(user, target, checkCanInteract: false, checkUseDelay: true, checkAccess: false, complexInteractions, checkDeletion: false);
			return;
		}
		BeforeInteractHandEvent ev = new BeforeInteractHandEvent(target);
		((EntitySystem)this).RaiseLocalEvent<BeforeInteractHandEvent>(user, ev, false);
		if (((HandledEntityEventArgs)ev).Handled)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(55, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" interacted with ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(", but it was handled by another system");
			adminLogger.Add(LogType.InteractHand, LogImpact.Low, ref handler);
			return;
		}
		InteractHandEvent message = new InteractHandEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<InteractHandEvent>(target, message, true);
		ISharedAdminLogManager adminLogger2 = _adminLogger;
		LogStringHandler handler2 = new LogStringHandler(17, 2);
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler2.AppendLiteral(" interacted with ");
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		adminLogger2.Add(LogType.InteractHand, LogImpact.Low, ref handler2);
		DoContactInteraction(user, target, (HandledEntityEventArgs?)(object)message);
		if (!((HandledEntityEventArgs)message).Handled)
		{
			InteractionActivate(user, target, checkCanInteract: false, checkUseDelay: true, checkAccess: false, complexInteractions, checkDeletion: false);
		}
	}

	public void InteractUsingRanged(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool inRangeUnobstructed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (IsDeleted(user) || IsDeleted(used) || IsDeleted(target))
		{
			return;
		}
		if (target.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(24, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler.AppendLiteral(" interacted with ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(target, (MetaDataComponent)null), "target", "ToPrettyString(target)");
			handler.AppendLiteral(" using ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used)), "used", "ToPrettyString(used)");
			adminLogger.Add(LogType.InteractUsing, LogImpact.Low, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(33, 2);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
			handler2.AppendLiteral(" interacted with *nothing* using ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used)), "used", "ToPrettyString(used)");
			adminLogger2.Add(LogType.InteractUsing, LogImpact.Low, ref handler2);
		}
		if (RangedInteractDoBefore(user, used, target, clickLocation, inRangeUnobstructed, checkDeletion: false))
		{
			return;
		}
		if (target.HasValue)
		{
			RangedInteractEvent rangedMsg = new RangedInteractEvent(user, used, target.Value, clickLocation);
			((EntitySystem)this).RaiseLocalEvent<RangedInteractEvent>(target.Value, rangedMsg, true);
			DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)rangedMsg);
			if (((HandledEntityEventArgs)rangedMsg).Handled)
			{
				return;
			}
		}
		InteractDoAfter(user, used, target, clickLocation, inRangeUnobstructed, checkDeletion: false);
	}

	protected bool ValidateInteractAndFace(EntityUid user, EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_transform.GetMapId(coordinates) != ((EntitySystem)this).Transform(user).MapID)
		{
			return false;
		}
		InputMoverComponent mover = default(InputMoverComponent);
		if (!((EntitySystem)this).HasComp<NoRotateOnInteractComponent>(user) && (!((EntitySystem)this).TryComp<InputMoverComponent>(user, ref mover) || (mover.HeldMoveButtons & MoveButtons.AnyDirection) == 0))
		{
			_rotateToFaceSystem.TryFaceCoordinates(user, _transform.ToMapCoordinates(coordinates, true).Position);
		}
		return true;
	}

	public float UnobstructedDistance(MapCoordinates origin, MapCoordinates other, int collisionMask = 130, Ignored? predicate = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Vector2 dir = other.Position - origin.Position;
		if (dir.LengthSquared().Equals(0f))
		{
			return 0f;
		}
		if (predicate == null)
		{
			predicate = (EntityUid _) => false;
		}
		CollisionRay ray = default(CollisionRay);
		((CollisionRay)(ref ray))._002Ector(origin.Position, Vector2Helpers.Normalized(dir), collisionMask);
		List<RayCastResults> rayResults = _broadphase.IntersectRayWithPredicate(origin.MapId, ray, dir.Length(), (Func<EntityUid, bool>)predicate.Invoke, false).ToList();
		if (rayResults.Count == 0)
		{
			return dir.Length();
		}
		RayCastResults val = rayResults[0];
		return (((RayCastResults)(ref val)).HitPos - origin.Position).Length();
	}

	public bool InRangeUnobstructed(MapCoordinates origin, MapCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool checkAccess = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (other.MapId != origin.MapId)
		{
			return false;
		}
		if (!checkAccess)
		{
			return true;
		}
		Vector2 dir = other.Position - origin.Position;
		float length = dir.Length();
		if (range > 0f && length > range)
		{
			return false;
		}
		if (MathHelper.CloseTo(length, 0f, 1E-07f))
		{
			return true;
		}
		if (predicate == null)
		{
			predicate = (EntityUid _) => false;
		}
		if (length > 100f)
		{
			((EntitySystem)this).Log.Warning("InRangeUnobstructed check performed over extreme range. Limiting CollisionRay size.");
			length = 100f;
		}
		CollisionRay ray = default(CollisionRay);
		((CollisionRay)(ref ray))._002Ector(origin.Position, Vector2Helpers.Normalized(dir), (int)collisionMask);
		return _broadphase.IntersectRayWithPredicate(origin.MapId, ray, length, (Func<EntityUid, bool>)predicate.Invoke, false).ToList().Count == 0;
	}

	public bool InRangeUnobstructed(Entity<TransformComponent?> origin, Entity<TransformComponent?> other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool popup = false, bool overlapCheck = true, EntityUid? user = null, bool lagCompensate = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(other), ref other.Comp, true))
		{
			return false;
		}
		InRangeOverrideEvent ev = new InRangeOverrideEvent(Entity<TransformComponent>.op_Implicit(origin), Entity<TransformComponent>.op_Implicit(other));
		((EntitySystem)this).RaiseLocalEvent<InRangeOverrideEvent>(Entity<TransformComponent>.op_Implicit(origin), ref ev, false);
		if (ev.Handled)
		{
			return ev.InRange;
		}
		EntityCoordinates otherCoordinates = other.Comp.Coordinates;
		Angle otherAngle = other.Comp.LocalRotation;
		ActorComponent originActor = default(ActorComponent);
		if (lagCompensate && ((EntitySystem)this).TryComp<ActorComponent>((EntityUid)(((_003F?)user) ?? Entity<TransformComponent>.op_Implicit(origin)), ref originActor))
		{
			(otherCoordinates, otherAngle) = _rmcLagCompensation.GetCoordinatesAngle(Entity<TransformComponent>.op_Implicit(other), originActor.PlayerSession);
		}
		return InRangeUnobstructed(origin, other, otherCoordinates, otherAngle, range, collisionMask, predicate, popup, overlapCheck);
	}

	public bool InRangeUnobstructed(Entity<TransformComponent?> origin, Entity<TransformComponent?> other, EntityCoordinates otherCoordinates, Angle otherAngle, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool popup = false, bool overlapCheck = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			range += _rmcLagCompensation.MarginTiles;
		}
		if (origin.Owner == other.Owner && ((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(other), ref other.Comp, false))
		{
			otherCoordinates = other.Comp.Coordinates;
			otherAngle = other.Comp.LocalRotation;
		}
		Ignored combinedPredicate = (EntityUid e) => e == origin.Owner || (predicate?.Invoke(e) ?? false);
		bool inRange = true;
		MapCoordinates originPos = default(MapCoordinates);
		MapCoordinates targetPos = _transform.ToMapCoordinates(otherCoordinates, true);
		Angle targetRot = _transform.GetWorldRotation(otherCoordinates.EntityId) + otherAngle;
		FixturesComponent fixtureA = default(FixturesComponent);
		FixturesComponent fixtureB = default(FixturesComponent);
		if (range > 0f && _fixtureQuery.TryComp(Entity<TransformComponent>.op_Implicit(origin), ref fixtureA) && fixtureA.FixtureCount > 0 && _fixtureQuery.TryComp(Entity<TransformComponent>.op_Implicit(other), ref fixtureB) && fixtureB.FixtureCount > 0 && ((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(origin), ref origin.Comp, true))
		{
			var (worldPosA, worldRotA) = _transform.GetWorldPositionRotation(origin.Comp);
			Transform xfA = default(Transform);
			((Transform)(ref xfA))._002Ector(worldPosA, worldRotA);
			Transform xfB = default(Transform);
			((Transform)(ref xfB))._002Ector(targetPos.Position, targetRot);
			Vector2 vector = default(Vector2);
			Vector2 vector2 = default(Vector2);
			float distance = default(float);
			if (!_broadphase.TryGetNearest(Entity<TransformComponent>.op_Implicit(origin), Entity<TransformComponent>.op_Implicit(other), ref vector, ref vector2, ref distance, xfA, xfB, fixtureA, fixtureB, (PhysicsComponent)null, (PhysicsComponent)null))
			{
				inRange = false;
			}
			else
			{
				if (overlapCheck && distance.Equals(0f))
				{
					return true;
				}
				if (!ShouldCheckAccess(Entity<TransformComponent>.op_Implicit(origin)))
				{
					return true;
				}
				if (distance > range)
				{
					originPos = _transform.GetMapCoordinates(Entity<TransformComponent>.op_Implicit(origin), origin.Comp);
				}
				else
				{
					originPos = _transform.GetMapCoordinates(Entity<TransformComponent>.op_Implicit(origin), origin.Comp);
					range = (originPos.Position - targetPos.Position).Length();
				}
			}
		}
		else
		{
			originPos = _transform.GetMapCoordinates(Entity<TransformComponent>.op_Implicit(origin), Entity<TransformComponent>.op_Implicit(origin));
		}
		if (inRange)
		{
			Ignored rayPredicate = GetPredicate(originPos, Entity<TransformComponent>.op_Implicit(other), targetPos, targetRot, collisionMask, combinedPredicate);
			inRange = InRangeUnobstructed(originPos, targetPos, range, collisionMask, rayPredicate, ShouldCheckAccess(Entity<TransformComponent>.op_Implicit(origin)));
		}
		if (!inRange && popup && _gameTiming.IsFirstTimePredicted)
		{
			string message = base.Loc.GetString("interaction-system-user-interaction-cannot-reach");
			_popupSystem.PopupClient(message, Entity<TransformComponent>.op_Implicit(origin), Entity<TransformComponent>.op_Implicit(origin));
		}
		return inRange;
	}

	public bool InRangeUnobstructed(MapCoordinates origin, EntityUid target, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = ((EntitySystem)this).Transform(target);
		var (position, rotation) = _transform.GetWorldPositionRotation(transform);
		MapCoordinates mapPos = default(MapCoordinates);
		((MapCoordinates)(ref mapPos))._002Ector(position, transform.MapID);
		Ignored combinedPredicate = GetPredicate(origin, target, mapPos, rotation, collisionMask, predicate);
		return InRangeUnobstructed(origin, mapPos, range, collisionMask, combinedPredicate);
	}

	private Ignored GetPredicate(MapCoordinates originCoords, EntityUid target, MapCoordinates targetCoords, Angle targetRotation, CollisionGroup collisionMask, Ignored? predicate = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> ignored = new HashSet<EntityUid>();
		PhysicsComponent physics = default(PhysicsComponent);
		WallMountComponent wallMount = default(WallMountComponent);
		if (_itemQuery.HasComp(target) && _physicsQuery.TryComp(target, ref physics) && physics.CanCollide)
		{
			PhysicsComponent otherBody = default(PhysicsComponent);
			foreach (EntityUid otherEnt in _lookup.GetEntitiesInRange(target, 0.01f, (LookupFlags)4))
			{
				if (!(target == otherEnt) && _physicsQuery.TryComp(otherEnt, ref otherBody) && otherBody.CanCollide && ((uint)collisionMask & (uint)otherBody.CollisionLayer) != 0)
				{
					ignored.Add(otherEnt);
				}
			}
		}
		else if (_wallMountQuery.TryComp(target, ref wallMount))
		{
			bool ignoreAnchored;
			if (Angle.op_Implicit(wallMount.Arc) >= Math.PI * 2.0)
			{
				ignoreAnchored = true;
			}
			else
			{
				Angle angle = Angle.FromWorldVec(originCoords.Position - targetCoords.Position);
				Angle val = wallMount.Direction + targetRotation - angle;
				val = ((Angle)(ref val)).Reduced();
				Angle angleDelta = ((Angle)(ref val)).FlipPositive();
				ignoreAnchored = Angle.op_Implicit(angleDelta) < Angle.op_Implicit(wallMount.Arc) / 2.0 || Angle.op_Implicit(Angle.op_Implicit(Math.PI * 2.0) - angleDelta) < Angle.op_Implicit(wallMount.Arc) / 2.0;
			}
			EntityUid gridUid = default(EntityUid);
			MapGridComponent grid = default(MapGridComponent);
			if (ignoreAnchored && _mapManager.TryFindGridAt(targetCoords, ref gridUid, ref grid))
			{
				ignored.UnionWith(_map.GetAnchoredEntities(Entity<MapGridComponent>.op_Implicit((gridUid, grid)), targetCoords));
				TransformComponent xform = default(TransformComponent);
				foreach (EntityUid ent in _lookup.GetEntitiesInRange(targetCoords, 0.2f, (LookupFlags)110))
				{
					if (((EntitySystem)this).TryComp(ent, ref xform) && xform.Anchored)
					{
						ignored.Add(ent);
					}
				}
			}
		}
		return delegate(EntityUid e)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!(e == target))
			{
				Ignored? ignored2 = predicate;
				if (ignored2 == null || !ignored2(e))
				{
					return ignored.Contains(e);
				}
			}
			return true;
		};
	}

	public bool InRangeUnobstructed(EntityUid origin, EntityCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool popup = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return InRangeUnobstructed(origin, _transform.ToMapCoordinates(other, true), range, collisionMask, predicate, popup);
	}

	public bool InRangeUnobstructed(EntityUid origin, MapCoordinates other, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool popup = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Ignored combinedPredicate = (EntityUid e) => e == origin || (predicate?.Invoke(e) ?? false);
		MapCoordinates originPosition = _transform.GetMapCoordinates(origin, (TransformComponent)null);
		bool num = InRangeUnobstructed(originPosition, other, range, collisionMask, combinedPredicate, ShouldCheckAccess(origin));
		if (!num && popup && _gameTiming.IsFirstTimePredicted)
		{
			string message = base.Loc.GetString("interaction-system-user-interaction-cannot-reach");
			_popupSystem.PopupEntity(message, origin, origin);
		}
		return num;
	}

	public bool RangedInteractDoBefore(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach, bool checkDeletion = true)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (checkDeletion && (IsDeleted(user) || IsDeleted(used) || IsDeleted(target)))
		{
			return false;
		}
		BeforeRangedInteractEvent ev = new BeforeRangedInteractEvent(user, used, target, clickLocation, canReach);
		((EntitySystem)this).RaiseLocalEvent<BeforeRangedInteractEvent>(used, ev, false);
		if (!((HandledEntityEventArgs)ev).Handled)
		{
			return false;
		}
		DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)ev);
		return ((HandledEntityEventArgs)ev).Handled;
	}

	public bool InteractUsing(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation, bool checkCanInteract = true, bool checkCanUse = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (IsDeleted(user) || IsDeleted(used) || IsDeleted(target))
		{
			return false;
		}
		if (checkCanInteract && !_actionBlockerSystem.CanInteract(user, target))
		{
			return false;
		}
		if (checkCanUse && !_actionBlockerSystem.CanUseHeldEntity(user, used))
		{
			return false;
		}
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(24, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler.AppendLiteral(" interacted with ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
		handler.AppendLiteral(" using ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used)), "used", "ToPrettyString(used)");
		adminLogger.Add(LogType.InteractUsing, LogImpact.Low, ref handler);
		if (RangedInteractDoBefore(user, used, target, clickLocation, canReach: true, checkDeletion: false))
		{
			return true;
		}
		InteractUsingEvent interactUsingEvent = new InteractUsingEvent(user, used, target, clickLocation);
		((EntitySystem)this).RaiseLocalEvent<InteractUsingEvent>(target, interactUsingEvent, true);
		DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)interactUsingEvent);
		DoContactInteraction(user, target, (HandledEntityEventArgs?)(object)interactUsingEvent);
		if (((HandledEntityEventArgs)interactUsingEvent).Handled)
		{
			return true;
		}
		if (InteractDoAfter(user, used, target, clickLocation, canReach: true, checkDeletion: false))
		{
			return true;
		}
		return false;
	}

	public bool InteractDoAfter(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach, bool checkDeletion = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (target.HasValue)
		{
			EntityUid valueOrDefault = target.GetValueOrDefault();
			if (!((EntityUid)(ref valueOrDefault)).Valid)
			{
				target = null;
			}
		}
		if (checkDeletion && (IsDeleted(user) || IsDeleted(used) || IsDeleted(target)))
		{
			return false;
		}
		AfterInteractEvent afterInteractEvent = new AfterInteractEvent(user, used, target, clickLocation, canReach);
		((EntitySystem)this).RaiseLocalEvent<AfterInteractEvent>(used, afterInteractEvent, false);
		DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)afterInteractEvent);
		if (canReach)
		{
			DoContactInteraction(user, target, (HandledEntityEventArgs?)(object)afterInteractEvent);
		}
		if (((HandledEntityEventArgs)afterInteractEvent).Handled)
		{
			return true;
		}
		if (!target.HasValue)
		{
			return false;
		}
		AfterInteractUsingEvent afterInteractUsingEvent = new AfterInteractUsingEvent(user, used, target, clickLocation, canReach);
		((EntitySystem)this).RaiseLocalEvent<AfterInteractUsingEvent>(target.Value, afterInteractUsingEvent, false);
		DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)afterInteractUsingEvent);
		if (canReach)
		{
			DoContactInteraction(user, target, (HandledEntityEventArgs?)(object)afterInteractUsingEvent);
		}
		return ((HandledEntityEventArgs)afterInteractUsingEvent).Handled;
	}

	private bool HandleActivateItemInWorld(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		_rmcLagCompensation.SendLastRealTick();
		if (!ValidateClientInput(session, coords, uid, out var user))
		{
			((EntitySystem)this).Log.Info("ActivateItemInWorld input validation failed");
			return false;
		}
		if (((EntitySystem)this).Deleted(uid, (MetaDataComponent)null))
		{
			return false;
		}
		InteractionActivate(user.Value, uid, checkCanInteract: true, checkUseDelay: true, ShouldCheckAccess(user.Value));
		return false;
	}

	public bool InteractionActivate(EntityUid user, EntityUid used, bool checkCanInteract = true, bool checkUseDelay = true, bool checkAccess = true, bool? complexInteractions = null, bool checkDeletion = true)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (checkDeletion && (IsDeleted(user) || IsDeleted(used)))
		{
			return false;
		}
		UseDelayComponent delayComponent = default(UseDelayComponent);
		_delayQuery.TryComp(used, ref delayComponent);
		if (checkUseDelay && delayComponent != null && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((used, delayComponent))))
		{
			return false;
		}
		if (checkCanInteract && !_actionBlockerSystem.CanInteract(user, used))
		{
			return false;
		}
		if (checkAccess && !InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(used)))
		{
			return false;
		}
		if (checkAccess && !IsAccessible(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(used)))
		{
			return false;
		}
		bool valueOrDefault = complexInteractions == true;
		if (!complexInteractions.HasValue)
		{
			valueOrDefault = _actionBlockerSystem.CanComplexInteract(user);
			complexInteractions = valueOrDefault;
		}
		ActivateInWorldEvent activateMsg = new ActivateInWorldEvent(user, used, complexInteractions.Value);
		((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(used, activateMsg, true);
		if (((HandledEntityEventArgs)activateMsg).Handled)
		{
			DoContactInteraction(user, used);
			if (!activateMsg.WasLogged)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(11, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
				handler.AppendLiteral(" activated ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used)), "used", "ToPrettyString(used)");
				adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref handler);
			}
			if (delayComponent != null)
			{
				_useDelay.TryResetDelay(used, checkDelayed: false, delayComponent);
			}
			return true;
		}
		UserActivateInWorldEvent userEv = new UserActivateInWorldEvent(user, used, complexInteractions.Value);
		((EntitySystem)this).RaiseLocalEvent<UserActivateInWorldEvent>(user, userEv, true);
		if (!((HandledEntityEventArgs)userEv).Handled)
		{
			return false;
		}
		DoContactInteraction(user, used);
		if (delayComponent != null)
		{
			_useDelay.TryResetDelay(used, checkDelayed: false, delayComponent);
		}
		ISharedAdminLogManager adminLogger2 = _adminLogger;
		LogStringHandler handler2 = new LogStringHandler(11, 2);
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler2.AppendLiteral(" activated ");
		handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(used)), "used", "ToPrettyString(used)");
		adminLogger2.Add(LogType.InteractActivate, LogImpact.Low, ref handler2);
		return true;
	}

	public bool UseInHandInteraction(EntityUid user, EntityUid used, bool checkCanUse = true, bool checkCanInteract = true, bool checkUseDelay = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (IsDeleted(user) || IsDeleted(used))
		{
			return false;
		}
		UseDelayComponent delayComponent = default(UseDelayComponent);
		_delayQuery.TryComp(used, ref delayComponent);
		if (checkUseDelay && delayComponent != null && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((used, delayComponent))))
		{
			return true;
		}
		if (checkCanInteract && !_actionBlockerSystem.CanInteract(user, used))
		{
			return false;
		}
		if (checkCanUse && !_actionBlockerSystem.CanUseHeldEntity(user, used))
		{
			return false;
		}
		UseInHandEvent useMsg = new UseInHandEvent(user);
		((EntitySystem)this).RaiseLocalEvent<UseInHandEvent>(used, useMsg, true);
		if (((HandledEntityEventArgs)useMsg).Handled)
		{
			DoContactInteraction(user, used, (HandledEntityEventArgs?)(object)useMsg);
			if (delayComponent != null && useMsg.ApplyDelay)
			{
				_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((used, delayComponent)));
			}
			return true;
		}
		return InteractionActivate(user, used, checkCanInteract: false, checkUseDelay: false, checkAccess: false, null, checkDeletion: false);
	}

	public bool AltInteract(EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		SortedSet<Verb> verbs = _verbSystem.GetLocalVerbs(target, user, typeof(AlternativeVerb));
		if (verbs.Count == 0)
		{
			return false;
		}
		_verbSystem.ExecuteVerb(verbs.First(), user, target);
		return true;
	}

	public void DroppedInteraction(EntityUid user, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDeleted(user) && !IsDeleted(item))
		{
			DroppedEvent dropMsg = new DroppedEvent(user);
			((EntitySystem)this).RaiseLocalEvent<DroppedEvent>(item, dropMsg, true);
			Angle rotation = Angle.Zero;
			InputMoverComponent mover = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(user, ref mover))
			{
				rotation = mover.TargetRelativeRotation;
			}
			((EntitySystem)this).Transform(item).LocalRotation = rotation;
		}
	}

	public bool InRangeAndAccessible(Entity<TransformComponent?> user, Entity<TransformComponent?> target, float range = 1.5f, CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable, Ignored? predicate = null, bool lagCompensated = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (user == target)
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(user), ref user.Comp, true))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(target), ref target.Comp, true))
		{
			return false;
		}
		if (IsAccessible(user, target))
		{
			return InRangeUnobstructed(user, target, range, collisionMask, predicate);
		}
		return false;
	}

	public bool IsAccessible(Entity<TransformComponent?> user, Entity<TransformComponent?> target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		AccessibleOverrideEvent ev = new AccessibleOverrideEvent(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target));
		((EntitySystem)this).RaiseLocalEvent<AccessibleOverrideEvent>(Entity<TransformComponent>.op_Implicit(user), ref ev, false);
		if (ev.Handled)
		{
			return ev.Accessible;
		}
		BaseContainer val = default(BaseContainer);
		BaseContainer container = default(BaseContainer);
		if (_containerSystem.IsInSameOrParentContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(user), Entity<TransformComponent, MetaDataComponent>.op_Implicit(target), ref val, ref container))
		{
			return true;
		}
		if (container != null)
		{
			return CanAccessViaStorage(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), container);
		}
		return false;
	}

	public bool CanAccessViaStorage(EntityUid user, EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(target, null, null)), ref container))
		{
			return false;
		}
		return CanAccessViaStorage(user, target, container);
	}

	public bool CanAccessViaStorage(EntityUid user, EntityUid target, BaseContainer container)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (StorageComponent.ContainerId != container.ID)
		{
			return false;
		}
		if (!_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(container.Owner), (Enum)StorageComponent.StorageUiKey.Key, user))
		{
			return ((EntitySystem)this).HasComp<RMCItemKeepUIOpenOnStorageClosedComponent>(target);
		}
		return true;
	}

	public bool CanAccessEquipment(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Deleted(target, (MetaDataComponent)null))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(target), ref container))
		{
			return false;
		}
		EntityUid wearer = container.Owner;
		if (!_inventory.TryGetSlot(wearer, container.ID, out SlotDefinition slotDef))
		{
			return false;
		}
		if (wearer == user)
		{
			return true;
		}
		if (_strippable.IsStripHidden(slotDef, user))
		{
			return false;
		}
		if (InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(wearer)))
		{
			return _containerSystem.IsInSameOrParentContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(user), Entity<TransformComponent, MetaDataComponent>.op_Implicit(wearer));
		}
		return false;
	}

	protected bool ValidateClientInput(ICommonSession? session, EntityCoordinates coords, EntityUid uid, [NotNullWhen(true)] out EntityUid? userEntity)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		userEntity = null;
		if (!((EntityCoordinates)(ref coords)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			((EntitySystem)this).Log.Info($"Invalid Coordinates: client={session}, coords={coords}");
			return false;
		}
		if (((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null))
		{
			((EntitySystem)this).Log.Warning($"Client sent interaction with client-side entity. Session={session}, Uid={uid}");
			return false;
		}
		userEntity = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
		if (userEntity.HasValue)
		{
			EntityUid value = userEntity.Value;
			if (((EntityUid)(ref value)).Valid)
			{
				if (!((EntitySystem)this).Exists(userEntity))
				{
					((EntitySystem)this).Log.Warning($"Client attempted interaction with a non-existent attached entity. Session={session},  entity={userEntity}");
					return false;
				}
				return _rateLimit.CountAction(session, "Interaction") == RateLimitStatus.Allowed;
			}
		}
		((EntitySystem)this).Log.Warning($"Client sent interaction with no attached entity. Session={session}");
		return false;
	}

	public void DoContactInteraction(EntityUid uidA, EntityUid? uidB, HandledEntityEventArgs? args = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent metaA = default(MetaDataComponent);
		MetaDataComponent metaB = default(MetaDataComponent);
		if (uidB.HasValue && (args == null || args.Handled) && !(uidA == uidB.Value) && ((EntitySystem)this).TryComp(uidA, ref metaA) && !metaA.EntityPaused && ((EntitySystem)this).TryComp(uidB, ref metaB) && !metaB.EntityPaused)
		{
			ContactInteractionEvent ev = new ContactInteractionEvent(uidB.Value);
			((EntitySystem)this).RaiseLocalEvent<ContactInteractionEvent>(uidA, ev, false);
			ev.Other = uidA;
			((EntitySystem)this).RaiseLocalEvent<ContactInteractionEvent>(uidB.Value, ev, false);
		}
	}

	private void HandleUserInterfaceRangeCheck(ref BoundUserInterfaceCheckRangeEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if ((int)ev.Result != 2)
		{
			ev.Result = (BoundUserInterfaceRangeResult)(UiRangeCheck(ev.Actor, Entity<TransformComponent>.op_Implicit(ev.Target), ev.Data.InteractionRange) ? 1 : 2);
		}
	}

	public bool TryGetUsedEntity(EntityUid user, [NotNullWhen(true)] out EntityUid? used, bool checkCanUse = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GetUsedEntityEvent ev = new GetUsedEntityEvent(user);
		((EntitySystem)this).RaiseLocalEvent<GetUsedEntityEvent>(user, ref ev, false);
		used = ev.Used;
		if (!ev.Handled)
		{
			return false;
		}
		if (checkCanUse && !_actionBlockerSystem.CanUseHeldEntity(user, ev.Used.Value))
		{
			used = null;
			return false;
		}
		return ev.Handled;
	}

	[Obsolete("Use ActionBlockerSystem")]
	public bool SupportsComplexInteractions(EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _actionBlockerSystem.CanComplexInteract(user);
	}

	public void SetRelay(EntityUid uid, EntityUid? relayEntity, InteractionRelayComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<InteractionRelayComponent>(uid, ref component, true))
		{
			component.RelayEntity = relayEntity;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
