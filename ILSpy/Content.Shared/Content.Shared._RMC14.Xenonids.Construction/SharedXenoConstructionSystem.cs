using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Sentry;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Buckle.Components;
using Content.Shared.Climbing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.Construction;

public sealed class SharedXenoConstructionSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedXenoAnnounceSystem _announce;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ISharedAdminLogManager _adminLogs;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private IMapManager _map;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private QueenEyeSystem _queenEye;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private XenoNestSystem _xenoNest;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedXenoWeedsSystem _xenoWeeds;

	[Dependency]
	private ITileDefinitionManager _tile;

	private static readonly ProtoId<TagPrototype> AirlockTag = ProtoId<TagPrototype>.op_Implicit("Airlock");

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private static readonly ProtoId<TagPrototype> PlatformTag = ProtoId<TagPrototype>.op_Implicit("Platform");

	private static readonly ImmutableArray<Direction> Directions = (from d in Enum.GetValues<Direction>()
		where (int)d != -1
		select d).ToImmutableArray();

	private EntityQuery<BlockXenoConstructionComponent> _blockXenoConstructionQuery;

	private EntityQuery<XenoConstructionSupportComponent> _constructionSupportQuery;

	private EntityQuery<XenoConstructionRequiresSupportComponent> _constructionRequiresSupportQuery;

	private EntityQuery<HiveConstructionNodeComponent> _hiveConstructionNodeQuery;

	private EntityQuery<SentryComponent> _sentryQuery;

	private EntityQuery<TransformComponent> _transformQuery;

	private EntityQuery<XenoConstructComponent> _xenoConstructQuery;

	private EntityQuery<XenoEggComponent> _xenoEggQuery;

	private EntityQuery<XenoTunnelComponent> _xenoTunnelQuery;

	private EntityQuery<QueenBuildingBoostComponent> _queenBoostQuery;

	private const string XenoStructuresAnimation = "RMCEffect";

	private const string XenoHiveCoreNodeId = "HiveCoreXenoConstructionNode";

	private float _densityThreshold;

	private TimeSpan _newResinPreventCollideTime;

	private readonly HashSet<EntityUid> _intersectingResin = new HashSet<EntityUid>();

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
		_blockXenoConstructionQuery = ((EntitySystem)this).GetEntityQuery<BlockXenoConstructionComponent>();
		_constructionSupportQuery = ((EntitySystem)this).GetEntityQuery<XenoConstructionSupportComponent>();
		_constructionRequiresSupportQuery = ((EntitySystem)this).GetEntityQuery<XenoConstructionRequiresSupportComponent>();
		_hiveConstructionNodeQuery = ((EntitySystem)this).GetEntityQuery<HiveConstructionNodeComponent>();
		_sentryQuery = ((EntitySystem)this).GetEntityQuery<SentryComponent>();
		_transformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		_xenoConstructQuery = ((EntitySystem)this).GetEntityQuery<XenoConstructComponent>();
		_xenoEggQuery = ((EntitySystem)this).GetEntityQuery<XenoEggComponent>();
		_xenoTunnelQuery = ((EntitySystem)this).GetEntityQuery<XenoTunnelComponent>();
		_queenBoostQuery = ((EntitySystem)this).GetEntityQuery<QueenBuildingBoostComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoPlantWeedsActionEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoPlantWeedsActionEvent>)OnXenoPlantWeedsAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoExpandWeedsActionEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoExpandWeedsActionEvent>)OnXenoExpandWeedsAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoChooseStructureActionEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoChooseStructureActionEvent>)OnXenoChooseStructureAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureActionEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoSecreteStructureActionEvent>)OnXenoSecreteStructureAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoSecreteStructureDoAfterEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoSecreteStructureDoAfterEvent>)OnXenoSecreteStructureDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionActionEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionActionEvent>)OnXenoOrderConstructionAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionComponent, XenoOrderConstructionDoAfterEvent>((EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionDoAfterEvent>)OnXenoOrderConstructionDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoCanAddPlasmaToConstructComponent, XenoConstructionAddPlasmaDoAfterEvent>((EntityEventRefHandler<XenoCanAddPlasmaToConstructComponent, XenoConstructionAddPlasmaDoAfterEvent>)OnHiveConstructionNodeAddPlasmaDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoChooseConstructionActionComponent, XenoConstructionChosenEvent>((EntityEventRefHandler<XenoChooseConstructionActionComponent, XenoConstructionChosenEvent>)OnActionConstructionChosen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionActionComponent, ActionValidateEvent>((EntityEventRefHandler<XenoConstructionActionComponent, ActionValidateEvent>)OnSecreteActionValidateTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveConstructionNodeComponent, ExaminedEvent>((EntityEventRefHandler<HiveConstructionNodeComponent, ExaminedEvent>)OnHiveConstructionNodeExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveConstructionNodeComponent, ActivateInWorldEvent>((EntityEventRefHandler<HiveConstructionNodeComponent, ActivateInWorldEvent>)OnHiveConstructionNodeActivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RepairableXenoStructureComponent, ActivateInWorldEvent>((EntityEventRefHandler<RepairableXenoStructureComponent, ActivateInWorldEvent>)OnHiveConstructionRepair, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RepairableXenoStructureComponent, XenoRepairStructureDoAfterEvent>((EntityEventRefHandler<RepairableXenoStructureComponent, XenoRepairStructureDoAfterEvent>)OnHiveConstructionRepairDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWeedsComponent, XenoStructureRepairedEvent>((EntityEventRefHandler<XenoWeedsComponent, XenoStructureRepairedEvent>)OnWeedStructureRepair, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionSupportComponent, ComponentRemove>((EntityEventRefHandler<XenoConstructionSupportComponent, ComponentRemove>)OnCheckAdjacentCollapse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructionSupportComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoConstructionSupportComponent, EntityTerminatingEvent>)OnCheckAdjacentCollapse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAnnounceStructureDestructionComponent, DestructionEventArgs>((EntityEventRefHandler<XenoAnnounceStructureDestructionComponent, DestructionEventArgs>)OnXenoStructureDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeleteXenoResinOnHitComponent, ProjectileHitEvent>((EntityEventRefHandler<DeleteXenoResinOnHitComponent, ProjectileHitEvent>)OnDeleteXenoResinHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<XenoOrderConstructionClickEvent>((EntitySessionEventHandler<XenoOrderConstructionClickEvent>)OnXenoOrderConstructionClick, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<XenoOrderConstructionCancelEvent>((EntitySessionEventHandler<XenoOrderConstructionCancelEvent>)OnXenoOrderConstructionCancel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructComponent, MapInitEvent>((EntityEventRefHandler<XenoConstructComponent, MapInitEvent>)OnXenoConstructMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoConstructComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoConstructComponent, EntityTerminatingEvent>)OnXenoConstructRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRecentlyConstructedComponent, PreventCollideEvent>((EntityEventRefHandler<XenoRecentlyConstructedComponent, PreventCollideEvent>)OnRecentlyPreventCollide, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoConstructionComponent>(((EntitySystem)this).Subs, (object)XenoChooseStructureUI.Key, (BuiEventSubscriber<XenoConstructionComponent>)delegate(Subscriber<XenoConstructionComponent> subs)
		{
			subs.Event<XenoChooseStructureBuiMsg>((EntityEventRefHandler<XenoConstructionComponent, XenoChooseStructureBuiMsg>)OnXenoChooseStructureBui);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<XenoConstructionComponent>(((EntitySystem)this).Subs, (object)XenoOrderConstructionUI.Key, (BuiEventSubscriber<XenoConstructionComponent>)delegate(Subscriber<XenoConstructionComponent> subs)
		{
			subs.Event<XenoOrderConstructionBuiMsg>((EntityEventRefHandler<XenoConstructionComponent, XenoOrderConstructionBuiMsg>)OnXenoOrderConstructionBui);
		});
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedPhysicsSystem));
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCResinConstructionDensityCostIncreaseThreshold, (Action<float>)delegate(float v)
		{
			_densityThreshold = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCNewResinPreventCollideTimeSeconds, (Action<int>)delegate(int v)
		{
			_newResinPreventCollideTime = TimeSpan.FromSeconds(v);
		}, true);
	}

	private void OnXenoOrderConstructionClick(XenoOrderConstructionClickEvent ev, EntitySessionEventArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid user = attachedEntity.GetValueOrDefault();
		XenoConstructionComponent construction = default(XenoConstructionComponent);
		if (!((EntitySystem)this).TryComp<XenoConstructionComponent>(user, ref construction) || !construction.OrderConstructionTargeting)
		{
			return;
		}
		EntProtoId? orderConstructionChoice = construction.OrderConstructionChoice;
		EntProtoId structureId = ev.StructureId;
		if (!orderConstructionChoice.HasValue || orderConstructionChoice.GetValueOrDefault() != structureId)
		{
			return;
		}
		EntityCoordinates target = ((EntitySystem)this).GetCoordinates(ev.Target);
		if (!CanOrderConstructionPopup(Entity<XenoConstructionComponent>.op_Implicit((user, construction)), target, ev.StructureId))
		{
			return;
		}
		XenoOrderConstructionDoAfterEvent doAfterEvent = new XenoOrderConstructionDoAfterEvent(ev.StructureId, ev.Target);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, construction.OrderConstructionDelay, doAfterEvent, user)
		{
			BreakOnMove = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			construction.OrderConstructionTargeting = false;
			construction.OrderConstructionChoice = null;
			if (construction.ConfirmOrderConstructionAction.HasValue)
			{
				SharedActionsSystem actions = _actions;
				attachedEntity = construction.ConfirmOrderConstructionAction;
				actions.SetToggled(attachedEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(attachedEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
			}
			((EntitySystem)this).Dirty(user, (IComponent)(object)construction, (MetaDataComponent)null);
		}
	}

	private void OnXenoOrderConstructionCancel(XenoOrderConstructionCancelEvent ev, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			XenoConstructionComponent construction = default(XenoConstructionComponent);
			if (((EntitySystem)this).TryComp<XenoConstructionComponent>(user, ref construction))
			{
				CancelOrderConstructionTargeting(Entity<XenoConstructionComponent>.op_Implicit((user, construction)));
			}
		}
	}

	private void OnXenoStructureDestruction(Entity<XenoAnnounceStructureDestructionComponent> ent, ref DestructionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<HiveConstructionSuppressAnnouncementsComponent>(Entity<XenoAnnounceStructureDestructionComponent>.op_Implicit(ent)))
		{
			return;
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner));
		if (!hive.HasValue)
		{
			return;
		}
		Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
		string locationName = "Unknown";
		string structureName = "Unknown";
		if (_area.TryGetArea(ent.Owner, out Entity<AreaComponent>? _, out EntityPrototype areaProto))
		{
			locationName = areaProto.Name;
		}
		if (ent.Comp.StructureName == null)
		{
			EntityPrototype entProto = ((EntitySystem)this).Prototype(ent.Owner, (MetaDataComponent)null);
			if (entProto != null)
			{
				structureName = entProto.Name;
			}
		}
		else
		{
			structureName = ent.Comp.StructureName;
		}
		string msg = base.Loc.GetString(LocId.op_Implicit(ent.Comp.MessageID), new(string, object)[3]
		{
			("location", locationName),
			("structureName", structureName),
			("destructionVerb", ent.Comp.DestructionVerb)
		});
		SharedXenoAnnounceSystem announce = _announce;
		EntityUid owner = ent.Owner;
		EntityUid hive3 = Entity<HiveComponent>.op_Implicit(hive2);
		Color? color = ent.Comp.MessageColor;
		announce.AnnounceToHive(owner, hive3, msg, null, null, color);
	}

	private void OnXenoPlantWeedsAction(Entity<XenoConstructionComponent> xeno, ref XenoPlantWeedsActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<XenoConstructionComponent>.op_Implicit(xeno)).SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridUid = grid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref gridComp))
		{
			return;
		}
		Entity<MapGridComponent> grid2 = default(Entity<MapGridComponent>);
		grid2._002Ector(gridUid, gridComp);
		if (_xenoWeeds.IsOnWeeds(grid2, coordinates, sourceOnly: true))
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-weeds-source-already-here"), xeno.Owner, xeno.Owner);
			return;
		}
		Vector2i tile = _mapSystem.CoordinatesToTile(gridUid, gridComp, coordinates);
		if (_xenoWeeds.CanSpreadWeedsPopup(grid2, tile, Entity<XenoConstructionComponent>.op_Implicit(xeno), args.UseOnSemiWeedable, source: true) && _xenoWeeds.CanPlaceWeedsPopup(Entity<XenoConstructionComponent>.op_Implicit(xeno), grid2, coordinates, args.LimitDistance) && _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_net.IsServer)
			{
				EntityUid weeds = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(args.Prototype), coordinates);
				ISharedAdminLogManager adminLogs = _adminLogs;
				LogStringHandler handler = new LogStringHandler(24, 3);
				handler.AppendLiteral("Xeno ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
				handler.AppendLiteral(" planted weeds ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(weeds)), "weeds", "ToPrettyString(weeds)");
				handler.AppendLiteral(" at ");
				handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
				adminLogs.Add(LogType.RMCXenoPlantWeeds, ref handler);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(weeds));
			}
			_audio.PlayPredicted(xeno.Comp.BuildSound, coordinates, (EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
	}

	private void OnXenoExpandWeedsAction(Entity<XenoConstructionComponent> xeno, ref XenoExpandWeedsActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = args.Target;
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridUid = grid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref gridComp) || (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !_queenEye.CanSeeTarget(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner), coordinates)))
		{
			return;
		}
		Entity<MapGridComponent> grid2 = default(Entity<MapGridComponent>);
		grid2._002Ector(gridUid, gridComp);
		Entity<XenoWeedsComponent>? existing = _xenoWeeds.GetWeedsOnFloor(grid2, coordinates);
		if (existing.HasValue)
		{
			XenoWeedsComponent comp = existing.GetValueOrDefault().Comp;
			if (comp != null && comp.IsSource)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-weeds-source-already-here"), xeno.Owner, xeno.Owner);
				return;
			}
		}
		if (!existing.HasValue)
		{
			bool hasAdjacent = false;
			ImmutableArray<Direction>.Enumerator enumerator = _rmcMap.CardinalDirections.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				if (_rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(coordinates, (Direction?)null, (DirectionFlag)0))
				{
					hasAdjacent = true;
					break;
				}
			}
		}
		EntProtoId toSpawn = ((!existing.HasValue) ? args.Expand : args.Source);
		Vector2i tile = _mapSystem.CoordinatesToTile(gridUid, gridComp, coordinates);
		if (_xenoWeeds.CanSpreadWeedsPopup(grid2, tile, Entity<XenoConstructionComponent>.op_Implicit(xeno), semiWeedable: false, source: true) && _xenoWeeds.CanPlaceWeedsPopup(Entity<XenoConstructionComponent>.op_Implicit(xeno), grid2, coordinates, limitDistance: false) && _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_net.IsServer)
			{
				EntityUid weeds = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(toSpawn), coordinates);
				ISharedAdminLogManager adminLogs = _adminLogs;
				LogStringHandler handler = new LogStringHandler(24, 3);
				handler.AppendLiteral("Xeno ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
				handler.AppendLiteral(" planted weeds ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(weeds)), "weeds", "ToPrettyString(weeds)");
				handler.AppendLiteral(" at ");
				handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
				adminLogs.Add(LogType.RMCXenoPlantWeeds, ref handler);
				_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(weeds));
			}
			_audio.PlayPredicted(xeno.Comp.BuildSound, coordinates, (EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
	}

	private void OnXenoChooseStructureAction(Entity<XenoConstructionComponent> xeno, ref XenoChooseStructureActionEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoChooseStructureUI.Key, Entity<XenoConstructionComponent>.op_Implicit(xeno), false);
	}

	private void OnXenoChooseStructureBui(Entity<XenoConstructionComponent> xeno, ref XenoChooseStructureBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.CanBuild.Contains(args.StructureId))
		{
			return;
		}
		xeno.Comp.BuildChoice = args.StructureId;
		if (xeno.Comp.OrderConstructionTargeting)
		{
			xeno.Comp.OrderConstructionTargeting = false;
			if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
			{
				SharedActionsSystem actions = _actions;
				EntityUid? confirmOrderConstructionAction = xeno.Comp.ConfirmOrderConstructionAction;
				actions.SetToggled(confirmOrderConstructionAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(confirmOrderConstructionAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
			}
		}
		((EntitySystem)this).Dirty<XenoConstructionComponent>(xeno, (MetaDataComponent)null);
		XenoConstructionChosenEvent ev = new XenoConstructionChosenEvent(args.StructureId, xeno.Owner);
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoConstructionComponent>.op_Implicit(xeno)))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid id = val;
			((EntitySystem)this).RaiseLocalEvent<XenoConstructionChosenEvent>(id, ref ev, false);
		}
	}

	private void OnXenoSecreteStructureAction(Entity<XenoConstructionComponent> xeno, ref XenoSecreteStructureActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.OrderConstructionTargeting)
		{
			HandleSecreteResinPlacement(xeno, ref args);
		}
	}

	private EntProtoId GetQueenVariant(EntProtoId originalId)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		return (EntProtoId)(((EntProtoId)(ref originalId)).Id switch
		{
			"WallXenoResin" => EntProtoId.op_Implicit("WallXenoResinQueen"), 
			"WallXenoMembrane" => EntProtoId.op_Implicit("WallXenoMembraneQueen"), 
			"DoorXenoResin" => EntProtoId.op_Implicit("DoorXenoResinQueen"), 
			_ => originalId, 
		});
	}

	private EntProtoId GetQueenAnimationVariant(EntProtoId originalId)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		return (EntProtoId)(((EntProtoId)(ref originalId)).Id switch
		{
			"WallXenoResin" => EntProtoId.op_Implicit("WallXenoResinThick"), 
			"WallXenoMembrane" => EntProtoId.op_Implicit("WallXenoMembraneThick"), 
			"DoorXenoResin" => EntProtoId.op_Implicit("DoorXenoResinThick"), 
			_ => originalId, 
		});
	}

	private void HandleSecreteResinPlacement(Entity<XenoConstructionComponent> xeno, ref XenoSecreteStructureActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates snapped = args.Target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		bool hasBoost = _queenBoostQuery.HasComp(xeno.Owner);
		EntProtoId? to;
		if ((xeno.Comp.CanUpgrade || hasBoost) && _rmcMap.HasAnchoredEntityEnumerator<XenoStructureUpgradeableComponent>(snapped, out Entity<XenoStructureUpgradeableComponent> upgradeable, (Direction?)null, (DirectionFlag)0))
		{
			to = upgradeable.Comp.To;
			if (to.HasValue)
			{
				EntProtoId to2 = to.GetValueOrDefault();
				if (_prototype.HasIndex(to2))
				{
					QueenBuildingBoostComponent boost = default(QueenBuildingBoostComponent);
					if (!_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !((hasBoost && _queenBoostQuery.TryComp(xeno.Owner, ref boost)) ? _transform.InRange(_transform.GetMoverCoordinates(xeno.Owner), args.Target, boost.RemoteUpgradeRange) : _transform.InRange(_transform.GetMoverCoordinates(xeno.Owner), args.Target, xeno.Comp.BuildRange.Float())))
					{
						return;
					}
					FixedPoint2 cost = upgradeable.Comp.Cost;
					if (_area.TryGetArea(snapped, out Entity<AreaComponent>? area, out EntityPrototype _))
					{
						cost = GetDensityCost(area.Value, xeno, cost);
					}
					if (hasBoost || _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), cost))
					{
						string msg = (hasBoost ? ("We regurgitate some resin and thicken the " + ((EntitySystem)this).Name(Entity<XenoStructureUpgradeableComponent>.op_Implicit(upgradeable), (MetaDataComponent)null) + " effortlessly.") : $"We regurgitate some resin and thicken the {((EntitySystem)this).Name(Entity<XenoStructureUpgradeableComponent>.op_Implicit(upgradeable), (MetaDataComponent)null)}, using {cost} plasma.");
						_popup.PopupClient(msg, Entity<XenoStructureUpgradeableComponent>.op_Implicit(upgradeable), Entity<XenoConstructionComponent>.op_Implicit(xeno));
						if (!_net.IsClient)
						{
							((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoStructureUpgradeableComponent>.op_Implicit(upgradeable));
							EntityUid spawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(to2), snapped);
							_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(spawn));
							((HandledEntityEventArgs)args).Handled = true;
						}
					}
					return;
				}
			}
		}
		to = xeno.Comp.BuildChoice;
		if (!to.HasValue)
		{
			return;
		}
		EntProtoId choice = to.GetValueOrDefault();
		if (!CanSecreteOnTilePopup(xeno, choice, args.Target, checkStructureSelected: true, checkWeeds: true))
		{
			return;
		}
		XenoSecreteStructureAttemptEvent attempt = new XenoSecreteStructureAttemptEvent(args.Target);
		((EntitySystem)this).RaiseLocalEvent<XenoSecreteStructureAttemptEvent>(Entity<XenoConstructionComponent>.op_Implicit(xeno), ref attempt, false);
		if (!attempt.Cancelled)
		{
			EntProtoId animationChoice = (hasBoost ? GetQueenAnimationVariant(choice) : choice);
			string effectId = "RMCEffect" + EntProtoId.op_Implicit(animationChoice);
			NetCoordinates coordinates = ((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null);
			EntityCoordinates entityCoords = ((EntitySystem)this).GetCoordinates(coordinates);
			EntityUid? effect = null;
			float buildMult = GetBuildSpeed(choice) ?? 1f;
			QueenBuildingBoostComponent boostComp = default(QueenBuildingBoostComponent);
			if (hasBoost && _queenBoostQuery.TryComp(xeno.Owner, ref boostComp))
			{
				buildMult *= boostComp.BuildSpeedMultiplier;
			}
			TimeSpan finalBuildTime = xeno.Comp.BuildDelay * buildMult;
			if (_net.IsServer && _prototype.HasIndex(EntProtoId.op_Implicit(effectId)))
			{
				effect = ((EntitySystem)this).Spawn(effectId, entityCoords);
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new XenoConstructionAnimationStartEvent(((EntitySystem)this).GetNetEntity(effect.Value, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(Entity<XenoConstructionComponent>.op_Implicit(xeno), (MetaDataComponent)null), finalBuildTime), Filter.PvsExcept(effect.Value, 2f, (IEntityManager)null), true);
			}
			XenoSecreteStructureDoAfterEvent ev = new XenoSecreteStructureDoAfterEvent(coordinates, choice, ((EntitySystem)this).GetNetEntity(effect, (MetaDataComponent)null));
			((HandledEntityEventArgs)args).Handled = true;
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoConstructionComponent>.op_Implicit(xeno), finalBuildTime, ev, Entity<XenoConstructionComponent>.op_Implicit(xeno))
			{
				BreakOnMove = true,
				RootEntity = true,
				CancelDuplicate = false
			};
			if (!_doAfter.TryStartDoAfter(doAfter) && effect.HasValue && _net.IsServer)
			{
				((EntitySystem)this).QueueDel(effect);
			}
		}
	}

	public void CancelOrderConstructionTargeting(Entity<XenoConstructionComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.OrderConstructionTargeting)
		{
			xeno.Comp.OrderConstructionTargeting = false;
			xeno.Comp.OrderConstructionChoice = null;
			if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
			{
				SharedActionsSystem actions = _actions;
				EntityUid? confirmOrderConstructionAction = xeno.Comp.ConfirmOrderConstructionAction;
				actions.SetToggled(confirmOrderConstructionAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(confirmOrderConstructionAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: false);
			}
			((EntitySystem)this).Dirty<XenoConstructionComponent>(xeno, (MetaDataComponent)null);
		}
	}

	private void OnXenoSecreteStructureDoAfter(Entity<XenoConstructionComponent> xeno, ref XenoSecreteStructureDoAfterEvent args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && args.Effect.HasValue)
		{
			((EntitySystem)this).QueueDel(((EntitySystem)this).GetEntity(args.Effect));
		}
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		if (!((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager) || !xeno.Comp.CanBuild.Contains(args.StructureId) || !CanSecreteOnTilePopup(xeno, args.StructureId, ((EntitySystem)this).GetCoordinates(args.Coordinates), checkStructureSelected: true, checkWeeds: true))
		{
			return;
		}
		bool hasBoost = _queenBoostQuery.HasComp(xeno.Owner);
		if (_area.TryGetArea(((EntitySystem)this).GetCoordinates(args.Coordinates), out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			FixedPoint2? structurePlasmaCost = GetStructurePlasmaCost(args.StructureId);
			if (structurePlasmaCost.HasValue)
			{
				FixedPoint2 baseCost = structurePlasmaCost.GetValueOrDefault();
				FixedPoint2 cost = baseCost;
				EntityPrototype structure = default(EntityPrototype);
				XenoConstructionPlasmaCostComponent plasmaCost = default(XenoConstructionPlasmaCostComponent);
				if (area.Value.Comp.ResinConstructCount != 0 && !area.Value.Comp.Unweedable && _prototype.TryIndex(args.StructureId, ref structure) && structure.TryGetComponent<XenoConstructionPlasmaCostComponent>(ref plasmaCost, _compFactory) && plasmaCost.ScalingCost)
				{
					cost = GetDensityCost(area.Value, xeno, cost);
				}
				if (!hasBoost && !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), cost))
				{
					return;
				}
			}
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			EntProtoId structureToSpawn = args.StructureId;
			if (hasBoost)
			{
				EntProtoId queenVariant = GetQueenVariant(args.StructureId);
				if (_prototype.HasIndex(queenVariant))
				{
					structureToSpawn = queenVariant;
				}
			}
			EntityUid structure2 = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(structureToSpawn), coordinates);
			_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(structure2));
			if (((EntitySystem)this).HasComp<XenoRecentlyBuiltPreventCollisionsComponent>(structure2))
			{
				_intersectingResin.Clear();
				_entityLookup.GetEntitiesIntersecting(structure2, _intersectingResin, (LookupFlags)110);
				XenoRecentlyConstructedComponent recently = null;
				foreach (EntityUid id in _intersectingResin)
				{
					if ((((EntitySystem)this).HasComp<MarineComponent>(id) || ((EntitySystem)this).HasComp<XenoComponent>(id)) && !(id == xeno.Owner))
					{
						if (recently == null)
						{
							recently = ((EntitySystem)this).EnsureComp<XenoRecentlyConstructedComponent>(structure2);
						}
						recently.StopCollide.Add(id);
					}
				}
				if (recently != null)
				{
					recently.ExpireAt = _timing.CurTime + _newResinPreventCollideTime;
					((EntitySystem)this).Dirty(structure2, (IComponent)(object)recently, (MetaDataComponent)null);
				}
			}
			ISharedAdminLogManager adminLogs = _adminLogs;
			LogStringHandler handler = new LogStringHandler(22, 3);
			handler.AppendLiteral("Xeno ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
			handler.AppendLiteral(" constructed ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(structure2)), "structure", "ToPrettyString(structure)");
			handler.AppendLiteral(" at ");
			handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
			adminLogs.Add(LogType.RMCXenoConstruct, ref handler);
		}
		_audio.PlayPredicted(xeno.Comp.BuildSound, coordinates, (EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (AudioParams?)null);
	}

	private void OnXenoOrderConstructionAction(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionActionEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoOrderConstructionUI.Key, Entity<XenoConstructionComponent>.op_Implicit(xeno), false);
	}

	private void OnXenoOrderConstructionBui(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.CanOrderConstruction.Contains(args.StructureId))
		{
			return;
		}
		xeno.Comp.OrderConstructionChoice = args.StructureId;
		xeno.Comp.OrderConstructionTargeting = true;
		if (xeno.Comp.ConfirmOrderConstructionAction.HasValue)
		{
			SharedActionsSystem actions = _actions;
			EntityUid? confirmOrderConstructionAction = xeno.Comp.ConfirmOrderConstructionAction;
			actions.SetToggled(confirmOrderConstructionAction.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(confirmOrderConstructionAction.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), toggled: true);
		}
		((EntitySystem)this).Dirty<XenoConstructionComponent>(xeno, (MetaDataComponent)null);
		XenoConstructionChosenEvent ev = new XenoConstructionChosenEvent(args.StructureId, xeno.Owner);
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoConstructionComponent>.op_Implicit(xeno)))
		{
			action.Deconstruct(ref val, ref actionComponent);
			EntityUid id = val;
			((EntitySystem)this).RaiseLocalEvent<XenoConstructionChosenEvent>(id, ref ev, false);
		}
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoOrderConstructionUI.Key);
	}

	private void OnXenoOrderConstructionDoAfter(Entity<XenoConstructionComponent> xeno, ref XenoOrderConstructionDoAfterEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates target = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		EntityPrototype prototype = default(EntityPrototype);
		if (!xeno.Comp.CanOrderConstruction.Contains(args.StructureId) || !CanOrderConstructionPopup(xeno, target, args.StructureId) || !((EntitySystem)this).TryComp<XenoPlasmaComponent>(Entity<XenoConstructionComponent>.op_Implicit(xeno), ref plasma) || !_prototype.TryIndex(args.StructureId, ref prototype))
		{
			return;
		}
		bool hasBoost = _queenBoostQuery.HasComp(xeno.Owner);
		HiveConstructionNodeComponent node = default(HiveConstructionNodeComponent);
		if ((prototype.TryGetComponent<HiveConstructionNodeComponent>(ref node, _compFactory) && !hasBoost && !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit((Entity<XenoConstructionComponent>.op_Implicit(xeno), plasma)), node.InitialPlasmaCost)) || _net.IsClient)
		{
			return;
		}
		EntityCoordinates coordinates = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		EntityUid structure = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(args.StructureId), coordinates);
		_hive.SetSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(structure));
		ISharedAdminLogManager adminLogs = _adminLogs;
		LogStringHandler handler = new LogStringHandler(34, 3);
		handler.AppendLiteral("Xeno ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoConstructionComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
		handler.AppendLiteral(" ordered construction of ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(structure)), "structure", "ToPrettyString(structure)");
		handler.AppendLiteral(" at ");
		handler.AppendFormatted<EntityCoordinates>(coordinates, "coordinates");
		adminLogs.Add(LogType.RMCXenoOrderConstruction, ref handler);
		EntityPrototype structureProto = default(EntityPrototype);
		if (_prototype.TryIndex(args.StructureId, ref structureProto))
		{
			HiveConstructionLimitedComponent hiveLimitedComp = default(HiveConstructionLimitedComponent);
			string msg;
			if (((EntitySystem)this).TryComp<HiveConstructionLimitedComponent>(structure, ref hiveLimitedComp) && CanPlaceLimitedHiveStructure(xeno.Owner, hiveLimitedComp, out var limit, out var curCount))
			{
				int? remainCount = limit - curCount;
				msg = base.Loc.GetString("rmc-xeno-order-construction-limited-structure-designated", new(string, object)[3]
				{
					("construct", structureProto.Name),
					("remainCount", remainCount),
					("maxCount", limit)
				});
				_popup.PopupEntity(msg, xeno.Owner, xeno.Owner);
			}
			string areaName = "Unknown";
			if (_area.TryGetArea(target, out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				areaName = areaProto.Name;
			}
			msg = base.Loc.GetString("rmc-xeno-order-construction-structure-designated", (ValueTuple<string, object>)("construct", structureProto.Name), (ValueTuple<string, object>)("area", areaName));
			_announce.AnnounceSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), msg, null, null, null, needsQueen: true);
		}
	}

	private void OnHiveConstructionNodeAddPlasmaDoAfter(Entity<XenoCanAddPlasmaToConstructComponent> xeno, ref XenoConstructionAddPlasmaDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		HiveConstructionNodeComponent node = default(HiveConstructionNodeComponent);
		TransformComponent transform = default(TransformComponent);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		if (!((EntitySystem)this).TryComp<HiveConstructionNodeComponent>(target2, ref node) || !((EntitySystem)this).TryComp(target2, ref transform) || !((EntitySystem)this).TryComp<XenoPlasmaComponent>(Entity<XenoCanAddPlasmaToConstructComponent>.op_Implicit(xeno), ref plasma) || !InRangePopup(args.User, transform.Coordinates, xeno.Comp.Range.Float()))
		{
			return;
		}
		bool num = _queenBoostQuery.HasComp(xeno.Owner);
		FixedPoint2 plasmaLeft = node.PlasmaCost - node.PlasmaStored;
		if (num)
		{
			node.PlasmaStored = node.PlasmaCost;
			plasmaLeft = 0;
		}
		else
		{
			FixedPoint2 subtract = FixedPoint2.Min(plasma.Plasma, plasmaLeft);
			if (plasmaLeft < FixedPoint2.Zero || plasma.Plasma < 1 || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit((args.User, plasma)), subtract))
			{
				return;
			}
			node.PlasmaStored += subtract;
			plasmaLeft = node.PlasmaCost - node.PlasmaStored;
		}
		((HandledEntityEventArgs)args).Handled = true;
		ISharedAdminLogManager adminLogs = _adminLogs;
		LogStringHandler handler = new LogStringHandler(26, 3);
		handler.AppendLiteral("Xeno ");
		handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoCanAddPlasmaToConstructComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
		handler.AppendLiteral(" added plasma to ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target2)), "target", "ToPrettyString(target)");
		handler.AppendLiteral(" at ");
		handler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "transform.Coordinates");
		adminLogs.Add(LogType.RMCXenoOrderConstructionPlasma, ref handler);
		if (node.PlasmaStored < node.PlasmaCost)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-requires-more-plasma", (ValueTuple<string, object>)("construction", target2), (ValueTuple<string, object>)("plasma", plasmaLeft)), target2, args.User);
		}
		else
		{
			TransformComponent xform = default(TransformComponent);
			if (!_transformQuery.TryComp(xeno.Owner, ref xform))
			{
				return;
			}
			target = _transform.GetGrid(Entity<TransformComponent>.op_Implicit((xeno.Owner, xform)));
			if (!target.HasValue)
			{
				return;
			}
			EntityUid gridId = target.GetValueOrDefault();
			MapGridComponent grid = default(MapGridComponent);
			if (!((EntityUid)(ref gridId)).Valid || !((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid))
			{
				return;
			}
			if (((EntitySystem)this).HasComp<HiveConstructionRequiresHiveWeedsComponent>(target2) && !_xenoWeeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid)), target2.ToCoordinates()))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-requires-hive-weeds", (ValueTuple<string, object>)("choice", target2)), target2, args.User);
				return;
			}
			if (((EntitySystem)this).HasComp<HiveConstructionRequiresSpaceComponent>(target2))
			{
				MapCoordinates mapCoords = _transform.GetMapCoordinates(target2, (TransformComponent)null);
				if (!CanPlaceSpaceRequiringStructurePopup(mapCoords, Entity<MapGridComponent>.op_Implicit((gridId, grid)), xeno.Owner, ((EntitySystem)this).MetaData(target2).EntityName))
				{
					return;
				}
			}
			if (!_net.IsClient)
			{
				EntityUid? floorWeeds = null;
				EntityPrototype spawnProto = default(EntityPrototype);
				if (_prototype.TryIndex(node.Spawn, ref spawnProto) && spawnProto.HasComponent<XenoWeedsComponent>((IComponentFactory?)null))
				{
					floorWeeds = _xenoWeeds.GetWeedsOnFloor(transform.Coordinates);
				}
				EntityUid spawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(node.Spawn), transform.Coordinates);
				Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(target2));
				SharedXenoHiveSystem hive2 = _hive;
				Entity<HiveMemberComponent> member = Entity<HiveMemberComponent>.op_Implicit(spawn);
				Entity<HiveComponent>? val = hive;
				hive2.SetHive(member, val.HasValue ? new EntityUid?(Entity<HiveComponent>.op_Implicit(val.GetValueOrDefault())) : ((EntityUid?)null));
				((EntitySystem)this).QueueDel((EntityUid?)target2);
				((EntitySystem)this).QueueDel(floorWeeds);
				ISharedAdminLogManager adminLogs2 = _adminLogs;
				LogStringHandler handler2 = new LogStringHandler(55, 4);
				handler2.AppendLiteral("Xeno ");
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<XenoCanAddPlasmaToConstructComponent>.op_Implicit(xeno), (MetaDataComponent)null), "xeno", "ToPrettyString(xeno)");
				handler2.AppendLiteral(" completed construction of ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target2)), "xeno", "ToPrettyString(target)");
				handler2.AppendLiteral(" which turned into ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(spawn)), "spawn", "ToPrettyString(spawn)");
				handler2.AppendLiteral(" at ");
				handler2.AppendFormatted<EntityCoordinates>(transform.Coordinates, "transform.Coordinates");
				adminLogs2.Add(LogType.RMCXenoOrderConstructionComplete, ref handler2);
			}
		}
	}

	private void OnActionConstructionChosen(Entity<XenoChooseConstructionActionComponent> xeno, ref XenoConstructionChosenEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		XenoConstructionComponent construction = default(XenoConstructionComponent);
		if (!((EntitySystem)this).TryComp<XenoConstructionComponent>(args.User, ref construction) || construction.OrderConstructionTargeting)
		{
			return;
		}
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(xeno.Owner));
		if (action.HasValue)
		{
			Entity<ActionComponent> action2 = action.GetValueOrDefault();
			if (_prototype.HasIndex(args.Choice))
			{
				EntProtoId displayChoice = (_queenBoostQuery.HasComp(args.User) ? GetQueenVariant(args.Choice) : args.Choice);
				_actions.SetIcon(action2.AsNullable(), (SpriteSpecifier?)new EntityPrototype(EntProtoId.op_Implicit(displayChoice)));
			}
		}
	}

	private void OnSecreteActionValidateTarget(Entity<XenoConstructionActionComponent> ent, ref ActionValidateEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructionComponent construction = default(XenoConstructionComponent);
		if (args.Invalid || !((EntitySystem)this).TryComp<XenoConstructionComponent>(args.User, ref construction))
		{
			return;
		}
		EntityCoordinates? coordinates = ((EntitySystem)this).GetCoordinates(args.Input.EntityCoordinatesTarget);
		if (!coordinates.HasValue)
		{
			return;
		}
		EntityCoordinates target = coordinates.GetValueOrDefault();
		EntityCoordinates snapped = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		XenoSecreteStructureAdjustFields adjustEv = new XenoSecreteStructureAdjustFields(snapped);
		((EntitySystem)this).RaiseLocalEvent<XenoSecreteStructureAdjustFields>(args.User, ref adjustEv, false);
		bool hasBoost = _queenBoostQuery.HasComp(args.User);
		QueenBuildingBoostComponent boost = default(QueenBuildingBoostComponent);
		if (ent.Comp.CanUpgrade && (construction.CanUpgrade || hasBoost) && _rmcMap.HasAnchoredEntityEnumerator<XenoStructureUpgradeableComponent>(snapped, out Entity<XenoStructureUpgradeableComponent> upgradeable, (Direction?)null, (DirectionFlag)0) && upgradeable.Comp.To.HasValue && (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(args.User)) || (hasBoost && _queenBoostQuery.TryComp(args.User, ref boost) && _transform.InRange(_transform.GetMoverCoordinates(args.User), target, boost.RemoteUpgradeRange)) || _transform.InRange(_transform.GetMoverCoordinates(args.User), upgradeable.Owner.ToCoordinates(), construction.BuildRange.Float())))
		{
			return;
		}
		if (construction.OrderConstructionTargeting && construction.OrderConstructionChoice.HasValue)
		{
			if (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(args.User)) && !_queenEye.CanSeeTarget(Entity<QueenEyeActionComponent>.op_Implicit(args.User), target))
			{
				args.Invalid = true;
			}
			else if (!CanOrderConstructionPopup(Entity<XenoConstructionComponent>.op_Implicit((args.User, construction)), target, construction.OrderConstructionChoice))
			{
				args.Invalid = true;
			}
		}
		else if (!CanSecreteOnTilePopup(Entity<XenoConstructionComponent>.op_Implicit((args.User, construction)), construction.BuildChoice, target, ent.Comp.CheckStructureSelected, ent.Comp.CheckWeeds))
		{
			args.Invalid = true;
		}
	}

	private void OnHiveConstructionNodeExamined(Entity<HiveConstructionNodeComponent> node, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 plasmaLeft = node.Comp.PlasmaCost - node.Comp.PlasmaStored;
		args.PushMarkup(base.Loc.GetString("cm-xeno-construction-plasma-left", (ValueTuple<string, object>)("construction", node.Owner), (ValueTuple<string, object>)("plasma", plasmaLeft)));
	}

	private void OnHiveConstructionNodeActivated(Entity<HiveConstructionNodeComponent> node, ref ActivateInWorldEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		FixedPoint2 plasmaLeft = node.Comp.PlasmaCost - node.Comp.PlasmaStored;
		XenoCanAddPlasmaToConstructComponent xeno = default(XenoCanAddPlasmaToConstructComponent);
		TransformComponent nodeTransform = default(TransformComponent);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoCanAddPlasmaToConstructComponent>(user, ref xeno) && !(plasmaLeft < FixedPoint2.Zero) && ((EntitySystem)this).TryComp(Entity<HiveConstructionNodeComponent>.op_Implicit(node), ref nodeTransform) && ((EntitySystem)this).TryComp<XenoPlasmaComponent>(user, ref plasma) && InRangePopup(user, nodeTransform.Coordinates, xeno.Range.Float()))
		{
			FixedPoint2 subtract = FixedPoint2.Min(plasma.Plasma, plasmaLeft);
			if (!(plasma.Plasma < 1) && _xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit((user, plasma)), subtract))
			{
				XenoConstructionAddPlasmaDoAfterEvent ev = new XenoConstructionAddPlasmaDoAfterEvent();
				TimeSpan delay = xeno.AddPlasmaDelay;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, user, Entity<HiveConstructionNodeComponent>.op_Implicit(node))
				{
					BreakOnMove = true
				};
				_doAfter.TryStartDoAfter(doAfter);
			}
		}
	}

	private void OnHiveConstructionRepair(Entity<RepairableXenoStructureComponent> xenoStructure, ref ActivateInWorldEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		FixedPoint2 plasmaLeft = xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma;
		XenoConstructionComponent xeno = default(XenoConstructionComponent);
		TransformComponent xenoStructureTransform = default(TransformComponent);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		DamageableComponent xenoStructureDamage = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<XenoConstructionComponent>(user, ref xeno) && !(plasmaLeft < FixedPoint2.Zero) && ((EntitySystem)this).TryComp(Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), ref xenoStructureTransform) && ((EntitySystem)this).TryComp<XenoPlasmaComponent>(user, ref plasma) && ((EntitySystem)this).TryComp<DamageableComponent>(Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), ref xenoStructureDamage))
		{
			if (xenoStructureDamage.TotalDamage <= 0)
			{
				string undamagedStructureMessage = base.Loc.GetString("rmc-xeno-construction-repair-structure-no-damage-failure", (ValueTuple<string, object>)("struct", xenoStructure.Owner));
				_popup.PopupClient(undamagedStructureMessage, xenoStructure.Owner.ToCoordinates(), user);
			}
			else if (InRangePopup(user, xenoStructureTransform.Coordinates, xeno.OrderConstructionRange.Float()) && !(plasma.Plasma < 1))
			{
				XenoRepairStructureDoAfterEvent ev = new XenoRepairStructureDoAfterEvent();
				TimeSpan delay = xenoStructure.Comp.RepairLength;
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure))
				{
					BreakOnMove = true,
					RootEntity = true
				};
				_doAfter.TryStartDoAfter(doAfter);
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-repair-structure-start-attempt", (ValueTuple<string, object>)("struct", xenoStructure.Owner)), xenoStructureTransform.Coordinates, user);
			}
		}
	}

	private void OnHiveConstructionRepairDoAfter(Entity<RepairableXenoStructureComponent> xenoStructure, ref XenoRepairStructureDoAfterEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid user = args.User;
		FixedPoint2 plasmaLeft = xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma;
		XenoConstructionComponent xeno = default(XenoConstructionComponent);
		TransformComponent xenoStructureTransform = default(TransformComponent);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		DamageableComponent xenoStructureDamage = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<XenoConstructionComponent>(user, ref xeno) || plasmaLeft < FixedPoint2.Zero || !((EntitySystem)this).TryComp(Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), ref xenoStructureTransform) || !((EntitySystem)this).TryComp<XenoPlasmaComponent>(user, ref plasma) || !((EntitySystem)this).TryComp<DamageableComponent>(Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), ref xenoStructureDamage) || xenoStructureDamage.TotalDamage <= 0)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!InRangePopup(user, xenoStructureTransform.Coordinates, xeno.OrderConstructionRange.Float()))
		{
			return;
		}
		FixedPoint2 subtract = FixedPoint2.Min(plasma.Plasma, plasmaLeft);
		if (!(plasma.Plasma < 1) && _xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit((user, plasma)), subtract))
		{
			xenoStructure.Comp.StoredPlasma += subtract;
			if (xenoStructure.Comp.StoredPlasma >= xenoStructure.Comp.PlasmaCost)
			{
				xenoStructure.Comp.StoredPlasma = 0;
				_damageable.SetAllDamage(xenoStructure.Owner, xenoStructureDamage, 0);
				XenoStructureRepairedEvent ev = new XenoStructureRepairedEvent();
				((EntitySystem)this).RaiseLocalEvent<XenoStructureRepairedEvent>(Entity<RepairableXenoStructureComponent>.op_Implicit(xenoStructure), ev, false);
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-repair-structure-success", (ValueTuple<string, object>)("struct", xenoStructure.Owner)), xenoStructureTransform.Coordinates, user);
			}
			else
			{
				string notEnoughPlasmaMessage = base.Loc.GetString("rmc-xeno-construction-repair-structure-insufficient-plasma-warn", (ValueTuple<string, object>)("struct", xenoStructure.Owner), (ValueTuple<string, object>)("remainingPlasma", xenoStructure.Comp.PlasmaCost - xenoStructure.Comp.StoredPlasma));
				_popup.PopupClient(notEnoughPlasmaMessage, xenoStructure.Owner.ToCoordinates(), user);
			}
		}
	}

	private void OnWeedStructureRepair(Entity<XenoWeedsComponent> weedsStructure, ref XenoStructureRepairedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Entity<XenoWeedsComponent> val = weedsStructure;
		EntityUid val2 = default(EntityUid);
		XenoWeedsComponent xenoWeedsComponent = default(XenoWeedsComponent);
		val.Deconstruct(ref val2, ref xenoWeedsComponent);
		EntityUid ent = val2;
		XenoWeedsComponent xenoWeedsComponent2 = xenoWeedsComponent;
		XenoWeedsSpreadingComponent spreaderComp = ((EntitySystem)this).EnsureComp<XenoWeedsSpreadingComponent>(ent);
		TimeSpan spreadTime = (spreaderComp.SpreadAt = _timing.CurTime + spreaderComp.RepairedSpreadDelay);
		((EntitySystem)this).Dirty(ent, (IComponent)(object)spreaderComp, (MetaDataComponent)null);
		foreach (EntityUid weed in xenoWeedsComponent2.Spread)
		{
			spreaderComp = ((EntitySystem)this).EnsureComp<XenoWeedsSpreadingComponent>(weed);
			spreaderComp.SpreadAt = spreadTime;
			((EntitySystem)this).Dirty(weed, (IComponent)(object)spreaderComp, (MetaDataComponent)null);
		}
	}

	private void OnCheckAdjacentCollapse<T>(Entity<XenoConstructionSupportComponent> ent, ref T args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (!_transformQuery.TryComp(Entity<XenoConstructionSupportComponent>.op_Implicit(ent), ref xform))
		{
			return;
		}
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit((Entity<XenoConstructionSupportComponent>.op_Implicit(ent), xform)));
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid gridId = grid.GetValueOrDefault();
		MapGridComponent grid2 = default(MapGridComponent);
		if (!((EntityUid)(ref gridId)).Valid || !((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
		{
			return;
		}
		MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<TransformComponent>.op_Implicit((Entity<XenoConstructionSupportComponent>.op_Implicit(ent), xform)));
		Vector2i indices = _mapSystem.TileIndicesFor(gridId, grid2, coordinates);
		EntityUid? uid = default(EntityUid?);
		for (int i = 0; i < 4; i++)
		{
			AtmosDirection dir = (AtmosDirection)(1 << i);
			Vector2i pos = indices.Offset(dir);
			AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, pos);
			while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
			{
				if (!((EntitySystem)this).TerminatingOrDeleted(uid.Value, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(uid.Value) && _constructionRequiresSupportQuery.HasComp(uid) && !IsSupported(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), pos))
				{
					((EntitySystem)this).QueueDel(uid);
				}
			}
		}
	}

	private void OnDeleteXenoResinHit(Entity<DeleteXenoResinOnHitComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer && _xenoConstructQuery.HasComp(args.Target))
		{
			((EntitySystem)this).QueueDel((EntityUid?)args.Target);
		}
	}

	private void OnXenoConstructMapInit(Entity<XenoConstructComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (_area.TryGetArea(Entity<XenoConstructComponent>.op_Implicit(ent), out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			area.Value.Comp.ResinConstructCount++;
			((EntitySystem)this).Dirty<AreaComponent>(area.Value, (MetaDataComponent)null);
		}
	}

	private void OnXenoConstructRemoved(Entity<XenoConstructComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (_area.TryGetArea(Entity<XenoConstructComponent>.op_Implicit(ent), out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			area.Value.Comp.ResinConstructCount--;
			((EntitySystem)this).Dirty<AreaComponent>(area.Value, (MetaDataComponent)null);
		}
	}

	private void OnRecentlyPreventCollide(Entity<XenoRecentlyConstructedComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.StopCollide.Contains(args.OtherEntity))
		{
			args.Cancelled = true;
		}
	}

	public FixedPoint2? GetStructurePlasmaCost(EntProtoId prototype)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype buildChoice = default(EntityPrototype);
		XenoConstructionPlasmaCostComponent cost = default(XenoConstructionPlasmaCostComponent);
		if (_prototype.TryIndex(prototype, ref buildChoice) && buildChoice.TryGetComponent<XenoConstructionPlasmaCostComponent>(ref cost, _compFactory))
		{
			return cost.Plasma;
		}
		return null;
	}

	public FixedPoint2 GetStructureMinRange(EntProtoId prototype)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		XenoConstructionMinRangeComponent minRangeComp = null;
		EntityPrototype buildChoice = default(EntityPrototype);
		if (_prototype.TryIndex(prototype, ref buildChoice))
		{
			buildChoice.TryGetComponent<XenoConstructionMinRangeComponent>(ref minRangeComp, _compFactory);
		}
		if (minRangeComp != null)
		{
			return minRangeComp.MinRange.Float();
		}
		return 0;
	}

	private float? GetBuildSpeed(EntProtoId prototype)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype buildChoice = default(EntityPrototype);
		XenoConstructionBuildSpeedComponent speed = default(XenoConstructionBuildSpeedComponent);
		if (_prototype.TryIndex(prototype, ref buildChoice) && buildChoice.TryGetComponent<XenoConstructionBuildSpeedComponent>(ref speed, _compFactory))
		{
			return speed.BuildTimeMult;
		}
		return null;
	}

	private FixedPoint2? GetStructurePlasmaCost(EntProtoId? building)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (building.HasValue)
		{
			EntProtoId choice = building.GetValueOrDefault();
			FixedPoint2? structurePlasmaCost = GetStructurePlasmaCost(choice);
			if (structurePlasmaCost.HasValue)
			{
				return structurePlasmaCost.GetValueOrDefault();
			}
		}
		return null;
	}

	public FixedPoint2 GetStructureMinRange(EntProtoId? building)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (building.HasValue)
		{
			EntProtoId choice = building.GetValueOrDefault();
			return GetStructureMinRange(choice);
		}
		return 0;
	}

	private bool TileSolidAndNotBlocked(EntityCoordinates target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		TileRef? tileRef = _turf.GetTileRef(target);
		if (tileRef.HasValue)
		{
			TileRef tile = tileRef.GetValueOrDefault();
			if (!_turf.IsSpace(tile) && _turf.GetContentTileDefinition(tile).Sturdy && !_turf.IsTileBlocked(tile, CollisionGroup.Impassable))
			{
				return !_xenoNest.HasAdjacentNestFacing(target);
			}
		}
		return false;
	}

	private bool InRangePopup(EntityUid xeno, EntityCoordinates target, float range, float minRange = 0f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates origin = _transform.GetMoverCoordinates(xeno);
		target = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
		if (!_transform.InRange(origin, target, range))
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-cant-reach-there"), target, xeno);
			return false;
		}
		if (minRange != 0f && _transform.InRange(origin, target, minRange))
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-cant-build-in-self"), target, xeno);
			return false;
		}
		return true;
	}

	private bool CanSecreteOnTilePopup(Entity<XenoConstructionComponent> xeno, EntProtoId? buildChoice, EntityCoordinates target, bool checkStructureSelected, bool checkWeeds)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		if (checkStructureSelected && !buildChoice.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-select-structure"), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
			return false;
		}
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				target = target.SnapToGrid((IEntityManager?)(object)base.EntityManager, _map);
				bool hasBoost = _queenBoostQuery.HasComp(xeno.Owner);
				if (checkWeeds && !_xenoWeeds.IsOnWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target))
				{
					_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-need-weeds"), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
					return false;
				}
				XenoConstructionRangeEvent ev = new XenoConstructionRangeEvent(xeno.Comp.BuildRange);
				((EntitySystem)this).RaiseLocalEvent<XenoConstructionRangeEvent>(Entity<XenoConstructionComponent>.op_Implicit(xeno), ref ev, false);
				if (ev.Range > 0 && !_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !InRangePopup(Entity<XenoConstructionComponent>.op_Implicit(xeno), target, ev.Range.Float(), GetStructureMinRange(buildChoice).Float()))
				{
					return false;
				}
				if (!TileSolidAndNotBlocked(target))
				{
					_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
					return false;
				}
				Vector2i tile = _mapSystem.CoordinatesToTile(gridId, grid2, target);
				AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, tile);
				EntityUid? uid = default(EntityUid?);
				DoorComponent door = default(DoorComponent);
				while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
				{
					if (_xenoConstructQuery.HasComp(uid) || _xenoEggQuery.HasComp(uid) || _xenoTunnelQuery.HasComp(uid) || _sentryQuery.HasComp(uid) || _blockXenoConstructionQuery.HasComp(uid))
					{
						_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
						return false;
					}
					if (!((EntitySystem)this).HasComp<BarricadeComponent>(uid) && (((_tags.HasAnyTag(uid.Value, StructureTag) || ((EntitySystem)this).HasComp<StrapComponent>(uid) || ((EntitySystem)this).HasComp<ClimbableComponent>(uid)) && !_tags.HasTag(uid.Value, PlatformTag) && !((EntitySystem)this).HasComp<DoorComponent>(uid)) || (((EntitySystem)this).TryComp<DoorComponent>(uid, ref door) && door.State != DoorState.Open)))
					{
						_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-blocked-structure"), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.SmallCaution);
						return false;
					}
				}
				if (checkStructureSelected)
				{
					FixedPoint2? structurePlasmaCost = GetStructurePlasmaCost(buildChoice);
					if (structurePlasmaCost.HasValue)
					{
						FixedPoint2 cost = structurePlasmaCost.GetValueOrDefault();
						if (!hasBoost && !_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), cost))
						{
							return false;
						}
					}
				}
				if (checkStructureSelected && buildChoice.HasValue)
				{
					EntProtoId choice = buildChoice.GetValueOrDefault();
					EntityPrototype choiceProto = default(EntityPrototype);
					if (_prototype.TryIndex(choice, ref choiceProto) && choiceProto.HasComponent<XenoConstructionRequiresSupportComponent>(_compFactory) && !IsSupported(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target))
					{
						_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-requires-support", (ValueTuple<string, object>)("choice", choiceProto.Name)), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
						return false;
					}
				}
				if (!_area.CanResinPopup(Entity<MapGridComponent, AreaGridComponent>.op_Implicit((ValueTuple<EntityUid, MapGridComponent, AreaGridComponent>)(gridId, grid2, null)), tile, Entity<XenoConstructionComponent>.op_Implicit(xeno)))
				{
					return false;
				}
				return true;
			}
		}
		_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-cant-build"), target, Entity<XenoConstructionComponent>.op_Implicit(xeno));
		return false;
	}

	private bool CanOrderConstructionPopup(Entity<XenoConstructionComponent> xeno, EntityCoordinates target, EntProtoId? choice)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		if (_queenEye.IsInQueenEye(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner)) && !_queenEye.CanSeeTarget(Entity<QueenEyeActionComponent>.op_Implicit(xeno.Owner), target))
		{
			return false;
		}
		if (!CanSecreteOnTilePopup(xeno, choice, target, checkStructureSelected: false, checkWeeds: false))
		{
			return false;
		}
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i tile = _mapSystem.TileIndicesFor(gridId, grid2, target);
				ImmutableArray<Direction>.Enumerator enumerator = Directions.GetEnumerator();
				EntityUid? ent = default(EntityUid?);
				HiveConstructionNodeComponent node = default(HiveConstructionNodeComponent);
				while (enumerator.MoveNext())
				{
					Direction direction = enumerator.Current;
					Vector2i pos = SharedMapSystem.GetDirection(tile, direction, 1);
					AnchoredEntitiesEnumerator directionEnumerator = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, pos);
					while (((AnchoredEntitiesEnumerator)(ref directionEnumerator)).MoveNext(ref ent))
					{
						if (_hiveConstructionNodeQuery.TryGetComponent(ent, ref node) && node.BlockOtherNodes)
						{
							_popup.PopupClient(base.Loc.GetString("cm-xeno-too-close-to-other-node", (ValueTuple<string, object>)("target", ent.Value)), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno));
							return false;
						}
					}
				}
				EntityPrototype choiceProto = default(EntityPrototype);
				if (choice.HasValue && _prototype.TryIndex(choice, ref choiceProto))
				{
					TileRef tileRef = default(TileRef);
					ITileDefinition tileDef = default(ITileDefinition);
					if (choiceProto.HasComponent<HiveConstructionRequiresWeedableSurfaceComponent>(_compFactory) && (!_mapSystem.TryGetTileRef(gridId, grid2, tile, ref tileRef) || !_tile.TryGetDefinition(tileRef.Tile.TypeId, ref tileDef) || ((IPrototype)tileDef).ID == "Space" || tileDef is ContentTileDefinition { WeedsSpreadable: false }))
					{
						_popup.PopupClient(base.Loc.GetString("cm-xeno-construction-failed-cant-build"), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno));
						return false;
					}
					if (choiceProto.HasComponent<HiveConstructionRequiresHiveCoreComponent>(_compFactory))
					{
						Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner));
						if (!hive.HasValue)
						{
							if (_net.IsServer)
							{
								_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-requires-hive-core", (ValueTuple<string, object>)("choice", choiceProto.Name)), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
							}
							return false;
						}
						Entity<HiveComponent> hiveEnt = hive.GetValueOrDefault();
						if (!_hive.HasHiveCore(hiveEnt))
						{
							if (_net.IsServer)
							{
								_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-requires-hive-core", (ValueTuple<string, object>)("choice", choiceProto.Name)), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
							}
							return false;
						}
					}
					if (choiceProto.HasComponent<HiveConstructionRequiresHiveWeedsComponent>(_compFactory) && !_xenoWeeds.IsOnHiveWeeds(Entity<MapGridComponent>.op_Implicit((gridId, grid2)), target))
					{
						if (_net.IsServer)
						{
							_popup.PopupEntity(base.Loc.GetString("rmc-xeno-construction-requires-hive-weeds", (ValueTuple<string, object>)("choice", choiceProto.Name)), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
						}
						return false;
					}
					if (choiceProto.HasComponent<HiveConstructionRequiresSpaceComponent>(_compFactory) && !CanPlaceSpaceRequiringStructurePopup(_transform.ToMapCoordinates(target, true), Entity<MapGridComponent>.op_Implicit((gridId, grid2)), xeno.Owner, choiceProto.Name))
					{
						return false;
					}
					HiveConstructionLimitedComponent limited = default(HiveConstructionLimitedComponent);
					if (choiceProto.TryGetComponent<HiveConstructionLimitedComponent>(ref limited, _compFactory) && !CanPlaceLimitedHiveStructure(xeno.Owner, limited, out var limit, out var _))
					{
						if (_net.IsServer)
						{
							string msg = ((limit == 1) ? base.Loc.GetString("rmc-xeno-construction-unique-exists", (ValueTuple<string, object>)("choice", choiceProto.Name)) : base.Loc.GetString("rmc-xeno-construction-hive-limit-met", (ValueTuple<string, object>)("choice", choiceProto.Name)));
							_popup.PopupEntity(msg, Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
						}
						return false;
					}
					if (choiceProto.ID == "HiveCoreXenoConstructionNode")
					{
						Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner));
						if (hive.HasValue && hive.GetValueOrDefault().Comp.NewCoreAt > _timing.CurTime)
						{
							if (_net.IsServer)
							{
								_popup.PopupEntity(base.Loc.GetString("rmc-xeno-cant-build-new-yet", (ValueTuple<string, object>)("choice", choiceProto.Name)), Entity<XenoConstructionComponent>.op_Implicit(xeno), Entity<XenoConstructionComponent>.op_Implicit(xeno), PopupType.MediumCaution);
							}
							return false;
						}
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool CanPlaceLimitedHiveStructure(EntityUid hiveMember, HiveConstructionLimitedComponent comp, [NotNullWhen(true)] out int? limit, [NotNullWhen(true)] out int? curCount)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		limit = null;
		curCount = null;
		EntProtoId id = comp.Id;
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(hiveMember));
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			if (_hive.TryGetStructureLimit(hive2, id, out var trueLimit))
			{
				limit = trueLimit;
				curCount = 0;
				EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent> limitedConstructs = ((EntitySystem)this).EntityQueryEnumerator<HiveConstructionLimitedComponent, HiveMemberComponent>();
				HiveConstructionLimitedComponent otherUnique = default(HiveConstructionLimitedComponent);
				HiveMemberComponent hiveMemberComponent = default(HiveMemberComponent);
				while (limitedConstructs.MoveNext(ref otherUnique, ref hiveMemberComponent))
				{
					if (otherUnique.Id == id)
					{
						curCount++;
					}
				}
				return limit > curCount;
			}
		}
		return false;
	}

	private bool IsSupported(Entity<MapGridComponent> grid, EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i indices = _mapSystem.TileIndicesFor(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		return IsSupported(grid, indices);
	}

	private bool IsSupported(Entity<MapGridComponent> grid, Vector2i tile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		bool supported = false;
		EntityUid? uid = default(EntityUid?);
		for (int i = 0; i < 4; i++)
		{
			AtmosDirection dir = (AtmosDirection)(1 << i);
			Vector2i pos = tile.Offset(dir);
			AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), pos);
			while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
			{
				if (!((EntitySystem)this).TerminatingOrDeleted(uid.Value, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(uid.Value) && _constructionSupportQuery.HasComp(uid))
				{
					supported = true;
					break;
				}
			}
			if (supported)
			{
				break;
			}
		}
		return supported;
	}

	private bool CanPlaceSpaceRequiringStructurePopup(MapCoordinates mapCoords, Entity<MapGridComponent> map, EntityUid user, string structName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		MapId mapId = mapCoords.MapId;
		Box2 aabbRange = default(Box2);
		((Box2)(ref aabbRange))._002Ector(((MapCoordinates)(ref mapCoords)).X - 1.5f, ((MapCoordinates)(ref mapCoords)).Y + 1.5f, ((MapCoordinates)(ref mapCoords)).X + 1.5f, ((MapCoordinates)(ref mapCoords)).Y - 1.5f);
		bool num = _entityLookup.AnyComponentsIntersecting(typeof(HiveConstructionLimitedComponent), mapId, aabbRange, (EntityUid?)null, (LookupFlags)110);
		TileRef centerTile = _mapSystem.GetTileRef(map, mapCoords);
		EntityCoordinates userCoords = _transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(user), mapCoords);
		if (num)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-requires-space", (ValueTuple<string, object>)("choice", structName)), userCoords, user);
			return false;
		}
		Vector2i adjacentTile = default(Vector2i);
		for (int adjacentX = ((TileRef)(ref centerTile)).X - 1; adjacentX <= ((TileRef)(ref centerTile)).X + 1; adjacentX++)
		{
			for (int adjacentY = ((TileRef)(ref centerTile)).Y - 1; adjacentY <= ((TileRef)(ref centerTile)).Y + 1; adjacentY++)
			{
				if (adjacentX != adjacentY || adjacentX != 0)
				{
					((Vector2i)(ref adjacentTile))._002Ector(adjacentX, adjacentY);
					if (_turf.IsTileBlocked(Entity<MapGridComponent>.op_Implicit(map), adjacentTile, CollisionGroup.MobMask, map.Comp))
					{
						_popup.PopupClient(base.Loc.GetString("rmc-xeno-construction-requires-space", (ValueTuple<string, object>)("choice", structName)), userCoords, user);
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool CanPlaceXenoStructure(EntityUid user, EntityCoordinates coords, [NotNullWhen(false)] out string? popupType, bool needsWeeds = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		popupType = null;
		EntityUid? grid = _transform.GetGrid(coords);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i tile = _mapSystem.TileIndicesFor(gridId, grid2, coords);
				AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, tile);
				bool hasWeeds = false;
				EntityUid? uid = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
				{
					if (((EntitySystem)this).HasComp<XenoEggComponent>(uid))
					{
						popupType = "rmc-xeno-construction-blocked";
						return false;
					}
					if (((EntitySystem)this).HasComp<XenoConstructComponent>(uid) || _tags.HasAnyTag(uid.Value, StructureTag, AirlockTag) || ((EntitySystem)this).HasComp<StrapComponent>(uid) || _xenoTunnelQuery.HasComp(uid) || _sentryQuery.HasComp(uid) || _blockXenoConstructionQuery.HasComp(uid))
					{
						popupType = "rmc-xeno-construction-blocked";
						return false;
					}
					if (((EntitySystem)this).HasComp<XenoWeedsComponent>(uid))
					{
						hasWeeds = true;
					}
				}
				if (_turf.IsTileBlocked(gridId, tile, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, grid2))
				{
					popupType = "rmc-xeno-construction-blocked";
					return false;
				}
				if (!hasWeeds && needsWeeds)
				{
					popupType = "rmc-xeno-construction-must-have-weeds";
					return false;
				}
				return true;
			}
		}
		popupType = "rmc-xeno-construction-no-map";
		return false;
	}

	public void GiveQueenBoost(EntityUid queen, float speedMultiplier, float remoteRange)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		QueenBuildingBoostComponent boost = ((EntitySystem)this).EnsureComp<QueenBuildingBoostComponent>(queen);
		boost.BuildSpeedMultiplier = speedMultiplier;
		boost.RemoteUpgradeRange = remoteRange;
		((EntitySystem)this).Dirty(queen, (IComponent)(object)boost, (MetaDataComponent)null);
	}

	public void RemoveQueenBoost(EntityUid queen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<QueenBuildingBoostComponent>(queen);
	}

	private FixedPoint2 GetDensityCost(Entity<AreaComponent> area, Entity<XenoConstructionComponent> xeno, FixedPoint2 cost)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float density = (float)area.Comp.ResinConstructCount / (float)area.Comp.BuildableTiles;
		if (density >= _densityThreshold)
		{
			cost = Math.Ceiling(cost.Float() * (density + xeno.Comp.DensityConstructionCostModifier) * xeno.Comp.DensityConstructionCostMultiplier);
		}
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		if (((EntitySystem)this).TryComp<XenoPlasmaComponent>(Entity<XenoConstructionComponent>.op_Implicit(xeno), ref plasma) && cost > plasma.MaxPlasma)
		{
			cost = plasma.MaxPlasma;
		}
		return cost;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoRecentlyConstructedComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoRecentlyConstructedComponent>();
		EntityUid uid = default(EntityUid);
		XenoRecentlyConstructedComponent comp = default(XenoRecentlyConstructedComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (time >= comp.ExpireAt)
			{
				((EntitySystem)this).RemCompDeferred<XenoRecentlyConstructedComponent>(uid);
				continue;
			}
			_intersectingResin.Clear();
			_entityLookup.GetEntitiesIntersecting(uid, _intersectingResin, (LookupFlags)110);
			for (int i = comp.StopCollide.Count - 1; i >= 0; i--)
			{
				EntityUid colliding = comp.StopCollide[i];
				if (!_intersectingResin.Contains(colliding))
				{
					comp.StopCollide.RemoveAt(i);
				}
			}
			if (comp.StopCollide.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<XenoRecentlyConstructedComponent>(uid);
			}
		}
	}
}
