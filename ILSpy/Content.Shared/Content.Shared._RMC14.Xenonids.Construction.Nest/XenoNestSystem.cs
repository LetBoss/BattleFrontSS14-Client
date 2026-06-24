using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Ghost;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Construction.Nest;

public sealed class XenoNestSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedGhostSystem _ghost;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private OccluderSystem _occluder;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedXenoParasiteSystem _parasite;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedXenoWeedsSystem _xenoWeeds;

	private EntityQuery<OccluderComponent> _occluderQuery;

	private EntityQuery<XenoNestComponent> _xenoNestQuery;

	private EntityQuery<XenoNestSurfaceComponent> _xenoNestSurfaceQuery;

	private EntityQuery<XenoWeedableComponent> _xenoWeedableQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_occluderQuery = ((EntitySystem)this).GetEntityQuery<OccluderComponent>();
		_xenoNestQuery = ((EntitySystem)this).GetEntityQuery<XenoNestComponent>();
		_xenoNestSurfaceQuery = ((EntitySystem)this).GetEntityQuery<XenoNestSurfaceComponent>();
		_xenoWeedableQuery = ((EntitySystem)this).GetEntityQuery<XenoWeedableComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GhostAttemptHandleEvent>((EntityEventHandler<GhostAttemptHandleEvent>)OnNestedGhostAttemptHandle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetUsedEntityEvent>((EntityEventRefHandler<XenoComponent, GetUsedEntityEvent>)OnXenoGetUsedEntity, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, InteractHandEvent>((EntityEventRefHandler<XenoNestSurfaceComponent, InteractHandEvent>)OnSurfaceInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, DoAfterAttemptEvent<XenoNestDoAfterEvent>>((EntityEventRefHandler<XenoNestSurfaceComponent, DoAfterAttemptEvent<XenoNestDoAfterEvent>>)OnSurfaceDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, XenoNestDoAfterEvent>((EntityEventRefHandler<XenoNestSurfaceComponent, XenoNestDoAfterEvent>)OnNestSurfaceDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, CanDropTargetEvent>((EntityEventRefHandler<XenoNestSurfaceComponent, CanDropTargetEvent>)OnSurfaceCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, DragDropTargetEvent>((EntityEventRefHandler<XenoNestSurfaceComponent, DragDropTargetEvent>)OnSurfaceDragDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestSurfaceComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoNestSurfaceComponent, EntityTerminatingEvent>)OnSurfaceTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestComponent, ComponentRemove>((EntityEventRefHandler<XenoNestComponent, ComponentRemove>)OnNestRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoNestComponent, EntityTerminatingEvent>)OnNestTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestableComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<XenoNestableComponent, BeforeRangedInteractEvent>)OnNestableBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestableComponent, ShouldHandleVirtualItemInteractEvent>((EntityEventRefHandler<XenoNestableComponent, ShouldHandleVirtualItemInteractEvent>)OnNestableShouldHandle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, ComponentStartup>((EntityEventRefHandler<XenoNestedComponent, ComponentStartup>)OnNestedAdd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, ComponentRemove>((EntityEventRefHandler<XenoNestedComponent, ComponentRemove>)OnNestedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, PreventCollideEvent>((EntityEventRefHandler<XenoNestedComponent, PreventCollideEvent>)OnNestedPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, PullAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, PullAttemptEvent>)OnNestedPullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, InteractionAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, InteractionAttemptEvent>)OnNestedInteractionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, UpdateCanMoveEvent>((EntityEventRefHandler<XenoNestedComponent, UpdateCanMoveEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, UseAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, UseAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, ThrowAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, ThrowAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, PickupAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, PickupAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, AttackAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, ChangeDirectionAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, ChangeDirectionAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, DownAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, DownAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, IsEquippingAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, IsEquippingAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, IsUnequippingAttemptEvent>((EntityEventRefHandler<XenoNestedComponent, IsUnequippingAttemptEvent>)OnNestedCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoNestedComponent, GetInfectedIncubationMultiplierEvent>((EntityEventRefHandler<XenoNestedComponent, GetInfectedIncubationMultiplierEvent>)OnInNestGetInfectedIncubationMultiplier, (Type[])null, (Type[])null);
	}

	private void OnXenoGetUsedEntity(Entity<XenoComponent> ent, ref GetUsedEntityEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		EntityUid? val = ((EntitySystem)this).CompOrNull<PullerComponent>(Entity<XenoComponent>.op_Implicit(ent))?.Pulling;
		if (val.HasValue)
		{
			EntityUid pulling = val.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<XenoNestableComponent>(pulling) && !((EntitySystem)this).HasComp<XenoNestedComponent>(pulling))
			{
				args.Used = pulling;
			}
		}
	}

	private void OnSurfaceInteractHand(Entity<XenoNestSurfaceComponent> ent, ref InteractHandEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ((EntitySystem)this).CompOrNull<PullerComponent>(args.User)?.Pulling;
		if (val.HasValue)
		{
			EntityUid pulling = val.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = true;
			TryStartNesting(args.User, ent, pulling);
		}
	}

	private void OnNestRemove(Entity<XenoNestComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		DetachNested(Entity<XenoNestComponent>.op_Implicit(ent), ent.Comp.Nested);
	}

	private void OnNestTerminating(Entity<XenoNestComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		DetachNested(Entity<XenoNestComponent>.op_Implicit(ent), ent.Comp.Nested);
	}

	private void OnNestableBeforeRangedInteract(Entity<XenoNestableComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		XenoNestSurfaceComponent surface = default(XenoNestSurfaceComponent);
		if (args.CanReach && ((EntitySystem)this).TryComp<XenoNestSurfaceComponent>(args.Target, ref surface))
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryStartNesting(args.User, Entity<XenoNestSurfaceComponent>.op_Implicit((args.Target.Value, surface)), Entity<XenoNestableComponent>.op_Implicit(ent));
		}
	}

	private void OnNestableShouldHandle(Entity<XenoNestableComponent> ent, ref ShouldHandleVirtualItemInteractEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.Event.User) && ((EntitySystem)this).HasComp<XenoNestSurfaceComponent>(args.Event.Target))
		{
			args.Handle = true;
		}
	}

	private void OnNestedAdd(Entity<XenoNestedComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_parasite.RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(ent.Owner));
	}

	private void OnNestedRemove(Entity<XenoNestedComponent> ent, ref ComponentRemove args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		DetachNested(null, Entity<XenoNestedComponent>.op_Implicit(ent));
		_actionBlocker.UpdateCanMove(Entity<XenoNestedComponent>.op_Implicit(ent));
		_parasite.RefreshIncubationMultipliers(Entity<VictimInfectedComponent>.op_Implicit(ent.Owner));
		if (((EntitySystem)this).HasComp<KnockedDownComponent>(Entity<XenoNestedComponent>.op_Implicit(ent)) || _mobState.IsIncapacitated(Entity<XenoNestedComponent>.op_Implicit(ent)))
		{
			_standing.Down(Entity<XenoNestedComponent>.op_Implicit(ent), playSound: true, dropHeldItems: true, force: false, changeCollision: true);
		}
		NetUserId? ghostedId = ent.Comp.GhostedId;
		if (!ghostedId.HasValue)
		{
			return;
		}
		NetUserId id = ghostedId.GetValueOrDefault();
		ICommonSession player = default(ICommonSession);
		if (!_player.TryGetSessionById((NetUserId?)id, ref player))
		{
			return;
		}
		EntityUid? attachedEntity = player.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid ghost = attachedEntity.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<GhostComponent>(ghost))
			{
				_rmcChat.ChatMessageToOne("\n[font size=24][color=red]You have been freed from your nest and may go back to your body![/color][/font]\n", ghost);
				RMCGhostReturnComponent returnTo = ((EntitySystem)this).EnsureComp<RMCGhostReturnComponent>(ghost);
				returnTo.Target = Entity<XenoNestedComponent>.op_Implicit(ent);
				((EntitySystem)this).Dirty(ghost, (IComponent)(object)returnTo, (MetaDataComponent)null);
				_ghost.SetCanReturnToBody(Entity<GhostComponent>.op_Implicit((ValueTuple<EntityUid, GhostComponent>)(ghost, null)), value: true);
			}
		}
	}

	private void OnSurfaceDoAfterAttempt(Entity<XenoNestSurfaceComponent> ent, ref DoAfterAttemptEvent<XenoNestDoAfterEvent> args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.DoAfter.Args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(target2, (MetaDataComponent)null) && CanNestPopup(args.DoAfter.Args.User, target2, ent, out var _, silent: false, args.Event.AllDirs))
			{
				return;
			}
		}
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnNestSurfaceDoAfter(Entity<XenoNestSurfaceComponent> ent, ref XenoNestDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected I4, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid victim = target.GetValueOrDefault();
		if (args.Cancelled)
		{
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(29, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler.AppendLiteral(" stopped nesting ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(victim)), "victim", "ToPrettyString(victim)");
			handler.AppendLiteral(" to surface ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoNestSurfaceComponent>.op_Implicit(ent), (MetaDataComponent)null), "surface", "ToPrettyString(ent)");
			adminLog.Add(LogType.RMCXenoNest, ref handler);
		}
		else
		{
			if (((HandledEntityEventArgs)args).Handled || !CanNestPopup(args.User, victim, ent, out var direction, silent: false, args.AllDirs))
			{
				return;
			}
			((HandledEntityEventArgs)args).Handled = true;
			PullableComponent pullable = default(PullableComponent);
			if (((EntitySystem)this).TryComp<PullableComponent>(victim, ref pullable))
			{
				_pulling.TryStopPull(victim, pullable);
			}
			if (_net.IsClient)
			{
				return;
			}
			EntityCoordinates nestCoordinates = ent.Owner.ToCoordinates();
			Vector2 offset = direction switch
			{
				(Direction)0L => new Vector2(0f, -0.52f), 
				(Direction)2L => new Vector2(0.52f, 0f), 
				(Direction)4L => new Vector2(0f, 0.52f), 
				(Direction)6L => new Vector2(-0.52f, 0f), 
				_ => Vector2.Zero, 
			};
			EntityUid nest = ((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(ent.Comp.Nest), nestCoordinates, (ComponentRegistry)null, DirectionExtensions.ToAngle(direction.Value));
			_transform.SetCoordinates(nest, ((EntityCoordinates)(ref nestCoordinates)).Offset(offset));
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(args.User), Entity<HiveMemberComponent>.op_Implicit(nest));
			ent.Comp.Nests[direction.Value] = nest;
			((EntitySystem)this).Dirty<XenoNestSurfaceComponent>(ent, (MetaDataComponent)null);
			XenoNestComponent nestComp = ((EntitySystem)this).EnsureComp<XenoNestComponent>(nest);
			nestComp.Surface = Entity<XenoNestSurfaceComponent>.op_Implicit(ent);
			nestComp.Nested = victim;
			((EntitySystem)this).Dirty(nest, (IComponent)(object)nestComp, (MetaDataComponent)null);
			XenoNestedComponent nestedComp = ((EntitySystem)this).EnsureComp<XenoNestedComponent>(victim);
			nestedComp.Nest = nest;
			((EntitySystem)this).Dirty(victim, (IComponent)(object)nestedComp, (MetaDataComponent)null);
			_transform.SetCoordinates(victim, nest.ToCoordinates());
			_transform.SetLocalRotation(victim, Angle.Zero, (TransformComponent)null);
			_standing.Stand(victim, null, null, force: true);
			_popup.PopupClient(base.Loc.GetString("cm-xeno-nest-securing-self", (ValueTuple<string, object>)("target", victim)), args.User, args.User);
			foreach (ICommonSession recipient2 in Filter.PvsExcept(args.User, 2f, (IEntityManager)null).Recipients)
			{
				target = recipient2.AttachedEntity;
				if (target.HasValue)
				{
					EntityUid recipient = target.GetValueOrDefault();
					if (recipient == victim)
					{
						_popup.PopupEntity(base.Loc.GetString("cm-xeno-nest-securing-target", (ValueTuple<string, object>)("user", args.User)), args.User, recipient, PopupType.MediumCaution);
					}
					else
					{
						_popup.PopupEntity(base.Loc.GetString("cm-xeno-nest-securing-observer", (ValueTuple<string, object>)("user", args.User), (ValueTuple<string, object>)("target", victim)), args.User, recipient);
					}
				}
			}
			ISharedAdminLogManager adminLog2 = _adminLog;
			LogStringHandler handler2 = new LogStringHandler(20, 3);
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "user", "ToPrettyString(args.User)");
			handler2.AppendLiteral(" nested ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(victim)), "victim", "ToPrettyString(victim)");
			handler2.AppendLiteral(" to surface ");
			handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoNestSurfaceComponent>.op_Implicit(ent), (MetaDataComponent)null), "surface", "ToPrettyString(ent)");
			adminLog2.Add(LogType.RMCXenoNest, ref handler2);
		}
	}

	private void OnSurfaceCanDropTarget(Entity<XenoNestSurfaceComponent> ent, ref CanDropTargetEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			args.CanDrop = CanBeNested(args.User, args.Dragged, Entity<XenoNestSurfaceComponent>.op_Implicit((Entity<XenoNestSurfaceComponent>.op_Implicit(ent), ent.Comp)), out List<Direction> _, silent: true);
			args.Handled = true;
		}
	}

	private void OnSurfaceDragDropTarget(Entity<XenoNestSurfaceComponent> ent, ref DragDropTargetEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		TryStartNesting(args.User, ent, args.Dragged);
	}

	private void OnSurfaceTerminating(Entity<XenoNestSurfaceComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		XenoWeedableComponent weedable = default(XenoWeedableComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Weedable, (MetaDataComponent)null) && _xenoWeedableQuery.TryComp(ent.Comp.Weedable, ref weedable))
		{
			EntityUid? entity = weedable.Entity;
			EntityUid val = Entity<XenoNestSurfaceComponent>.op_Implicit(ent);
			if (entity.HasValue && !(entity.GetValueOrDefault() != val))
			{
				weedable.Entity = null;
				((EntitySystem)this).Dirty(ent.Comp.Weedable.Value, (IComponent)(object)weedable, (MetaDataComponent)null);
			}
		}
	}

	private void OnNestedPreventCollide(Entity<XenoNestedComponent> ent, ref PreventCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnNestedPullAttempt(Entity<XenoNestedComponent> ent, ref PullAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnNestedInteractionAttempt(Entity<XenoNestedComponent> ent, ref InteractionAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnNestedCancel<T>(Entity<XenoNestedComponent> ent, ref T args) where T : CancellableEntityEventArgs
	{
		((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
	}

	private void OnInNestGetInfectedIncubationMultiplier(Entity<XenoNestedComponent> ent, ref GetInfectedIncubationMultiplierEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)ent.Comp).Running)
		{
			args.Multiply(ent.Comp.IncubationMultiplier);
		}
	}

	private void OnNestedGhostAttemptHandle(GhostAttemptHandleEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? currentEntity = args.Mind.CurrentEntity;
		if (!currentEntity.HasValue)
		{
			return;
		}
		EntityUid ent = currentEntity.GetValueOrDefault();
		XenoNestedComponent nested = default(XenoNestedComponent);
		if (((EntitySystem)this).TryComp<XenoNestedComponent>(ent, ref nested))
		{
			NetUserId? userId = args.Mind.UserId;
			if (userId.HasValue)
			{
				NetUserId userId2 = userId.GetValueOrDefault();
				nested.GhostedId = userId2;
				((EntitySystem)this).Dirty(ent, (IComponent)(object)nested, (MetaDataComponent)null);
			}
		}
	}

	public bool TryStartNesting(EntityUid user, Entity<XenoNestSurfaceComponent> surface, EntityUid victim, out DoAfterId? doAfterId, bool allDirs = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		doAfterId = null;
		if (!((EntitySystem)this).HasComp<XenoComponent>(user) || !((EntitySystem)this).HasComp<HandsComponent>(user) || !CanNestPopup(user, victim, surface, out var _, silent: false, allDirs))
		{
			return false;
		}
		XenoNestDoAfterEvent ev = new XenoNestDoAfterEvent
		{
			AllDirs = allDirs
		};
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, surface.Comp.DoAfter, ev, Entity<XenoNestSurfaceComponent>.op_Implicit(surface), victim)
		{
			BreakOnMove = true,
			AttemptFrequency = AttemptFrequency.EveryTick
		};
		if (!_doAfter.TryStartDoAfter(doAfter, out doAfterId))
		{
			return true;
		}
		_popup.PopupClient(base.Loc.GetString("cm-xeno-nest-pin-self", (ValueTuple<string, object>)("target", victim)), user, user);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(user, 2f, (IEntityManager)null).Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				if (recipient == victim)
				{
					_popup.PopupEntity(base.Loc.GetString("cm-xeno-nest-pin-target", (ValueTuple<string, object>)("user", user)), user, recipient, PopupType.MediumCaution);
				}
				else
				{
					_popup.PopupEntity(base.Loc.GetString("cm-xeno-nest-pin-observer", (ValueTuple<string, object>)("user", user), (ValueTuple<string, object>)("target", victim)), user, recipient);
				}
			}
		}
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(29, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "user", "ToPrettyString(user)");
		handler.AppendLiteral(" started nesting ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(victim)), "victim", "ToPrettyString(victim)");
		handler.AppendLiteral(" to surface ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoNestSurfaceComponent>.op_Implicit(surface), (MetaDataComponent)null), "surface", "ToPrettyString(surface)");
		adminLog.Add(LogType.RMCXenoNest, ref handler);
		return true;
	}

	public bool TryStartNesting(EntityUid user, Entity<XenoNestSurfaceComponent> surface, EntityUid victim)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DoAfterId? doAfterId;
		return TryStartNesting(user, surface, victim, out doAfterId);
	}

	private bool CanBeNested(EntityUid user, EntityUid? victim, Entity<XenoNestSurfaceComponent?> surface, out List<Direction> directions, bool silent = false, bool allDirs = false)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		directions = new List<Direction>();
		if (victim.HasValue)
		{
			if (!((EntitySystem)this).HasComp<XenoNestableComponent>(victim))
			{
				if (!silent)
				{
					_popup.PopupClient(base.Loc.GetString("cm-xeno-nest-failed", (ValueTuple<string, object>)("target", victim)), Entity<XenoNestSurfaceComponent>.op_Implicit(surface), user);
				}
				return false;
			}
			if (_mobState.IsDead(victim.Value))
			{
				if (!silent)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-nest-failed-dead", (ValueTuple<string, object>)("target", victim)), Entity<XenoNestSurfaceComponent>.op_Implicit(surface), user);
				}
				return false;
			}
		}
		EntityCoordinates userCoords = _transform.GetMoverCoordinates(user);
		EntityCoordinates nestCoords = _transform.GetMoverCoordinates(Entity<XenoNestSurfaceComponent>.op_Implicit(surface));
		Vector2 delta = default(Vector2);
		if (!((EntityCoordinates)(ref userCoords)).TryDelta((IEntityManager)(object)base.EntityManager, _transform, nestCoords, ref delta))
		{
			return false;
		}
		Direction attemptedDirection = DirectionExtensions.GetDir(delta);
		Angle val = Angle.FromWorldVec(delta);
		Direction priorityDir = ((Angle)(ref val)).GetCardinalDir();
		if ((int)attemptedDirection == -1)
		{
			return false;
		}
		directions.Add(priorityDir);
		DirectionFlag flags = DirectionExtensions.AsFlag(attemptedDirection);
		if (!directions.Contains((Direction)0) && (allDirs || ((Enum)flags).HasFlag((Enum)(object)(DirectionFlag)1)))
		{
			directions.Add((Direction)0);
		}
		if (!directions.Contains((Direction)2) && (allDirs || ((Enum)flags).HasFlag((Enum)(object)(DirectionFlag)2)))
		{
			directions.Add((Direction)2);
		}
		if (!directions.Contains((Direction)4) && (allDirs || ((Enum)flags).HasFlag((Enum)(object)(DirectionFlag)4)))
		{
			directions.Add((Direction)4);
		}
		if (!directions.Contains((Direction)6) && (allDirs || ((Enum)flags).HasFlag((Enum)(object)(DirectionFlag)8)))
		{
			directions.Add((Direction)6);
		}
		if (!((EntitySystem)this).Resolve<XenoNestSurfaceComponent>(Entity<XenoNestSurfaceComponent>.op_Implicit(surface), ref surface.Comp, true) || !IsNestSurfaceFromHiveWeeds(Entity<XenoNestSurfaceComponent>.op_Implicit((surface.Owner, surface.Comp)), priorityDir, user))
		{
			if (!silent)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-nest-failed-cant-there"), Entity<XenoNestSurfaceComponent>.op_Implicit(surface), user);
			}
			return false;
		}
		return true;
	}

	public bool CanNestPopup(EntityUid user, EntityUid? victim, Entity<XenoNestSurfaceComponent> surface, [NotNullWhen(true)] out Direction? direction, bool silent = false, bool allDirs = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		direction = null;
		if (!CanBeNested(user, victim, Entity<XenoNestSurfaceComponent>.op_Implicit((Entity<XenoNestSurfaceComponent>.op_Implicit(surface), surface.Comp)), out List<Direction> directions, silent, allDirs))
		{
			return false;
		}
		if (victim.HasValue && !_standing.IsDown(victim.Value))
		{
			if (!silent)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-nest-failed-target-resisting", (ValueTuple<string, object>)("target", victim)), victim.Value, user, PopupType.MediumCaution);
			}
			return false;
		}
		EntityCoordinates nestCoords = _transform.GetMoverCoordinates(Entity<XenoNestSurfaceComponent>.op_Implicit(surface));
		string response = null;
		foreach (Direction dir in directions)
		{
			TileRef? tileRef = _turf.GetTileRef(((EntityCoordinates)(ref nestCoords)).Offset(DirectionExtensions.ToVec(dir)));
			if (tileRef.HasValue)
			{
				TileRef tile = tileRef.GetValueOrDefault();
				if (!_turf.IsTileBlocked(tile, CollisionGroup.Impassable))
				{
					if (surface.Comp.Nests.ContainsKey(dir))
					{
						if (response == null)
						{
							response = base.Loc.GetString("cm-xeno-nest-failed-cant-already-there");
						}
						continue;
					}
					response = null;
					direction = dir;
					break;
				}
			}
			if (response == null)
			{
				response = base.Loc.GetString("cm-xeno-nest-failed-cant-there");
			}
		}
		if (!direction.HasValue)
		{
			if (!silent)
			{
				_popup.PopupClient(response, Entity<XenoNestSurfaceComponent>.op_Implicit(surface), user);
			}
			return false;
		}
		return true;
	}

	private bool IsNestSurfaceFromHiveWeeds(Entity<XenoNestSurfaceComponent> nestSurface, Direction dir, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoNestSurfaceComponent> val = nestSurface;
		EntityUid val2 = default(EntityUid);
		XenoNestSurfaceComponent xenoNestSurfaceComponent = default(XenoNestSurfaceComponent);
		val.Deconstruct(ref val2, ref xenoNestSurfaceComponent);
		EntityUid ent = val2;
		EntityUid? weedable = xenoNestSurfaceComponent.Weedable;
		if (!weedable.HasValue)
		{
			EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(ent));
			MapGridComponent gridComp = default(MapGridComponent);
			if (!grid.HasValue || !((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
			{
				return false;
			}
			if (((EntitySystem)this).HasComp<XenoNestIgnoreWeedsUserComponent>(user))
			{
				return true;
			}
			return _xenoWeeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((grid.Value, gridComp)), ent.ToCoordinates());
		}
		EntityCoordinates nestCoords = _transform.GetMoverCoordinates(Entity<XenoNestSurfaceComponent>.op_Implicit(nestSurface));
		EntityCoordinates underNestCooords = ((EntityCoordinates)(ref nestCoords)).Offset(DirectionExtensions.ToVec(dir));
		weedable = _transform.GetGrid(underNestCooords);
		if (weedable.HasValue)
		{
			EntityUid gridEntity = weedable.GetValueOrDefault();
			MapGridComponent gridComp2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridEntity, ref gridComp2))
			{
				if (((EntitySystem)this).HasComp<XenoNestIgnoreWeedsUserComponent>(user))
				{
					return true;
				}
				return _xenoWeeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((gridEntity, gridComp2)), underNestCooords);
			}
		}
		return false;
	}

	private void DetachNested(EntityUid? nest, EntityUid? nestedNullable)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || !nestedNullable.HasValue)
		{
			return;
		}
		EntityUid nested = nestedNullable.GetValueOrDefault();
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TerminatingOrDeleted(nested, (MetaDataComponent)null) || !((EntitySystem)this).TryComp(nested, ref xform))
		{
			return;
		}
		XenoNestedComponent nestedComp = default(XenoNestedComponent);
		if (((EntitySystem)this).TryComp<XenoNestedComponent>(nested, ref nestedComp))
		{
			EntityUid value = nest.GetValueOrDefault();
			if (!nest.HasValue)
			{
				value = nestedComp.Nest;
				nest = value;
			}
			if (nestedComp.Detached)
			{
				return;
			}
			nestedComp.Detached = true;
			((EntitySystem)this).Dirty(nested, (IComponent)(object)nestedComp, (MetaDataComponent)null);
			XenoNestComponent nestComp = default(XenoNestComponent);
			XenoNestSurfaceComponent surfaceComp = default(XenoNestSurfaceComponent);
			if (((EntitySystem)this).TryComp<XenoNestComponent>(nest, ref nestComp) && ((EntitySystem)this).TryComp<XenoNestSurfaceComponent>(nestComp.Surface, ref surfaceComp))
			{
				foreach (KeyValuePair<Direction, EntityUid> nest2 in surfaceComp.Nests)
				{
					nest2.Deconstruct(out var key, out value);
					Direction dir = key;
					value = value;
					EntityUid? val = nest;
					if (val.HasValue && !(value != val.GetValueOrDefault()))
					{
						surfaceComp.Nests.Remove(dir);
						break;
					}
				}
				((EntitySystem)this).Dirty(nestComp.Surface.Value, (IComponent)(object)surfaceComp, (MetaDataComponent)null);
			}
		}
		Vector2 position = xform.LocalPosition;
		SharedTransformSystem transform = _transform;
		Angle localRotation = xform.LocalRotation;
		transform.SetLocalPosition(nested, position + ((Angle)(ref localRotation)).ToWorldVec() / 2f, (TransformComponent)null);
		_transform.AttachToGridOrMap(nested, xform);
		((EntitySystem)this).RemCompDeferred<XenoNestedComponent>(nested);
		((EntitySystem)this).QueueDel(nest);
	}

	private bool TryGetNestedWallOccluder(Entity<XenoNestedComponent> nested, out Entity<OccluderComponent> occluder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		occluder = default(Entity<OccluderComponent>);
		XenoNestComponent nest = default(XenoNestComponent);
		if (!_xenoNestQuery.TryComp(nested.Comp.Nest, ref nest))
		{
			return false;
		}
		EntityUid? surface = nest.Surface;
		if (surface.HasValue)
		{
			EntityUid surface2 = surface.GetValueOrDefault();
			OccluderComponent occluderComp = default(OccluderComponent);
			if (_occluderQuery.TryComp(surface2, ref occluderComp))
			{
				occluder = Entity<OccluderComponent>.op_Implicit((surface2, occluderComp));
				return true;
			}
			XenoNestSurfaceComponent nestSurface = default(XenoNestSurfaceComponent);
			if (_xenoNestSurfaceQuery.TryComp(surface2, ref nestSurface) && _occluderQuery.TryComp(nestSurface.Weedable, ref occluderComp))
			{
				occluder = Entity<OccluderComponent>.op_Implicit((nestSurface.Weedable.Value, occluderComp));
				return true;
			}
			return false;
		}
		return false;
	}

	public bool HasAdjacentNestFacing(EntityCoordinates coordinates)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ImmutableArray<Direction>.Enumerator enumerator = _rmcMap.CardinalDirections.GetEnumerator();
		XenoNestSurfaceComponent surface = default(XenoNestSurfaceComponent);
		while (enumerator.MoveNext())
		{
			Direction cardinal = enumerator.Current;
			RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(coordinates, cardinal, (DirectionFlag)0);
			Direction opposite = DirectionExtensions.GetOpposite(cardinal);
			EntityUid uid;
			while (anchored.MoveNext(out uid))
			{
				if (_xenoNestSurfaceQuery.TryComp(uid, ref surface) && surface.Nests.TryGetValue(opposite, out var nest) && !((EntitySystem)this).TerminatingOrDeleted(nest, (MetaDataComponent)null))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			return;
		}
		EntityUid? localEntity = _player.LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid ent = localEntity.GetValueOrDefault();
			XenoNestedComponent nested = default(XenoNestedComponent);
			if (((EntitySystem)this).TryComp<XenoNestedComponent>(ent, ref nested) && TryGetNestedWallOccluder(Entity<XenoNestedComponent>.op_Implicit((ent, nested)), out Entity<OccluderComponent> occluder))
			{
				_occluder.SetEnabled(Entity<OccluderComponent>.op_Implicit(occluder), false, Entity<OccluderComponent>.op_Implicit(occluder), (MetaDataComponent)null);
			}
		}
	}
}
