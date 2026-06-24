using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.HyperSleep;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.Coordinates;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Evacuation;

public abstract class SharedEvacuationSystem : EntitySystem
{
	[Dependency]
	private SharedAmbientSoundSystem _ambientSound;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoorSystem _door;

	[Dependency]
	private SharedHyperSleepChamberSystem _hyperSleep;

	[Dependency]
	private MapLoaderSystem _mapLoader;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCExplosionSystem _rmcExplosion;

	[Dependency]
	private SharedRMCPowerSystem _rmcPower;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	private EntityQuery<AreaComponent> _areaQuery;

	private EntityQuery<DoorComponent> _doorQuery;

	private EntityQuery<MobStateComponent> _mobStateQuery;

	private MapId? _map;

	private int _index;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_areaQuery = ((EntitySystem)this).GetEntityQuery<AreaComponent>();
		_doorQuery = ((EntitySystem)this).GetEntityQuery<DoorComponent>();
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackLandedEvent>((EntityEventRefHandler<DropshipHijackLandedEvent>)OnDropshipHijackLanded, (Type[])null, new Type[1] { typeof(SharedRMCPowerSystem) });
		((EntitySystem)this).SubscribeLocalEvent<EvacuationEnabledEvent>((EntityEventRefHandler<EvacuationEnabledEvent>)OnEvacuationEnabled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationDisabledEvent>((EntityEventRefHandler<EvacuationDisabledEvent>)OnEvacuationDisabled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationProgressEvent>((EntityEventRefHandler<EvacuationProgressEvent>)OnEvacuationProgress, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GridSpawnerComponent, MapInitEvent>((EntityEventRefHandler<GridSpawnerComponent, MapInitEvent>)OnGridSpawnerMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationDoorComponent, BeforeDoorOpenedEvent>((EntityEventRefHandler<EvacuationDoorComponent, BeforeDoorOpenedEvent>)OnEvacuationDoorBeforeOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationDoorComponent, BeforeDoorClosedEvent>((EntityEventRefHandler<EvacuationDoorComponent, BeforeDoorClosedEvent>)OnEvacuationDoorBeforeClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationDoorComponent, BeforePryEvent>((EntityEventRefHandler<EvacuationDoorComponent, BeforePryEvent>)OnEvacuationDoorBeforePry, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationComputerComponent, ExaminedEvent>((EntityEventRefHandler<EvacuationComputerComponent, ExaminedEvent>)OnEvacuationComputerExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationComputerComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<EvacuationComputerComponent, ActivatableUIOpenAttemptEvent>)OnEvacuationComputerUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LifeboatComputerComponent, ActivatableUIOpenAttemptEvent>((EntityEventRefHandler<LifeboatComputerComponent, ActivatableUIOpenAttemptEvent>)OnLifeboatComputerUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationPumpComponent, ExaminedEvent>((EntityEventRefHandler<EvacuationPumpComponent, ExaminedEvent>)OnEvacuationPumpExamined, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<EvacuationComputerComponent>(((EntitySystem)this).Subs, (object)EvacuationComputerUi.Key, (BuiEventSubscriber<EvacuationComputerComponent>)delegate(Subscriber<EvacuationComputerComponent> subs)
		{
			subs.Event<EvacuationComputerLaunchBuiMsg>((EntityEventRefHandler<EvacuationComputerComponent, EvacuationComputerLaunchBuiMsg>)OnEvacuationComputerLaunch);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<LifeboatComputerComponent>(((EntitySystem)this).Subs, (object)LifeboatComputerUi.Key, (BuiEventSubscriber<LifeboatComputerComponent>)delegate(Subscriber<LifeboatComputerComponent> subs)
		{
			subs.Event<LifeboatComputerLaunchBuiMsg>((EntityEventRefHandler<LifeboatComputerComponent, LifeboatComputerLaunchBuiMsg>)OnLifeboatComputerLaunch);
		});
	}

	private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<EvacuationProgressComponent>(ev.Map).DropShipCrashed = true;
		EntityQueryEnumerator<EvacuationDoorComponent> doors = ((EntitySystem)this).EntityQueryEnumerator<EvacuationDoorComponent>();
		EntityUid uid = default(EntityUid);
		EvacuationDoorComponent door = default(EvacuationDoorComponent);
		while (doors.MoveNext(ref uid, ref door))
		{
			door.Locked = false;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)door, (MetaDataComponent)null);
		}
		_config.SetCVar<bool>(CCVars.GameDisallowLateJoins, true, false);
	}

	private void OnEvacuationEnabled(ref EvacuationEnabledEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<LifeboatComputerComponent> lifeboats = ((EntitySystem)this).EntityQueryEnumerator<LifeboatComputerComponent>();
		EntityUid uid = default(EntityUid);
		LifeboatComputerComponent computer = default(LifeboatComputerComponent);
		while (lifeboats.MoveNext(ref uid, ref computer))
		{
			computer.Enabled = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)computer, (MetaDataComponent)null);
		}
		EntityQueryEnumerator<EvacuationComputerComponent> evacuation = ((EntitySystem)this).EntityQueryEnumerator<EvacuationComputerComponent>();
		EntityUid computerId = default(EntityUid);
		EvacuationComputerComponent computer2 = default(EvacuationComputerComponent);
		while (evacuation.MoveNext(ref computerId, ref computer2))
		{
			if (computer2.Mode == EvacuationComputerMode.Disabled)
			{
				computer2.Mode = EvacuationComputerMode.Ready;
				((EntitySystem)this).Dirty(computerId, (IComponent)(object)computer2, (MetaDataComponent)null);
			}
		}
	}

	private void OnEvacuationDisabled(ref EvacuationDisabledEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<LifeboatComputerComponent> lifeboats = ((EntitySystem)this).EntityQueryEnumerator<LifeboatComputerComponent>();
		EntityUid uid = default(EntityUid);
		LifeboatComputerComponent computer = default(LifeboatComputerComponent);
		while (lifeboats.MoveNext(ref uid, ref computer))
		{
			computer.Enabled = false;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)computer, (MetaDataComponent)null);
		}
	}

	private void OnEvacuationProgress(ref EvacuationProgressEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<EvacuationComputerComponent> evacuation = ((EntitySystem)this).EntityQueryEnumerator<EvacuationComputerComponent>();
		EntityUid computerId = default(EntityUid);
		EvacuationComputerComponent computer = default(EvacuationComputerComponent);
		while (evacuation.MoveNext(ref computerId, ref computer))
		{
			if (computer.Mode == EvacuationComputerMode.Disabled)
			{
				computer.Mode = EvacuationComputerMode.Ready;
				((EntitySystem)this).Dirty(computerId, (IComponent)(object)computer, (MetaDataComponent)null);
			}
		}
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_map = null;
		_index = 0;
	}

	private void OnGridSpawnerMapInit(Entity<GridSpawnerComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		ResPath? spawn = ent.Comp.Spawn;
		if (!spawn.HasValue)
		{
			return;
		}
		ResPath spawn2 = spawn.GetValueOrDefault();
		if (_net.IsClient || !_config.GetCVar<bool>(CCVars.GridFill))
		{
			return;
		}
		if (!_map.HasValue)
		{
			MapId mapId = default(MapId);
			_mapSystem.CreateMap(ref mapId, true);
			_map = mapId;
		}
		Vector2 offset = new Vector2(_index * 50, _index * 50);
		_index++;
		if (!_mapSystem.MapExists(_map))
		{
			return;
		}
		MapLoaderSystem mapLoader = _mapLoader;
		MapId value = _map.Value;
		Vector2 vector = offset;
		Entity<MapGridComponent>? result = default(Entity<MapGridComponent>?);
		if (mapLoader.TryLoadGrid(value, spawn2, ref result, (DeserializationOptions?)null, vector, default(Angle)))
		{
			Entity<MapGridComponent> grid = result.Value;
			TransformComponent xform = ((EntitySystem)this).Transform(Entity<GridSpawnerComponent>.op_Implicit(ent));
			MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<GridSpawnerComponent>.op_Implicit(ent), xform);
			coordinates = ((MapCoordinates)(ref coordinates)).Offset(ent.Comp.Offset);
			_transform.SetMapCoordinates(Entity<MapGridComponent>.op_Implicit(grid), coordinates);
			PhysicsComponent physics = default(PhysicsComponent);
			FixturesComponent fixtures = default(FixturesComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<MapGridComponent>.op_Implicit(grid), ref physics) && ((EntitySystem)this).TryComp<FixturesComponent>(Entity<MapGridComponent>.op_Implicit(grid), ref fixtures))
			{
				_physics.SetBodyType(Entity<MapGridComponent>.op_Implicit(grid), (BodyType)4, fixtures, physics, (TransformComponent)null);
				_physics.SetBodyStatus(Entity<MapGridComponent>.op_Implicit(grid), physics, (BodyStatus)0, true);
				_physics.SetFixedRotation(Entity<MapGridComponent>.op_Implicit(grid), true, true, fixtures, physics);
			}
		}
	}

	private void OnEvacuationDoorBeforeOpened(Entity<EvacuationDoorComponent> ent, ref BeforeDoorOpenedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && ent.Comp.Locked)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnEvacuationDoorBeforeClosed(Entity<EvacuationDoorComponent> ent, ref BeforeDoorClosedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Locked)
		{
			args.PerformCollisionCheck = false;
		}
	}

