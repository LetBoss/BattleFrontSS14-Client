using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions.Events;
using Content.Shared.Administration.Managers;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Electrocution;
using Content.Shared.Holopad;
using Content.Shared.IdentityManagement;
using Content.Shared.Intellicard;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Mind;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.StationAi;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Silicons.StationAi;

public abstract class SharedStationAiSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminManager _admin;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private ItemToggleSystem _toggles;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private MetaDataSystem _metadata;

	[Dependency]
	private SharedAirlockSystem _airlocks;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _containers;

	[Dependency]
	private SharedDoorSystem _doors;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedElectrocutionSystem _electrify;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	protected SharedMapSystem Maps;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedPowerReceiverSystem PowerReceiver;

	[Dependency]
	private SharedTransformSystem _xforms;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private StationAiVisionSystem _vision;

	[Dependency]
	private IPrototypeManager _protoManager;

	private EntityQuery<BroadphaseComponent> _broadphaseQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private static readonly EntProtoId DefaultAi = EntProtoId.op_Implicit("StationAiBrain");

	private const float MaxVisionMultiplier = 5f;

	private ProtoId<StationAiCustomizationGroupPrototype> _stationAiCoreCustomGroupProtoId = ProtoId<StationAiCustomizationGroupPrototype>.op_Implicit("StationAiCoreIconography");

	private ProtoId<StationAiCustomizationGroupPrototype> _stationAiHologramCustomGroupProtoId = ProtoId<StationAiCustomizationGroupPrototype>.op_Implicit("StationAiHolograms");

	private const string JobNameLocId = "job-name-station-ai";

	private void InitializeAirlock()
	{
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, StationAiBoltEvent>((ComponentEventHandler<DoorBoltComponent, StationAiBoltEvent>)OnAirlockBolt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, StationAiEmergencyAccessEvent>((ComponentEventHandler<AirlockComponent, StationAiEmergencyAccessEvent>)OnAirlockEmergencyAccess, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ElectrifiedComponent, StationAiElectrifiedEvent>((ComponentEventHandler<ElectrifiedComponent, StationAiElectrifiedEvent>)OnElectrified, (Type[])null, (Type[])null);
	}

	private void OnAirlockBolt(EntityUid ent, DoorBoltComponent component, StationAiBoltEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (component.BoltWireCut)
		{
			ShowDeviceNotRespondingPopup(args.User);
		}
		else if (!_doors.TrySetBoltDown(Entity<DoorBoltComponent>.op_Implicit((ent, component)), args.Bolted, args.User, predicted: true))
		{
			ShowDeviceNotRespondingPopup(args.User);
		}
	}

	private void OnAirlockEmergencyAccess(EntityUid ent, AirlockComponent component, StationAiEmergencyAccessEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!PowerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent)))
		{
			ShowDeviceNotRespondingPopup(args.User);
		}
		else
		{
			_airlocks.SetEmergencyAccess(Entity<AirlockComponent>.op_Implicit((ent, component)), args.EmergencyAccess, args.User, predicted: true);
		}
	}

	private void OnElectrified(EntityUid ent, ElectrifiedComponent component, StationAiElectrifiedEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsWireCut || !PowerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent)))
		{
			ShowDeviceNotRespondingPopup(args.User);
			return;
		}
		_electrify.SetElectrified(Entity<ElectrifiedComponent>.op_Implicit((ent, component)), args.Electrified);
		SoundPathSpecifier soundToPlay = (component.Enabled ? component.AirlockElectrifyDisabled : component.AirlockElectrifyEnabled);
		_audio.PlayLocal((SoundSpecifier)(object)soundToPlay, ent, (EntityUid?)args.User, (AudioParams?)null);
	}

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_broadphaseQuery = ((EntitySystem)this).GetEntityQuery<BroadphaseComponent>();
		_gridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		InitializeAirlock();
		InitializeHeld();
		InitializeLight();
		InitializeCustomization();
		((EntitySystem)this).SubscribeLocalEvent<StationAiWhitelistComponent, BoundUserInterfaceCheckRangeEvent>((EntityEventRefHandler<StationAiWhitelistComponent, BoundUserInterfaceCheckRangeEvent>)OnAiBuiCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, AccessibleOverrideEvent>((EntityEventRefHandler<StationAiOverlayComponent, AccessibleOverrideEvent>)OnAiAccessible, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, InRangeOverrideEvent>((EntityEventRefHandler<StationAiOverlayComponent, InRangeOverrideEvent>)OnAiInRange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, MenuVisibilityEvent>((EntityEventRefHandler<StationAiOverlayComponent, MenuVisibilityEvent>)OnAiMenu, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, ComponentInit>((EntityEventRefHandler<StationAiHolderComponent, ComponentInit>)OnHolderInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, ComponentRemove>((EntityEventRefHandler<StationAiHolderComponent, ComponentRemove>)OnHolderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, AfterInteractEvent>((EntityEventRefHandler<StationAiHolderComponent, AfterInteractEvent>)OnHolderInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, MapInitEvent>((EntityEventRefHandler<StationAiHolderComponent, MapInitEvent>)OnHolderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<StationAiHolderComponent, EntInsertedIntoContainerMessage>)OnHolderConInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<StationAiHolderComponent, EntRemovedFromContainerMessage>)OnHolderConRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHolderComponent, IntellicardDoAfterEvent>((EntityEventRefHandler<StationAiHolderComponent, IntellicardDoAfterEvent>)OnIntellicardDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<StationAiCoreComponent, EntInsertedIntoContainerMessage>)OnAiInsert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<StationAiCoreComponent, EntRemovedFromContainerMessage>)OnAiRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, MapInitEvent>((EntityEventRefHandler<StationAiCoreComponent, MapInitEvent>)OnAiMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, ComponentShutdown>((EntityEventRefHandler<StationAiCoreComponent, ComponentShutdown>)OnAiShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, PowerChangedEvent>((EntityEventRefHandler<StationAiCoreComponent, PowerChangedEvent>)OnCorePower, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<StationAiCoreComponent, GetVerbsEvent<Verb>>)OnCoreVerbs, (Type[])null, (Type[])null);
	}

	private void OnCoreVerbs(Entity<StationAiCoreComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		EntityUid user = args.User;
		if (_admin.IsAdmin(args.User) && !TryGetHeld(Entity<StationAiCoreComponent>.op_Implicit((ent.Owner, ent.Comp)), out var _))
		{
			args.Verbs.Add(new Verb
			{
				Text = base.Loc.GetString("station-ai-takeover"),
				Category = VerbCategory.Debug,
				Act = delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_003a: Unknown result type (might be due to invalid IL or missing references)
					EntityUid target = ((EntitySystem)this).SpawnInContainerOrDrop(EntProtoId.op_Implicit(DefaultAi), ent.Owner, "station_ai_mind_slot", (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
					_mind.ControlMob(user, target);
				},
				Impact = LogImpact.High
			});
		}
		if (TryGetHeld(Entity<StationAiCoreComponent>.op_Implicit((Entity<StationAiCoreComponent>.op_Implicit(ent), ent.Comp)), out var insertedAi) && insertedAi == user)
		{
			args.Verbs.Add(new Verb
			{
				Text = base.Loc.GetString("station-ai-customization-menu"),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_uiSystem.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)StationAiCustomizationUiKey.Key, insertedAi, false);
				},
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/emotes.svg.192dpi.png"))
			});
		}
	}

	private void OnAiAccessible(Entity<StationAiOverlayComponent> ent, ref AccessibleOverrideEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		BaseContainer targetContainer = default(BaseContainer);
		if (!_containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.Target), ref targetContainer) && _containers.IsInSameOrTransparentContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.User), Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.Target), (BaseContainer)null, targetContainer, false))
		{
			args.Accessible = true;
		}
	}

	private void OnAiMenu(Entity<StationAiOverlayComponent> ent, ref MenuVisibilityEvent args)
	{
		args.Visibility &= ~MenuVisibility.NoFov;
	}

	private void OnAiBuiCheck(Entity<StationAiWhitelistComponent> ent, ref BoundUserInterfaceCheckRangeEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<StationAiHeldComponent>(Entity<TransformComponent>.op_Implicit(args.Actor)))
		{
			return;
		}
		args.Result = (BoundUserInterfaceRangeResult)2;
		TransformComponent targetXform = ((EntitySystem)this).Transform(args.Target);
		EntityUid? gridUid = targetXform.GridUid;
		EntityUid? gridUid2 = args.Actor.Comp.GridUid;
		BroadphaseComponent broadphase = default(BroadphaseComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (gridUid.HasValue != gridUid2.HasValue || (gridUid.HasValue && gridUid.GetValueOrDefault() != gridUid2.GetValueOrDefault()) || !_broadphaseQuery.TryComp(targetXform.GridUid, ref broadphase) || !_gridQuery.TryComp(targetXform.GridUid, ref grid))
		{
			return;
		}
		Vector2i targetTile = Maps.LocalToTile(targetXform.GridUid.Value, grid, targetXform.Coordinates);
		lock (_vision)
		{
			if (_vision.IsAccessible(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((targetXform.GridUid.Value, broadphase, grid)), targetTile, 8.5f, fastPath: true))
			{
				args.Result = (BoundUserInterfaceRangeResult)1;
			}
		}
	}

	private void OnAiInRange(Entity<StationAiOverlayComponent> ent, ref InRangeOverrideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		TransformComponent targetXform = ((EntitySystem)this).Transform(args.Target);
		EntityUid? gridUid = targetXform.GridUid;
		EntityUid? gridUid2 = ((EntitySystem)this).Transform(args.User).GridUid;
		BroadphaseComponent broadphase = default(BroadphaseComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (gridUid.HasValue == gridUid2.HasValue && (!gridUid.HasValue || !(gridUid.GetValueOrDefault() != gridUid2.GetValueOrDefault())) && _broadphaseQuery.TryComp(targetXform.GridUid, ref broadphase) && _gridQuery.TryComp(targetXform.GridUid, ref grid))
		{
			Vector2i targetTile = Maps.LocalToTile(targetXform.GridUid.Value, grid, targetXform.Coordinates);
			args.InRange = _vision.IsAccessible(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((targetXform.GridUid.Value, broadphase, grid)), targetTile);
		}
	}

	private void OnIntellicardDoAfter(Entity<StationAiHolderComponent> ent, ref IntellicardDoAfterEvent args)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		StationAiHolderComponent targetHolder = default(StationAiHolderComponent);
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<StationAiHolderComponent>(args.Args.Target, ref targetHolder))
		{
			return;
		}
		if (_slots.CanEject(ent.Owner, args.User, ent.Comp.Slot))
		{
			if (_slots.TryInsert(args.Args.Target.Value, targetHolder.Slot, ent.Comp.Slot.Item.Value, args.User, excludeUserAudio: true))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
		else if (_slots.CanEject(args.Args.Target.Value, args.User, targetHolder.Slot) && _slots.TryInsert(ent.Owner, ent.Comp.Slot, targetHolder.Slot.Item.Value, args.User, excludeUserAudio: true))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnHolderInteract(Entity<StationAiHolderComponent> ent, ref AfterInteractEvent args)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		StationAiHolderComponent targetHolder = default(StationAiHolderComponent);
		IntellicardComponent intelliComp = default(IntellicardComponent);
		if (((HandledEntityEventArgs)args).Handled || !args.CanReach || !args.Target.HasValue || !((EntitySystem)this).TryComp<StationAiHolderComponent>(args.Target, ref targetHolder) || ((EntitySystem)this).HasComp<IntellicardComponent>(args.Target) || !((EntitySystem)this).TryComp<IntellicardComponent>(args.Used, ref intelliComp))
		{
			return;
		}
		bool cardHasAi = _slots.CanEject(ent.Owner, args.User, ent.Comp.Slot);
		bool coreHasAi = _slots.CanEject(args.Target.Value, args.User, targetHolder.Slot);
		if (cardHasAi && coreHasAi)
		{
			_popup.PopupClient(base.Loc.GetString("intellicard-core-occupied"), args.User, args.User, PopupType.Medium);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		if (!cardHasAi && !coreHasAi)
		{
			_popup.PopupClient(base.Loc.GetString("intellicard-core-empty"), args.User, args.User, PopupType.Medium);
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		if (TryGetHeld(Entity<StationAiHolderComponent>.op_Implicit((args.Target.Value, targetHolder)), out var held) && _timing.CurTime > intelliComp.NextWarningAllowed)
		{
			intelliComp.NextWarningAllowed = _timing.CurTime + intelliComp.WarningDelay;
			AnnounceIntellicardUsage(held, intelliComp.WarningSound);
		}
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, cardHasAi ? intelliComp.UploadTime : intelliComp.DownloadTime, new IntellicardDoAfterEvent(), args.Target, ent.Owner)
		{
			BreakOnDamage = true,
			BreakOnMove = true,
			NeedHand = true,
			BreakOnDropItem = true
		};
		_doAfter.TryStartDoAfter(doAfterArgs);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnHolderInit(Entity<StationAiHolderComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_slots.AddItemSlot(ent.Owner, "station_ai_mind_slot", ent.Comp.Slot);
	}

	private void OnHolderRemove(Entity<StationAiHolderComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_slots.RemoveItemSlot(ent.Owner, ent.Comp.Slot);
	}

	private void OnHolderConInsert(Entity<StationAiHolderComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<StationAiHolderComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void OnHolderConRemove(Entity<StationAiHolderComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<StationAiHolderComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void OnHolderMapInit(Entity<StationAiHolderComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<StationAiHolderComponent>.op_Implicit(ent.Owner));
	}

	private void OnAiShutdown(Entity<StationAiCoreComponent> ent, ref ComponentShutdown args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).QueueDel(ent.Comp.RemoteEntity);
			ent.Comp.RemoteEntity = null;
		}
	}

	private void OnCorePower(Entity<StationAiCoreComponent> ent, ref PowerChangedEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Powered)
		{
			if (SetupEye(ent))
			{
				AttachEye(ent);
			}
		}
		else
		{
			ClearEye(ent);
		}
	}

	private void OnAiMapInit(Entity<StationAiCoreComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SetupEye(ent);
		AttachEye(ent);
	}

	public void SwitchRemoteEntityMode(Entity<StationAiCoreComponent?> entity, bool isRemote)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		StationAiCoreComponent comp = entity.Comp;
		if (comp == null)
		{
			return;
		}
		_ = comp.Remote;
		if (0 == 0 && entity.Comp.Remote != isRemote)
		{
			Entity<StationAiCoreComponent> ent = default(Entity<StationAiCoreComponent>);
			ent._002Ector(entity.Owner, entity.Comp);
			ent.Comp.Remote = isRemote;
			EntityCoordinates? coords = (ent.Comp.RemoteEntity.HasValue ? new EntityCoordinates?(((EntitySystem)this).Transform(ent.Comp.RemoteEntity.Value).Coordinates) : ((EntityCoordinates?)null));
			EntityUid? oldEye = ent.Comp.RemoteEntity;
			ClearEye(ent);
			if (SetupEye(ent, coords))
			{
				AttachEye(ent);
			}
			if (oldEye.HasValue)
			{
				StationAiRemoteEntityReplacementEvent ev = new StationAiRemoteEntityReplacementEvent(ent.Comp.RemoteEntity);
				((EntitySystem)this).RaiseLocalEvent<StationAiRemoteEntityReplacementEvent>(oldEye.Value, ref ev, false);
			}
			EntityUid? user = GetInsertedAI(ent);
			EyeComponent eye = default(EyeComponent);
			if (((EntitySystem)this).TryComp<EyeComponent>(user, ref eye))
			{
				_eye.SetDrawFov(user.Value, !isRemote, (EyeComponent)null);
			}
		}
	}

	private bool SetupEye(Entity<StationAiCoreComponent> ent, EntityCoordinates? coords = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		if (ent.Comp.RemoteEntity.HasValue)
		{
			return false;
		}
		EntProtoId? proto = ent.Comp.RemoteEntityProto;
		if (!coords.HasValue)
		{
			coords = ((EntitySystem)this).Transform(ent.Owner).Coordinates;
		}
		if (!ent.Comp.Remote)
		{
			proto = ent.Comp.PhysicalEntityProto;
		}
		if (proto.HasValue)
		{
			StationAiCoreComponent comp = ent.Comp;
			EntProtoId? val = proto;
			comp.RemoteEntity = ((EntitySystem)this).SpawnAtPosition(val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, coords.Value, (ComponentRegistry)null);
			((EntitySystem)this).Dirty<StationAiCoreComponent>(ent, (MetaDataComponent)null);
		}
		return true;
	}

	private void ClearEye(Entity<StationAiCoreComponent> ent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).QueueDel(ent.Comp.RemoteEntity);
			ent.Comp.RemoteEntity = null;
			((EntitySystem)this).Dirty<StationAiCoreComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void AttachEye(Entity<StationAiCoreComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (ent.Comp.RemoteEntity.HasValue && _containers.TryGetContainer(ent.Owner, "station_ai_mind_slot", ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count == 1)
		{
			EntityUid user = container.ContainedEntities[0];
			EyeComponent eyeComp = default(EyeComponent);
			if (((EntitySystem)this).TryComp<EyeComponent>(user, ref eyeComp))
			{
				_eye.SetDrawFov(user, false, eyeComp);
				_eye.SetTarget(user, (EntityUid?)ent.Comp.RemoteEntity.Value, eyeComp);
			}
			_mover.SetRelay(user, ent.Comp.RemoteEntity.Value);
		}
	}

	private EntityUid? GetInsertedAI(Entity<StationAiCoreComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_containers.TryGetContainer(ent.Owner, "station_ai_mind_slot", ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count != 1)
		{
			return null;
		}
		return container.ContainedEntities[0];
	}

	private void OnAiInsert(Entity<StationAiCoreComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != "station_ai_mind_slot") && !_timing.ApplyingState)
		{
			ent.Comp.Remote = true;
			SetupEye(ent);
			_metadata.SetEntityName(ent.Owner, ((EntitySystem)this).MetaData(((ContainerModifiedMessage)args).Entity).EntityName, (MetaDataComponent)null, true);
			AttachEye(ent);
		}
	}

	private void OnAiRemove(Entity<StationAiCoreComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			ent.Comp.Remote = true;
			MetaDataSystem metadata = _metadata;
			EntityUid owner = ent.Owner;
			EntityPrototype obj = ((EntitySystem)this).Prototype(ent.Owner, (MetaDataComponent)null);
			metadata.SetEntityName(owner, ((obj != null) ? obj.Name : null) ?? string.Empty, (MetaDataComponent)null, true);
			((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(((ContainerModifiedMessage)args).Entity);
			EyeComponent eyeComp = default(EyeComponent);
			if (((EntitySystem)this).TryComp<EyeComponent>(((ContainerModifiedMessage)args).Entity, ref eyeComp))
			{
				_eye.SetDrawFov(((ContainerModifiedMessage)args).Entity, true, eyeComp);
				_eye.SetTarget(((ContainerModifiedMessage)args).Entity, (EntityUid?)null, eyeComp);
			}
			ClearEye(ent);
		}
	}

	private void UpdateAppearance(Entity<StationAiHolderComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StationAiHolderComponent>(entity.Owner, ref entity.Comp, false))
		{
			StationAiState state = StationAiState.Empty;
			BaseContainer container = default(BaseContainer);
			if (_containers.TryGetContainer(entity.Owner, "station_ai_mind_slot", ref container, (ContainerManagerComponent)null) && container.Count > 0)
			{
				state = StationAiState.Occupied;
			}
			StationAiCoreComponent stationAiCore = default(StationAiCoreComponent);
			if (((EntitySystem)this).TryComp<StationAiCoreComponent>(Entity<StationAiHolderComponent>.op_Implicit(entity), ref stationAiCore))
			{
				CustomizeAppearance(Entity<StationAiCoreComponent>.op_Implicit((Entity<StationAiHolderComponent>.op_Implicit(entity), stationAiCore)), state);
			}
			else
			{
				_appearance.SetData(entity.Owner, (Enum)StationAiVisualState.Key, (object)state, (AppearanceComponent)null);
			}
		}
	}

	public virtual void AnnounceIntellicardUsage(EntityUid uid, SoundSpecifier? cue = null)
	{
	}

	public virtual bool SetVisionEnabled(Entity<StationAiVisionComponent> entity, bool enabled, bool announce = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Enabled == enabled)
		{
			return false;
		}
		entity.Comp.Enabled = enabled;
		((EntitySystem)this).Dirty<StationAiVisionComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	public virtual bool SetWhitelistEnabled(Entity<StationAiWhitelistComponent> entity, bool value, bool announce = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Enabled == value)
		{
			return false;
		}
		entity.Comp.Enabled = value;
		((EntitySystem)this).Dirty<StationAiWhitelistComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	private bool ValidateAi(Entity<StationAiHeldComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationAiHeldComponent>(entity.Owner, ref entity.Comp, false))
		{
			return false;
		}
		return _blocker.CanComplexInteract(entity.Owner);
	}

	private void InitializeCustomization()
	{
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, StationAiCustomizationMessage>((EntityEventRefHandler<StationAiCoreComponent, StationAiCustomizationMessage>)OnStationAiCustomization, (Type[])null, (Type[])null);
	}

	private void OnStationAiCustomization(Entity<StationAiCoreComponent> entity, ref StationAiCustomizationMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		StationAiCustomizationGroupPrototype groupPrototype = default(StationAiCustomizationGroupPrototype);
		StationAiCustomizationPrototype customizationProto = default(StationAiCustomizationPrototype);
		StationAiCustomizationComponent stationAiCustomization = default(StationAiCustomizationComponent);
		if (_protoManager.TryIndex<StationAiCustomizationGroupPrototype>(args.GroupProtoId, ref groupPrototype) && _protoManager.TryIndex<StationAiCustomizationPrototype>(args.CustomizationProtoId, ref customizationProto) && TryGetHeld(Entity<StationAiCoreComponent>.op_Implicit((Entity<StationAiCoreComponent>.op_Implicit(entity), entity.Comp)), out var held) && ((EntitySystem)this).TryComp<StationAiCustomizationComponent>(held, ref stationAiCustomization) && (!stationAiCustomization.ProtoIds.TryGetValue(args.GroupProtoId, out ProtoId<StationAiCustomizationPrototype> protoId) || !(protoId == args.CustomizationProtoId)))
		{
			stationAiCustomization.ProtoIds[args.GroupProtoId] = args.CustomizationProtoId;
			((EntitySystem)this).Dirty(held, (IComponent)(object)stationAiCustomization, (MetaDataComponent)null);
			if (groupPrototype.Category == StationAiCustomizationType.Hologram)
			{
				UpdateHolographicAvatar(Entity<StationAiCustomizationComponent>.op_Implicit((held, stationAiCustomization)));
			}
			StationAiHolderComponent stationAiHolder = default(StationAiHolderComponent);
			if (groupPrototype.Category == StationAiCustomizationType.CoreIconography && ((EntitySystem)this).TryComp<StationAiHolderComponent>(Entity<StationAiCoreComponent>.op_Implicit(entity), ref stationAiHolder))
			{
				UpdateAppearance(Entity<StationAiHolderComponent>.op_Implicit((Entity<StationAiCoreComponent>.op_Implicit(entity), stationAiHolder)));
			}
		}
	}

	private void UpdateHolographicAvatar(Entity<StationAiCustomizationComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		HolographicAvatarComponent avatar = default(HolographicAvatarComponent);
		StationAiCustomizationPrototype prototype = default(StationAiCustomizationPrototype);
		if (((EntitySystem)this).TryComp<HolographicAvatarComponent>(Entity<StationAiCustomizationComponent>.op_Implicit(entity), ref avatar) && entity.Comp.ProtoIds.TryGetValue(_stationAiHologramCustomGroupProtoId, out ProtoId<StationAiCustomizationPrototype> protoId) && _protoManager.TryIndex<StationAiCustomizationPrototype>(protoId, ref prototype) && prototype.LayerData.TryGetValue(StationAiState.Hologram.ToString(), out PrototypeLayerData layerData))
		{
			avatar.LayerData = (PrototypeLayerData[]?)(object)new PrototypeLayerData[1] { layerData };
			((EntitySystem)this).Dirty(Entity<StationAiCustomizationComponent>.op_Implicit(entity), (IComponent)(object)avatar, (MetaDataComponent)null);
		}
	}

	private void CustomizeAppearance(Entity<StationAiCoreComponent> entity, StationAiState state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? stationAi = GetInsertedAI(entity);
		StationAiCustomizationComponent stationAiCustomization = default(StationAiCustomizationComponent);
		ProtoId<StationAiCustomizationPrototype> protoId;
		StationAiCustomizationPrototype prototype = default(StationAiCustomizationPrototype);
		PrototypeLayerData layerData;
		if (!stationAi.HasValue)
		{
			_appearance.RemoveData(entity.Owner, (Enum)StationAiVisualState.Key, (AppearanceComponent)null);
		}
		else if (((EntitySystem)this).TryComp<StationAiCustomizationComponent>(stationAi, ref stationAiCustomization) && stationAiCustomization.ProtoIds.TryGetValue(_stationAiCoreCustomGroupProtoId, out protoId) && _protoManager.TryIndex<StationAiCustomizationPrototype>(protoId, ref prototype) && prototype.LayerData.TryGetValue(state.ToString(), out layerData))
		{
			_appearance.SetData(entity.Owner, (Enum)StationAiVisualState.Key, (object)layerData, (AppearanceComponent)null);
		}
	}

	private void InitializeHeld()
	{
		((EntitySystem)this).SubscribeLocalEvent<StationAiRadialMessage>((EntityEventHandler<StationAiRadialMessage>)OnRadialMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiWhitelistComponent, BoundUserInterfaceMessageAttempt>((EntityEventRefHandler<StationAiWhitelistComponent, BoundUserInterfaceMessageAttempt>)OnMessageAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiWhitelistComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<StationAiWhitelistComponent, GetVerbsEvent<AlternativeVerb>>)OnTargetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHeldComponent, InteractionAttemptEvent>((EntityEventRefHandler<StationAiHeldComponent, InteractionAttemptEvent>)OnHeldInteraction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHeldComponent, AttemptRelayActionComponentChangeEvent>((EntityEventRefHandler<StationAiHeldComponent, AttemptRelayActionComponentChangeEvent>)OnHeldRelay, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiHeldComponent, JumpToCoreEvent>((EntityEventRefHandler<StationAiHeldComponent, JumpToCoreEvent>)OnCoreJump, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TryGetIdentityShortInfoEvent>((EntityEventHandler<TryGetIdentityShortInfoEvent>)OnTryGetIdentityShortInfo, (Type[])null, (Type[])null);
	}

	private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).HasComp<StationAiHeldComponent>(args.ForActor))
		{
			args.Title = ((EntitySystem)this).Name(args.ForActor, (MetaDataComponent)null) + " (" + base.Loc.GetString("job-name-station-ai") + ")";
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnCoreJump(Entity<StationAiHeldComponent> ent, ref JumpToCoreEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCore(ent.Owner, out Entity<StationAiCoreComponent> core))
		{
			StationAiCoreComponent comp = core.Comp;
			if (comp != null && comp.RemoteEntity.HasValue)
			{
				_xforms.DropNextTo(Entity<TransformComponent>.op_Implicit(core.Comp.RemoteEntity.Value), Entity<TransformComponent>.op_Implicit(core.Owner));
			}
		}
	}

	public bool TryGetHeld(Entity<StationAiCoreComponent?> entity, out EntityUid held)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		held = EntityUid.Invalid;
		if (!((EntitySystem)this).Resolve<StationAiCoreComponent>(entity.Owner, ref entity.Comp, true))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_containers.TryGetContainer(entity.Owner, "station_ai_mind_slot", ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			return false;
		}
		held = container.ContainedEntities[0];
		return true;
	}

	public bool TryGetHeld(Entity<StationAiHolderComponent?> entity, out EntityUid held)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		StationAiCoreComponent stationAiCore = default(StationAiCoreComponent);
		((EntitySystem)this).TryComp<StationAiCoreComponent>(entity.Owner, ref stationAiCore);
		return TryGetHeld(Entity<StationAiCoreComponent>.op_Implicit((entity.Owner, stationAiCore)), out held);
	}

	public bool TryGetCore(EntityUid entity, out Entity<StationAiCoreComponent?> core)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(entity);
		MetaDataComponent meta = ((EntitySystem)this).MetaData(entity);
		Entity<TransformComponent, MetaDataComponent> ent = default(Entity<TransformComponent, MetaDataComponent>);
		ent._002Ector(entity, xform, meta);
		BaseContainer container = default(BaseContainer);
		StationAiCoreComponent coreComp = default(StationAiCoreComponent);
		if (!_containers.TryGetContainingContainer(ent, ref container) || container.ID != "station_ai_mind_slot" || !((EntitySystem)this).TryComp<StationAiCoreComponent>(container.Owner, ref coreComp) || !coreComp.RemoteEntity.HasValue)
		{
			core = Entity<StationAiCoreComponent>.op_Implicit((ValueTuple<EntityUid, StationAiCoreComponent>)(EntityUid.Invalid, null));
			return false;
		}
		core = Entity<StationAiCoreComponent>.op_Implicit((container.Owner, coreComp));
		return true;
	}

	private void OnHeldRelay(Entity<StationAiHeldComponent> ent, ref AttemptRelayActionComponentChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCore(ent.Owner, out Entity<StationAiCoreComponent> core))
		{
			args.Target = core.Comp?.RemoteEntity;
		}
	}

	private void OnRadialMessage(StationAiRadialMessage ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(((BoundUserInterfaceMessage)ev).Entity, ref target))
		{
			ev.Event.User = ((BaseBoundUserInterfaceEvent)ev).Actor;
			((EntitySystem)this).RaiseLocalEvent(target.Value, (object)ev.Event, false);
		}
	}

	private void OnMessageAttempt(Entity<StationAiWhitelistComponent> ent, ref BoundUserInterfaceMessageAttempt ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		StationAiHeldComponent aiComp = default(StationAiHeldComponent);
		StationAiWhitelistComponent whitelistComponent = default(StationAiWhitelistComponent);
		if (ev.Actor == ev.Target || !((EntitySystem)this).TryComp<StationAiHeldComponent>(ev.Actor, ref aiComp) || (((EntitySystem)this).TryComp<StationAiWhitelistComponent>(ev.Target, ref whitelistComponent) && ValidateAi(Entity<StationAiHeldComponent>.op_Implicit((ev.Actor, aiComp)))))
		{
			return;
		}
		if (!PowerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ev.Target)))
		{
			ShowDeviceNotRespondingPopup(ev.Actor);
			((CancellableEntityEventArgs)ev).Cancel();
			return;
		}
		if (whitelistComponent != null && !whitelistComponent.Enabled)
		{
			ShowDeviceNotRespondingPopup(ev.Actor);
		}
		((CancellableEntityEventArgs)ev).Cancel();
	}

	private void OnHeldInteraction(Entity<StationAiHeldComponent> ent, ref InteractionAttemptEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		StationAiWhitelistComponent whitelistComponent = default(StationAiWhitelistComponent);
		int cancelled;
		if (!((EntitySystem)this).TryComp<StationAiWhitelistComponent>(args.Target, ref whitelistComponent) || !whitelistComponent.Enabled)
		{
			EntityUid owner = ent.Owner;
			EntityUid? target = args.Target;
			if (!target.HasValue || owner != target.GetValueOrDefault())
			{
				cancelled = (args.Target.HasValue ? 1 : 0);
				goto IL_0050;
			}
		}
		cancelled = 0;
		goto IL_0050;
		IL_0050:
		args.Cancelled = (byte)cancelled != 0;
		if (whitelistComponent != null && !whitelistComponent.Enabled)
		{
			ShowDeviceNotRespondingPopup(ent.Owner);
		}
	}

	private void OnTargetVerbs(Entity<StationAiWhitelistComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!_uiSystem.HasUi(args.Target, (Enum)AiUi.Key, (UserInterfaceComponent)null) || !args.CanComplexInteract || !((EntitySystem)this).HasComp<StationAiHeldComponent>(args.User) || !args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid target = args.Target;
		bool isOpen = _uiSystem.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(target), (Enum)AiUi.Key, user);
		AlternativeVerb verb = new AlternativeVerb
		{
			Text = (isOpen ? base.Loc.GetString("ai-close") : base.Loc.GetString("ai-open")),
			Act = delegate
			{
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				if (isOpen)
				{
					_uiSystem.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)AiUi.Key, (EntityUid?)user, false);
				}
				else
				{
					_uiSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)AiUi.Key, (EntityUid?)user, false);
				}
			}
		};
		args.Verbs.Add(verb);
	}

	private void ShowDeviceNotRespondingPopup(EntityUid toEntity)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("ai-device-not-responding"), toEntity, PopupType.MediumCaution);
	}

	private void InitializeLight()
	{
		((EntitySystem)this).SubscribeLocalEvent<ItemTogglePointLightComponent, StationAiLightEvent>((ComponentEventHandler<ItemTogglePointLightComponent, StationAiLightEvent>)OnLight, (Type[])null, (Type[])null);
	}

	private void OnLight(EntityUid ent, ItemTogglePointLightComponent component, StationAiLightEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (args.Enabled)
		{
			_toggles.TryActivate(Entity<ItemToggleComponent>.op_Implicit(ent), args.User);
		}
		else
		{
			_toggles.TryDeactivate(Entity<ItemToggleComponent>.op_Implicit(ent), args.User);
		}
	}
}
