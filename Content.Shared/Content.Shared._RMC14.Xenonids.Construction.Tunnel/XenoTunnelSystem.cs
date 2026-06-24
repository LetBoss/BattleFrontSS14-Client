using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Construction.Tunnel;

public sealed class XenoTunnelSystem : EntitySystem
{
	private const string TunnelPrototypeId = "XenoTunnel";

	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedTacticalMapSystem _tacticalMap;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedXenoWeedsSystem _xenoWeeds;

	[Dependency]
	private SharedXenoConstructionSystem _xenoConstruct;

	private readonly List<string> _greekLetters = new List<string> { "alpha", "beta", "gamma", "delta", "zeta", "theta", "phi", "psi", "omega" };

	private int NextTempTunnelId { get; set; }

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoDigTunnelActionEvent>((EntityEventRefHandler<XenoComponent, XenoDigTunnelActionEvent>)OnCreateTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>((EntityEventRefHandler<XenoComponent, XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent>)OnCompleteRemoveWeedSource, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoDigTunnelDoAfter>((EntityEventRefHandler<XenoComponent, XenoDigTunnelDoAfter>)OnFinishCreateTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, InteractHandEvent>((EntityEventRefHandler<XenoTunnelComponent, InteractHandEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<XenoTunnelComponent, GetVerbsEvent<InteractionVerb>>)OnGetInteractVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, ContainerRelayMovementEntityEvent>((EntityEventRefHandler<XenoTunnelComponent, ContainerRelayMovementEntityEvent>)OnAttemptMoveInTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, TraverseXenoTunnelMessage>((EntityEventRefHandler<XenoTunnelComponent, TraverseXenoTunnelMessage>)OnMoveThroughTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, EnterXenoTunnelDoAfterEvent>((EntityEventRefHandler<XenoTunnelComponent, EnterXenoTunnelDoAfterEvent>)OnFinishEnterTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, TraverseXenoTunnelDoAfterEvent>((EntityEventRefHandler<XenoTunnelComponent, TraverseXenoTunnelDoAfterEvent>)OnFinishMoveThroughTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, OpenBoundInterfaceMessage>((EntityEventRefHandler<XenoTunnelComponent, OpenBoundInterfaceMessage>)GetAllAvailableTunnels, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, ExaminedEvent>((EntityEventRefHandler<XenoTunnelComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<XenoTunnelComponent, GetVerbsEvent<ActivationVerb>>)OnGetRenameVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, InteractUsingEvent>((EntityEventRefHandler<XenoTunnelComponent, InteractUsingEvent>)OnFillTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, XenoCollapseTunnelDoAfterEvent>((EntityEventRefHandler<XenoTunnelComponent, XenoCollapseTunnelDoAfterEvent>)OnCollapseTunnelFinish, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoTunnelComponent, EntityTerminatingEvent>)OnDeleteTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<XenoTunnelComponent, EntInsertedIntoContainerMessage>)OnTunnelEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoTunnelComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<XenoTunnelComponent, ContainerIsInsertingAttemptEvent>)OnInsertEntityIntoTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InXenoTunnelComponent, RegurgitateEvent>((EntityEventRefHandler<InXenoTunnelComponent, RegurgitateEvent>)OnRegurgitateInTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InXenoTunnelComponent, ComponentInit>((EntityEventRefHandler<InXenoTunnelComponent, ComponentInit>)OnInTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InXenoTunnelComponent, ComponentRemove>((EntityEventRefHandler<InXenoTunnelComponent, ComponentRemove>)OnOutTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InXenoTunnelComponent, DropAttemptEvent>((EntityEventRefHandler<InXenoTunnelComponent, DropAttemptEvent>)OnTryDropInTunnel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InXenoTunnelComponent, MobStateChangedEvent>((EntityEventRefHandler<InXenoTunnelComponent, MobStateChangedEvent>)OnDeathInTunnel, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoTunnelComponent>(((EntitySystem)this).Subs, (object)NameTunnelUI.Key, (BuiEventSubscriber<XenoTunnelComponent>)delegate(Subscriber<XenoTunnelComponent> subs)
		{
			subs.Event<NameTunnelMessage>((EntityEventRefHandler<XenoTunnelComponent, NameTunnelMessage>)OnNameTunnel);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<XenoTunnelComponent>(((EntitySystem)this).Subs, (object)SelectDestinationTunnelUI.Key, (BuiEventSubscriber<XenoTunnelComponent>)delegate(Subscriber<XenoTunnelComponent> subs)
		{
			subs.Event<BoundUIOpenedEvent>((EntityEventRefHandler<XenoTunnelComponent, BoundUIOpenedEvent>)OnTunnelUIOpened);
			subs.Event<BoundUIClosedEvent>((EntityEventRefHandler<XenoTunnelComponent, BoundUIClosedEvent>)OnTunnelUIClosed);
		});
	}

	private void OnTunnelUIOpened(Entity<XenoTunnelComponent> tunnel, ref BoundUIOpenedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((EntitySystem)this).EnsureComp<TunnelUIUserComponent>(((BaseBoundUserInterfaceEvent)args).Actor);
		}
	}

	private void OnTunnelUIClosed(Entity<XenoTunnelComponent> tunnel, ref BoundUIClosedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((EntitySystem)this).RemCompDeferred<TunnelUIUserComponent>(((BaseBoundUserInterfaceEvent)args).Actor);
		}
	}

	private void OnExamine(Entity<XenoTunnelComponent> xenoTunnel, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner) || !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(args.Examiner), Entity<HiveMemberComponent>.op_Implicit(xenoTunnel.Owner)))
		{
			LocId message = LocId.op_Implicit("rmc-xeno-construction-tunnel-examine-not-xeno-empty");
			if (((BaseContainer)_container.EnsureContainer<Container>(Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel), "rmc_xeno_tunnel_mob_container", (ContainerManagerComponent)null)).ContainedEntities.Count > 0)
			{
				message = LocId.op_Implicit("rmc-xeno-construction-tunnel-examine-not-xeno");
			}
			using (args.PushGroup("XenoTunnelComponent"))
			{
				args.PushMarkup(base.Loc.GetString(LocId.op_Implicit(message)));
				return;
			}
		}
		if (!TryGetHiveTunnelName(xenoTunnel, out string tunnelName))
		{
			return;
		}
		using (args.PushGroup("XenoTunnelComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-construction-tunnel-examine", (ValueTuple<string, object>)("tunnelName", tunnelName)));
		}
	}

	public bool TryGetHiveTunnelName(Entity<XenoTunnelComponent> xenoTunnel, [NotNullWhen(true)] out string? tunnelName)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		tunnelName = null;
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xenoTunnel.Owner));
		if (!hive.HasValue)
		{
			return false;
		}
		foreach (KeyValuePair<string, EntityUid> tunnel in hive.GetValueOrDefault().Comp.HiveTunnels)
		{
			if (tunnel.Value == xenoTunnel.Owner)
			{
				tunnelName = tunnel.Key;
				return true;
			}
		}
		return false;
	}

	public bool TryPlaceTunnel(EntityUid associatedHiveEnt, string? name, EntityCoordinates buildLocation, [NotNullWhen(true)] out EntityUid? tunnelEnt)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		tunnelEnt = null;
		HiveComponent hiveComp = default(HiveComponent);
		if (!((EntitySystem)this).TryComp<HiveComponent>(associatedHiveEnt, ref hiveComp))
		{
			return false;
		}
		Dictionary<string, EntityUid> tunnels = hiveComp.HiveTunnels;
		if (name == null)
		{
			MapCoordinates mapCoords = _transform.ToMapCoordinates(CoordinatesExtensions.AlignWithClosestGridTile(buildLocation, 1.5f, (IEntityManager)null, (IMapManager)null), true);
			string areaName = base.Loc.GetString("rmc-xeno-construction-default-area-name");
			string randomGreekLetter = RandomExtensions.Pick<string>(_random, (IReadOnlyList<string>)_greekLetters);
			if (_area.TryGetArea(buildLocation, out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				areaName = areaProto.Name;
			}
			name = base.Loc.GetString("rmc-xeno-construction-default-tunnel-name", new(string, object)[4]
			{
				("areaName", areaName),
				("coordX", ((MapCoordinates)(ref mapCoords)).X),
				("coordY", ((MapCoordinates)(ref mapCoords)).Y),
				("greekLetter", randomGreekLetter)
			});
		}
		if (tunnels.ContainsKey(name))
		{
			return false;
		}
		EntityUid newTunnel = ((EntitySystem)this).Spawn("XenoTunnel", buildLocation);
		tunnelEnt = newTunnel;
		_hive.SetHive(Entity<HiveMemberComponent>.op_Implicit(newTunnel), associatedHiveEnt);
		return hiveComp.HiveTunnels.TryAdd(name, newTunnel);
	}

	private void OnCreateTunnel(Entity<XenoComponent> xenoBuilder, ref XenoDigTunnelActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityCoordinates location = _transform.GetMoverCoordinates(Entity<XenoComponent>.op_Implicit(xenoBuilder)).SnapToGrid((IEntityManager?)(object)base.EntityManager);
		if (!CanPlaceTunnelPopup(args.Performer, location))
		{
			return;
		}
		EntityUid? grid = _transform.GetGrid(location);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2) && !((EntitySystem)this).HasComp<AlmayerComponent>(gridId))
			{
				if (!_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xenoBuilder.Owner), args.PlasmaCost, predicted: false))
				{
					return;
				}
				if (!_area.TryGetArea(location, out Entity<AreaComponent>? area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-bad-area-tunnel"), Entity<XenoComponent>.op_Implicit(xenoBuilder), Entity<XenoComponent>.op_Implicit(xenoBuilder));
					return;
				}
				Entity<XenoWeedsComponent>? weedsOnFloor = _xenoWeeds.GetWeedsOnFloor(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), location, sourceOnly: true);
				if (weedsOnFloor.HasValue)
				{
					Entity<XenoWeedsComponent> weedSource = weedsOnFloor.GetValueOrDefault();
					XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent weedRemovalEv = new XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent
					{
						CreateTunnelDelay = args.CreateTunnelDelay,
						PlasmaCost = args.PlasmaCost,
						Prototype = args.Prototype
					};
					DoAfterArgs doAfterWeedRemovalArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, xenoBuilder.Owner, args.DestroyWeedSourceDelay, weedRemovalEv, xenoBuilder.Owner, Entity<XenoWeedsComponent>.op_Implicit(weedSource))
					{
						BlockDuplicate = true,
						BreakOnMove = true,
						DuplicateCondition = DuplicateConditions.SameTarget
					};
					_doAfter.TryStartDoAfter(doAfterWeedRemovalArgs);
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-resin-tunnel-uproot"), args.Performer, args.Performer);
					((HandledEntityEventArgs)args).Handled = true;
				}
				else
				{
					XenoDigTunnelDoAfter createTunnelEv = new XenoDigTunnelDoAfter(args.Prototype, args.PlasmaCost);
					DoAfterArgs doAfterTunnelCreationArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, xenoBuilder.Owner, args.CreateTunnelDelay, createTunnelEv, xenoBuilder.Owner)
					{
						BlockDuplicate = true,
						BreakOnMove = true,
						DuplicateCondition = DuplicateConditions.SameTarget,
						RootEntity = true
					};
					_doAfter.TryStartDoAfter(doAfterTunnelCreationArgs);
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-resin-tunnel-create-tunnel"), args.Performer, args.Performer);
					((HandledEntityEventArgs)args).Handled = true;
				}
				return;
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-bad-area-tunnel"), Entity<XenoComponent>.op_Implicit(xenoBuilder), Entity<XenoComponent>.op_Implicit(xenoBuilder));
	}

