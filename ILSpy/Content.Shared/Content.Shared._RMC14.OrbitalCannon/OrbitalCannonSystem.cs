using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.GameStates;
using Content.Shared._RMC14.Intel;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Mortar;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared._RMC14.Rules;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Explosion;
using Content.Shared.Ghost;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.OrbitalCannon;

public sealed class OrbitalCannonSystem : EntitySystem
{
	private const string WallTag = "Wall";

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private IntelSystem _intel;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private SharedMortarSystem _mortar;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PowerLoaderSystem _powerLoader;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCCameraShakeSystem _rmcCameraShake;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private SharedRMCFlammableSystem _rmcFlammable;

	[Dependency]
	private SharedRMCExplosionSystem _rmcExplosion;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private SharedRMCPvsSystem _rmcPvs;

	[Dependency]
	private TagSystem _tags;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private static readonly EntProtoId OrbitalTargetMarker = EntProtoId.op_Implicit("RMCLaserDropshipTarget");

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonComponent, MapInitEvent>((EntityEventRefHandler<OrbitalCannonComponent, MapInitEvent>)OnOrbitalCannonMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonComponent, PowerLoaderGrabEvent>((EntityEventRefHandler<OrbitalCannonComponent, PowerLoaderGrabEvent>)OnOrbitalCannonPowerLoaderGrab, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonWarheadComponent, PowerLoaderInteractEvent>((EntityEventRefHandler<OrbitalCannonWarheadComponent, PowerLoaderInteractEvent>)OnWarheadPowerLoaderInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonWarheadComponent, OrbitalBombardmentFireEvent>((EntityEventRefHandler<OrbitalCannonWarheadComponent, OrbitalBombardmentFireEvent>)OnWarheadOrbitalBombardmentFire, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonFuelComponent, PowerLoaderInteractEvent>((EntityEventRefHandler<OrbitalCannonFuelComponent, PowerLoaderInteractEvent>)OnFuelPowerLoaderInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonComputerComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<OrbitalCannonComputerComponent, BeforeActivatableUIOpenEvent>)OnComputerBeforeActivatableUIOpen, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<OrbitalCannonComputerComponent>(((EntitySystem)this).Subs, (object)OrbitalCannonComputerUI.Key, (BuiEventSubscriber<OrbitalCannonComputerComponent>)delegate(Subscriber<OrbitalCannonComputerComponent> subs)
		{
			subs.Event<OrbitalCannonComputerLoadBuiMsg>((EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerLoadBuiMsg>)OnComputerLoad);
			subs.Event<OrbitalCannonComputerUnloadBuiMsg>((EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerUnloadBuiMsg>)OnComputerUnload);
			subs.Event<OrbitalCannonComputerChamberBuiMsg>((EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerChamberBuiMsg>)OnComputerChamber);
		});
	}

	private void OnOrbitalCannonMapInit(Entity<OrbitalCannonComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		List<int> possibleFuels = ent.Comp.PossibleFuelRequirements.ToList();
		EntProtoId<OrbitalCannonWarheadComponent>[] warheadTypes = ent.Comp.WarheadTypes;
		foreach (EntProtoId<OrbitalCannonWarheadComponent> warhead in warheadTypes)
		{
			if (possibleFuels.Count <= 0)
			{
				possibleFuels = ent.Comp.PossibleFuelRequirements.ToList();
			}
			if (possibleFuels.Count <= 0)
			{
				((EntitySystem)this).Log.Error($"No possible fuel choice found for {warhead}");
				return;
			}
			int fuel = RandomExtensions.PickAndTake<int>(_random, (IList<int>)possibleFuels);
			ent.Comp.FuelRequirements.Add(new WarheadFuelRequirement(warhead, fuel));
		}
		((EntitySystem)this).Dirty<OrbitalCannonComponent>(ent, (MetaDataComponent)null);
		_appearance.SetData(Entity<OrbitalCannonComponent>.op_Implicit(ent), (Enum)OrbitalCannonVisuals.Base, (object)ent.Comp.Status, (AppearanceComponent)null);
	}

	private void OnOrbitalCannonPowerLoaderGrab(Entity<OrbitalCannonComponent> ent, ref PowerLoaderGrabEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && ent.Comp.Status == OrbitalCannonStatus.Unloaded)
		{
			BaseContainer fuel = default(BaseContainer);
			BaseContainer warhead = default(BaseContainer);
			if (_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(ent), ent.Comp.FuelContainer, ref fuel, (ContainerManagerComponent)null) && fuel.ContainedEntities.Count > 0)
			{
				IReadOnlyList<EntityUid> containedEntities = fuel.ContainedEntities;
				args.ToGrab = containedEntities[containedEntities.Count - 1];
				args.Handled = true;
			}
			else if (_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(ent), ent.Comp.WarheadContainer, ref warhead, (ContainerManagerComponent)null) && warhead.ContainedEntities.Count > 0)
			{
				IReadOnlyList<EntityUid> containedEntities2 = warhead.ContainedEntities;
				args.ToGrab = containedEntities2[containedEntities2.Count - 1];
				args.Handled = true;
			}
			if (args.Handled && _net.IsServer)
			{
				_audio.PlayPvs(ent.Comp.UnloadItemSound, args.Target, (AudioParams?)null);
			}
		}
	}

