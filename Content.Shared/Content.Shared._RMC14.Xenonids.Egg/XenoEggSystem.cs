using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg.EggRetriever;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Ghost;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Components;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Egg;

public sealed class XenoEggSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FixtureSystem _fixture;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedXenoParasiteSystem _parasite;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private SharedXenoWeedsSystem _weeds;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private EntityManager _entities;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedJitteringSystem _jitter;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDestructibleSystem _destruction;

	[Dependency]
	private NameModifierSystem _nameModifier;

	private static readonly ProtoId<TagPrototype> AirlockTag = ProtoId<TagPrototype>.op_Implicit("Airlock");

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private EntityQuery<StepTriggerComponent> _stepTriggerQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_stepTriggerQuery = ((EntitySystem)this).GetEntityQuery<StepTriggerComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackStartEvent>((EntityEventRefHandler<DropshipHijackStartEvent>)OnDropshipHijackStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoGrowOvipositorActionEvent>((EntityEventRefHandler<XenoComponent, XenoGrowOvipositorActionEvent>)OnXenoGrowOvipositorAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoGrowOvipositorDoAfterEvent>((EntityEventRefHandler<XenoComponent, XenoGrowOvipositorDoAfterEvent>)OnXenoGrowOvipositorDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAttachedOvipositorComponent, MapInitEvent>((EntityEventRefHandler<XenoAttachedOvipositorComponent, MapInitEvent>)OnXenoAttachedMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAttachedOvipositorComponent, ComponentRemove>((EntityEventRefHandler<XenoAttachedOvipositorComponent, ComponentRemove>)OnXenoAttachedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAttachedOvipositorComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoAttachedOvipositorComponent, MobStateChangedEvent>)OnXenoMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAttachedOvipositorComponent, XenoConstructionRangeEvent>((EntityEventRefHandler<XenoAttachedOvipositorComponent, XenoConstructionRangeEvent>)OnXenoConstructionRange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<XenoEggComponent, AfterAutoHandleStateEvent>)OnXenoEggAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<XenoEggComponent, GettingPickedUpAttemptEvent>)OnXenoEggPickedUpAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, UseInHandEvent>((EntityEventRefHandler<XenoEggComponent, UseInHandEvent>)OnXenoEggUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, InteractUsingEvent>((EntityEventRefHandler<XenoEggComponent, InteractUsingEvent>)OnXenoEggInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, XenoEggReturnParasiteDoAfterEvent>((EntityEventRefHandler<XenoEggComponent, XenoEggReturnParasiteDoAfterEvent>)OnXenoEggReturnParasiteDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, AfterInteractEvent>((EntityEventRefHandler<XenoEggComponent, AfterInteractEvent>)OnXenoEggAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, XenoEggPlaceDoAfterEvent>((EntityEventRefHandler<XenoEggComponent, XenoEggPlaceDoAfterEvent>)OnXenoEggPlaceDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, ActivateInWorldEvent>((EntityEventRefHandler<XenoEggComponent, ActivateInWorldEvent>)OnXenoEggActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, StepTriggerAttemptEvent>((EntityEventRefHandler<XenoEggComponent, StepTriggerAttemptEvent>)OnXenoEggStepTriggerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, StepTriggeredOffEvent>((EntityEventRefHandler<XenoEggComponent, StepTriggeredOffEvent>)OnXenoEggStepTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<XenoEggComponent, BeforeDamageChangedEvent>)OnXenoEggBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<XenoEggComponent, GetVerbsEvent<ActivationVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggComponent, DestructionEventArgs>((EntityEventRefHandler<XenoEggComponent, DestructionEventArgs>)OnDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFragileEggComponent, ComponentShutdown>((EntityEventRefHandler<XenoFragileEggComponent, ComponentShutdown>)OnFragileConvert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFragileEggComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<XenoFragileEggComponent, RefreshNameModifiersEvent>)OnFragileRefreshModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFragileEggComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoFragileEggComponent, EntityTerminatingEvent>)OnFragileDelete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggSustainerComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoEggSustainerComponent, EntityTerminatingEvent>)OnEggSustainerDelete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEggSustainerComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoEggSustainerComponent, MobStateChangedEvent>)OnEggSustainerDeath, (Type[])null, (Type[])null);
	}

	private void OnDropshipHijackStart(ref DropshipHijackStartEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<XenoOvipositorCapableComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoOvipositorCapableComponent>();
		EntityUid uid = default(EntityUid);
		XenoOvipositorCapableComponent xenoOvipositorCapableComponent = default(XenoOvipositorCapableComponent);
		while (query.MoveNext(ref uid, ref xenoOvipositorCapableComponent))
		{
			foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoGrowOvipositorActionEvent>(uid))
			{
				_actions.ClearCooldown(action.AsNullable());
			}
		}
	}

	private void OnXenoGrowOvipositorAction(Entity<XenoComponent> xeno, ref XenoGrowOvipositorActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		bool hasOvipositor = ((EntitySystem)this).HasComp<XenoAttachedOvipositorComponent>(Entity<XenoComponent>.op_Implicit(xeno));
		if (hasOvipositor || _plasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.AttachPlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoGrowOvipositorDoAfterEvent ev = new XenoGrowOvipositorDoAfterEvent
			{
				PlasmaCost = args.AttachPlasmaCost
			};
			TimeSpan delay = args.AttachDoAfter;
			LocId popup = default(LocId);
			((LocId)(ref popup))._002Ector("cm-xeno-ovipositor-attach");
			PopupType popupType = PopupType.Medium;
			if (hasOvipositor)
			{
				ev.PlasmaCost = FixedPoint2.Zero;
				delay = args.DetachDoAfter;
				popup = LocId.op_Implicit("cm-xeno-ovipositor-detach");
				popupType = PopupType.MediumCaution;
			}
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoComponent>.op_Implicit(xeno), delay, ev, Entity<XenoComponent>.op_Implicit(xeno))
			{
				BreakOnMove = true,
				MovementThreshold = 0.001f,
				BreakOnRest = !hasOvipositor
			};
			if (_doAfter.TryStartDoAfter(doAfterArgs))
			{
				_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(popup)), Entity<XenoComponent>.op_Implicit(xeno), Entity<XenoComponent>.op_Implicit(xeno), popupType);
			}
		}
	}

	private void OnXenoGrowOvipositorDoAfter(Entity<XenoComponent> xeno, ref XenoGrowOvipositorDoAfterEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && _plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoAttachedOvipositorComponent attached = default(XenoAttachedOvipositorComponent);
			if (((EntitySystem)this).TryComp<XenoAttachedOvipositorComponent>(Entity<XenoComponent>.op_Implicit(xeno), ref attached))
			{
				DetachOvipositor(Entity<XenoAttachedOvipositorComponent>.op_Implicit((Entity<XenoComponent>.op_Implicit(xeno), attached)));
			}
			else
			{
				AttachOvipositor(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno.Owner));
			}
		}
	}

	private void OnXenoAttachedMapInit(Entity<XenoAttachedOvipositorComponent> attached, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), ref xform))
		{
			_transform.AnchorEntity(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), xform);
		}
		XenoOvipositorChangedEvent ev = new XenoOvipositorChangedEvent(Attached: true);
		((EntitySystem)this).RaiseLocalEvent<XenoOvipositorChangedEvent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), ref ev, true);
	}

	private void OnXenoAttachedRemove(Entity<XenoAttachedOvipositorComponent> attached, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), (MetaDataComponent)null) && ((EntitySystem)this).TryComp(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), ref xform))
		{
			_transform.Unanchor(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), xform, true);
			_physics.TrySetBodyType(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), (BodyType)2, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		}
		XenoOvipositorChangedEvent ev = new XenoOvipositorChangedEvent(Attached: false);
		((EntitySystem)this).RaiseLocalEvent<XenoOvipositorChangedEvent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(attached), ref ev, true);
	}

	private void OnXenoMobStateChanged(Entity<XenoAttachedOvipositorComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DetachOvipositor(ent);
	}

	private void OnXenoConstructionRange(Entity<XenoAttachedOvipositorComponent> ent, ref XenoConstructionRangeEvent args)
	{
		args.Range = 0;
	}

	private void OnXenoEggAfterState(Entity<XenoEggComponent> egg, ref AfterAutoHandleStateEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		XenoEggStateChangedEvent ev = default(XenoEggStateChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoEggStateChangedEvent>(Entity<XenoEggComponent>.op_Implicit(egg), ref ev, false);
	}

	private void OnXenoEggPickedUpAttempt(Entity<XenoEggComponent> egg, ref GettingPickedUpAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (egg.Comp.State != XenoEggState.Item)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnXenoEggUseInHand(Entity<XenoEggComponent> egg, ref UseInHandEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		XenoEggUseInHandEvent ev = new XenoEggUseInHandEvent(_entities.GetNetEntity(egg.Owner, (MetaDataComponent)null));
		((EntitySystem)this).RaiseLocalEvent<XenoEggUseInHandEvent>(args.User, ev, false);
		((HandledEntityEventArgs)args).Handled = ((HandledEntityEventArgs)ev).Handled;
	}

	private void OnXenoEggAfterInteract(Entity<XenoEggComponent> egg, ref AfterInteractEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || egg.Comp.State != XenoEggState.Item || !((EntitySystem)this).HasComp<TransformComponent>(Entity<XenoEggComponent>.op_Implicit(egg)))
		{
			return;
		}
		EggPlantingDistanceComponent plantDis = default(EggPlantingDistanceComponent);
		if ((!((EntitySystem)this).HasComp<EggPlantingDistanceComponent>(args.User) && !args.CanReach) || (((EntitySystem)this).TryComp<EggPlantingDistanceComponent>(args.User, ref plantDis) && !_interaction.InRangeUnobstructed(args.User, args.ClickLocation, plantDis.Distance)))
		{
			if (_timing.IsFirstTimePredicted)
			{
				_popup.PopupCoordinates(base.Loc.GetString("cm-xeno-cant-reach-there"), args.ClickLocation, Filter.Local(), recordReplay: true);
			}
			return;
		}
		if (!CanPlaceEggPopup(args.User, egg, args.ClickLocation, ((HandledEntityEventArgs)args).Handled, out var _))
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		TimeSpan plantTime = TimeSpan.FromSeconds(3.5);
		EggPlantTimeComponent time = default(EggPlantTimeComponent);
		if (((EntitySystem)this).TryComp<EggPlantTimeComponent>(args.User, ref time))
		{
			plantTime = time.PlantTime;
		}
		XenoEggPlaceDoAfterEvent ev = new XenoEggPlaceDoAfterEvent(((EntitySystem)this).GetNetCoordinates(args.ClickLocation, (MetaDataComponent)null));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, plantTime, ev, Entity<XenoEggComponent>.op_Implicit(egg))
		{
			BreakOnMove = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			RootEntity = true
		};
		_popup.PopupPredicted(base.Loc.GetString("rmc-xeno-egg-plant-self"), base.Loc.GetString("rmc-xeno-egg-plant", (ValueTuple<string, object>)("user", args.User)), Entity<XenoEggComponent>.op_Implicit(egg), args.User);
		_doAfter.TryStartDoAfter(doAfter);
	}

	private void OnXenoEggPlaceDoAfter(Entity<XenoEggComponent> egg, ref XenoEggPlaceDoAfterEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		if (CanPlaceEggPopup(args.User, egg, coordinates, handled: false, out var hiveweeds) && _plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(args.User), 30))
		{
			if (!hiveweeds)
			{
				EggsacSustain(args.User, egg);
			}
			_transform.SetCoordinates(Entity<XenoEggComponent>.op_Implicit(egg), ((EntitySystem)this).GetCoordinates(args.Coordinates));
			_transform.SetLocalRotation(Entity<XenoEggComponent>.op_Implicit(egg), Angle.op_Implicit(0f), (TransformComponent)null);
			SetEggState(egg, XenoEggState.Growing);
			_transform.AnchorEntity(Entity<XenoEggComponent>.op_Implicit(egg), ((EntitySystem)this).Transform(Entity<XenoEggComponent>.op_Implicit(egg)));
			_audio.PlayPredicted(egg.Comp.PlantSound, Entity<XenoEggComponent>.op_Implicit(egg), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void EggsacSustain(EntityUid user, Entity<XenoEggComponent> egg)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		SetEggSprite(egg, egg.Comp.SustainedSprite);
		XenoEggSustainerComponent sustainer = default(XenoEggSustainerComponent);
		if (_net.IsClient || !((EntitySystem)this).TryComp<XenoEggSustainerComponent>(user, ref sustainer))
		{
			return;
		}
		XenoFragileEggComponent fragile = ((EntitySystem)this).EnsureComp<XenoFragileEggComponent>(Entity<XenoEggComponent>.op_Implicit(egg));
		fragile.SustainedBy = user;
		fragile.SustainRange = sustainer.SustainedEggsRange;
		fragile.ExpireAt = _timing.CurTime + sustainer.SustainedEggMaxTime;
		fragile.ShortExpireAt = _timing.CurTime + fragile.SustainDuration;
		fragile.CheckSustainAt = _timing.CurTime + fragile.SustainCheckEvery;
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(egg.Owner));
		((EntitySystem)this).Dirty(Entity<XenoEggComponent>.op_Implicit(egg), (IComponent)(object)fragile, (MetaDataComponent)null);
		sustainer.SustainedEggs.Add(Entity<XenoEggComponent>.op_Implicit(egg));
		if (sustainer.SustainedEggs.Count > sustainer.MaxSustainedEggs)
		{
			EntityUid decayEgg = sustainer.SustainedEggs[0];
			sustainer.SustainedEggs.Remove(decayEgg);
			XenoFragileEggComponent fragileDecay = default(XenoFragileEggComponent);
			XenoEggComponent eggDecay = default(XenoEggComponent);
			if (((EntitySystem)this).TryComp<XenoFragileEggComponent>(decayEgg, ref fragileDecay) && ((EntitySystem)this).TryComp<XenoEggComponent>(decayEgg, ref eggDecay))
			{
				UnsustainEgg(decayEgg, eggDecay, fragileDecay, decay: true);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-sustain-egg-decaying", (ValueTuple<string, object>)("max", sustainer.MaxSustainedEggs)), user, user, PopupType.SmallCaution);
			}
		}
	}

	private void OnXenoEggActivateInWorld(Entity<XenoEggComponent> egg, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transformComp = default(TransformComponent);
		if (((EntitySystem)this).TryComp(egg.Owner, ref transformComp) && transformComp.Anchored && (((EntitySystem)this).HasComp<XenoParasiteComponent>(args.User) || (((EntitySystem)this).HasComp<XenoComponent>(args.User) && ((EntitySystem)this).HasComp<HandsComponent>(args.User))) && Open(egg, args.User, out var _))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnXenoEggInteractUsing(Entity<XenoEggComponent> egg, ref InteractUsingEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid used = args.Used;
		if (!((EntitySystem)this).HasComp<XenoParasiteComponent>(used) || !_rmcHands.IsPickupByAllowed(Entity<WhitelistPickupByComponent>.op_Implicit(args.Used), Entity<WhitelistPickupComponent>.op_Implicit(user)))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!_net.IsClient && CanReturnParasitePopup(user, used, egg))
		{
			TimeSpan plantTime = TimeSpan.FromSeconds(3.5);
			EggPlantTimeComponent time = default(EggPlantTimeComponent);
			if (((EntitySystem)this).TryComp<EggPlantTimeComponent>(args.User, ref time))
			{
				plantTime = time.PlantTime;
			}
			XenoEggReturnParasiteDoAfterEvent ev = new XenoEggReturnParasiteDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, plantTime, ev, Entity<XenoEggComponent>.op_Implicit(egg), Entity<XenoEggComponent>.op_Implicit(egg), used)
			{
				BreakOnMove = true,
				BlockDuplicate = true,
				DuplicateCondition = DuplicateConditions.SameEvent
			};
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-return-start"), args.User, args.User);
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnXenoEggReturnParasiteDoAfter(Entity<XenoEggComponent> egg, ref XenoEggReturnParasiteDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? used = args.Used;
		if (used.HasValue)
		{
			EntityUid used2 = used.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = true;
			if (!_net.IsClient && CanReturnParasitePopup(args.User, used2, egg))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-return-user"), args.User, args.User);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-return", (ValueTuple<string, object>)("user", args.User), (ValueTuple<string, object>)("parasite", args.Used)), Entity<XenoEggComponent>.op_Implicit(egg), Filter.PvsExcept(args.User, 2f, (IEntityManager)null), recordReplay: true);
				SetEggState(egg, XenoEggState.Grown);
				((EntitySystem)this).QueueDel(args.Used);
			}
		}
	}

	private void OnXenoEggStepTriggerAttempt(Entity<XenoEggComponent> egg, ref StepTriggerAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (CanTrigger(args.Tripper))
		{
			args.Continue = true;
		}
	}

	private void OnXenoEggStepTriggered(Entity<XenoEggComponent> egg, ref StepTriggeredOffEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		TryTrigger(egg, args.Tripper);
	}

	private void OnGetVerbs(Entity<XenoEggComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = args.User;
		XenoFragileEggComponent fragile = default(XenoFragileEggComponent);
		if (((EntitySystem)this).HasComp<ActorComponent>(uid) && ((EntitySystem)this).HasComp<GhostComponent>(uid) && ent.Comp.State == XenoEggState.Grown && (!((EntitySystem)this).TryComp<XenoFragileEggComponent>(Entity<XenoEggComponent>.op_Implicit(ent), ref fragile) || !fragile.SustainedBy.HasValue))
		{
			ActivationVerb parasiteVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("rmc-xeno-egg-ghost-verb"),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)XenoParasiteGhostUI.Key, uid, false);
				},
				Impact = LogImpact.High
			};
			args.Verbs.Add(parasiteVerb);
		}
	}

	private bool CanTrigger(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		InfectableComponent infected = default(InfectableComponent);
		if (((EntitySystem)this).TryComp<InfectableComponent>(user, ref infected) && !infected.BeingInfected && !_mobState.IsDead(user))
		{
			return !((EntitySystem)this).HasComp<VictimInfectedComponent>(user);
		}
		return false;
	}

	public bool Open(Entity<XenoEggComponent> egg, EntityUid? user, out EntityUid? spawned)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		spawned = null;
		if (egg.Comp.State == XenoEggState.Opening)
		{
			return false;
		}
		if (egg.Comp.State == XenoEggState.Opened)
		{
			if (((EntitySystem)this).HasComp<XenoParasiteComponent>(user))
			{
				if (_mobState.IsDead(user.Value))
				{
					return true;
				}
				SetEggState(egg, XenoEggState.Grown);
				if (_net.IsClient)
				{
					return true;
				}
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-return-self", (ValueTuple<string, object>)("parasite", user)), Entity<XenoEggComponent>.op_Implicit(egg));
				((EntitySystem)this).QueueDel(user);
				return true;
			}
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-egg-clear"), Entity<XenoEggComponent>.op_Implicit(egg), user.Value);
			}
			if (_net.IsClient)
			{
				return true;
			}
			((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoEggComponent>.op_Implicit(egg));
			return true;
		}
		if (((EntitySystem)this).HasComp<XenoParasiteComponent>(user))
		{
			if (egg.Comp.State == XenoEggState.Grown || egg.Comp.State == XenoEggState.Growing)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-egg-has-child"), user.Value);
			}
			return true;
		}
		if (egg.Comp.State != XenoEggState.Grown)
		{
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-egg-not-developed"), Entity<XenoEggComponent>.op_Implicit(egg), user.Value);
			}
			return false;
		}
		SetEggState(egg, XenoEggState.Opening);
		if (_timing.IsFirstTimePredicted)
		{
			_audio.PlayPredicted(egg.Comp.OpenSound, Entity<XenoEggComponent>.op_Implicit(egg), user, (AudioParams?)null);
		}
		if (_net.IsClient)
		{
			return true;
		}
		ContainerSlot eggContainer = _container.EnsureContainer<ContainerSlot>(egg.Owner, egg.Comp.CreatureContainerId, (ContainerManagerComponent)null);
		spawned = ((EntitySystem)this).SpawnInContainerOrDrop(EntProtoId.op_Implicit(egg.Comp.Spawn), egg.Owner, ((BaseContainer)eggContainer).ID, (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(egg.Owner), Entity<HiveMemberComponent>.op_Implicit(spawned.Value));
		egg.Comp.SpawnedCreature = spawned;
		((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
		ParasiteAIComponent ai = default(ParasiteAIComponent);
		if (((EntitySystem)this).TryComp<ParasiteAIComponent>(spawned, ref ai))
		{
			_parasite.GoIdle(Entity<ParasiteAIComponent>.op_Implicit((spawned.Value, ai)));
		}
		return true;
	}

	private void SetEggState(Entity<XenoEggComponent> egg, XenoEggState state)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		egg.Comp.State = state;
		((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
		if (state == XenoEggState.Opened)
		{
			((EntitySystem)this).RemCompDeferred<XenoFriendlyComponent>(Entity<XenoEggComponent>.op_Implicit(egg));
		}
		UpdateEggSprite(egg);
		switch (state)
		{
		case XenoEggState.Item:
			if (egg.Comp.GrownFixtures)
			{
				egg.Comp.GrownFixtures = false;
				((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
				Fixture fixture2 = _fixture.GetFixtureOrNull(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingLayerFixture, (FixturesComponent)null);
				if (fixture2 != null)
				{
					_physics.SetCollisionLayer(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingLayerFixture, fixture2, 0, (FixturesComponent)null, (PhysicsComponent)null);
				}
				_fixture.DestroyFixture(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingMaskFixture, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null);
			}
			break;
		case XenoEggState.Growing:
		case XenoEggState.Grown:
		case XenoEggState.Opening:
		case XenoEggState.Opened:
		case XenoEggState.Fragile:
		case XenoEggState.Sustained:
			if (!egg.Comp.GrownFixtures)
			{
				egg.Comp.GrownFixtures = true;
				((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
				_fixture.TryCreateFixture(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingMaskShape, egg.Comp.GrowingMaskFixture, 1f, false, 0, (int)egg.Comp.GrowingMask, 0.4f, 0f, true, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
				Fixture fixture = _fixture.GetFixtureOrNull(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingLayerFixture, (FixturesComponent)null);
				if (fixture != null)
				{
					_physics.SetCollisionLayer(Entity<XenoEggComponent>.op_Implicit(egg), egg.Comp.GrowingLayerFixture, fixture, (int)egg.Comp.GrowingLayer, (FixturesComponent)null, (PhysicsComponent)null);
				}
			}
			break;
		}
	}

	private void SetEggSprite(Entity<XenoEggComponent> egg, string sprite)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		egg.Comp.CurrentSprite = sprite;
		((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
		UpdateEggSprite(egg);
	}

	private void UpdateEggSprite(Entity<XenoEggComponent> egg)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		XenoEggStateChangedEvent ev = default(XenoEggStateChangedEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoEggStateChangedEvent>(Entity<XenoEggComponent>.op_Implicit(egg), ref ev, false);
	}

	private void AttachOvipositor(Entity<XenoAttachedOvipositorComponent?> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		XenoAttachedOvipositorComponent attached = default(XenoAttachedOvipositorComponent);
		if (((EntitySystem)this).EnsureComp<XenoAttachedOvipositorComponent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno), ref attached))
		{
			return;
		}
		xeno.Comp = attached;
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		XenoGrowOvipositorActionComponent action = default(XenoGrowOvipositorActionComponent);
		foreach (Entity<ActionComponent> action3 in _actions.GetActions(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno)))
		{
			action3.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (((EntitySystem)this).TryComp<XenoGrowOvipositorActionComponent>(actionId, ref action))
			{
				_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionId), action.AttachCooldown);
				_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionId), toggled: true);
			}
		}
		XenoOvipositorCapableComponent capable = default(XenoOvipositorCapableComponent);
		if (((EntitySystem)this).TryComp<XenoOvipositorCapableComponent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno), ref capable))
		{
			RemoveOvipositorActions(Entity<XenoOvipositorCapableComponent>.op_Implicit((xeno.Owner, capable)));
			EntProtoId[] actionIds = capable.ActionIds;
			foreach (EntProtoId actionId2 in actionIds)
			{
				EntityUid? val2 = _actions.AddAction(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno), EntProtoId.op_Implicit(actionId2));
				if (val2.HasValue)
				{
					EntityUid action2 = val2.GetValueOrDefault();
					capable.Actions[actionId2] = action2;
				}
			}
		}
		((EntitySystem)this).EnsureComp<EggPlantingDistanceComponent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno)).Distance = 3.5f;
	}

	private void DetachOvipositor(Entity<XenoAttachedOvipositorComponent> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).RemCompDeferred<XenoAttachedOvipositorComponent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno)))
		{
			return;
		}
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		XenoGrowOvipositorActionComponent action = default(XenoGrowOvipositorActionComponent);
		foreach (Entity<ActionComponent> action2 in _actions.GetActions(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno)))
		{
			action2.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (((EntitySystem)this).TryComp<XenoGrowOvipositorActionComponent>(actionId, ref action))
			{
				_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionId), action.DetachCooldown);
				_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionId), toggled: false);
			}
		}
		RemoveOvipositorActions(Entity<XenoOvipositorCapableComponent>.op_Implicit(xeno.Owner));
		_popup.PopupClient(base.Loc.GetString("cm-xeno-ovipositor-detach"), Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno), Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno));
		((EntitySystem)this).RemCompDeferred<EggPlantingDistanceComponent>(Entity<XenoAttachedOvipositorComponent>.op_Implicit(xeno));
	}

	private bool TryTrigger(Entity<XenoEggComponent> egg, EntityUid tripper)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (egg.Comp.State != XenoEggState.Grown || !CanTrigger(tripper))
		{
			return false;
		}
		XenoParasiteComponent parasite = default(XenoParasiteComponent);
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(egg.Owner), Entity<TransformComponent>.op_Implicit(tripper)) || !Open(egg, tripper, out var spawned) || !((EntitySystem)this).TryComp<XenoParasiteComponent>(spawned, ref parasite))
		{
			return false;
		}
		egg.Comp.InfectTarget = tripper;
		((EntitySystem)this).Dirty<XenoEggComponent>(egg, (MetaDataComponent)null);
		return true;
	}

	private bool CanPlaceEggPopup(EntityUid user, Entity<XenoEggComponent> egg, EntityCoordinates coordinates, bool handled, out bool hasHiveWeeds)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		hasHiveWeeds = false;
		if (((EntitySystem)this).HasComp<MarineComponent>(user))
		{
			if (!handled)
			{
				_hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), Entity<XenoEggComponent>.op_Implicit(egg), coordinates);
				_popup.PopupClient(base.Loc.GetString("cm-xeno-egg-failed-plant-outside"), user, user);
			}
			return false;
		}
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i tile = _map.TileIndicesFor(gridId, grid2, coordinates);
				AnchoredEntitiesEnumerator anchored = _map.GetAnchoredEntitiesEnumerator(gridId, grid2, tile);
				hasHiveWeeds = _weeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), coordinates);
				EntityUid? uid = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
				{
					if (((EntitySystem)this).HasComp<XenoEggComponent>(uid))
					{
						string msg = base.Loc.GetString("cm-xeno-egg-failed-already-there");
						_popup.PopupClient(msg, uid.Value, user, PopupType.SmallCaution);
						return false;
					}
					if (((EntitySystem)this).HasComp<XenoConstructComponent>(uid) || _tags.HasAnyTag(uid.Value, StructureTag, AirlockTag) || ((EntitySystem)this).HasComp<StrapComponent>(uid) || ((EntitySystem)this).HasComp<XenoTunnelComponent>(uid))
					{
						string msg2 = base.Loc.GetString("cm-xeno-egg-blocked");
						_popup.PopupClient(msg2, uid.Value, user, PopupType.SmallCaution);
						return false;
					}
				}
				if (_turf.IsTileBlocked(gridId, tile, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, grid2))
				{
					string msg3 = base.Loc.GetString("cm-xeno-egg-blocked");
					_popup.PopupClient(msg3, coordinates, user, PopupType.SmallCaution);
					return false;
				}
				if (!hasHiveWeeds)
				{
					if (!((EntitySystem)this).HasComp<XenoEggSustainerComponent>(user))
					{
						_popup.PopupClient(base.Loc.GetString("cm-xeno-egg-failed-must-hive-weeds"), user, user);
					}
					else
					{
						if (_weeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), coordinates))
						{
							return true;
						}
						_popup.PopupClient(base.Loc.GetString("cm-xeno-egg-failed-must-weeds"), user, user);
					}
					return false;
				}
				return true;
			}
		}
		return false;
	}

	private bool CanReturnParasitePopup(EntityUid user, EntityUid used, Entity<XenoEggComponent> egg)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (_mobState.IsDead(used))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-dead-child"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (egg.Comp.State == XenoEggState.Growing || egg.Comp.State == XenoEggState.Grown)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-has-child"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (egg.Comp.State != XenoEggState.Opened)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-fail-return"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (!((EntitySystem)this).HasComp<ParasiteAIComponent>(used))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-egg-awake-child", (ValueTuple<string, object>)("parasite", used)), user, user, PopupType.SmallCaution);
			return false;
		}
		return true;
	}

	private void OnXenoEggBeforeDamageChanged(Entity<XenoEggComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == XenoEggState.Item)
		{
			args.Cancelled = true;
		}
	}

	private void RemoveOvipositorActions(Entity<XenoOvipositorCapableComponent?> capable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoOvipositorCapableComponent>(Entity<XenoOvipositorCapableComponent>.op_Implicit(capable), ref capable.Comp, false))
		{
			return;
		}
		foreach (KeyValuePair<EntProtoId, EntityUid> action in capable.Comp.Actions)
		{
			_actions.RemoveAction(Entity<ActionComponent>.op_Implicit(action.Value));
		}
		capable.Comp.Actions.Clear();
	}

	private void OnDestruction(Entity<XenoEggComponent> ent, ref DestructionEventArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			string proto = EntProtoId.op_Implicit(ent.Comp.EggDestroyed);
			if (ent.Comp.CurrentSprite == ent.Comp.FragileSprite)
			{
				proto = EntProtoId.op_Implicit(ent.Comp.EggDestroyedFragile);
			}
			else if (ent.Comp.CurrentSprite == ent.Comp.SustainedSprite)
			{
				proto = EntProtoId.op_Implicit(ent.Comp.EggDestroyedSustained);
			}
			EntityUid egg = ((EntitySystem)this).SpawnAtPosition(proto, ent.Owner.ToCoordinates(), (ComponentRegistry)null);
			_audio.PlayPvs(ent.Comp.BurstSound, egg, (AudioParams?)null);
		}
	}

	private void OnFragileConvert(Entity<XenoFragileEggComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		XenoEggSustainerComponent sustain = default(XenoEggSustainerComponent);
		if (ent.Comp.SustainedBy.HasValue && ((EntitySystem)this).TryComp<XenoEggSustainerComponent>(ent.Comp.SustainedBy, ref sustain))
		{
			sustain.SustainedEggs.Remove(Entity<XenoFragileEggComponent>.op_Implicit(ent));
		}
		XenoEggComponent egg = default(XenoEggComponent);
		if (((EntitySystem)this).TryComp<XenoEggComponent>(Entity<XenoFragileEggComponent>.op_Implicit(ent), ref egg))
		{
			SetEggSprite(Entity<XenoEggComponent>.op_Implicit((Entity<XenoFragileEggComponent>.op_Implicit(ent), egg)), egg.NormalSprite);
		}
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
	}

	private void OnFragileDelete(Entity<XenoFragileEggComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		XenoEggSustainerComponent sustain = default(XenoEggSustainerComponent);
		if (ent.Comp.SustainedBy.HasValue && ((EntitySystem)this).TryComp<XenoEggSustainerComponent>(ent.Comp.SustainedBy, ref sustain))
		{
			sustain.SustainedEggs.Remove(Entity<XenoFragileEggComponent>.op_Implicit(ent));
		}
	}

	private void OnFragileRefreshModifier(Entity<XenoFragileEggComponent> ent, ref RefreshNameModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoFragileEggComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			args.AddModifier(LocId.op_Implicit("rmc-xeno-fragile-egg-prefix"), 0);
		}
	}

	private void UnsustainEgg(EntityUid egg, XenoEggComponent eggComp, XenoFragileEggComponent fragile, bool decay = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			SetEggSprite(Entity<XenoEggComponent>.op_Implicit((egg, eggComp)), eggComp.FragileSprite);
			fragile.SustainedBy = null;
			((EntitySystem)this).Dirty(egg, (IComponent)(object)fragile, (MetaDataComponent)null);
			if (decay && !fragile.BurstAt.HasValue)
			{
				fragile.BurstAt = _timing.CurTime + fragile.BurstDelay;
				_jitter.DoJitter(egg, fragile.BurstDelay / 2.0, refresh: true, 40f, 8f, forceValueChange: true);
			}
		}
	}

	private void OnEggSustainerDelete(Entity<XenoEggSustainerComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		XenoFragileEggComponent frag = default(XenoFragileEggComponent);
		XenoEggComponent eggComp = default(XenoEggComponent);
		foreach (EntityUid egg in ent.Comp.SustainedEggs)
		{
			if (((EntitySystem)this).TryComp<XenoFragileEggComponent>(egg, ref frag) && ((EntitySystem)this).TryComp<XenoEggComponent>(egg, ref eggComp))
			{
				UnsustainEgg(egg, eggComp, frag);
			}
		}
	}

	private void OnEggSustainerDeath(Entity<XenoEggSustainerComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.ApplyingState || args.NewMobState != MobState.Dead)
		{
			return;
		}
		XenoFragileEggComponent frag = default(XenoFragileEggComponent);
		XenoEggComponent eggComp = default(XenoEggComponent);
		foreach (EntityUid egg in ent.Comp.SustainedEggs)
		{
			if (((EntitySystem)this).TryComp<XenoFragileEggComponent>(egg, ref frag) && ((EntitySystem)this).TryComp<XenoEggComponent>(egg, ref eggComp))
			{
				UnsustainEgg(egg, eggComp, frag, decay: true);
			}
		}
		ent.Comp.SustainedEggs.Clear();
		if (_timing.IsFirstTimePredicted)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-sustain-death", (ValueTuple<string, object>)("xeno", ent)), Entity<XenoEggSustainerComponent>.op_Implicit(ent), PopupType.MediumCaution);
			_audio.PlayPredicted(ent.Comp.DeathSound, Entity<XenoEggSustainerComponent>.op_Implicit(ent), (EntityUid?)Entity<XenoEggSustainerComponent>.op_Implicit(ent), (AudioParams?)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoOvipositorCapableComponent, XenoAttachedOvipositorComponent, TransformComponent> oviQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoOvipositorCapableComponent, XenoAttachedOvipositorComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		XenoOvipositorCapableComponent capable = default(XenoOvipositorCapableComponent);
		XenoAttachedOvipositorComponent attached = default(XenoAttachedOvipositorComponent);
		TransformComponent xform = default(TransformComponent);
		MobStateComponent state = default(MobStateComponent);
		EntityCoordinates val;
		while (oviQuery.MoveNext(ref uid, ref capable, ref attached, ref xform))
		{
			if (!attached.NextEgg.HasValue)
			{
				attached.NextEgg = time + capable.Cooldown;
				continue;
			}
			TimeSpan value = time;
			TimeSpan? nextEgg = attached.NextEgg;
			if (!(value < nextEgg) && (!((EntitySystem)this).TryComp<MobStateComponent>(uid, ref state) || !_mobState.IsIncapacitated(uid, state)))
			{
				attached.NextEgg = time + capable.Cooldown;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)attached, (MetaDataComponent)null);
				string text = EntProtoId.op_Implicit(capable.Spawn);
				val = xform.Coordinates;
				EntityUid egg = ((EntitySystem)this).SpawnAtPosition(text, ((EntityCoordinates)(ref val)).Offset(capable.Offset), (ComponentRegistry)null);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(egg));
				_transform.SetLocalRotation(egg, Angle.Zero, (TransformComponent)null);
			}
		}
		EntityQueryEnumerator<XenoEggComponent, TransformComponent> eggQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoEggComponent, TransformComponent>();
		EntityUid uid2 = default(EntityUid);
		XenoEggComponent egg2 = default(XenoEggComponent);
		TransformComponent xform2 = default(TransformComponent);
		StepTriggerComponent stepTrigger = default(StepTriggerComponent);
		MapGridComponent grid2 = default(MapGridComponent);
		XenoFragileEggComponent fragile = default(XenoFragileEggComponent);
		BaseContainer container = default(BaseContainer);
		XenoParasiteComponent para = default(XenoParasiteComponent);
		while (eggQuery.MoveNext(ref uid2, ref egg2, ref xform2))
		{
			if (egg2.State == XenoEggState.Grown && _stepTriggerQuery.TryComp(uid2, ref stepTrigger) && stepTrigger.CurrentlySteppedOn.Count > 0)
			{
				foreach (EntityUid current in stepTrigger.CurrentlySteppedOn)
				{
					if (TryTrigger(Entity<XenoEggComponent>.op_Implicit((uid2, egg2)), current))
					{
						break;
					}
				}
			}
			if (!xform2.Anchored)
			{
				continue;
			}
			if (time >= egg2.CheckWeedsAt)
			{
				egg2.CheckWeedsAt = time + egg2.CheckWeedsDelay;
				EntityUid? grid = _transform.GetGrid(uid2.ToCoordinates());
				if (!grid.HasValue)
				{
					continue;
				}
				EntityUid gridId = grid.GetValueOrDefault();
				if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
				{
					continue;
				}
				if (_weeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), uid2.ToCoordinates()))
				{
					if (((EntitySystem)this).HasComp<XenoFragileEggComponent>(uid2))
					{
						((EntitySystem)this).RemCompDeferred<XenoFragileEggComponent>(uid2);
					}
				}
				else if (!((EntitySystem)this).EnsureComp<XenoFragileEggComponent>(uid2, ref fragile))
				{
					fragile.ExpireAt = time + egg2.FragileEggDuration;
					SetEggSprite(Entity<XenoEggComponent>.op_Implicit((uid2, egg2)), egg2.FragileSprite);
					_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(uid2));
				}
			}
			XenoEggComponent xenoEggComponent;
			TimeSpan value;
			TimeSpan? nextEgg;
			if (egg2.State == XenoEggState.Growing)
			{
				xenoEggComponent = egg2;
				value = xenoEggComponent.GrowAt.GetValueOrDefault();
				if (!xenoEggComponent.GrowAt.HasValue)
				{
					value = time + _random.Next(egg2.MinTime, egg2.MaxTime);
					xenoEggComponent.GrowAt = value;
				}
				value = time;
				nextEgg = egg2.GrowAt;
				if (value < nextEgg || egg2.State != XenoEggState.Growing)
				{
					continue;
				}
				SetEggState(Entity<XenoEggComponent>.op_Implicit((uid2, egg2)), XenoEggState.Grown);
			}
			if (egg2.State != XenoEggState.Opening)
			{
				continue;
			}
			xenoEggComponent = egg2;
			value = xenoEggComponent.OpenAt.GetValueOrDefault();
			if (!xenoEggComponent.OpenAt.HasValue)
			{
				value = time + egg2.EggOpenTime;
				xenoEggComponent.OpenAt = value;
			}
			value = time;
			nextEgg = egg2.OpenAt;
			if (value < nextEgg || egg2.State != XenoEggState.Opening)
			{
				continue;
			}
			SetEggState(Entity<XenoEggComponent>.op_Implicit((uid2, egg2)), XenoEggState.Opened);
			EntityCoordinates coords = _transform.GetMoverCoordinates(uid2);
			if (_container.TryGetContainer(uid2, egg2.CreatureContainerId, ref container, (ContainerManagerComponent)null))
			{
				_container.EmptyContainer(container, false, (EntityCoordinates?)coords, true);
			}
			if (egg2.SpawnedCreature.HasValue)
			{
				_jitter.DoJitter(egg2.SpawnedCreature.Value, egg2.CreatureExitEggJitterDuration, refresh: true, 80f, 8f, forceValueChange: true);
				if (egg2.InfectTarget.HasValue && ((EntitySystem)this).TryComp<XenoParasiteComponent>(egg2.SpawnedCreature, ref para))
				{
					_parasite.Infect(Entity<XenoParasiteComponent>.op_Implicit((egg2.SpawnedCreature.Value, para)), egg2.InfectTarget.Value, popup: true, force: true);
					_stun.TryParalyze(egg2.InfectTarget.Value, egg2.KnockdownTime, refresh: true);
				}
				egg2.InfectTarget = null;
				egg2.SpawnedCreature = null;
				egg2.OpenAt = null;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)egg2, (MetaDataComponent)null);
			}
		}
		EntityQueryEnumerator<XenoFragileEggComponent> fragileEggQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFragileEggComponent>();
		EntityUid uid3 = default(EntityUid);
		XenoFragileEggComponent fragile2 = default(XenoFragileEggComponent);
		float distance = default(float);
		while (fragileEggQuery.MoveNext(ref uid3, ref fragile2))
		{
			TimeSpan value;
			TimeSpan? nextEgg;
			if (fragile2.BurstAt.HasValue)
			{
				value = time;
				nextEgg = fragile2.BurstAt;
				if (!(value < nextEgg))
				{
					_destruction.DestroyEntity(uid3);
				}
				continue;
			}
			if (fragile2.SustainedBy.HasValue)
			{
				if (!fragile2.InRange)
				{
					value = time;
					nextEgg = fragile2.ShortExpireAt;
					if (value >= nextEgg)
					{
						fragile2.BurstAt = time + fragile2.BurstDelay;
						_jitter.DoJitter(uid3, fragile2.BurstDelay / 2.0, refresh: true, 40f, 8f, forceValueChange: true);
						continue;
					}
				}
				value = time;
				nextEgg = fragile2.CheckSustainAt;
				if (value >= nextEgg)
				{
					fragile2.CheckSustainAt = time + fragile2.SustainCheckEvery;
					val = uid3.ToCoordinates();
					if (!((EntityCoordinates)(ref val)).TryDistance((IEntityManager)(object)base.EntityManager, fragile2.SustainedBy.Value.ToCoordinates(), ref distance))
					{
						continue;
					}
					if (distance > fragile2.SustainRange)
					{
						fragile2.InRange = false;
					}
					else
					{
						fragile2.InRange = true;
						fragile2.ShortExpireAt = time + fragile2.SustainDuration;
					}
				}
			}
			value = time;
			nextEgg = fragile2.ExpireAt;
			if (!(value < nextEgg))
			{
				fragile2.BurstAt = time + fragile2.BurstDelay;
				_jitter.DoJitter(uid3, fragile2.BurstDelay / 2.0, refresh: true, 40f, 8f, forceValueChange: true);
			}
		}
	}
}