	private void OnCompleteRemoveWeedSource(Entity<XenoComponent> xenoBuilder, ref XenoPlaceResinTunnelDestroyWeedSourceDoAfterEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoDigTunnelActionEvent>(xenoBuilder.Owner))
			{
				_action.ClearCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))));
			}
		}
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled && args.Target.HasValue && _xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xenoBuilder.Owner), args.PlasmaCost, predicted: false))
		{
			if (_net.IsClient)
			{
				((EntitySystem)this).QueueDel(args.Target);
			}
			XenoDigTunnelDoAfter createTunnelEv = new XenoDigTunnelDoAfter(args.Prototype, args.PlasmaCost);
			DoAfterArgs doAfterTunnelCreationArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, xenoBuilder.Owner, args.CreateTunnelDelay, createTunnelEv, xenoBuilder.Owner)
			{
				BlockDuplicate = true,
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameTarget,
				RootEntity = true
			};
			_doAfter.TryStartDoAfter(doAfterTunnelCreationArgs);
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-resin-tunnel-create-tunnel"), xenoBuilder.Owner, xenoBuilder.Owner);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnFinishCreateTunnel(Entity<XenoComponent> xenoBuilder, ref XenoDigTunnelDoAfter args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			using IEnumerator<Entity<ActionComponent>> enumerator = _rmcActions.GetActionsWithEvent<XenoDigTunnelActionEvent>(xenoBuilder.Owner).GetEnumerator();
			if (enumerator.MoveNext())
			{
				Entity<ActionComponent> action = enumerator.Current;
				_action.ClearCooldown(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))));
			}
		}
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || !_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xenoBuilder.Owner), args.PlasmaCost))
		{
			return;
		}
		string tunnelFailureMessage = base.Loc.GetString("rmc-xeno-construction-failed-tunnel-rename");
		EntityCoordinates location = _transform.GetMoverCoordinates(Entity<XenoComponent>.op_Implicit(xenoBuilder)).SnapToGrid((IEntityManager?)(object)base.EntityManager);
		if (!CanPlaceTunnelPopup(xenoBuilder.Owner, location))
		{
			_popup.PopupClient(tunnelFailureMessage, xenoBuilder.Owner, xenoBuilder.Owner);
			return;
		}
		_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(xenoBuilder.Owner), args.PlasmaCost);
		if (!_net.IsClient)
		{
			if (!TryPlaceTunnel(Entity<HiveMemberComponent>.op_Implicit(xenoBuilder.Owner), null, out var newTunnelEnt))
			{
				_popup.PopupClient(tunnelFailureMessage, xenoBuilder.Owner, xenoBuilder.Owner);
				return;
			}
			NextTempTunnelId++;
			_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(newTunnelEnt.Value), (Enum)NameTunnelUI.Key, (EntityUid?)xenoBuilder.Owner, false);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnNameTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref NameTunnelMessage args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		string name = args.TunnelName;
		if (name.Length > 50)
		{
			name = name.Substring(0, 50);
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xenoTunnel.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		Dictionary<string, EntityUid> hiveTunnels = hive.Value.Comp.HiveTunnels;
		string curName = null;
		foreach (KeyValuePair<string, EntityUid> item in hiveTunnels)
		{
			if (item.Value == xenoTunnel.Owner)
			{
				curName = item.Key;
			}
		}
		if (!hiveTunnels.TryAdd(name, xenoTunnel.Owner))
		{
			_popup.PopupCursor(base.Loc.GetString("rmc-xeno-construction-failed-tunnel-rename"), ((BaseBoundUserInterfaceEvent)args).Actor);
			return;
		}
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(13, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" renamed ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel), (MetaDataComponent)null), "ToPrettyString(xenoTunnel)");
		handler.AppendLiteral(" to ");
		handler.AppendFormatted(name);
		adminLog.Add(LogType.RMCXenoTunnel, ref handler);
		if (curName != null)
		{
			hiveTunnels.Remove(curName);
		}
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xenoTunnel.Owner), (Enum)NameTunnelUI.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
	}

	private void OnGetInteractVerbs(Entity<XenoTunnelComponent> xenoTunnel, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract)
		{
			EntityUid user = args.User;
			EntityUid target = args.Target;
			InteractionVerb interactVerb = new InteractionVerb
			{
				Act = delegate
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					InteractHandEvent interactHandEvent = new InteractHandEvent(user, target);
					((EntitySystem)this).RaiseLocalEvent<InteractHandEvent>(Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel), interactHandEvent, false);
				},
				Text = base.Loc.GetString("xeno-ui-enter-tunnel-verb")
			};
			args.Verbs.Add(interactVerb);
		}
	}

	private void OnInteract(Entity<XenoTunnelComponent> xenoTunnel, ref InteractHandEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid enteringEntity = args.User;
		if (_container.ContainsEntity(xenoTunnel.Owner, enteringEntity, (ContainerManagerComponent)null))
		{
			OpenDestinationUI(xenoTunnel, enteringEntity);
			return;
		}
		Container mobContainer = _container.EnsureContainer<Container>(xenoTunnel.Owner, "rmc_xeno_tunnel_mob_container", (ContainerManagerComponent)null);
		RMCSizeComponent xenoSize = default(RMCSizeComponent);
		if (!((EntitySystem)this).HasComp<XenoComponent>(enteringEntity))
		{
			string msg = ((((BaseContainer)mobContainer).Count == 0) ? base.Loc.GetString("rmc-xeno-construction-tunnel-empty-non-xeno-enter-failure") : base.Loc.GetString("rmc-xeno-construction-tunnel-occupied-non-xeno-enter-failure"));
			_popup.PopupClient(msg, enteringEntity, enteringEntity);
		}
		else if (((BaseContainer)mobContainer).Count >= xenoTunnel.Comp.MaxMobs)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), enteringEntity, enteringEntity);
		}
		else if (!_actionBlocker.CanMove(enteringEntity) || ((EntitySystem)this).Transform(enteringEntity).Anchored)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-xeno-immobile-failure"), enteringEntity, enteringEntity);
		}
		else if (((EntitySystem)this).TryComp<RMCSizeComponent>(enteringEntity, ref xenoSize))
		{
			TimeSpan enterDelay = xenoTunnel.Comp.StandardXenoEnterDelay;
			TryGetHiveTunnelName(xenoTunnel, out string tunnelName);
			string enterMessageLocId = "rmc-xeno-construction-tunnel-default-xeno-enter";
			switch (xenoSize.Size)
			{
			case RMCSizes.Small:
				enterDelay = xenoTunnel.Comp.SmallXenoEnterDelay;
				enterMessageLocId = "rmc-xeno-construction-tunnel-default-xeno-enter";
				break;
			case RMCSizes.Big:
			case RMCSizes.Immobile:
				enterDelay = xenoTunnel.Comp.LargeXenoEnterDelay;
				enterMessageLocId = "rmc-xeno-construction-tunnel-large-xeno-enter";
				break;
			}
			if (tunnelName != null)
			{
				_popup.PopupClient(base.Loc.GetString(enterMessageLocId, (ValueTuple<string, object>)("tunnelName", tunnelName)), enteringEntity, enteringEntity);
			}
			EnterXenoTunnelDoAfterEvent ev = new EnterXenoTunnelDoAfterEvent();
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, enteringEntity, enterDelay, ev, xenoTunnel.Owner)
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnMoveThroughTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref TraverseXenoTunnelMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid startingTunnel = ((EntitySystem)this).GetEntity(((BoundUserInterfaceMessage)args).Entity);
		EntityUid traversingXeno = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (!_container.ContainsEntity(startingTunnel, traversingXeno, (ContainerManagerComponent)null))
		{
			return;
		}
		EntityUid destinationTunnel = ((EntitySystem)this).GetEntity(args.DestinationTunnel);
		if (!((EntitySystem)this).HasComp<XenoTunnelComponent>(destinationTunnel))
		{
			return;
		}
		RMCSizeComponent xenoSize = default(RMCSizeComponent);
		if (((BaseContainer)_container.EnsureContainer<Container>(destinationTunnel, "rmc_xeno_tunnel_mob_container", (ContainerManagerComponent)null)).Count >= xenoTunnel.Comp.MaxMobs)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), traversingXeno, traversingXeno);
		}
		else if (((EntitySystem)this).TryComp<RMCSizeComponent>(traversingXeno, ref xenoSize))
		{
			TimeSpan timeSpan;
			switch (xenoSize.Size)
			{
			case RMCSizes.Small:
				timeSpan = xenoTunnel.Comp.SmallXenoMoveDelay;
				break;
			case RMCSizes.Big:
			case RMCSizes.Immobile:
				timeSpan = xenoTunnel.Comp.LargeXenoMoveDelay;
				break;
			default:
				timeSpan = xenoTunnel.Comp.StandardXenoMoveDelay;
				break;
			}
			TimeSpan moveDelay = timeSpan;
			TraverseXenoTunnelDoAfterEvent ev = new TraverseXenoTunnelDoAfterEvent();
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, traversingXeno, moveDelay, ev, destinationTunnel, null, xenoTunnel.Owner)
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
		}
	}

	private void OnAttemptMoveInTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref ContainerRelayMovementEntityEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(args.Entity), Entity<TransformComponent>.op_Implicit(xenoTunnel.Owner));
		((EntitySystem)this).RemCompDeferred<InXenoTunnelComponent>(args.Entity);
	}

	private void OnFinishEnterTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref EnterXenoTunnelDoAfterEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid enteringEntity = args.User;
			Container mobContainer = _container.EnsureContainer<Container>(Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel), "rmc_xeno_tunnel_mob_container", (ContainerManagerComponent)null);
			if (((BaseContainer)mobContainer).Count >= xenoTunnel.Comp.MaxMobs)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), enteringEntity, enteringEntity);
				return;
			}
			if (!_actionBlocker.CanMove(enteringEntity) || ((EntitySystem)this).Transform(enteringEntity).Anchored)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-xeno-immobile-failure"), enteringEntity, enteringEntity);
				return;
			}
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(enteringEntity), (BaseContainer)(object)mobContainer, (TransformComponent)null, false);
			OpenDestinationUI(xenoTunnel, enteringEntity);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnFinishMoveThroughTunnel(Entity<XenoTunnelComponent> destinationXenoTunnel, ref TraverseXenoTunnelDoAfterEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid traversingXeno = args.User;
		EntityUid startingTunnel = args.Used.Value;
		if (!_container.ContainsEntity(startingTunnel, traversingXeno, (ContainerManagerComponent)null))
		{
			return;
		}
		EntityUid? map = _transform.GetMap(Entity<TransformComponent>.op_Implicit(startingTunnel));
		EntityUid? map2 = _transform.GetMap(Entity<TransformComponent>.op_Implicit(destinationXenoTunnel.Owner));
		if (map.HasValue == map2.HasValue && (!map.HasValue || !(map.GetValueOrDefault() != map2.GetValueOrDefault())))
		{
			Container mobContainer = _container.EnsureContainer<Container>(Entity<XenoTunnelComponent>.op_Implicit(destinationXenoTunnel), "rmc_xeno_tunnel_mob_container", (ContainerManagerComponent)null);
			if (((BaseContainer)mobContainer).Count >= destinationXenoTunnel.Comp.MaxMobs)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-full-xeno-failure"), traversingXeno, traversingXeno);
				return;
			}
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(traversingXeno), (BaseContainer)(object)mobContainer, (TransformComponent)null, false);
			OpenDestinationUI(destinationXenoTunnel, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnGetRenameVerb(Entity<XenoTunnelComponent> xenoTunnel, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && ((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			EntityUid uid = args.User;
			ActivationVerb renameTunnelVerb = new ActivationVerb
			{
				Text = base.Loc.GetString("xeno-ui-rename-tunnel-verb"),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xenoTunnel.Owner), (Enum)NameTunnelUI.Key, uid, false);
				},
				Impact = LogImpact.Low
			};
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xenoTunnel.Owner), Entity<HiveMemberComponent>.op_Implicit(uid)) && ((EntitySystem)this).HasComp<TunnelRenamerComponent>(uid))
			{
				args.Verbs.Add(renameTunnelVerb);
			}
		}
	}

	private void GetAllAvailableTunnels(Entity<XenoTunnelComponent> destinationXenoTunnel, ref OpenBoundInterfaceMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(destinationXenoTunnel.Owner));
		Entity<HiveComponent>? val = hive;
		HiveComponent hiveComp = default(HiveComponent);
		if (!((EntitySystem)this).TryComp<HiveComponent>(val.HasValue ? new EntityUid?(Entity<HiveComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null), ref hiveComp))
		{
			return;
		}
		Dictionary<string, EntityUid> hiveTunnels = hiveComp.HiveTunnels;
		Dictionary<string, NetEntity> netHiveTunnels = new Dictionary<string, NetEntity>();
		foreach (var (name, tunnel) in hiveTunnels)
		{
			netHiveTunnels.Add(name, ((EntitySystem)this).GetNetEntity(tunnel, (MetaDataComponent)null));
		}
		SelectDestinationTunnelInterfaceState newState = new SelectDestinationTunnelInterfaceState(netHiveTunnels);
		_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(destinationXenoTunnel.Owner), (Enum)SelectDestinationTunnelUI.Key, (BoundUserInterfaceState)(object)newState);
	}

	private void OnFillTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			EntityUid tool = args.Used;
			XenoTunnelFillerComponent tunnelFillerComp = default(XenoTunnelFillerComponent);
			ItemToggleComponent toggleComp = default(ItemToggleComponent);
			if (((EntitySystem)this).TryComp<XenoTunnelFillerComponent>(tool, ref tunnelFillerComp) && (!((EntitySystem)this).TryComp<ItemToggleComponent>(tool, ref toggleComp) || toggleComp.Activated))
			{
				((HandledEntityEventArgs)args).Handled = true;
				XenoCollapseTunnelDoAfterEvent ev = new XenoCollapseTunnelDoAfterEvent();
				DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, tunnelFillerComp.FillDelay, ev, xenoTunnel.Owner, Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel), tool)
				{
					BreakOnMove = true,
					NeedHand = true,
					BreakOnDropItem = true,
					BreakOnHandChange = true,
					RootEntity = true
				};
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-tunnel-fill"), args.User, args.User);
				_doAfter.TryStartDoAfter(doAfterArgs);
			}
		}
	}

	private void OnCollapseTunnelFinish(Entity<XenoTunnelComponent> xenoTunnel, ref XenoCollapseTunnelDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			CollapseTunnel(xenoTunnel);
		}
	}

	private void OnDeleteTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CollapseTunnel(xenoTunnel);
	}

	private void OnInsertEntityIntoTunnel(Entity<XenoTunnelComponent> xenoTunnel, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !(((ContainerAttemptEventBase)args).Container.ID != "rmc_xeno_tunnel_mob_container") && (!((EntitySystem)this).HasComp<XenoComponent>(((ContainerAttemptEventBase)args).EntityUid) || !_mobState.IsAlive(((ContainerAttemptEventBase)args).EntityUid)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnTunnelEntInserted(Entity<XenoTunnelComponent> xenoTunnel, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && ((EntitySystem)this).HasComp<MobStateComponent>(((ContainerModifiedMessage)args).Entity))
		{
			if (!_mobState.IsAlive(((ContainerModifiedMessage)args).Entity))
			{
				RemoveFromTunnel(((ContainerModifiedMessage)args).Entity, Entity<XenoTunnelComponent>.op_Implicit(xenoTunnel));
			}
			((EntitySystem)this).EnsureComp<InXenoTunnelComponent>(((ContainerModifiedMessage)args).Entity);
		}
	}

	private void CollapseTunnel(Entity<XenoTunnelComponent> xenoTunnel)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xenoTunnel.Owner));
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			if (TryGetHiveTunnelName(xenoTunnel, out string tunnelName))
			{
				hive2.Comp.HiveTunnels.Remove(tunnelName);
			}
		}
		BaseContainer mobContainer = default(BaseContainer);
		if (_container.TryGetContainer(xenoTunnel.Owner, "rmc_xeno_tunnel_mob_container", ref mobContainer, (ContainerManagerComponent)null))
		{
			EntityUid[] array = mobContainer.ContainedEntities.ToArray();
			foreach (EntityUid mob in array)
			{
				RemoveFromTunnel(mob, mobContainer.Owner);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-tunnel-fill-xeno-drop"), mob, mob);
			}
		}
		((EntitySystem)this).QueueDel((EntityUid?)xenoTunnel.Owner);
	}

	private void OnInTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DisableAllAbilities(tunneledXeno.Owner);
	}

	private void OnOutTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EnableAllAbilities(tunneledXeno.Owner);
	}

	private void OnTryDropInTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref DropAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnDeathInTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref MobStateChangedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer mobContainer = default(BaseContainer);
		if (args.NewMobState == MobState.Dead && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<InXenoTunnelComponent>.op_Implicit(tunneledXeno), null, null)), ref mobContainer))
		{
			EntityUid tunnel = mobContainer.Owner;
			RemoveFromTunnel(Entity<InXenoTunnelComponent>.op_Implicit(tunneledXeno), tunnel);
		}
	}

	private void OnRegurgitateInTunnel(Entity<InXenoTunnelComponent> tunneledXeno, ref RegurgitateEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		EntityUid regurgitated = ((EntitySystem)this).GetEntity(args.NetRegurgitated);
		BaseContainer mobContainer = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<InXenoTunnelComponent>.op_Implicit(tunneledXeno), null, null)), ref mobContainer))
		{
			EntityUid tunnel = mobContainer.Owner;
			RemoveFromTunnel(regurgitated, tunnel);
		}
	}

	private void RemoveFromTunnel(EntityUid tunneledMob, EntityUid tunnel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<InXenoTunnelComponent>(tunneledMob);
		_transform.DropNextTo(Entity<TransformComponent>.op_Implicit(tunneledMob), Entity<TransformComponent>.op_Implicit(tunnel));
	}

	private bool CanPlaceTunnelPopup(EntityUid user, EntityCoordinates coords)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!_xenoConstruct.CanPlaceXenoStructure(user, coords, out string popupType, needsWeeds: false))
		{
			popupType += "-tunnel";
			_popup.PopupClient(base.Loc.GetString(popupType), user, user, PopupType.SmallCaution);
			return false;
		}
		EntityUid? gridUid = ((EntitySystem)this).Transform(user).GridUid;
		if (gridUid.HasValue)
		{
			EntityUid gridId = gridUid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref gridComp))
			{
				TileRef tileRef = _map.GetTileRef(gridId, gridComp, coords);
				if (!_turf.GetContentTileDefinition(tileRef).CanPlaceTunnel)
				{
					_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-bad-tile-tunnel"), user, user, PopupType.SmallCaution);
					return false;
				}
				return true;
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-bad-tile-tunnel"), user, user, PopupType.SmallCaution);
		return false;
	}

	private void DisableAllAbilities(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetEnabledStatusAllAbilities(ent, newStatus: false);
	}

	private void EnableAllAbilities(EntityUid ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetEnabledStatusAllAbilities(ent, newStatus: true);
	}

	private void SetEnabledStatusAllAbilities(EntityUid ent, bool newStatus)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _action.GetActions(ent))
		{
			_action.SetEnabled(action.AsNullable(), newStatus);
		}
	}

	private bool TryPlaceTunnel(Entity<HiveMemberComponent?> builder, string? name, [NotNullWhen(true)] out EntityUid? tunnelEnt)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		tunnelEnt = null;
		if (((EntitySystem)this).Resolve<HiveMemberComponent>(Entity<HiveMemberComponent>.op_Implicit(builder), ref builder.Comp, true))
		{
			EntityUid? hive = builder.Comp.Hive;
			if (hive.HasValue)
			{
				bool result = TryPlaceTunnel(builder.Comp.Hive.Value, name, builder.Owner.ToCoordinates(), out tunnelEnt);
				if (tunnelEnt.HasValue)
				{
					_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(builder.Owner), Entity<HiveMemberComponent>.op_Implicit(tunnelEnt.Value));
				}
				return result;
			}
		}
		return false;
	}

	private void OpenDestinationUI(Entity<XenoTunnelComponent> tunnel, EntityUid enteringEntity)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		TacticalMapUserComponent userComp = default(TacticalMapUserComponent);
		if (_tacticalMap.TryGetTacticalMap(out Entity<TacticalMapComponent> map) && ((EntitySystem)this).TryComp<TacticalMapUserComponent>(enteringEntity, ref userComp))
		{
			_tacticalMap.UpdateUserData(Entity<TacticalMapUserComponent>.op_Implicit((enteringEntity, userComp)), Entity<TacticalMapComponent>.op_Implicit(map));
		}
		_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(tunnel.Owner), (Enum)SelectDestinationTunnelUI.Key, (EntityUid?)enteringEntity, false);
	}
}