	private void OnWarheadPowerLoaderInteract(Entity<OrbitalCannonWarheadComponent> ent, ref PowerLoaderInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		OrbitalCannonComponent cannon = default(OrbitalCannonComponent);
		if (!((EntitySystem)this).TryComp<OrbitalCannonComponent>(args.Target, ref cannon))
		{
			return;
		}
		args.Handled = true;
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(args.Target, cannon.WarheadContainer, (ContainerManagerComponent)null);
		if (container.ContainedEntity.HasValue)
		{
			foreach (EntityUid buckled in args.Buckled)
			{
				_popup.PopupClient("There is already a warhead loaded!", args.Target, buckled, PopupType.MediumCaution);
			}
			return;
		}
		if (cannon.Status != OrbitalCannonStatus.Unloaded)
		{
			foreach (EntityUid buckled2 in args.Buckled)
			{
				_popup.PopupClient("The cannon isn't unloaded!", args.Target, buckled2, PopupType.MediumCaution);
			}
			return;
		}
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			foreach (EntityUid buckled3 in args.Buckled)
			{
				_popup.PopupClient($"You can't insert {((EntitySystem)this).Name(args.Used, (MetaDataComponent)null)} into the {((EntitySystem)this).Name(args.Target, (MetaDataComponent)null)}!", args.Target, buckled3, PopupType.MediumCaution);
			}
		}
		_popup.PopupClient($"You load {((EntitySystem)this).Name(args.Used, (MetaDataComponent)null)} into the {((EntitySystem)this).Name(args.Target, (MetaDataComponent)null)}!", args.Target, args.Target, PopupType.Medium);
		_powerLoader.TrySyncHands(Entity<PowerLoaderComponent>.op_Implicit(args.PowerLoader));
		if (_net.IsServer)
		{
			_audio.PlayPvs(cannon.LoadItemSound, args.Target, (AudioParams?)null);
		}
	}

	private void OnWarheadOrbitalBombardmentFire(Entity<OrbitalCannonWarheadComponent> ent, ref OrbitalBombardmentFireEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = _transform.ToCoordinates(args.Coordinates);
		if (TileHasIndestructibleWalls(coordinates))
		{
			bool found = false;
			Vector2i offset = default(Vector2i);
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
					{
						continue;
					}
					((Vector2i)(ref offset))._002Ector(x, y);
					MapCoordinates coordinates2 = args.Coordinates;
					MapCoordinates testMapCoordinates = ((MapCoordinates)(ref coordinates2)).Offset(Vector2i.op_Implicit(offset));
					if (_rmcMap.TryGetTileDef(testMapCoordinates, out ContentTileDefinition tile) && !(tile.ID == "Space"))
					{
						EntityCoordinates testCoordinates = _transform.ToCoordinates(testMapCoordinates);
						if (_area.CanOrbitalBombard(testCoordinates, out var _) && !TileHasIndestructibleWalls(testCoordinates))
						{
							coordinates = testCoordinates;
							((EntitySystem)this).Log.Info("Orbital bombardment impact redirected due to indestructible wall at impact site");
							found = true;
							break;
						}
					}
				}
				if (found)
				{
					break;
				}
			}
			if (!found)
			{
				((EntitySystem)this).Log.Info("Orbital bombardment impact blocked by indestructible walls, no valid alternative found");
				return;
			}
		}
		((EntitySystem)this).Spawn(EntProtoId<OrbitalCannonExplosionComponent>.op_Implicit(ent.Comp.Explosion), coordinates);
	}

	private void OnFuelPowerLoaderInteract(Entity<OrbitalCannonFuelComponent> ent, ref PowerLoaderInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		OrbitalCannonComponent cannon = default(OrbitalCannonComponent);
		if (!((EntitySystem)this).TryComp<OrbitalCannonComponent>(args.Target, ref cannon))
		{
			return;
		}
		args.Handled = true;
		BaseContainer warheadContainer = default(BaseContainer);
		if (!_container.TryGetContainer(args.Target, cannon.WarheadContainer, ref warheadContainer, (ContainerManagerComponent)null) || warheadContainer.ContainedEntities.Count == 0)
		{
			foreach (EntityUid buckled in args.Buckled)
			{
				_popup.PopupClient("A warhead must be placed in the " + ((EntitySystem)this).Name(args.Target, (MetaDataComponent)null) + " first.", args.Target, buckled, PopupType.MediumCaution);
			}
			return;
		}
		if (cannon.Status != OrbitalCannonStatus.Unloaded)
		{
			foreach (EntityUid buckled2 in args.Buckled)
			{
				_popup.PopupClient("The " + ((EntitySystem)this).Name(args.Target, (MetaDataComponent)null) + " isn't unloaded!", args.Target, buckled2, PopupType.MediumCaution);
			}
			return;
		}
		Container fuelContainer = _container.EnsureContainer<Container>(args.Target, cannon.FuelContainer, (ContainerManagerComponent)null);
		if (((BaseContainer)fuelContainer).ContainedEntities.Count >= cannon.MaxFuel)
		{
			foreach (EntityUid buckled3 in args.Buckled)
			{
				_popup.PopupClient("The " + ((EntitySystem)this).Name(args.Target, (MetaDataComponent)null) + " can't accept more solid fuel!", args.Target, buckled3, PopupType.MediumCaution);
			}
			return;
		}
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)fuelContainer, (TransformComponent)null, false))
		{
			foreach (EntityUid buckled4 in args.Buckled)
			{
				_popup.PopupClient($"You can't insert {((EntitySystem)this).Name(args.Used, (MetaDataComponent)null)} into the {((EntitySystem)this).Name(args.Target, (MetaDataComponent)null)}!", args.Target, buckled4, PopupType.MediumCaution);
			}
			return;
		}
		_popup.PopupClient($"You load {((EntitySystem)this).Name(args.Used, (MetaDataComponent)null)} into the {((EntitySystem)this).Name(args.Target, (MetaDataComponent)null)}!", args.Target, args.Target, PopupType.Medium);
		_powerLoader.TrySyncHands(Entity<PowerLoaderComponent>.op_Implicit(args.PowerLoader));
		if (_net.IsServer)
		{
			_audio.PlayPvs(cannon.LoadItemSound, args.Target, (AudioParams?)null);
		}
	}

	private void OnComputerBeforeActivatableUIOpen(Entity<OrbitalCannonComputerComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetClosestCannon(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), out Entity<OrbitalCannonComponent> cannon))
		{
			BaseContainer warheadContainer = default(BaseContainer);
			ent.Comp.Warhead = ((_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.WarheadContainer, ref warheadContainer, (ContainerManagerComponent)null) && warheadContainer.ContainedEntities.Count > 0) ? ((EntitySystem)this).Name(warheadContainer.ContainedEntities[0], (MetaDataComponent)null) : null);
			BaseContainer fuelContainer = default(BaseContainer);
			ent.Comp.Fuel = (_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.FuelContainer, ref fuelContainer, (ContainerManagerComponent)null) ? fuelContainer.ContainedEntities.Count : 0);
			ent.Comp.FuelRequirements = cannon.Comp.FuelRequirements;
			ent.Comp.Status = cannon.Comp.Status;
			((EntitySystem)this).Dirty<OrbitalCannonComputerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnComputerLoad(Entity<OrbitalCannonComputerComponent> ent, ref OrbitalCannonComputerLoadBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetClosestCannon(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), out Entity<OrbitalCannonComponent> cannon) || cannon.Comp.Status != OrbitalCannonStatus.Unloaded || !CannonHasWarhead(cannon) || CannonGetFuel(cannon) <= 0)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (!(time < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown))
		{
			cannon.Comp.LastToggledAt = time;
			cannon.Comp.Status = OrbitalCannonStatus.Loaded;
			((EntitySystem)this).Dirty<OrbitalCannonComponent>(cannon, (MetaDataComponent)null);
			ent.Comp.Status = cannon.Comp.Status;
			((EntitySystem)this).Dirty<OrbitalCannonComputerComponent>(ent, (MetaDataComponent)null);
			if (_net.IsServer)
			{
				_audio.PlayPvs(cannon.Comp.LoadSound, Entity<OrbitalCannonComponent>.op_Implicit(cannon), (AudioParams?)null);
			}
			CannonStatusChanged(cannon);
		}
	}

	private void OnComputerUnload(Entity<OrbitalCannonComputerComponent> ent, ref OrbitalCannonComputerUnloadBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetClosestCannon(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), out Entity<OrbitalCannonComponent> cannon) || cannon.Comp.Status != OrbitalCannonStatus.Loaded)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (!(time < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown))
		{
			cannon.Comp.LastToggledAt = time;
			cannon.Comp.Status = OrbitalCannonStatus.Unloaded;
			((EntitySystem)this).Dirty<OrbitalCannonComponent>(cannon, (MetaDataComponent)null);
			ent.Comp.Status = cannon.Comp.Status;
			((EntitySystem)this).Dirty<OrbitalCannonComputerComponent>(ent, (MetaDataComponent)null);
			if (_net.IsServer)
			{
				_audio.PlayPvs(cannon.Comp.UnloadSound, Entity<OrbitalCannonComponent>.op_Implicit(cannon), (AudioParams?)null);
			}
			CannonStatusChanged(cannon);
		}
	}

	private void OnComputerChamber(Entity<OrbitalCannonComputerComponent> ent, ref OrbitalCannonComputerChamberBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetClosestCannon(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), out Entity<OrbitalCannonComponent> cannon) || cannon.Comp.Status != OrbitalCannonStatus.Loaded || !CannonHasWarhead(cannon) || CannonGetFuel(cannon) <= 0)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (!(time < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown))
		{
			cannon.Comp.LastToggledAt = time;
			cannon.Comp.Status = OrbitalCannonStatus.Chambered;
			((EntitySystem)this).Dirty<OrbitalCannonComponent>(cannon, (MetaDataComponent)null);
			ent.Comp.Status = cannon.Comp.Status;
			((EntitySystem)this).Dirty<OrbitalCannonComputerComponent>(ent, (MetaDataComponent)null);
			if (_net.IsServer)
			{
				_audio.PlayPvs(cannon.Comp.ChamberSound, Entity<OrbitalCannonComponent>.op_Implicit(cannon), (AudioParams?)null);
			}
			CannonStatusChanged(cannon);
		}
	}

	public bool TryGetClosestCannon(EntityUid to, out Entity<OrbitalCannonComponent> cannon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		cannon = default(Entity<OrbitalCannonComponent>);
		TransformComponent transform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(to, ref transform))
		{
			return false;
		}
		float last = float.MaxValue;
		EntityQueryEnumerator<OrbitalCannonComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<OrbitalCannonComponent, TransformComponent>();
		EntityUid cannonId = default(EntityUid);
		OrbitalCannonComponent cannonComp = default(OrbitalCannonComponent);
		TransformComponent cannonTransform = default(TransformComponent);
		float distance = default(float);
		while (query.MoveNext(ref cannonId, ref cannonComp, ref cannonTransform))
		{
			EntityCoordinates coordinates = transform.Coordinates;
			if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, cannonTransform.Coordinates, ref distance) && distance < last)
			{
				last = distance;
				cannon = Entity<OrbitalCannonComponent>.op_Implicit((cannonId, cannonComp));
			}
		}
		return cannon != default(Entity<OrbitalCannonComponent>);
	}

	private bool CannonHasWarhead(Entity<OrbitalCannonComponent> cannon, out EntityUid warhead)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.WarheadContainer, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count > 0 && !base.EntityManager.IsQueuedForDeletion(container.ContainedEntities[0]))
		{
			warhead = container.ContainedEntities[0];
			return true;
		}
		warhead = default(EntityUid);
		return false;
	}

	private bool CannonHasWarhead(Entity<OrbitalCannonComponent> cannon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EntityUid warhead;
		return CannonHasWarhead(cannon, out warhead);
	}

	private int CannonGetFuel(Entity<OrbitalCannonComponent> cannon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.FuelContainer, ref container, (ContainerManagerComponent)null))
		{
			return 0;
		}
		return container.ContainedEntities.Count;
	}

	private void CannonStatusChanged(Entity<OrbitalCannonComponent> cannon)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<OrbitalCannonComponent>.op_Implicit(cannon), (Enum)OrbitalCannonVisuals.Base, (object)cannon.Comp.Status, (AppearanceComponent)null);
		OrbitalCannonChangedEvent ev = new OrbitalCannonChangedEvent(cannon, CannonHasWarhead(cannon), CannonGetFuel(cannon));
		((EntitySystem)this).RaiseLocalEvent<OrbitalCannonChangedEvent>(Entity<OrbitalCannonComponent>.op_Implicit(cannon), ref ev, true);
	}

	private bool TileHasIndestructibleWalls(EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		RMCAnchoredEntitiesEnumerator anchoredEntities = _rmcMap.GetAnchoredEntitiesEnumerator(coordinates, null, (DirectionFlag)0);
		EntityUid entity;
		while (anchoredEntities.MoveNext(out entity))
		{
			if (((EntitySystem)this).HasComp<TagComponent>(entity) && _tags.HasTag(entity, ProtoId<TagPrototype>.op_Implicit("Wall")) && !((EntitySystem)this).HasComp<DamageableComponent>(entity))
			{
				return true;
			}
		}
		return false;
	}

	public bool Fire(Entity<OrbitalCannonComponent> cannon, Vector2i fireCoordinates, EntityUid user, EntityUid squad)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return false;
		}
		if (cannon.Comp.Status != OrbitalCannonStatus.Chambered)
		{
			return false;
		}
		TimeSpan time = _timing.CurTime;
		if (cannon.Comp.LastFireAt.HasValue)
		{
			TimeSpan value = time;
			TimeSpan? timeSpan = cannon.Comp.LastFireAt + cannon.Comp.FireCooldown;
			if (value < timeSpan)
			{
				return false;
			}
		}
		BaseContainer warheadContainer = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.WarheadContainer, ref warheadContainer, (ContainerManagerComponent)null) || warheadContainer.ContainedEntities.Count == 0)
		{
			_popup.PopupCursor("The orbital cannon has no ammo chambered.", user, PopupType.LargeCaution);
			return false;
		}
		if (!_rmcPlanet.TryPlanetToCoordinates(fireCoordinates, out var planetCoordinates))
		{
			_popup.PopupCursor("The target zone appears to be out of bounds. Please check coordinates.", user, PopupType.LargeCaution);
			return false;
		}
		if (!_rmcMap.TryGetTileDef(planetCoordinates, out ContentTileDefinition tile) || tile.ID == "Space")
		{
			_popup.PopupCursor("The target zone appears to be out of bounds. Please check coordinates.", user, PopupType.LargeCaution);
			return false;
		}
		if (!_area.CanOrbitalBombard(_transform.ToCoordinates(planetCoordinates), out var roofed))
		{
			if (roofed)
			{
				_popup.PopupCursor("The target zone has strong biological protection. The orbital strike cannot reach here.", user, PopupType.LargeCaution);
				return false;
			}
			_popup.PopupCursor("The target zone is deep underground. The orbital strike cannot reach here.", user, PopupType.LargeCaution);
			return false;
		}
		_popup.PopupCursor("Orbital bombardment request accepted. Orbital cannons are now calibrating.", PopupType.Large);
		EntityUid warhead = warheadContainer.ContainedEntities[0];
		int misfuel = 0;
		BaseContainer fuelContainer = default(BaseContainer);
		if (_container.TryGetContainer(Entity<OrbitalCannonComponent>.op_Implicit(cannon), cannon.Comp.FuelContainer, ref fuelContainer, (ContainerManagerComponent)null))
		{
			int fuel = fuelContainer.ContainedEntities.Count;
			EntityPrototype obj = ((EntitySystem)this).Prototype(warhead, (MetaDataComponent)null);
			string warheadProto = ((obj != null) ? obj.ID : null);
			WarheadFuelRequirement? requirement = default(WarheadFuelRequirement?);
			if (Extensions.TryFirstOrNull<WarheadFuelRequirement>((IEnumerable<WarheadFuelRequirement>)cannon.Comp.FuelRequirements, (Func<WarheadFuelRequirement, bool>)((WarheadFuelRequirement f) => f.Warhead.Id == warheadProto), ref requirement))
			{
				misfuel = Math.Abs(fuel - requirement.Value.Fuel);
			}
		}
		int num = misfuel + 1;
		int offsetX = num * _random.Next(-3, 3);
		int offsetY = num * _random.Next(-3, 3);
		Vector2i adjustedCoords = fireCoordinates + new Vector2i(offsetX, offsetY);
		OrbitalCannonFiringComponent firing = ((EntitySystem)this).EnsureComp<OrbitalCannonFiringComponent>(Entity<OrbitalCannonComponent>.op_Implicit(cannon));
		firing.Coordinates = adjustedCoords;
		firing.WarheadName = ((EntitySystem)this).Name(warhead, (MetaDataComponent)null);
		firing.Squad = squad;
		firing.StartedAt = time;
		OrbitalCannonWarheadComponent warheadComp = default(OrbitalCannonWarheadComponent);
		if (((EntitySystem)this).TryComp<OrbitalCannonWarheadComponent>(warhead, ref warheadComp))
		{
			firing.FirstWarningRange = warheadComp.FirstWarningRange;
			firing.SecondWarningRange = warheadComp.SecondWarningRange;
			firing.ThirdWarningRange = warheadComp.ThirdWarningRange;
			if (warheadComp.IntelPointsAwarded > 0 && _net.IsServer)
			{
				_intel.AddPoints(warheadComp.IntelPointsAwarded);
			}
		}
		((EntitySystem)this).Dirty(Entity<OrbitalCannonComponent>.op_Implicit(cannon), (IComponent)(object)firing, (MetaDataComponent)null);
		_popup.PopupCursor("Orbital bombardment launched!", user);
		string logMessage = $"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user))} launched orbital bombardment at {fireCoordinates} for squad {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(squad))}, misfuel: {misfuel}, final coords: {adjustedCoords}";
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(0, 1);
		handler.AppendFormatted(logMessage);
		adminLog.Add(LogType.RMCOrbitalBombardment, ref handler);
		OrbitalCannonLaunchEvent ev = new OrbitalCannonLaunchEvent(cannon.Comp.FireCooldown + firing.ImpactDelay);
		((EntitySystem)this).RaiseLocalEvent<OrbitalCannonLaunchEvent>(ref ev);
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<OrbitalCannonFiringComponent, OrbitalCannonComponent> firingQuery = ((EntitySystem)this).EntityQueryEnumerator<OrbitalCannonFiringComponent, OrbitalCannonComponent>();
		EntityUid uid = default(EntityUid);
		OrbitalCannonFiringComponent firing = default(OrbitalCannonFiringComponent);
		OrbitalCannonComponent cannon = default(OrbitalCannonComponent);
		OrbitalCannonWarheadComponent foundWarheadComp = default(OrbitalCannonWarheadComponent);
		Entity<OrbitalCannonComponent> cannonEnt = default(Entity<OrbitalCannonComponent>);
		BaseContainer fuelContainer = default(BaseContainer);
		BaseContainer warheadContainer = default(BaseContainer);
		while (firingQuery.MoveNext(ref uid, ref firing, ref cannon))
		{
			if (!_rmcPlanet.TryPlanetToCoordinates(firing.Coordinates, out var planetCoordinates))
			{
				((EntitySystem)this).RemCompDeferred<OrbitalCannonFiringComponent>(uid);
				continue;
			}
			if (!firing.Alerted && time > firing.StartedAt + firing.AlertDelay)
			{
				firing.Alerted = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				Filter groundFilter = Filter.BroadcastMap(planetCoordinates.MapId).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => !((EntitySystem)this).HasComp<SquadMemberComponent>(e) && !((EntitySystem)this).HasComp<GhostComponent>(e)));
				_audio.PlayGlobal(cannon.GroundAlertSound, groundFilter, true, (AudioParams?)null);
				string msg = "[font size=16][color=red]Orbital bombardment launch command detected![/color][/font]";
				_rmcChat.ChatMessageToMany(msg, msg, groundFilter, ChatChannel.Radio);
				if (_area.TryGetArea(planetCoordinates, out Entity<AreaComponent>? _, out EntityPrototype areaProto))
				{
					msg = $"[color=red]Launch command informs {firing.WarheadName}. Estimated impact area: {areaProto.Name}[/color]";
					_rmcChat.ChatMessageToMany(msg, msg, groundFilter, ChatChannel.Radio);
				}
			}
			if (!firing.BegunFire && time > firing.StartedAt + firing.BeginFireDelay)
			{
				firing.BegunFire = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				Filter sameMap = Filter.BroadcastMap(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(uid)));
				_rmcCameraShake.ShakeCamera(sameMap, 10, 1);
				string msg2 = "[color=red]The deck of the UNS Almayer shudders as the orbital cannons open fire on the colony.[/color]";
				_rmcChat.ChatMessageToMany(msg2, msg2, sameMap, ChatChannel.Radio);
				_marineAnnounce.AnnounceSquad("WARNING! Ballistic trans-atmospheric launch detected! Get outside of Danger Close!", firing.Squad);
			}
			if (!firing.Fired && time > firing.StartedAt + firing.FireDelay)
			{
				firing.Fired = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				_audio.PlayPvs(cannon.FireSound, uid, (AudioParams?)null);
				EntityCoordinates planetEntCoordinates = _transform.ToCoordinates(planetCoordinates);
				_audio.PlayPvs(cannon.TravelSound, planetEntCoordinates, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithMaxDistance(75f));
				_mortar.PopupWarning(planetCoordinates, firing.FirstWarningRange, LocId.op_Implicit("rmc-ob-warning-one"), LocId.op_Implicit("rmc-ob-warning-above-one"), chat: true);
			}
			if (!firing.WarnedOne && time > firing.StartedAt + firing.WarnOneDelay)
			{
				firing.WarnedOne = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				_mortar.PopupWarning(planetCoordinates, firing.SecondWarningRange, LocId.op_Implicit("rmc-ob-warning-two"), LocId.op_Implicit("rmc-ob-warning-above-two"), chat: true);
			}
			if (!firing.WarnedTwo && time > firing.StartedAt + firing.WarnTwoDelay)
			{
				firing.WarnedTwo = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				_mortar.PopupWarning(planetCoordinates, firing.ThirdWarningRange, LocId.op_Implicit("rmc-ob-warning-three"), LocId.op_Implicit("rmc-ob-warning-above-three"), chat: true);
			}
			if (!firing.AegisBoomed && time > firing.StartedAt + firing.AegisBoomDelay)
			{
				firing.AegisBoomed = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)firing, (MetaDataComponent)null);
				if (CannonHasWarhead(Entity<OrbitalCannonComponent>.op_Implicit((uid, cannon)), out var foundWarhead) && ((EntitySystem)this).TryComp<OrbitalCannonWarheadComponent>(foundWarhead, ref foundWarheadComp) && foundWarheadComp.IsAegis)
				{
					EntityCoordinates planetEntCoordinates2 = _transform.ToCoordinates(planetCoordinates);
					(EntityUid, AudioComponent)? sound = _audio.PlayPvs(cannon.AegisBoomSound, planetEntCoordinates2, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithMaxDistance(300f));
					if (sound.HasValue)
					{
						_rmcPvs.AddGlobalOverride(sound.Value.Item1);
					}
				}
			}
			if (!firing.Impacted && time > firing.StartedAt + firing.ImpactDelay)
			{
				firing.Impacted = true;
				cannon.Status = OrbitalCannonStatus.Unloaded;
				cannon.LastFireAt = time;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)cannon, (MetaDataComponent)null);
				cannonEnt._002Ector(uid, cannon);
				int fuel = CannonGetFuel(cannonEnt);
				if (CannonHasWarhead(cannonEnt, out var warhead))
				{
					OrbitalBombardmentFireEvent ev = new OrbitalBombardmentFireEvent(cannonEnt, warhead, fuel, planetCoordinates);
					((EntitySystem)this).RaiseLocalEvent<OrbitalBombardmentFireEvent>(warhead, ref ev, false);
				}
				CannonStatusChanged(cannonEnt);
				((EntitySystem)this).RemCompDeferred<OrbitalCannonFiringComponent>(uid);
				if (_container.TryGetContainer(uid, cannon.FuelContainer, ref fuelContainer, (ContainerManagerComponent)null))
				{
					_container.CleanContainer(fuelContainer);
				}
				if (_container.TryGetContainer(uid, cannon.WarheadContainer, ref warheadContainer, (ContainerManagerComponent)null))
				{
					_container.CleanContainer(warheadContainer);
				}
			}
		}
		EntityQueryEnumerator<OrbitalCannonExplosionComponent> explosionQuery = ((EntitySystem)this).EntityQueryEnumerator<OrbitalCannonExplosionComponent>();
		EntityUid uid2 = default(EntityUid);
		OrbitalCannonExplosionComponent explosion = default(OrbitalCannonExplosionComponent);
		while (explosionQuery.MoveNext(ref uid2, ref explosion))
		{
			if (!explosion.Laser)
			{
				explosion.Laser = true;
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(OrbitalTargetMarker), _transform.GetMapCoordinates(uid2, (TransformComponent)null), (ComponentRegistry)null, default(Angle));
			}
			if (explosion.Current == 0 && explosion.LastAt == default(TimeSpan))
			{
				explosion.LastAt = time;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)explosion, (MetaDataComponent)null);
			}
			if (explosion.Current >= explosion.Steps.Count)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid2);
				continue;
			}
			OrbitalCannonExplosion step = explosion.Steps[explosion.Current];
			if (!(time >= explosion.LastAt + step.Delay))
			{
				continue;
			}
			if (step.Times <= 1)
			{
				explosion.Current++;
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)explosion, (MetaDataComponent)null);
			}
			else
			{
				if (time < explosion.LastStepAt + step.DelayPer)
				{
					continue;
				}
				explosion.Step++;
				explosion.LastStepAt = time;
				if (explosion.Step >= step.Times)
				{
					explosion.Current++;
					explosion.Step = 0;
					explosion.LastStepAt = default(TimeSpan);
					((EntitySystem)this).Dirty(uid2, (IComponent)(object)explosion, (MetaDataComponent)null);
				}
			}
			for (int i = 0; i < step.TimesPer; i++)
			{
				MapCoordinates mapCoordinates = _transform.GetMapCoordinates(uid2, (TransformComponent)null);
				EntityCoordinates coordinates = _transform.GetMoverCoordinates(uid2);
				if (step.Spread > 0)
				{
					Vector2 spread = _random.NextVector2((float)(-step.Spread), (float)step.Spread);
					mapCoordinates = ((MapCoordinates)(ref mapCoordinates)).Offset(spread);
					coordinates = ((EntityCoordinates)(ref coordinates)).Offset(spread);
				}
				if (step.CheckProtectionPer && !_area.CanOrbitalBombard(coordinates, out var _))
				{
					continue;
				}
				if (step.ExplosionEffect.HasValue)
				{
					EntityUid effect = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(step.ExplosionEffect.Value), mapCoordinates, (ComponentRegistry)null, default(Angle));
					_rmcExplosion.TryDoEffect(Entity<CMExplosionEffectComponent>.op_Implicit(effect));
				}
				ProtoId<ExplosionPrototype>? type = step.Type;
				if (type.HasValue)
				{
					ProtoId<ExplosionPrototype> type2 = type.GetValueOrDefault();
					_rmcExplosion.QueueExplosion(mapCoordinates, ProtoId<ExplosionPrototype>.op_Implicit(type2), step.Total, step.Slope, step.Max, uid2, 1f, int.MaxValue, canCreateVacuum: false);
				}
				EntProtoId? fire = step.Fire;
				if (fire.HasValue)
				{
					EntProtoId fire2 = fire.GetValueOrDefault();
					if (step.FireRange > 0)
					{
						_rmcFlammable.SpawnFireDiamond(fire2, coordinates, step.FireRange);
					}
				}
			}
		}
	}
}