	private void OnEvacuationDoorBeforePry(Entity<EvacuationDoorComponent> ent, ref BeforePryEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Locked)
		{
			args.Cancelled = true;
		}
	}

	private void OnEvacuationComputerExamined(Entity<EvacuationComputerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		int? maxMobs = ent.Comp.MaxMobs;
		if (maxMobs.HasValue)
		{
			int maxMobs2 = maxMobs.GetValueOrDefault();
			using (args.PushGroup("EvacuationComputerComponent"))
			{
				args.PushMarkup($"[color=red]This pod is only rated for a maximum of {maxMobs2} occupants! Any more may cause it to crash and burn.[/color]");
			}
		}
	}

	private void OnEvacuationComputerUIOpenAttempt(Entity<EvacuationComputerComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && ent.Comp.Mode != EvacuationComputerMode.Ready)
		{
			((CancellableEntityEventArgs)args).Cancel();
			string msg = ent.Comp.Mode switch
			{
				EvacuationComputerMode.Disabled => "Evacuation has not started.", 
				EvacuationComputerMode.Ready => "", 
				EvacuationComputerMode.Travelling => "The escape pod has already been launched!", 
				EvacuationComputerMode.Crashed => "This escape pod has crashed!", 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			_popup.PopupClient(msg, Entity<EvacuationComputerComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
		}
	}

	private void OnEvacuationPumpExamined(Entity<EvacuationPumpComponent> ent, ref ExaminedEvent args)
	{
		if (!IsEvacuationInProgress())
		{
			return;
		}
		using (args.PushGroup("EvacuationPumpComponent"))
		{
			int progress = GetEvacuationProgress();
			if (progress < 25)
			{
				args.PushMarkup("It looks like it barely has any fuel yet.");
			}
			else if (progress < 50)
			{
				args.PushMarkup("It looks like it has accumulated some fuel.");
			}
			else if (progress < 75)
			{
				args.PushMarkup("It looks like the fuel tank is a little over half full.");
			}
			else if (progress < 100)
			{
				args.PushMarkup("It looks like the fuel tank is almost full.");
			}
			else
			{
				args.PushMarkup("It looks like the fuel tank is full.");
			}
		}
	}

	private void OnLifeboatComputerUIOpenAttempt(Entity<LifeboatComputerComponent> ent, ref ActivatableUIOpenAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !ent.Comp.Enabled)
		{
			((CancellableEntityEventArgs)args).Cancel();
			_popup.PopupClient("Evacuation has not been authorized.", Entity<LifeboatComputerComponent>.op_Implicit(ent), args.User, PopupType.SmallCaution);
		}
	}

	private unsafe void OnEvacuationComputerLaunch(Entity<EvacuationComputerComponent> ent, ref EvacuationComputerLaunchBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (ent.Comp.Mode != EvacuationComputerMode.Ready)
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} tried to activate evacuation computer {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<EvacuationComputerComponent>.op_Implicit(ent), (MetaDataComponent)null)} that is not ready. Mode: {ent.Comp.Mode}");
			return;
		}
		EntityUid? gridUid = ((EntitySystem)this).Transform(Entity<EvacuationComputerComponent>.op_Implicit(ent)).GridUid;
		if (gridUid.HasValue)
		{
			EntityUid gridId = gridUid.GetValueOrDefault();
			TransformComponent gridTransform = ((EntitySystem)this).Transform(gridId);
			int? maxMobs = ent.Comp.MaxMobs;
			if (maxMobs.HasValue)
			{
				int maxMobs2 = maxMobs.GetValueOrDefault();
				HashSet<EntityUid> mobs = new HashSet<EntityUid>();
				TransformChildrenEnumerator children = gridTransform.ChildEnumerator;
				EntityUid uid = default(EntityUid);
				ContainerManagerComponent containerManager = default(ContainerManagerComponent);
				DoorComponent door = default(DoorComponent);
				while (((TransformChildrenEnumerator)(ref children)).MoveNext(ref uid))
				{
					if (_mobStateQuery.HasComp(uid))
					{
						mobs.Add(uid);
					}
					else if (((EntitySystem)this).TryComp<ContainerManagerComponent>(uid, ref containerManager))
					{
						AllContainersEnumerable allContainers = _container.GetAllContainers(uid, containerManager);
						AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
						try
						{
							while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
							{
								foreach (EntityUid mob in ((IEnumerable<EntityUid>)((AllContainersEnumerator)(ref enumerator)).Current.ContainedEntities).Where((Func<EntityUid, bool>)_mobStateQuery.HasComp).ToList())
								{
									mobs.Add(mob);
								}
							}
						}
						finally
						{
							((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
						}
					}
					if (_doorQuery.TryComp(uid, ref door))
					{
						EvacuationDoorComponent evacuationDoor = ((EntitySystem)this).EnsureComp<EvacuationDoorComponent>(uid);
						evacuationDoor.Locked = true;
						((EntitySystem)this).Dirty(uid, (IComponent)(object)evacuationDoor, (MetaDataComponent)null);
						_door.TryClose(uid, door);
					}
				}
				if (mobs.Count > maxMobs2)
				{
					_popup.PopupPredicted("The evacuation pod is overloaded with this many people inside!", Entity<EvacuationComputerComponent>.op_Implicit(ent), null, PopupType.LargeCaution);
					ent.Comp.Mode = EvacuationComputerMode.Crashed;
					((EntitySystem)this).Dirty<EvacuationComputerComponent>(ent, (MetaDataComponent)null);
					TimeSpan time = _timing.CurTime;
					DetonatingEvacuationComputerComponent detonatingEvacuationComputerComponent = ((EntitySystem)this).EnsureComp<DetonatingEvacuationComputerComponent>(Entity<EvacuationComputerComponent>.op_Implicit(ent));
					detonatingEvacuationComputerComponent.DetonateAt = time + ent.Comp.DetonateDelay;
					detonatingEvacuationComputerComponent.EjectAt = time + ent.Comp.EjectDelay;
				}
			}
			_audio.PlayPredicted(ent.Comp.WarmupSound, Entity<EvacuationComputerComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
			if (ent.Comp.Mode != EvacuationComputerMode.Crashed)
			{
				ent.Comp.Mode = EvacuationComputerMode.Travelling;
				((EntitySystem)this).Dirty<EvacuationComputerComponent>(ent, (MetaDataComponent)null);
				float crashChance = (IsEvacuationComplete() ? 0f : ent.Comp.EarlyCrashChance);
				LaunchEvacuationFTL(gridId, crashChance, ent.Comp.LaunchSound);
			}
		}
		else
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} tried to activate evacuation computer {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<EvacuationComputerComponent>.op_Implicit(ent), (MetaDataComponent)null)} not on grid");
		}
	}

	private void OnLifeboatComputerLaunch(Entity<LifeboatComputerComponent> ent, ref LifeboatComputerLaunchBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((BaseBoundUserInterfaceEvent)args).Actor;
		if (!ent.Comp.Enabled)
		{
			((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} tried to activate lifeboat computer {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<LifeboatComputerComponent>.op_Implicit(ent), (MetaDataComponent)null)} that is not ready.");
			return;
		}
		EntityUid? gridUid = ((EntitySystem)this).Transform(Entity<LifeboatComputerComponent>.op_Implicit(ent)).GridUid;
		if (gridUid.HasValue)
		{
			EntityUid gridId = gridUid.GetValueOrDefault();
			ent.Comp.Enabled = false;
			((EntitySystem)this).Dirty<LifeboatComputerComponent>(ent, (MetaDataComponent)null);
			float crashChance = (IsEvacuationComplete() ? 0f : ent.Comp.EarlyCrashChance);
			LaunchEvacuationFTL(gridId, crashChance, null);
		}
	}

	protected virtual void LaunchEvacuationFTL(EntityUid grid, float crashLandChance, SoundSpecifier? launchSound)
	{
	}

	private void SetPumpAppearance(EvacuationPumpVisuals visual)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<EvacuationPumpComponent> pumps = ((EntitySystem)this).EntityQueryEnumerator<EvacuationPumpComponent>();
		EntityUid uid = default(EntityUid);
		EvacuationPumpComponent evacuationPumpComponent = default(EvacuationPumpComponent);
		while (pumps.MoveNext(ref uid, ref evacuationPumpComponent))
		{
			_appearance.SetData(uid, (Enum)EvacuationPumpLayers.Layer, (object)visual, (AppearanceComponent)null);
		}
	}

	private void SetPumpAmbience()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<EvacuationPumpComponent> pumps = ((EntitySystem)this).EntityQueryEnumerator<EvacuationPumpComponent>();
		EntityUid uid = default(EntityUid);
		EvacuationPumpComponent pump = default(EvacuationPumpComponent);
		while (pumps.MoveNext(ref uid, ref pump))
		{
			_ambientSound.SetSound(uid, pump.ActiveSound);
		}
	}

	private IEnumerable<EntityUid> GetEvacuationAreas(EntityCoordinates coordinates)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_area.TryGetAllAreas(coordinates, out Entity<AreaGridComponent>? areaGrid))
		{
			yield break;
		}
		AreaComponent area = default(AreaComponent);
		foreach (EntityUid areaId in areaGrid.Value.Comp.AreaEntities.Values)
		{
			if (_areaQuery.TryComp(areaId, ref area) && area.HijackEvacuationArea)
			{
				yield return areaId;
			}
		}
	}

	private bool IsAreaPumpPowered(EntityUid area)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _rmcPower.IsAreaPowered(Entity<RMCAreaPowerComponent>.op_Implicit(area), RMCPowerChannel.Equipment);
	}

	public void ToggleEvacuation(SoundSpecifier? startSound, SoundSpecifier? cancelSound, EntityUid? map)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EvacuationProgressComponent progress = ((EntitySystem)this).EnsureComp<EvacuationProgressComponent>(map.Value);
		progress.Enabled = !progress.Enabled;
		((EntitySystem)this).Dirty(map.Value, (IComponent)(object)progress, (MetaDataComponent)null);
		if (progress.Enabled)
		{
			_marineAnnounce.AnnounceARESStaging(null, "Attention. Emergency. All personnel must evacuate immediately.", startSound);
			EvacuationEnabledEvent ev = default(EvacuationEnabledEvent);
			((EntitySystem)this).RaiseLocalEvent<EvacuationEnabledEvent>(map.Value, ref ev, true);
		}
		else
		{
			_marineAnnounce.AnnounceARESStaging(null, "Evacuation has been cancelled.", cancelSound);
			EvacuationDisabledEvent ev2 = default(EvacuationDisabledEvent);
			((EntitySystem)this).RaiseLocalEvent<EvacuationDisabledEvent>(map.Value, ref ev2, true);
		}
	}

	public bool IsEvacuationInProgress()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EvacuationProgressComponent evacuationProgressComponent = default(EvacuationProgressComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<EvacuationProgressComponent>().MoveNext(ref evacuationProgressComponent))
		{
			return true;
		}
		return false;
	}

	public bool IsEvacuationEnabled()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<EvacuationProgressComponent> query = ((EntitySystem)this).EntityQueryEnumerator<EvacuationProgressComponent>();
		EvacuationProgressComponent progress = default(EvacuationProgressComponent);
		while (query.MoveNext(ref progress))
		{
			if (progress.Enabled)
			{
				return true;
			}
		}
		return false;
	}

	public int GetEvacuationProgress()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		EvacuationProgressComponent progress = default(EvacuationProgressComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<EvacuationProgressComponent>().MoveNext(ref progress))
		{
			return (int)progress.Progress;
		}
		return 0;
	}

	public bool IsEvacuationComplete()
	{
		return GetEvacuationProgress() >= 100;
	}

	private void ProcessEvacuation()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<EvacuationProgressComponent> query = ((EntitySystem)this).EntityQueryEnumerator<EvacuationProgressComponent>();
		EntityUid uid = default(EntityUid);
		EvacuationProgressComponent progress = default(EvacuationProgressComponent);
		AreaComponent area = default(AreaComponent);
		while (query.MoveNext(ref uid, ref progress) && progress.DropShipCrashed)
		{
			if (!progress.StartAnnounced)
			{
				progress.StartAnnounced = true;
				SetPumpAppearance(EvacuationPumpVisuals.Empty);
				SetPumpAmbience();
				StringBuilder areas = new StringBuilder();
				foreach (EntityUid areaId in GetEvacuationAreas(uid.ToCoordinates()))
				{
					bool powered = IsAreaPumpPowered(areaId);
					string line = $"[{((EntitySystem)this).Name(areaId, (MetaDataComponent)null)}] - [{(powered ? "Online" : "Offline")}]";
					areas.AppendLine(line);
				}
				areas.Append("Due to low orbit, extra fuel is required for non-surface evacuations.\nMaintain fueling functionality for optimal evacuation conditions.");
				_marineAnnounce.AnnounceARESStaging(null, areas.ToString());
			}
			if (progress.NextUpdate > time)
			{
				continue;
			}
			progress.NextUpdate = time + progress.UpdateEvery;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)progress, (MetaDataComponent)null);
			double progressAdd = 0.0;
			double progressMultiply = 1.0;
			foreach (EntityUid areaId2 in GetEvacuationAreas(uid.ToCoordinates()))
			{
				if (!_areaQuery.TryComp(areaId2, ref area) || !area.HijackEvacuationArea)
				{
					continue;
				}
				bool powered2 = IsAreaPumpPowered(areaId2);
				if (progress.LastPower.TryGetValue(areaId2, out var lastPower) && lastPower != powered2)
				{
					_marineAnnounce.AnnounceARESStaging(null, ((EntitySystem)this).Name(areaId2, (MetaDataComponent)null) + " - [" + (powered2 ? "Online" : "Offline") + "]");
				}
				progress.LastPower[areaId2] = powered2;
				if (powered2)
				{
					switch (area.HijackEvacuationType)
					{
					case AreaHijackEvacuationType.Add:
						progressAdd += area.HijackEvacuationWeight;
						break;
					case AreaHijackEvacuationType.Multiply:
						progressMultiply += area.HijackEvacuationWeight;
						break;
					}
				}
			}
			progress.Progress = Math.Min(progress.Required, progress.Progress + progressAdd * progressMultiply);
			if (!(progress.Progress >= (double)progress.NextAnnounce))
			{
				continue;
			}
			int current = progress.NextAnnounce;
			progress.NextAnnounce = current + progress.AnnounceEvery;
			string onAreas = string.Join(", ", from kvp in progress.LastPower
				where kvp.Value
				select ((EntitySystem)this).Name(kvp.Key, (MetaDataComponent)null));
			string offAreas = string.Join(", ", from kvp in progress.LastPower
				where !kvp.Value
				select ((EntitySystem)this).Name(kvp.Key, (MetaDataComponent)null));
			if (progress.Progress >= progress.Required)
			{
				_marineAnnounce.AnnounceARESStaging(null, "Emergency fuel replenishment is at 100 percent. Safe utilization of lifeboats and pods is now possible.");
				_xenoAnnounce.AnnounceAll(default(EntityUid), "The talls have completed their goals!");
				SetPumpAppearance(EvacuationPumpVisuals.Full);
				EvacuationProgressEvent ev = new EvacuationProgressEvent(100);
				((EntitySystem)this).RaiseLocalEvent<EvacuationProgressEvent>(uid, ref ev, true);
			}
			else if (progress.Progress >= progress.Required * 0.75)
			{
				_marineAnnounce.AnnounceARESStaging(null, MarinePercentageString(75));
				string xenoAnnounce = "The talls are three quarters of the way towards their goals.";
				if (onAreas.Length > 0)
				{
					xenoAnnounce = xenoAnnounce + " Disable the following areas: " + onAreas;
				}
				_xenoAnnounce.AnnounceAll(default(EntityUid), xenoAnnounce);
				SetPumpAppearance(EvacuationPumpVisuals.SeventyFive);
				EvacuationProgressEvent ev2 = new EvacuationProgressEvent(75);
				((EntitySystem)this).RaiseLocalEvent<EvacuationProgressEvent>(uid, ref ev2, true);
			}
			else if (progress.Progress >= progress.Required * 0.5)
			{
				_marineAnnounce.AnnounceARESStaging(null, MarinePercentageString(50));
				string xenoAnnounce2 = "The talls are half way towards their goals.";
				if (onAreas.Length > 0)
				{
					xenoAnnounce2 = xenoAnnounce2 + " Disable the following areas: " + onAreas;
				}
				_xenoAnnounce.AnnounceAll(default(EntityUid), xenoAnnounce2);
				SetPumpAppearance(EvacuationPumpVisuals.Fifty);
				EvacuationProgressEvent ev3 = new EvacuationProgressEvent(50);
				((EntitySystem)this).RaiseLocalEvent<EvacuationProgressEvent>(uid, ref ev3, true);
			}
			else if (progress.Progress >= progress.Required * 0.25)
			{
				string marineAnnounce = "Emergency fuel replenishment is at 25 percent. Lifeboat emergency early launch is now available.";
				marineAnnounce = ((offAreas.Length != 0) ? (marineAnnounce + " To increase speed, restore power to the following areas: " + offAreas) : (marineAnnounce + " All fueling areas operational."));
				_marineAnnounce.AnnounceARESStaging(null, marineAnnounce);
				string xenoAnnounce3 = "The talls are a quarter of the way towards their goals.";
				if (onAreas.Length > 0)
				{
					xenoAnnounce3 = xenoAnnounce3 + " Disable the following areas: " + onAreas;
				}
				_xenoAnnounce.AnnounceAll(default(EntityUid), xenoAnnounce3);
				SetPumpAppearance(EvacuationPumpVisuals.TwentyFive);
				EvacuationProgressEvent ev4 = new EvacuationProgressEvent(25);
				((EntitySystem)this).RaiseLocalEvent<EvacuationProgressEvent>(uid, ref ev4, true);
			}
			string MarinePercentageString(int percentage)
			{
				string marineAnnounce2 = $"Emergency fuel replenishment is at {percentage} percent.";
				if (offAreas.Length == 0)
				{
					return marineAnnounce2 + " All fueling areas operational.";
				}
				return marineAnnounce2 + "To increase speed, restore power to the following areas: " + offAreas;
			}
		}
	}

	private void ProcessExplodingPods()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DetonatingEvacuationComputerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<DetonatingEvacuationComputerComponent>();
		EntityUid uid = default(EntityUid);
		DetonatingEvacuationComputerComponent detonating = default(DetonatingEvacuationComputerComponent);
		MapGridComponent gridComp = default(MapGridComponent);
		EntityUid child = default(EntityUid);
		DoorComponent door = default(DoorComponent);
		while (query.MoveNext(ref uid, ref detonating))
		{
			EntityUid? gridUid = ((EntitySystem)this).Transform(uid).GridUid;
			if (!gridUid.HasValue)
			{
				continue;
			}
			EntityUid grid = gridUid.GetValueOrDefault();
			if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref gridComp))
			{
				continue;
			}
			TransformComponent gridTransform = ((EntitySystem)this).Transform(grid);
			if (!detonating.Detonated && time >= detonating.DetonateAt)
			{
				detonating.Detonated = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)detonating, (MetaDataComponent)null);
				MapCoordinates coordinates = _transform.ToMapCoordinates(gridTransform.Coordinates, true);
				_rmcExplosion.QueueExplosion(coordinates, "RMC", 40f, 5f, 25f, uid, 1f, int.MaxValue, canCreateVacuum: false);
			}
			if (!detonating.Ejected && time >= detonating.EjectAt)
			{
				detonating.Ejected = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)detonating, (MetaDataComponent)null);
				TransformChildrenEnumerator children = gridTransform.ChildEnumerator;
				while (((TransformChildrenEnumerator)(ref children)).MoveNext(ref child))
				{
					_hyperSleep.EjectChamber(Entity<HyperSleepChamberComponent>.op_Implicit(child));
					if (_doorQuery.TryComp(child, ref door))
					{
						EvacuationDoorComponent evacuationDoor = ((EntitySystem)this).EnsureComp<EvacuationDoorComponent>(child);
						evacuationDoor.Locked = false;
						((EntitySystem)this).Dirty(child, (IComponent)(object)evacuationDoor, (MetaDataComponent)null);
						_door.SetState(child, DoorState.Emagging, door);
					}
				}
			}
			if (detonating.Detonated && detonating.Ejected)
			{
				((EntitySystem)this).RemCompDeferred<DetonatingEvacuationComputerComponent>(uid);
			}
		}
	}

	public override void Update(float frameTime)
	{
		ProcessEvacuation();
		ProcessExplodingPods();
	}
}
