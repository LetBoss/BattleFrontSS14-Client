using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Teleporter;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleSystem : EntitySystem
{
	[Dependency]
	private readonly SharedEyeSystem _eye;

	[Dependency]
	private readonly VehicleViewToggleSystem _viewToggle;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly IMapManager _mapManager;

	[Dependency]
	private readonly SharedDoAfterSystem _doAfter;

	[Dependency]
	private readonly MapLoaderSystem _mapLoader;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly SharedRMCTeleporterSystem _rmcTeleporter;

	[Dependency]
	private readonly SkillsSystem _skills;

	[Dependency]
	private readonly MetaDataSystem _meta;

	[Dependency]
	private readonly MobStateSystem _mobState;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly Content.Shared.Vehicle.VehicleSystem _vehicles;

	[Dependency]
	private readonly VehicleLockSystem _vehicleLock;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleEnterComponent, ActivateInWorldEvent>((EntityEventRefHandler<VehicleEnterComponent, ActivateInWorldEvent>)OnVehicleEnterActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleEnterComponent, ComponentShutdown>((EntityEventRefHandler<VehicleEnterComponent, ComponentShutdown>)OnVehicleEnterShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleExitComponent, ActivateInWorldEvent>((EntityEventRefHandler<VehicleExitComponent, ActivateInWorldEvent>)OnVehicleExitActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleEnterComponent, VehicleEnterDoAfterEvent>((EntityEventRefHandler<VehicleEnterComponent, VehicleEnterDoAfterEvent>)OnVehicleEnterDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleExitComponent, VehicleExitDoAfterEvent>((EntityEventRefHandler<VehicleExitComponent, VehicleExitDoAfterEvent>)OnVehicleExitDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDriverSeatComponent, StrapAttemptEvent>((EntityEventRefHandler<VehicleDriverSeatComponent, StrapAttemptEvent>)OnDriverSeatStrapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDriverSeatComponent, StrappedEvent>((EntityEventRefHandler<VehicleDriverSeatComponent, StrappedEvent>)OnDriverSeatStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDriverSeatComponent, UnstrappedEvent>((EntityEventRefHandler<VehicleDriverSeatComponent, UnstrappedEvent>)OnDriverSeatUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleOperatorComponent, OnVehicleEnteredEvent>((EntityEventRefHandler<VehicleOperatorComponent, OnVehicleEnteredEvent>)OnVehicleOperatorEntered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleOperatorComponent, OnVehicleExitedEvent>((EntityEventRefHandler<VehicleOperatorComponent, OnVehicleExitedEvent>)OnVehicleOperatorExited, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleInteriorOccupantComponent, ComponentStartup>((EntityEventRefHandler<VehicleInteriorOccupantComponent, ComponentStartup>)OnOccupantStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleInteriorOccupantComponent, ComponentRemove>((EntityEventRefHandler<VehicleInteriorOccupantComponent, ComponentRemove>)OnOccupantRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleInteriorOccupantComponent, MapUidChangedEvent>((EntityEventRefHandler<VehicleInteriorOccupantComponent, MapUidChangedEvent>)OnOccupantMapChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleInteriorOccupantComponent, MetaFlagRemoveAttemptEvent>((EntityEventRefHandler<VehicleInteriorOccupantComponent, MetaFlagRemoveAttemptEvent>)OnOccupantMetaFlagRemoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointIntegrityComponent, VehicleCanRunEvent>((EntityEventRefHandler<HardpointIntegrityComponent, VehicleCanRunEvent>)OnFrameVehicleCanRun, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCConstructionAttemptEvent>((EntityEventRefHandler<RMCConstructionAttemptEvent>)OnConstructionAttempt, (Type[])null, (Type[])null);
	}

	private void OnVehicleEnterActivate(Entity<VehicleEnterComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		if (IsEntryBlockedByLock(ent.Owner, args.User))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-locked"), args.User, args.User, PopupType.SmallCaution);
			((HandledEntityEventArgs)args).Handled = true;
		}
		else
		{
			if (((HandledEntityEventArgs)args).Handled)
			{
				return;
			}
			if (!TryFindEntry(ent, args.User, out var entryIndex))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-use-doorway"), args.User, args.User);
				return;
			}
			VehicleInteriorComponent interior = ((EntitySystem)this).EnsureComp<VehicleInteriorComponent>(ent.Owner);
			if (!interior.EntryLocks.Add(entryIndex))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-busy"), args.User, args.User);
				return;
			}
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.EnterDoAfter, new VehicleEnterDoAfterEvent
			{
				EntryIndex = entryIndex
			}, ent.Owner)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				NeedHand = false
			};
			if (!_doAfter.TryStartDoAfter(doAfter))
			{
				interior.EntryLocks.Remove(entryIndex);
			}
			else
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private bool TryEnter(Entity<VehicleEnterComponent> ent, EntityUid user, int entryIndex = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (IsEntryBlockedByLock(ent.Owner, user))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-locked"), user, user, PopupType.SmallCaution);
			return false;
		}
		if (!EnsureInterior(ent, out VehicleInteriorComponent interior))
		{
			return false;
		}
		PruneTrackedOccupants(ent.Owner, interior);
		bool isXeno = ((EntitySystem)this).HasComp<XenoComponent>(user);
		if (isXeno)
		{
			if (ent.Comp.MaxXenos > 0 && !interior.Xenos.Contains(user) && CountLivingOccupants(interior.Xenos) >= ent.Comp.MaxXenos)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-xeno-full"), user, user);
				return false;
			}
		}
		else if (ent.Comp.MaxPassengers > 0 && !interior.Passengers.Contains(user) && CountLivingOccupants(interior.Passengers) >= ent.Comp.MaxPassengers)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-passenger-full"), user, user);
			return false;
		}
		EntityCoordinates coords = interior.Entry;
		MapCoordinates targetMapCoords;
		if (entryIndex >= 0 && entryIndex < ent.Comp.EntryPoints.Count)
		{
			Vector2? interiorCoords = ent.Comp.EntryPoints[entryIndex].InteriorCoords;
			if (interiorCoords.HasValue)
			{
				Vector2 interiorCoord = interiorCoords.GetValueOrDefault();
				EntityUid parent = (((EntityUid)(ref interior.Grid)).IsValid() ? interior.Grid : interior.EntryParent);
				EntityCoordinates entityCoords = default(EntityCoordinates);
				((EntityCoordinates)(ref entityCoords))._002Ector(parent, interiorCoord);
				targetMapCoords = _transform.ToMapCoordinates(entityCoords, true);
				_rmcTeleporter.HandlePulling(user, targetMapCoords);
				TrackOccupant(user, ent.Owner, isXeno);
				return true;
			}
		}
		targetMapCoords = _transform.ToMapCoordinates(coords, true);
		_rmcTeleporter.HandlePulling(user, targetMapCoords);
		TrackOccupant(user, ent.Owner, isXeno);
		return true;
	}

	private bool EnsureInterior(Entity<VehicleEnterComponent> ent, [NotNullWhen(true)] out VehicleInteriorComponent? interior)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TryComp<VehicleInteriorComponent>(ent.Owner, ref interior) && interior.MapId != MapId.Nullspace && _mapManager.MapExists((MapId?)interior.MapId))
		{
			return true;
		}
		interior = null;
		if (_net.IsClient)
		{
			return false;
		}
		interior = ((EntitySystem)this).EnsureComp<VehicleInteriorComponent>(ent.Owner);
		DeserializationOptions val = default(DeserializationOptions);
		((DeserializationOptions)(ref val))._002Ector();
		val.InitializeMaps = true;
		DeserializationOptions deserializeOptions = val;
		Entity<MapComponent>? loadedMap = default(Entity<MapComponent>?);
		HashSet<Entity<MapGridComponent>> hashSet = default(HashSet<Entity<MapGridComponent>>);
		if (!_mapLoader.TryLoadMap(ent.Comp.InteriorPath, ref loadedMap, ref hashSet, (DeserializationOptions?)deserializeOptions, default(Vector2), default(Angle)))
		{
			((EntitySystem)this).Log.Error($"[VehicleEnter] Failed to load interior for {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner))} at {ent.Comp.InteriorPath}");
			return false;
		}
		if (loadedMap.HasValue)
		{
			Entity<MapComponent> map = loadedMap.GetValueOrDefault();
			MapId mapId = map.Comp.MapId;
			EntityUid mapUid = map.Owner;
			EntityUid entryParent = map.Owner;
			EntityUid interiorGrid = EntityUid.Invalid;
			EntityQueryEnumerator<MapGridComponent, TransformComponent> gridEnum = ((EntitySystem)this).EntityQueryEnumerator<MapGridComponent, TransformComponent>();
			EntityUid gridUid = default(EntityUid);
			MapGridComponent val2 = default(MapGridComponent);
			TransformComponent gridXform = default(TransformComponent);
			while (gridEnum.MoveNext(ref gridUid, ref val2, ref gridXform))
			{
				if (!(gridXform.MapID != mapId))
				{
					entryParent = gridUid;
					interiorGrid = gridUid;
					break;
				}
			}
			EntityCoordinates entryCoords = default(EntityCoordinates);
			((EntityCoordinates)(ref entryCoords))._002Ector(entryParent, Vector2.Zero);
			EntityQueryEnumerator<VehicleExitComponent, TransformComponent> exitQuery = ((EntitySystem)this).EntityQueryEnumerator<VehicleExitComponent, TransformComponent>();
			EntityUid parentUid = default(EntityUid);
			VehicleExitComponent vehicleExitComponent = default(VehicleExitComponent);
			TransformComponent xform = default(TransformComponent);
			while (exitQuery.MoveNext(ref parentUid, ref vehicleExitComponent, ref xform))
			{
				if (!(xform.MapID != mapId))
				{
					entryCoords = xform.Coordinates;
					parentUid = xform.ParentUid;
					entryParent = (((EntityUid)(ref parentUid)).IsValid() ? xform.ParentUid : entryParent);
					break;
				}
			}
			interior.Map = mapUid;
			interior.MapId = mapId;
			interior.Entry = entryCoords;
			interior.EntryParent = entryParent;
			interior.Grid = interiorGrid;
			interior.Passengers.Clear();
			interior.Xenos.Clear();
			((EntitySystem)this).EnsureComp<VehicleInteriorLinkComponent>(mapUid).Vehicle = ent.Owner;
			return true;
		}
		return false;
	}

	private void OnVehicleEnterShutdown(Entity<VehicleEnterComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		CleanupInterior(ent.Owner);
	}

	private void CleanupInterior(EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior))
		{
			return;
		}
		VehicleInteriorOccupantComponent occupant = default(VehicleInteriorOccupantComponent);
		foreach (EntityUid passenger in new List<EntityUid>(interior.Passengers))
		{
			if (((EntitySystem)this).TryComp<VehicleInteriorOccupantComponent>(passenger, ref occupant) && occupant.Vehicle == vehicle)
			{
				((EntitySystem)this).RemComp<VehicleInteriorOccupantComponent>(passenger);
			}
		}
		VehicleInteriorOccupantComponent occupant2 = default(VehicleInteriorOccupantComponent);
		foreach (EntityUid xeno in new List<EntityUid>(interior.Xenos))
		{
			if (((EntitySystem)this).TryComp<VehicleInteriorOccupantComponent>(xeno, ref occupant2) && occupant2.Vehicle == vehicle)
			{
				((EntitySystem)this).RemComp<VehicleInteriorOccupantComponent>(xeno);
			}
		}
		VehicleInteriorLinkComponent link = default(VehicleInteriorLinkComponent);
		if (((EntityUid)(ref interior.Map)).IsValid() && base.EntityManager.EntityExists(interior.Map) && ((EntitySystem)this).TryComp<VehicleInteriorLinkComponent>(interior.Map, ref link) && link.Vehicle == vehicle)
		{
			((EntitySystem)this).RemComp<VehicleInteriorLinkComponent>(interior.Map);
		}
		((EntitySystem)this).RemComp<VehicleInteriorComponent>(vehicle);
		if (!_net.IsClient)
		{
			if (interior.MapId != MapId.Nullspace && _mapManager.MapExists((MapId?)interior.MapId))
			{
				_mapManager.DeleteMap(interior.MapId);
			}
			else if (((EntityUid)(ref interior.Map)).IsValid() && base.EntityManager.EntityExists(interior.Map))
			{
				((EntitySystem)this).Del((EntityUid?)interior.Map);
			}
		}
	}

	private void OnVehicleExitActivate(Entity<VehicleExitComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (ent.Comp.PendingExit)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-exit-busy"), args.User, args.User);
		}
		else
		{
			TransformComponent exitXform = default(TransformComponent);
			if (!((EntitySystem)this).TryComp(Entity<VehicleExitComponent>.op_Implicit(ent), ref exitXform) || exitXform.MapID == MapId.Nullspace || !TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !vehicle.HasValue)
			{
				return;
			}
			EntityUid vehicleUid = vehicle.GetValueOrDefault();
			VehicleEnterComponent enter = default(VehicleEnterComponent);
			if (!((EntitySystem)this).TryComp<VehicleEnterComponent>(vehicleUid, ref enter))
			{
				return;
			}
			if (IsExitBlockedByLock(vehicleUid, args.User))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-locked"), args.User, args.User, PopupType.SmallCaution);
				((HandledEntityEventArgs)args).Handled = true;
				return;
			}
			ent.Comp.PendingExit = true;
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, enter.ExitDoAfter, new VehicleExitDoAfterEvent(), ent.Owner)
			{
				BreakOnMove = true
			};
			if (!_doAfter.TryStartDoAfter(doAfter))
			{
				ent.Comp.PendingExit = false;
			}
			else
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private bool TryFindEntry(Entity<VehicleEnterComponent> ent, EntityUid user, out int entryIndex)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		entryIndex = -1;
		if (ent.Comp.EntryPoints.Count == 0)
		{
			return true;
		}
		HardpointIntegrityComponent frameIntegrity = default(HardpointIntegrityComponent);
		bool bypassEntry = ((EntitySystem)this).TryComp<HardpointIntegrityComponent>(ent.Owner, ref frameIntegrity) && frameIntegrity.BypassEntryOnZero && frameIntegrity.Integrity <= 0f;
		TransformComponent vehicleXform = ((EntitySystem)this).Transform(ent.Owner);
		TransformComponent userXform = ((EntitySystem)this).Transform(user);
		if (vehicleXform.MapID != userXform.MapID || vehicleXform.MapID == MapId.Nullspace)
		{
			return false;
		}
		Vector2 vehiclePos = _transform.GetWorldPosition(vehicleXform);
		Vector2 delta = _transform.GetWorldPosition(userXform) - vehiclePos;
		Angle val = -vehicleXform.LocalRotation;
		Vector2 localDelta = ((Angle)(ref val)).RotateVec(ref delta);
		if (bypassEntry)
		{
			float closestDistance = float.MaxValue;
			int closestIndex = -1;
			for (int i = 0; i < ent.Comp.EntryPoints.Count; i++)
			{
				VehicleEntryPoint entry = ent.Comp.EntryPoints[i];
				float distance = (localDelta - entry.Offset).LengthSquared();
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestIndex = i;
				}
			}
			if (closestIndex >= 0)
			{
				entryIndex = closestIndex;
				return true;
			}
			return false;
		}
		for (int j = 0; j < ent.Comp.EntryPoints.Count; j++)
		{
			VehicleEntryPoint entry2 = ent.Comp.EntryPoints[j];
			if ((localDelta - entry2.Offset).Length() <= entry2.Radius)
			{
				entryIndex = j;
				return true;
			}
		}
		return false;
	}

	private void OnVehicleEnterDoAfter(Entity<VehicleEnterComponent> ent, ref VehicleEnterDoAfterEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (((EntitySystem)this).TryComp<VehicleInteriorComponent>(ent.Owner, ref interior))
		{
			interior.EntryLocks.Remove(args.EntryIndex);
		}
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryEnter(ent, args.User, args.EntryIndex);
		}
	}

	private bool TryExit(Entity<VehicleExitComponent> ent, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent exitXform = default(TransformComponent);
		if (!((EntitySystem)this).TryComp(Entity<VehicleExitComponent>.op_Implicit(ent), ref exitXform) || exitXform.MapID == MapId.Nullspace)
		{
			return false;
		}
		EntityUid vehicleUid;
		VehicleEnterComponent enter = default(VehicleEnterComponent);
		TransformComponent vehicleXform;
		EntityUid? parent;
		EntityUid value;
		if (TryGetVehicleFromInterior(ent.Owner, out var vehicle) && vehicle.HasValue)
		{
			vehicleUid = vehicle.GetValueOrDefault();
			if (!((EntitySystem)this).TryComp<VehicleEnterComponent>(vehicleUid, ref enter))
			{
				return false;
			}
			if (IsExitBlockedByLock(vehicleUid, user))
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-vehicle-enter-locked"), user, user, PopupType.SmallCaution);
				return false;
			}
			vehicleXform = ((EntitySystem)this).Transform(vehicleUid);
			parent = vehicleXform.ParentUid;
			if (parent.HasValue)
			{
				value = parent.Value;
				if (((EntityUid)(ref value)).IsValid())
				{
					goto IL_00bb;
				}
			}
			parent = vehicleXform.MapUid;
			goto IL_00bb;
		}
		return false;
		IL_00bb:
		if (parent.HasValue)
		{
			value = parent.Value;
			if (((EntityUid)(ref value)).IsValid())
			{
				int entryIndex = ent.Comp.EntryIndex;
				Vector2 offset = ((entryIndex < 0 || entryIndex >= enter.EntryPoints.Count) ? enter.ExitOffset : enter.EntryPoints[entryIndex].Offset);
				Angle localRotation = vehicleXform.LocalRotation;
				Vector2 rotated = ((Angle)(ref localRotation)).RotateVec(ref offset);
				Vector2 position = vehicleXform.LocalPosition + rotated;
				EntityCoordinates exitCoords = default(EntityCoordinates);
				((EntityCoordinates)(ref exitCoords))._002Ector(parent.Value, position);
				MapCoordinates exitMapCoords = _transform.ToMapCoordinates(exitCoords, true);
				_rmcTeleporter.HandlePulling(user, exitMapCoords);
				UntrackOccupant(user, vehicleUid);
				return true;
			}
		}
		return false;
	}

	private void OnVehicleExitDoAfter(Entity<VehicleExitComponent> ent, ref VehicleExitDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.PendingExit = false;
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = TryExit(ent, args.User);
		}
	}

	private void OnOccupantStartup(Entity<VehicleInteriorOccupantComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_meta.AddFlag(Entity<VehicleInteriorOccupantComponent>.op_Implicit(ent), (MetaDataFlags)32, (MetaDataComponent)null);
	}

	private void OnOccupantRemove(Entity<VehicleInteriorOccupantComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		_meta.RemoveFlag(Entity<VehicleInteriorOccupantComponent>.op_Implicit(ent), (MetaDataFlags)32, (MetaDataComponent)null);
		if (((EntityUid)(ref ent.Comp.Vehicle)).IsValid())
		{
			UnregisterTrackedOccupant(ent.Comp.Vehicle, ent.Owner, ent.Comp.IsXeno);
		}
		if (!_net.IsClient)
		{
			((EntitySystem)this).RemCompDeferred<RMCVehicleInteriorOccupantComponent>(ent.Owner);
		}
	}

	private void OnOccupantMapChanged(Entity<VehicleInteriorOccupantComponent> ent, ref MapUidChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Vehicle == EntityUid.Invalid)
		{
			return;
		}
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (((EntitySystem)this).TryComp<VehicleInteriorComponent>(ent.Comp.Vehicle, ref interior))
		{
			MapId? newMapId = ((MapUidChangedEvent)(ref args)).NewMapId;
			MapId mapId = interior.MapId;
			if (newMapId.HasValue && newMapId.GetValueOrDefault() == mapId)
			{
				RegisterTrackedOccupant(ent.Comp.Vehicle, ent.Owner, ent.Comp.IsXeno, interior);
				return;
			}
		}
		((EntitySystem)this).RemCompDeferred<VehicleInteriorOccupantComponent>(ent.Owner);
	}

	private void OnOccupantMetaFlagRemoveAttempt(Entity<VehicleInteriorOccupantComponent> ent, ref MetaFlagRemoveAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		if ((args.ToRemove & 0x20) != 0 && (int)((Component)ent.Comp).LifeStage <= 6)
		{
			ref MetaDataFlags toRemove = ref args.ToRemove;
			toRemove = (MetaDataFlags)((uint)toRemove & 0xDFu);
		}
	}

	private void TrackOccupant(EntityUid user, EntityUid vehicle, bool isXeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorOccupantComponent occupant = ((EntitySystem)this).EnsureComp<VehicleInteriorOccupantComponent>(user);
		if (((EntityUid)(ref occupant.Vehicle)).IsValid() && occupant.Vehicle != vehicle)
		{
			UnregisterTrackedOccupant(occupant.Vehicle, user, occupant.IsXeno);
		}
		occupant.Vehicle = vehicle;
		occupant.IsXeno = isXeno;
		RegisterTrackedOccupant(vehicle, user, isXeno);
		SetInteriorOccupantVehicle(user, vehicle);
	}

	private void SetInteriorOccupantVehicle(EntityUid user, EntityUid vehicle)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			RMCVehicleInteriorOccupantComponent occupant = ((EntitySystem)this).EnsureComp<RMCVehicleInteriorOccupantComponent>(user);
			EntityUid? vehicle2 = occupant.Vehicle;
			if (!vehicle2.HasValue || !(vehicle2.GetValueOrDefault() == vehicle))
			{
				occupant.Vehicle = vehicle;
				((EntitySystem)this).Dirty(user, (IComponent)(object)occupant, (MetaDataComponent)null);
			}
		}
	}

	private void UntrackOccupant(EntityUid user, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorOccupantComponent occupant = default(VehicleInteriorOccupantComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorOccupantComponent>(user, ref occupant) || occupant.Vehicle != vehicle)
		{
			UnregisterTrackedOccupant(vehicle, user, ((EntitySystem)this).HasComp<XenoComponent>(user));
		}
		else
		{
			((EntitySystem)this).RemComp<VehicleInteriorOccupantComponent>(user);
		}
	}

	private void RegisterTrackedOccupant(EntityUid vehicle, EntityUid user, bool isXeno, VehicleInteriorComponent? interior = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<VehicleInteriorComponent>(vehicle, ref interior, false))
		{
			if (isXeno)
			{
				interior.Passengers.Remove(user);
				interior.Xenos.Add(user);
			}
			else
			{
				interior.Xenos.Remove(user);
				interior.Passengers.Add(user);
			}
		}
	}

	private void UnregisterTrackedOccupant(EntityUid vehicle, EntityUid user, bool isXeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior))
		{
			if (isXeno)
			{
				interior.Xenos.Remove(user);
			}
			else
			{
				interior.Passengers.Remove(user);
			}
		}
	}

	private void PruneTrackedOccupants(EntityUid vehicle, VehicleInteriorComponent interior)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		VehicleInteriorOccupantComponent occupant = default(VehicleInteriorOccupantComponent);
		foreach (EntityUid passenger in new List<EntityUid>(interior.Passengers))
		{
			if (!((EntitySystem)this).TryComp<VehicleInteriorOccupantComponent>(passenger, ref occupant) || !(occupant.Vehicle == vehicle) || occupant.IsXeno || !(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(passenger)) == interior.MapId))
			{
				interior.Passengers.Remove(passenger);
			}
		}
		VehicleInteriorOccupantComponent occupant2 = default(VehicleInteriorOccupantComponent);
		foreach (EntityUid xeno in new List<EntityUid>(interior.Xenos))
		{
			if (!((EntitySystem)this).TryComp<VehicleInteriorOccupantComponent>(xeno, ref occupant2) || !(occupant2.Vehicle == vehicle) || !occupant2.IsXeno || !(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(xeno)) == interior.MapId))
			{
				interior.Xenos.Remove(xeno);
			}
		}
	}

	private int CountLivingOccupants(HashSet<EntityUid> occupants)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		foreach (EntityUid occupant in occupants)
		{
			if (!_mobState.IsDead(occupant))
			{
				count++;
			}
		}
		return count;
	}

	private void OnDriverSeatStrapAttempt(Entity<VehicleDriverSeatComponent> ent, ref StrapAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(args.Buckle.Owner), ent.Comp.Skills) && args.Popup)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-skills-cant-operate", (ValueTuple<string, object>)("target", ent)), Entity<BuckleComponent>.op_Implicit(args.Buckle), args.User);
		}
	}

	private void OnDriverSeatStrapped(Entity<VehicleDriverSeatComponent> ent, ref StrappedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (!_net.IsClient && TryGetVehicleFromInterior(ent.Owner, out var vehicle) && ((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp))
		{
			_vehicles.TrySetOperator(Entity<VehicleComponent>.op_Implicit((vehicle.Value, vehicleComp)), args.Buckle.Owner);
			((EntitySystem)this).EnsureComp<VehicleOperatorComponent>(args.Buckle.Owner);
			_vehicleLock.EnableLockAction(args.Buckle.Owner, vehicle.Value);
		}
	}

	private void OnDriverSeatUnstrapped(Entity<VehicleDriverSeatComponent> ent, ref UnstrappedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (_net.IsClient || !TryGetVehicleFromInterior(ent.Owner, out var vehicle) || !((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp))
		{
			return;
		}
		_vehicleLock.DisableLockAction(args.Buckle.Owner, vehicle.Value);
		EntityUid? val = vehicleComp.Operator;
		EntityUid owner = args.Buckle.Owner;
		if (val.HasValue && !(val.GetValueOrDefault() != owner))
		{
			_vehicles.TryRemoveOperator(Entity<VehicleComponent>.op_Implicit((vehicle.Value, vehicleComp)));
			if (!IsOperatingOtherVehicle(args.Buckle.Owner))
			{
				((EntitySystem)this).RemCompDeferred<VehicleOperatorComponent>(args.Buckle.Owner);
			}
		}
	}

	private void OnVehicleOperatorEntered(Entity<VehicleOperatorComponent> ent, ref OnVehicleEnteredEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && ((EntitySystem)this).HasComp<VehicleEnterComponent>(args.Vehicle.Owner))
		{
			_eye.SetTarget(ent.Owner, (EntityUid?)args.Vehicle.Owner, (EyeComponent)null);
			_viewToggle.EnableViewToggle(ent.Owner, args.Vehicle.Owner, args.Vehicle.Owner, null, isOutside: true);
		}
	}

	private void OnVehicleOperatorExited(Entity<VehicleOperatorComponent> ent, ref OnVehicleExitedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent eye = default(EyeComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<EyeComponent>(Entity<VehicleOperatorComponent>.op_Implicit(ent), ref eye))
		{
			_viewToggle.DisableViewToggle(ent.Owner, args.Vehicle.Owner);
			EntityUid? target = eye.Target;
			EntityUid owner = args.Vehicle.Owner;
			if (target.HasValue && !(target.GetValueOrDefault() != owner))
			{
				_eye.SetTarget(ent.Owner, (EntityUid?)null, eye);
			}
		}
	}

	private bool IsOperatingOtherVehicle(EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuckleComponent buckle = default(BuckleComponent);
		if (!((EntitySystem)this).TryComp<BuckleComponent>(entity, ref buckle))
		{
			return false;
		}
		if (!buckle.BuckledTo.HasValue)
		{
			return false;
		}
		return ((EntitySystem)this).HasComp<VehicleDriverSeatComponent>(buckle.BuckledTo);
	}

	private void OnFrameVehicleCanRun(Entity<HardpointIntegrityComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanRun && !(ent.Comp.Integrity > 0f))
		{
			args.CanRun = false;
		}
	}

	private void OnConstructionAttempt(ref RMCConstructionAttemptEvent ev)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!ev.Cancelled && !_net.IsClient && TryGetVehicleFromInterior(ev.Location.EntityId, out var _))
		{
			ev.Cancelled = true;
			ev.Popup = base.Loc.GetString("construction-system-inside-container");
		}
	}

	private bool IsEntryBlockedByLock(EntityUid vehicle, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		VehicleLockComponent vehicleLock = default(VehicleLockComponent);
		if (!((EntitySystem)this).TryComp<VehicleLockComponent>(vehicle, ref vehicleLock) || !vehicleLock.Locked)
		{
			return false;
		}
		return !CanBypassLockWithDestroyedFrame(vehicle, user);
	}

	private bool IsExitBlockedByLock(EntityUid vehicle, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		VehicleLockComponent vehicleLock = default(VehicleLockComponent);
		if (!((EntitySystem)this).TryComp<VehicleLockComponent>(vehicle, ref vehicleLock) || !vehicleLock.Locked)
		{
			return false;
		}
		return !CanBypassLockWithDestroyedFrame(vehicle, user);
	}

	private bool CanBypassLockWithDestroyedFrame(EntityUid vehicle, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			return false;
		}
		HardpointIntegrityComponent frameIntegrity = default(HardpointIntegrityComponent);
		if (!((EntitySystem)this).TryComp<HardpointIntegrityComponent>(vehicle, ref frameIntegrity))
		{
			return false;
		}
		if (frameIntegrity.BypassEntryOnZero)
		{
			return frameIntegrity.Integrity <= 0f;
		}
		return false;
	}

	public bool TryGetVehicleFromInterior(EntityUid interiorEntity, out EntityUid? vehicle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		vehicle = null;
		MapId mapId = _transform.GetMapId(Entity<TransformComponent>.op_Implicit(interiorEntity));
		if (mapId == MapId.Nullspace || !_mapManager.MapExists((MapId?)mapId))
		{
			return false;
		}
		EntityUid mapUid = _mapManager.GetMapEntityId(mapId);
		VehicleInteriorLinkComponent link = default(VehicleInteriorLinkComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorLinkComponent>(mapUid, ref link) || ((EntitySystem)this).Deleted(link.Vehicle, (MetaDataComponent)null))
		{
			return false;
		}
		vehicle = link.Vehicle;
		return true;
	}

	public bool TryResolveControlledVehicle(EntityUid user, out EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		vehicle = EntityUid.Invalid;
		VehicleOperatorComponent op = default(VehicleOperatorComponent);
		if (((EntitySystem)this).TryComp<VehicleOperatorComponent>(user, ref op))
		{
			EntityUid? vehicle2 = op.Vehicle;
			if (vehicle2.HasValue)
			{
				EntityUid operatedVehicle = vehicle2.GetValueOrDefault();
				if (base.EntityManager.EntityExists(operatedVehicle))
				{
					vehicle = operatedVehicle;
					return true;
				}
			}
		}
		if (TryGetVehicleFromInterior(user, out var interiorVehicle) && interiorVehicle.HasValue)
		{
			EntityUid interiorVehicleUid = interiorVehicle.GetValueOrDefault();
			if (base.EntityManager.EntityExists(interiorVehicleUid))
			{
				vehicle = interiorVehicleUid;
				return true;
			}
		}
		return false;
	}

	public bool TryGetInteriorMapId(EntityUid vehicle, out MapId mapId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		mapId = MapId.Nullspace;
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior))
		{
			return false;
		}
		mapId = interior.MapId;
		return mapId != MapId.Nullspace;
	}

	public bool TryGetInteriorEntryCoordinates(EntityUid vehicle, out EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		coordinates = EntityCoordinates.Invalid;
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior))
		{
			return false;
		}
		coordinates = interior.Entry;
		return ((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager);
	}

	public bool TryEnsureInteriorEntry(EntityUid vehicle, out EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		coordinates = EntityCoordinates.Invalid;
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior))
		{
			VehicleEnterComponent enter = default(VehicleEnterComponent);
			if (!((EntitySystem)this).TryComp<VehicleEnterComponent>(vehicle, ref enter))
			{
				return false;
			}
			if (!EnsureInterior(Entity<VehicleEnterComponent>.op_Implicit((vehicle, enter)), out interior))
			{
				return false;
			}
		}
		coordinates = interior.Entry;
		return ((EntityCoordinates)(ref coordinates)).IsValid((IEntityManager)(object)base.EntityManager);
	}

	public bool TryGetInteriorGrid(EntityUid vehicle, out EntityUid grid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		grid = EntityUid.Invalid;
		VehicleInteriorComponent interior = default(VehicleInteriorComponent);
		if (!((EntitySystem)this).TryComp<VehicleInteriorComponent>(vehicle, ref interior) || !((EntityUid)(ref interior.Grid)).IsValid() || !((EntitySystem)this).Exists(interior.Grid))
		{
			return false;
		}
		grid = interior.Grid;
		return true;
	}

	public bool TryExitFromInterior(EntityUid user, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		VehicleEnterComponent enter = default(VehicleEnterComponent);
		if (!((EntitySystem)this).TryComp<VehicleEnterComponent>(vehicle, ref enter))
		{
			return false;
		}
		if (IsExitBlockedByLock(vehicle, user))
		{
			return false;
		}
		TransformComponent vehicleXform = ((EntitySystem)this).Transform(vehicle);
		EntityUid? parent = vehicleXform.ParentUid;
		EntityUid value;
		if (parent.HasValue)
		{
			value = parent.Value;
			if (((EntityUid)(ref value)).IsValid())
			{
				goto IL_0050;
			}
		}
		parent = vehicleXform.MapUid;
		goto IL_0050;
		IL_0050:
		if (parent.HasValue)
		{
			value = parent.Value;
			if (((EntityUid)(ref value)).IsValid())
			{
				Angle localRotation = vehicleXform.LocalRotation;
				Vector2 rotated = ((Angle)(ref localRotation)).RotateVec(ref enter.ExitOffset);
				Vector2 position = vehicleXform.LocalPosition + rotated;
				EntityCoordinates exitCoords = default(EntityCoordinates);
				((EntityCoordinates)(ref exitCoords))._002Ector(parent.Value, position);
				MapCoordinates exitMapCoords = _transform.ToMapCoordinates(exitCoords, true);
				_rmcTeleporter.HandlePulling(user, exitMapCoords);
				UntrackOccupant(user, vehicle);
				((EntitySystem)this).RemCompDeferred<RMCVehicleInteriorOccupantComponent>(user);
				return true;
			}
		}
		return false;
	}

	public bool TryGetDisplayEntity(EntityUid entity, out EntityUid displayEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		displayEntity = entity;
		RMCVehicleInteriorOccupantComponent occupant = default(RMCVehicleInteriorOccupantComponent);
		if (((EntitySystem)this).TryComp<RMCVehicleInteriorOccupantComponent>(entity, ref occupant))
		{
			EntityUid? vehicle = occupant.Vehicle;
			if (vehicle.HasValue)
			{
				EntityUid occupantVehicle = vehicle.GetValueOrDefault();
				if (((EntitySystem)this).Exists(occupantVehicle))
				{
					displayEntity = occupantVehicle;
					return true;
				}
			}
		}
		if (TryGetVehicleFromInterior(entity, out var interiorVehicle) && interiorVehicle.HasValue)
		{
			EntityUid interiorVehicleUid = interiorVehicle.GetValueOrDefault();
			if (((EntitySystem)this).Exists(interiorVehicleUid))
			{
				displayEntity = interiorVehicleUid;
				return true;
			}
		}
		return ((EntitySystem)this).Exists(entity);
	}

	public bool TryGetDisplayMapCoordinates(EntityUid entity, out MapCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		coordinates = MapCoordinates.Nullspace;
		if (!TryGetDisplayEntity(entity, out var displayEntity))
		{
			return false;
		}
		coordinates = _transform.GetMapCoordinates(displayEntity, (TransformComponent)null);
		return coordinates.MapId != MapId.Nullspace;
	}

	public bool TryGetDisplayMapId(EntityUid entity, out MapId mapId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		mapId = MapId.Nullspace;
		if (!TryGetDisplayMapCoordinates(entity, out var coordinates))
		{
			return false;
		}
		mapId = coordinates.MapId;
		return mapId != MapId.Nullspace;
	}
}
