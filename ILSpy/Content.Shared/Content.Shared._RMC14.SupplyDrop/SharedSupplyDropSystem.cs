using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Extensions;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Rules;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.GameTicking;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.SupplyDrop;

public abstract class SharedSupplyDropSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private RMCCameraShakeSystem _rmcCameraShake;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCPullingSystem _rmcpulling;

	private int _supplyDropCount;

	private MapId? _supplyDropMap;

	private readonly HashSet<Entity<CanBeSupplyDroppedComponent>> _crates = new HashSet<Entity<CanBeSupplyDroppedComponent>>();

	private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BeingSupplyDroppedComponent, StorageOpenAttemptEvent>((EntityEventRefHandler<BeingSupplyDroppedComponent, StorageOpenAttemptEvent>)OnBeingSupplyDroppedOpenAttempt, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<SupplyDropComputerComponent>(((EntitySystem)this).Subs, (object)SupplyDropComputerUi.Key, (BuiEventSubscriber<SupplyDropComputerComponent>)delegate(Subscriber<SupplyDropComputerComponent> subs)
		{
			subs.Event<SupplyDropComputerLongitudeBuiMsg>((EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLongitudeBuiMsg>)OnSupplyDropComputerLongitudeMsg);
			subs.Event<SupplyDropComputerLatitudeBuiMsg>((EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLatitudeBuiMsg>)OnSupplyDropComputerLatitudeMsg);
			subs.Event<SupplyDropComputerUpdateBuiMsg>((EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerUpdateBuiMsg>)OnSupplyDropComputerUpdateMsg);
			subs.Event<SupplyDropComputerLaunchBuiMsg>((EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLaunchBuiMsg>)OnSupplyDropComputerLaunchMsg);
		});
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_supplyDropCount = 0;
		_supplyDropMap = null;
	}

	private void OnBeingSupplyDroppedOpenAttempt(Entity<BeingSupplyDroppedComponent> ent, ref StorageOpenAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void OnSupplyDropComputerLongitudeMsg(Entity<SupplyDropComputerComponent> ent, ref SupplyDropComputerLongitudeBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SetLongitude(Entity<SupplyDropComputerComponent>.op_Implicit((Entity<SupplyDropComputerComponent>.op_Implicit(ent), Entity<SupplyDropComputerComponent>.op_Implicit(ent))), args.Longitude);
	}

	private void OnSupplyDropComputerLatitudeMsg(Entity<SupplyDropComputerComponent> ent, ref SupplyDropComputerLatitudeBuiMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SetLatitude(Entity<SupplyDropComputerComponent>.op_Implicit((Entity<SupplyDropComputerComponent>.op_Implicit(ent), Entity<SupplyDropComputerComponent>.op_Implicit(ent))), args.Latitude);
	}

	private void OnSupplyDropComputerUpdateMsg(Entity<SupplyDropComputerComponent> ent, ref SupplyDropComputerUpdateBuiMsg args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			UpdateHasCrate(ent);
		}
	}

	private void OnSupplyDropComputerLaunchMsg(Entity<SupplyDropComputerComponent> ent, ref SupplyDropComputerLaunchBuiMsg args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			TryLaunchSupplyDropPopup(ent, ((BaseBoundUserInterfaceEvent)args).Actor);
		}
	}

	private bool TryGetPad(EntProtoId<SquadTeamComponent> squad, out Entity<SupplyDropPadComponent> pad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SupplyDropPadComponent> pads = ((EntitySystem)this).EntityQueryEnumerator<SupplyDropPadComponent>();
		EntityUid uid = default(EntityUid);
		SupplyDropPadComponent comp = default(SupplyDropPadComponent);
		while (pads.MoveNext(ref uid, ref comp))
		{
			EntProtoId<SquadTeamComponent>? squad2 = comp.Squad;
			if (squad2.HasValue && squad2.GetValueOrDefault() == squad)
			{
				pad = Entity<SupplyDropPadComponent>.op_Implicit((uid, comp));
				return true;
			}
		}
		pad = default(Entity<SupplyDropPadComponent>);
		return false;
	}

	public void SetSquad(Entity<SupplyDropComputerComponent?> computer, EntProtoId<SquadTeamComponent>? squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SupplyDropComputerComponent>(Entity<SupplyDropComputerComponent>.op_Implicit(computer), ref computer.Comp, false))
		{
			computer.Comp.Squad = squad;
			((EntitySystem)this).Dirty<SupplyDropComputerComponent>(computer, (MetaDataComponent)null);
		}
	}

	public void SetLongitude(Entity<SupplyDropComputerComponent?> computer, int longitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SupplyDropComputerComponent>(Entity<SupplyDropComputerComponent>.op_Implicit(computer), ref computer.Comp, false))
		{
			longitude.Cap(computer.Comp.MaxCoordinate);
			computer.Comp.Coordinates = new Vector2i(longitude, computer.Comp.Coordinates.Y);
			((EntitySystem)this).Dirty<SupplyDropComputerComponent>(computer, (MetaDataComponent)null);
		}
	}

	public void SetLatitude(Entity<SupplyDropComputerComponent?> computer, int latitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SupplyDropComputerComponent>(Entity<SupplyDropComputerComponent>.op_Implicit(computer), ref computer.Comp, false))
		{
			latitude.Cap(computer.Comp.MaxCoordinate);
			computer.Comp.Coordinates = new Vector2i(computer.Comp.Coordinates.X, latitude);
			((EntitySystem)this).Dirty<SupplyDropComputerComponent>(computer, (MetaDataComponent)null);
		}
	}

	public bool TryFindCrate(Entity<SupplyDropComputerComponent> computer, out Entity<CanBeSupplyDroppedComponent> crate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		crate = default(Entity<CanBeSupplyDroppedComponent>);
		EntProtoId<SquadTeamComponent>? squad = computer.Comp.Squad;
		if (squad.HasValue)
		{
			EntProtoId<SquadTeamComponent> squad2 = squad.GetValueOrDefault();
			if (TryGetPad(squad2, out Entity<SupplyDropPadComponent> pad))
			{
				_crates.Clear();
				_entityLookup.GetEntitiesInRange<CanBeSupplyDroppedComponent>(pad.Owner.ToCoordinates(), 0.25f, _crates, (LookupFlags)110);
				if (_crates.Count == 0)
				{
					return false;
				}
				crate = _crates.First();
				return true;
			}
		}
		return false;
	}

	public bool TryLaunchSupplyDropPopup(Entity<SupplyDropComputerComponent> computer, EntityUid user)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		if (time < computer.Comp.NextLaunchAt)
		{
			return false;
		}
		EntProtoId<SquadTeamComponent>? squad = computer.Comp.Squad;
		if (squad.HasValue)
		{
			EntProtoId<SquadTeamComponent> squad2 = squad.GetValueOrDefault();
			if (_rmcPlanet.TryPlanetToCoordinates(computer.Comp.Coordinates, out var mapCoordinates) && CanSupplyDropSquad(squad2))
			{
				if (!TryFindCrate(computer, out Entity<CanBeSupplyDroppedComponent> crate))
				{
					_popup.PopupCursor(base.Loc.GetString("rmc-supply-drop-no-crate"), user, PopupType.MediumCaution);
					return false;
				}
				if (!_area.CanSupplyDrop(mapCoordinates))
				{
					_popup.PopupCursor(base.Loc.GetString("rmc-supply-drop-underground"), user, PopupType.MediumCaution);
					return false;
				}
				if (_rmcMap.IsTileBlocked(mapCoordinates) || (_rmcMap.TryGetTileDef(mapCoordinates, out ContentTileDefinition tile) && tile.ID == "Space"))
				{
					_popup.PopupCursor(base.Loc.GetString("rmc-supply-drop-blocked"), user, PopupType.MediumCaution);
					return false;
				}
				SharedEntityStorageComponent storage = null;
				if (_entityStorage.ResolveStorage(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate), ref storage) && storage.Open)
				{
					_popup.PopupCursor(base.Loc.GetString("rmc-supply-drop-crate-open"), user, PopupType.MediumCaution);
					return false;
				}
				EntityCoordinates crateCoordinates = _transform.GetMoverCoordinates(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate));
				_popup.PopupClient(base.Loc.GetString("rmc-supply-drop-crate-load", (ValueTuple<string, object>)("crate", crate)), crateCoordinates, user, PopupType.Medium);
				_audio.PlayPredicted(crate.Comp.LaunchSound, crateCoordinates, (EntityUid?)user, (AudioParams?)null);
				_marineAnnounce.AnnounceSquad(base.Loc.GetString("rmc-supply-drop-squad-announcement", (ValueTuple<string, object>)("crate", crate)), squad2);
				_rmcpulling.TryStopAllPullsFromAndOn(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate));
				MapId mapId = EnsureMap();
				_transform.SetMapCoordinates(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate), new MapCoordinates((float)(_supplyDropCount++ * 50), 0f, mapId));
				BeingSupplyDroppedComponent dropping = ((EntitySystem)this).EnsureComp<BeingSupplyDroppedComponent>(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate));
				EntityCoordinates val = _transform.ToCoordinates(mapCoordinates);
				dropping.Target = ((EntityCoordinates)(ref val)).Offset(new Vector2(0.5f, -0.5f));
				dropping.ArrivingSoundAt = time + crate.Comp.ArrivingSoundDelay;
				dropping.DropAt = time + crate.Comp.DropDelay;
				dropping.OpenAt = time + crate.Comp.OpenDelay;
				dropping.LandingEffect = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(crate.Comp.LandingEffectId), dropping.Target);
				dropping.LandingDamage = crate.Comp.LandingDamage;
				((EntitySystem)this).Dirty(Entity<CanBeSupplyDroppedComponent>.op_Implicit(crate), (IComponent)(object)dropping, (MetaDataComponent)null);
				computer.Comp.LastLaunchAt = time;
				computer.Comp.NextLaunchAt = time + computer.Comp.Cooldown;
				((EntitySystem)this).Dirty<SupplyDropComputerComponent>(computer, (MetaDataComponent)null);
				return true;
			}
		}
		_popup.PopupCursor(base.Loc.GetString("rmc-supply-drop-not-operational"), user, PopupType.MediumCaution);
		return false;
	}

	private MapId EnsureMap()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_map.MapExists(_supplyDropMap))
		{
			_supplyDropMap = null;
		}
		if (!_supplyDropMap.HasValue)
		{
			MapId map = default(MapId);
			_map.CreateMap(ref map, true);
			_supplyDropMap = map;
		}
		return _supplyDropMap.Value;
	}

	private void UpdateHasCrate(Entity<SupplyDropComputerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		bool hasCrate = ent.Comp.HasCrate;
		ent.Comp.HasCrate = TryFindCrate(ent, out Entity<CanBeSupplyDroppedComponent> _);
		if (hasCrate != ent.Comp.HasCrate)
		{
			((EntitySystem)this).Dirty<SupplyDropComputerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private bool CanSupplyDropSquad(EntProtoId<SquadTeamComponent> squad)
	{
		SquadTeamComponent comp = default(SquadTeamComponent);
		if (!squad.TryGet(ref comp, _prototypes, _compFactory))
		{
			return true;
		}
		return comp.CanSupplyDrop;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<SupplyDropComputerComponent> computerQuery = ((EntitySystem)this).EntityQueryEnumerator<SupplyDropComputerComponent>();
		EntityUid uid = default(EntityUid);
		SupplyDropComputerComponent computer = default(SupplyDropComputerComponent);
		while (computerQuery.MoveNext(ref uid, ref computer))
		{
			if (!(time < computer.NextUpdate))
			{
				computer.NextUpdate = time + computer.UpdateEvery;
				UpdateHasCrate(Entity<SupplyDropComputerComponent>.op_Implicit((uid, computer)));
			}
		}
		EntityQueryEnumerator<BeingSupplyDroppedComponent> droppingQuery = ((EntitySystem)this).EntityQueryEnumerator<BeingSupplyDroppedComponent>();
		EntityUid uid2 = default(EntityUid);
		BeingSupplyDroppedComponent dropping = default(BeingSupplyDroppedComponent);
		TransformComponent effectXform = default(TransformComponent);
		while (droppingQuery.MoveNext(ref uid2, ref dropping))
		{
			if (!dropping.PlayedArrivingSound && time > dropping.ArrivingSoundAt && dropping.LandingEffect.HasValue)
			{
				EntityCoordinates soundCoordinates = dropping.Target;
				if (((EntitySystem)this).TryComp(dropping.LandingEffect.Value, ref effectXform))
				{
					soundCoordinates = _transform.GetMoverCoordinates(dropping.LandingEffect.Value, effectXform);
				}
				dropping.PlayedArrivingSound = true;
				_audio.PlayPvs(dropping.ArrivingSound, soundCoordinates, (AudioParams?)null);
				((EntitySystem)this).Dirty(uid2, (IComponent)(object)dropping, (MetaDataComponent)null);
			}
			if (time < dropping.DropAt)
			{
				continue;
			}
			if (!dropping.Landed)
			{
				dropping.Landed = true;
				if (!((EntitySystem)this).TerminatingOrDeleted(dropping.LandingEffect, (MetaDataComponent)null))
				{
					((EntitySystem)this).QueueDel(dropping.LandingEffect);
					dropping.LandingEffect = null;
					((EntitySystem)this).Dirty(uid2, (IComponent)(object)dropping, (MetaDataComponent)null);
				}
				DamageSpecifier landingDamage = dropping.LandingDamage;
				if (landingDamage != null)
				{
					_intersecting.Clear();
					_entityLookup.GetEntitiesInRange(dropping.Target, 0.33f, _intersecting, (LookupFlags)110);
					foreach (EntityUid intersecting in _intersecting)
					{
						_damageable.TryChangeDamage(intersecting, landingDamage, ignoreResistances: true);
					}
				}
				_transform.SetCoordinates(uid2, _transform.GetMoverCoordinates(dropping.Target));
				MapCoordinates mapPos = _transform.ToMapCoordinates(dropping.Target, true);
				foreach (ICommonSession recipient in Filter.Empty().AddInRange(mapPos, 7f, (ISharedPlayerManager)null, (IEntityManager)null).Recipients)
				{
					EntityUid? attachedEntity = recipient.AttachedEntity;
					if (attachedEntity.HasValue)
					{
						EntityUid player = attachedEntity.GetValueOrDefault();
						_rmcCameraShake.ShakeCamera(player, 4, 5);
					}
				}
			}
			if (!(time < dropping.OpenAt))
			{
				((EntitySystem)this).RemCompDeferred<BeingSupplyDroppedComponent>(uid2);
				_audio.PlayPvs(dropping.OpenSound, uid2, (AudioParams?)null);
			}
		}
	}
}
