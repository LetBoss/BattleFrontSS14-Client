// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.SupplyDrop.SharedSupplyDropSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
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
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<BeingSupplyDroppedComponent, StorageOpenAttemptEvent>(new EntityEventRefHandler<BeingSupplyDroppedComponent, StorageOpenAttemptEvent>(this.OnBeingSupplyDroppedOpenAttempt));
    this.Subs.BuiEvents<SupplyDropComputerComponent>((object) SupplyDropComputerUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<SupplyDropComputerComponent>) (subs =>
    {
      subs.Event<SupplyDropComputerLongitudeBuiMsg>(new EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLongitudeBuiMsg>(this.OnSupplyDropComputerLongitudeMsg));
      subs.Event<SupplyDropComputerLatitudeBuiMsg>(new EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLatitudeBuiMsg>(this.OnSupplyDropComputerLatitudeMsg));
      subs.Event<SupplyDropComputerUpdateBuiMsg>(new EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerUpdateBuiMsg>(this.OnSupplyDropComputerUpdateMsg));
      subs.Event<SupplyDropComputerLaunchBuiMsg>(new EntityEventRefHandler<SupplyDropComputerComponent, SupplyDropComputerLaunchBuiMsg>(this.OnSupplyDropComputerLaunchMsg));
    }));
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._supplyDropCount = 0;
    this._supplyDropMap = new MapId?();
  }

  private void OnBeingSupplyDroppedOpenAttempt(
    Entity<BeingSupplyDroppedComponent> ent,
    ref StorageOpenAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnSupplyDropComputerLongitudeMsg(
    Entity<SupplyDropComputerComponent> ent,
    ref SupplyDropComputerLongitudeBuiMsg args)
  {
    this.SetLongitude((Entity<SupplyDropComputerComponent>) ((EntityUid) ent, (SupplyDropComputerComponent) ent), args.Longitude);
  }

  private void OnSupplyDropComputerLatitudeMsg(
    Entity<SupplyDropComputerComponent> ent,
    ref SupplyDropComputerLatitudeBuiMsg args)
  {
    this.SetLatitude((Entity<SupplyDropComputerComponent>) ((EntityUid) ent, (SupplyDropComputerComponent) ent), args.Latitude);
  }

  private void OnSupplyDropComputerUpdateMsg(
    Entity<SupplyDropComputerComponent> ent,
    ref SupplyDropComputerUpdateBuiMsg args)
  {
    if (this._net.IsClient)
      return;
    this.UpdateHasCrate(ent);
  }

  private void OnSupplyDropComputerLaunchMsg(
    Entity<SupplyDropComputerComponent> ent,
    ref SupplyDropComputerLaunchBuiMsg args)
  {
    if (this._net.IsClient)
      return;
    this.TryLaunchSupplyDropPopup(ent, args.Actor);
  }

  private bool TryGetPad(
    EntProtoId<SquadTeamComponent> squad,
    out Entity<SupplyDropPadComponent> pad)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<SupplyDropPadComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SupplyDropPadComponent>();
    EntityUid uid;
    SupplyDropPadComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntProtoId<SquadTeamComponent>? squad1 = comp1.Squad;
      EntProtoId<SquadTeamComponent> entProtoId = squad;
      if ((squad1.HasValue ? (squad1.GetValueOrDefault() == entProtoId ? 1 : 0) : 0) != 0)
      {
        pad = (Entity<SupplyDropPadComponent>) (uid, comp1);
        return true;
      }
    }
    pad = new Entity<SupplyDropPadComponent>();
    return false;
  }

  public void SetSquad(
    Entity<SupplyDropComputerComponent?> computer,
    EntProtoId<SquadTeamComponent>? squad)
  {
    if (!this.Resolve<SupplyDropComputerComponent>((EntityUid) computer, ref computer.Comp, false))
      return;
    computer.Comp.Squad = squad;
    this.Dirty<SupplyDropComputerComponent>(computer);
  }

  public void SetLongitude(Entity<SupplyDropComputerComponent?> computer, int longitude)
  {
    if (!this.Resolve<SupplyDropComputerComponent>((EntityUid) computer, ref computer.Comp, false))
      return;
    longitude.Cap(computer.Comp.MaxCoordinate);
    computer.Comp.Coordinates = new Vector2i(longitude, computer.Comp.Coordinates.Y);
    this.Dirty<SupplyDropComputerComponent>(computer);
  }

  public void SetLatitude(Entity<SupplyDropComputerComponent?> computer, int latitude)
  {
    if (!this.Resolve<SupplyDropComputerComponent>((EntityUid) computer, ref computer.Comp, false))
      return;
    latitude.Cap(computer.Comp.MaxCoordinate);
    computer.Comp.Coordinates = new Vector2i(computer.Comp.Coordinates.X, latitude);
    this.Dirty<SupplyDropComputerComponent>(computer);
  }

  public bool TryFindCrate(
    Entity<SupplyDropComputerComponent> computer,
    out Entity<CanBeSupplyDroppedComponent> crate)
  {
    crate = new Entity<CanBeSupplyDroppedComponent>();
    EntProtoId<SquadTeamComponent>? squad = computer.Comp.Squad;
    Entity<SupplyDropPadComponent> pad;
    if (!squad.HasValue || !this.TryGetPad(squad.GetValueOrDefault(), out pad))
      return false;
    this._crates.Clear();
    this._entityLookup.GetEntitiesInRange<CanBeSupplyDroppedComponent>(pad.Owner.ToCoordinates(), 0.25f, this._crates);
    if (this._crates.Count == 0)
      return false;
    crate = this._crates.First<Entity<CanBeSupplyDroppedComponent>>();
    return true;
  }

  public bool TryLaunchSupplyDropPopup(Entity<SupplyDropComputerComponent> computer, EntityUid user)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < computer.Comp.NextLaunchAt)
      return false;
    EntProtoId<SquadTeamComponent>? squad = computer.Comp.Squad;
    if (squad.HasValue)
    {
      EntProtoId<SquadTeamComponent> valueOrDefault = squad.GetValueOrDefault();
      MapCoordinates mapCoordinates;
      if (this._rmcPlanet.TryPlanetToCoordinates(computer.Comp.Coordinates, out mapCoordinates) && this.CanSupplyDropSquad(valueOrDefault))
      {
        Entity<CanBeSupplyDroppedComponent> crate;
        if (!this.TryFindCrate(computer, out crate))
        {
          this._popup.PopupCursor(this.Loc.GetString("rmc-supply-drop-no-crate"), user, PopupType.MediumCaution);
          return false;
        }
        if (!this._area.CanSupplyDrop(mapCoordinates))
        {
          this._popup.PopupCursor(this.Loc.GetString("rmc-supply-drop-underground"), user, PopupType.MediumCaution);
          return false;
        }
        ContentTileDefinition def;
        if (this._rmcMap.IsTileBlocked(mapCoordinates) || this._rmcMap.TryGetTileDef(mapCoordinates, out def) && def.ID == "Space")
        {
          this._popup.PopupCursor(this.Loc.GetString("rmc-supply-drop-blocked"), user, PopupType.MediumCaution);
          return false;
        }
        SharedEntityStorageComponent component = (SharedEntityStorageComponent) null;
        if (this._entityStorage.ResolveStorage((EntityUid) crate, ref component) && component.Open)
        {
          this._popup.PopupCursor(this.Loc.GetString("rmc-supply-drop-crate-open"), user, PopupType.MediumCaution);
          return false;
        }
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) crate);
        this._popup.PopupClient(this.Loc.GetString("rmc-supply-drop-crate-load", ("crate", (object) crate)), moverCoordinates, new EntityUid?(user), PopupType.Medium);
        this._audio.PlayPredicted(crate.Comp.LaunchSound, moverCoordinates, new EntityUid?(user));
        this._marineAnnounce.AnnounceSquad(this.Loc.GetString("rmc-supply-drop-squad-announcement", ("crate", (object) crate)), valueOrDefault);
        this._rmcpulling.TryStopAllPullsFromAndOn((EntityUid) crate);
        MapId mapId = this.EnsureMap();
        this._transform.SetMapCoordinates((EntityUid) crate, new MapCoordinates((float) (this._supplyDropCount++ * 50), 0.0f, mapId));
        BeingSupplyDroppedComponent droppedComponent = this.EnsureComp<BeingSupplyDroppedComponent>((EntityUid) crate);
        droppedComponent.Target = this._transform.ToCoordinates(mapCoordinates).Offset(new Vector2(0.5f, -0.5f));
        droppedComponent.ArrivingSoundAt = curTime + crate.Comp.ArrivingSoundDelay;
        droppedComponent.DropAt = curTime + crate.Comp.DropDelay;
        droppedComponent.OpenAt = curTime + crate.Comp.OpenDelay;
        droppedComponent.LandingEffect = new EntityUid?(this.Spawn((string) crate.Comp.LandingEffectId, droppedComponent.Target));
        droppedComponent.LandingDamage = crate.Comp.LandingDamage;
        this.Dirty((EntityUid) crate, (IComponent) droppedComponent);
        computer.Comp.LastLaunchAt = curTime;
        computer.Comp.NextLaunchAt = curTime + computer.Comp.Cooldown;
        this.Dirty<SupplyDropComputerComponent>(computer);
        return true;
      }
    }
    this._popup.PopupCursor(this.Loc.GetString("rmc-supply-drop-not-operational"), user, PopupType.MediumCaution);
    return false;
  }

  private MapId EnsureMap()
  {
    if (!this._map.MapExists(this._supplyDropMap))
      this._supplyDropMap = new MapId?();
    if (!this._supplyDropMap.HasValue)
    {
      MapId mapId;
      this._map.CreateMap(out mapId);
      this._supplyDropMap = new MapId?(mapId);
    }
    return this._supplyDropMap.Value;
  }

  private void UpdateHasCrate(Entity<SupplyDropComputerComponent> ent)
  {
    int num1 = ent.Comp.HasCrate ? 1 : 0;
    ent.Comp.HasCrate = this.TryFindCrate(ent, out Entity<CanBeSupplyDroppedComponent> _);
    int num2 = ent.Comp.HasCrate ? 1 : 0;
    if (num1 == num2)
      return;
    this.Dirty<SupplyDropComputerComponent>(ent);
  }

  private bool CanSupplyDropSquad(EntProtoId<SquadTeamComponent> squad)
  {
    SquadTeamComponent comp;
    return !squad.TryGet(out comp, this._prototypes, this._compFactory) || comp.CanSupplyDrop;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SupplyDropComputerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<SupplyDropComputerComponent>();
    EntityUid uid1;
    SupplyDropComputerComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(curTime < comp1_1.NextUpdate))
      {
        comp1_1.NextUpdate = curTime + comp1_1.UpdateEvery;
        this.UpdateHasCrate((Entity<SupplyDropComputerComponent>) (uid1, comp1_1));
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<BeingSupplyDroppedComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<BeingSupplyDroppedComponent>();
    EntityUid uid2;
    BeingSupplyDroppedComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!comp1_2.PlayedArrivingSound && curTime > comp1_2.ArrivingSoundAt && comp1_2.LandingEffect.HasValue)
      {
        EntityCoordinates coordinates = comp1_2.Target;
        TransformComponent comp;
        if (this.TryComp(comp1_2.LandingEffect.Value, out comp))
          coordinates = this._transform.GetMoverCoordinates(comp1_2.LandingEffect.Value, comp);
        comp1_2.PlayedArrivingSound = true;
        this._audio.PlayPvs(comp1_2.ArrivingSound, coordinates);
        this.Dirty(uid2, (IComponent) comp1_2);
      }
      if (!(curTime < comp1_2.DropAt))
      {
        if (!comp1_2.Landed)
        {
          comp1_2.Landed = true;
          if (!this.TerminatingOrDeleted(comp1_2.LandingEffect))
          {
            this.QueueDel(comp1_2.LandingEffect);
            comp1_2.LandingEffect = new EntityUid?();
            this.Dirty(uid2, (IComponent) comp1_2);
          }
          DamageSpecifier landingDamage = comp1_2.LandingDamage;
          if (landingDamage != null)
          {
            this._intersecting.Clear();
            this._entityLookup.GetEntitiesInRange(comp1_2.Target, 0.33f, this._intersecting);
            foreach (EntityUid entityUid in this._intersecting)
              this._damageable.TryChangeDamage(new EntityUid?(entityUid), landingDamage, true);
          }
          this._transform.SetCoordinates(uid2, this._transform.GetMoverCoordinates(comp1_2.Target));
          MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(comp1_2.Target);
          foreach (ICommonSession recipient in Filter.Empty().AddInRange(mapCoordinates, 7f).Recipients)
          {
            EntityUid? attachedEntity = recipient.AttachedEntity;
            if (attachedEntity.HasValue)
              this._rmcCameraShake.ShakeCamera(attachedEntity.GetValueOrDefault(), 4, 5);
          }
        }
        if (!(curTime < comp1_2.OpenAt))
        {
          this.RemCompDeferred<BeingSupplyDroppedComponent>(uid2);
          this._audio.PlayPvs(comp1_2.OpenSound, uid2);
        }
      }
    }
  }
}
