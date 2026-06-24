// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.OrbitalCannon.OrbitalCannonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
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
  private static readonly EntProtoId OrbitalTargetMarker = (EntProtoId) "RMCLaserDropshipTarget";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<OrbitalCannonComponent, MapInitEvent>(new EntityEventRefHandler<OrbitalCannonComponent, MapInitEvent>(this.OnOrbitalCannonMapInit));
    this.SubscribeLocalEvent<OrbitalCannonComponent, PowerLoaderGrabEvent>(new EntityEventRefHandler<OrbitalCannonComponent, PowerLoaderGrabEvent>(this.OnOrbitalCannonPowerLoaderGrab));
    this.SubscribeLocalEvent<OrbitalCannonWarheadComponent, PowerLoaderInteractEvent>(new EntityEventRefHandler<OrbitalCannonWarheadComponent, PowerLoaderInteractEvent>(this.OnWarheadPowerLoaderInteract));
    this.SubscribeLocalEvent<OrbitalCannonWarheadComponent, OrbitalBombardmentFireEvent>(new EntityEventRefHandler<OrbitalCannonWarheadComponent, OrbitalBombardmentFireEvent>(this.OnWarheadOrbitalBombardmentFire));
    this.SubscribeLocalEvent<OrbitalCannonFuelComponent, PowerLoaderInteractEvent>(new EntityEventRefHandler<OrbitalCannonFuelComponent, PowerLoaderInteractEvent>(this.OnFuelPowerLoaderInteract));
    this.SubscribeLocalEvent<OrbitalCannonComputerComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<OrbitalCannonComputerComponent, BeforeActivatableUIOpenEvent>(this.OnComputerBeforeActivatableUIOpen));
    this.Subs.BuiEvents<OrbitalCannonComputerComponent>((object) OrbitalCannonComputerUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<OrbitalCannonComputerComponent>) (subs =>
    {
      subs.Event<OrbitalCannonComputerLoadBuiMsg>(new EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerLoadBuiMsg>(this.OnComputerLoad));
      subs.Event<OrbitalCannonComputerUnloadBuiMsg>(new EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerUnloadBuiMsg>(this.OnComputerUnload));
      subs.Event<OrbitalCannonComputerChamberBuiMsg>(new EntityEventRefHandler<OrbitalCannonComputerComponent, OrbitalCannonComputerChamberBuiMsg>(this.OnComputerChamber));
    }));
  }

  private void OnOrbitalCannonMapInit(Entity<OrbitalCannonComponent> ent, ref MapInitEvent args)
  {
    List<int> list = ((IEnumerable<int>) ent.Comp.PossibleFuelRequirements).ToList<int>();
    foreach (EntProtoId<OrbitalCannonWarheadComponent> warheadType in ent.Comp.WarheadTypes)
    {
      if (list.Count <= 0)
        list = ((IEnumerable<int>) ent.Comp.PossibleFuelRequirements).ToList<int>();
      if (list.Count <= 0)
      {
        this.Log.Error($"No possible fuel choice found for {warheadType}");
        return;
      }
      int Fuel = this._random.PickAndTake<int>((IList<int>) list);
      ent.Comp.FuelRequirements.Add(new WarheadFuelRequirement(warheadType, Fuel));
    }
    this.Dirty<OrbitalCannonComponent>(ent);
    this._appearance.SetData((EntityUid) ent, (Enum) OrbitalCannonVisuals.Base, (object) ent.Comp.Status);
  }

  private void OnOrbitalCannonPowerLoaderGrab(
    Entity<OrbitalCannonComponent> ent,
    ref PowerLoaderGrabEvent args)
  {
    if (args.Handled || ent.Comp.Status != OrbitalCannonStatus.Unloaded)
      return;
    BaseContainer container1;
    if (this._container.TryGetContainer((EntityUid) ent, ent.Comp.FuelContainer, out container1) && container1.ContainedEntities.Count > 0)
    {
      ref PowerLoaderGrabEvent local = ref args;
      IReadOnlyList<EntityUid> containedEntities = container1.ContainedEntities;
      EntityUid? nullable = new EntityUid?(containedEntities[containedEntities.Count - 1]);
      local.ToGrab = nullable;
      args.Handled = true;
    }
    else
    {
      BaseContainer container2;
      if (this._container.TryGetContainer((EntityUid) ent, ent.Comp.WarheadContainer, out container2) && container2.ContainedEntities.Count > 0)
      {
        ref PowerLoaderGrabEvent local = ref args;
        IReadOnlyList<EntityUid> containedEntities = container2.ContainedEntities;
        EntityUid? nullable = new EntityUid?(containedEntities[containedEntities.Count - 1]);
        local.ToGrab = nullable;
        args.Handled = true;
      }
    }
    if (!args.Handled || !this._net.IsServer)
      return;
    this._audio.PlayPvs(ent.Comp.UnloadItemSound, args.Target);
  }

  private void OnWarheadPowerLoaderInteract(
    Entity<OrbitalCannonWarheadComponent> ent,
    ref PowerLoaderInteractEvent args)
  {
    OrbitalCannonComponent comp;
    if (!this.TryComp<OrbitalCannonComponent>(args.Target, out comp))
      return;
    args.Handled = true;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>(args.Target, comp.WarheadContainer);
    if (container.ContainedEntity.HasValue)
    {
      foreach (EntityUid entityUid in args.Buckled)
        this._popup.PopupClient("There is already a warhead loaded!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
    }
    else if (comp.Status != OrbitalCannonStatus.Unloaded)
    {
      foreach (EntityUid entityUid in args.Buckled)
        this._popup.PopupClient("The cannon isn't unloaded!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
    }
    else
    {
      if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container))
      {
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient($"You can't insert {this.Name(args.Used)} into the {this.Name(args.Target)}!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
      }
      this._popup.PopupClient($"You load {this.Name(args.Used)} into the {this.Name(args.Target)}!", args.Target, new EntityUid?(args.Target), PopupType.Medium);
      this._powerLoader.TrySyncHands((Entity<PowerLoaderComponent>) args.PowerLoader);
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(comp.LoadItemSound, args.Target);
    }
  }

  private void OnWarheadOrbitalBombardmentFire(
    Entity<OrbitalCannonWarheadComponent> ent,
    ref OrbitalBombardmentFireEvent args)
  {
    EntityCoordinates coordinates1 = this._transform.ToCoordinates(args.Coordinates);
    if (this.TileHasIndestructibleWalls(coordinates1))
    {
      bool flag = false;
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
          {
            Vector2i vector2i;
            // ISSUE: explicit constructor call
            ((Vector2i) ref vector2i).\u002Ector(index1, index2);
            MapCoordinates coordinates2 = args.Coordinates.Offset(Vector2i.op_Implicit(vector2i));
            ContentTileDefinition def;
            if (this._rmcMap.TryGetTileDef(coordinates2, out def) && !(def.ID == "Space"))
            {
              EntityCoordinates coordinates3 = this._transform.ToCoordinates(coordinates2);
              if (this._area.CanOrbitalBombard(coordinates3, out bool _) && !this.TileHasIndestructibleWalls(coordinates3))
              {
                coordinates1 = coordinates3;
                this.Log.Info("Orbital bombardment impact redirected due to indestructible wall at impact site");
                flag = true;
                break;
              }
            }
          }
        }
        if (flag)
          break;
      }
      if (!flag)
      {
        this.Log.Info("Orbital bombardment impact blocked by indestructible walls, no valid alternative found");
        return;
      }
    }
    this.Spawn((string) ent.Comp.Explosion, coordinates1);
  }

  private void OnFuelPowerLoaderInteract(
    Entity<OrbitalCannonFuelComponent> ent,
    ref PowerLoaderInteractEvent args)
  {
    OrbitalCannonComponent comp;
    if (!this.TryComp<OrbitalCannonComponent>(args.Target, out comp))
      return;
    args.Handled = true;
    BaseContainer container1;
    if (!this._container.TryGetContainer(args.Target, comp.WarheadContainer, out container1) || container1.ContainedEntities.Count == 0)
    {
      foreach (EntityUid entityUid in args.Buckled)
        this._popup.PopupClient($"A warhead must be placed in the {this.Name(args.Target)} first.", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
    }
    else if (comp.Status != OrbitalCannonStatus.Unloaded)
    {
      foreach (EntityUid entityUid in args.Buckled)
        this._popup.PopupClient($"The {this.Name(args.Target)} isn't unloaded!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
    }
    else
    {
      Container container2 = this._container.EnsureContainer<Container>(args.Target, comp.FuelContainer);
      if (container2.ContainedEntities.Count >= comp.MaxFuel)
      {
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient($"The {this.Name(args.Target)} can't accept more solid fuel!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
      }
      else if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container2))
      {
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient($"You can't insert {this.Name(args.Used)} into the {this.Name(args.Target)}!", args.Target, new EntityUid?(entityUid), PopupType.MediumCaution);
      }
      else
      {
        this._popup.PopupClient($"You load {this.Name(args.Used)} into the {this.Name(args.Target)}!", args.Target, new EntityUid?(args.Target), PopupType.Medium);
        this._powerLoader.TrySyncHands((Entity<PowerLoaderComponent>) args.PowerLoader);
        if (!this._net.IsServer)
          return;
        this._audio.PlayPvs(comp.LoadItemSound, args.Target);
      }
    }
  }

  private void OnComputerBeforeActivatableUIOpen(
    Entity<OrbitalCannonComputerComponent> ent,
    ref BeforeActivatableUIOpenEvent args)
  {
    Entity<OrbitalCannonComponent> cannon;
    if (!this.TryGetClosestCannon((EntityUid) ent, out cannon))
      return;
    BaseContainer container1;
    ent.Comp.Warhead = !this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.WarheadContainer, out container1) || container1.ContainedEntities.Count <= 0 ? (string) null : this.Name(container1.ContainedEntities[0]);
    BaseContainer container2;
    ent.Comp.Fuel = this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.FuelContainer, out container2) ? container2.ContainedEntities.Count : 0;
    ent.Comp.FuelRequirements = cannon.Comp.FuelRequirements;
    ent.Comp.Status = cannon.Comp.Status;
    this.Dirty<OrbitalCannonComputerComponent>(ent);
  }

  private void OnComputerLoad(
    Entity<OrbitalCannonComputerComponent> ent,
    ref OrbitalCannonComputerLoadBuiMsg args)
  {
    Entity<OrbitalCannonComponent> cannon;
    if (!this.TryGetClosestCannon((EntityUid) ent, out cannon) || cannon.Comp.Status != OrbitalCannonStatus.Unloaded || !this.CannonHasWarhead(cannon) || this.CannonGetFuel(cannon) <= 0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown)
      return;
    cannon.Comp.LastToggledAt = curTime;
    cannon.Comp.Status = OrbitalCannonStatus.Loaded;
    this.Dirty<OrbitalCannonComponent>(cannon);
    ent.Comp.Status = cannon.Comp.Status;
    this.Dirty<OrbitalCannonComputerComponent>(ent);
    if (this._net.IsServer)
      this._audio.PlayPvs(cannon.Comp.LoadSound, (EntityUid) cannon);
    this.CannonStatusChanged(cannon);
  }

  private void OnComputerUnload(
    Entity<OrbitalCannonComputerComponent> ent,
    ref OrbitalCannonComputerUnloadBuiMsg args)
  {
    Entity<OrbitalCannonComponent> cannon;
    if (!this.TryGetClosestCannon((EntityUid) ent, out cannon) || cannon.Comp.Status != OrbitalCannonStatus.Loaded)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown)
      return;
    cannon.Comp.LastToggledAt = curTime;
    cannon.Comp.Status = OrbitalCannonStatus.Unloaded;
    this.Dirty<OrbitalCannonComponent>(cannon);
    ent.Comp.Status = cannon.Comp.Status;
    this.Dirty<OrbitalCannonComputerComponent>(ent);
    if (this._net.IsServer)
      this._audio.PlayPvs(cannon.Comp.UnloadSound, (EntityUid) cannon);
    this.CannonStatusChanged(cannon);
  }

  private void OnComputerChamber(
    Entity<OrbitalCannonComputerComponent> ent,
    ref OrbitalCannonComputerChamberBuiMsg args)
  {
    Entity<OrbitalCannonComponent> cannon;
    if (!this.TryGetClosestCannon((EntityUid) ent, out cannon) || cannon.Comp.Status != OrbitalCannonStatus.Loaded || !this.CannonHasWarhead(cannon) || this.CannonGetFuel(cannon) <= 0)
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < cannon.Comp.LastToggledAt + cannon.Comp.ToggleCooldown)
      return;
    cannon.Comp.LastToggledAt = curTime;
    cannon.Comp.Status = OrbitalCannonStatus.Chambered;
    this.Dirty<OrbitalCannonComponent>(cannon);
    ent.Comp.Status = cannon.Comp.Status;
    this.Dirty<OrbitalCannonComputerComponent>(ent);
    if (this._net.IsServer)
      this._audio.PlayPvs(cannon.Comp.ChamberSound, (EntityUid) cannon);
    this.CannonStatusChanged(cannon);
  }

  public bool TryGetClosestCannon(EntityUid to, out Entity<OrbitalCannonComponent> cannon)
  {
    cannon = new Entity<OrbitalCannonComponent>();
    TransformComponent comp;
    if (!this.TryComp(to, out comp))
      return false;
    float num = float.MaxValue;
    Robust.Shared.GameObjects.EntityQueryEnumerator<OrbitalCannonComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OrbitalCannonComponent, TransformComponent>();
    EntityUid uid;
    OrbitalCannonComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      float distance;
      if (comp.Coordinates.TryDistance((IEntityManager) this.EntityManager, this._transform, comp2.Coordinates, out distance) && (double) distance < (double) num)
      {
        num = distance;
        cannon = (Entity<OrbitalCannonComponent>) (uid, comp1);
      }
    }
    return cannon != new Entity<OrbitalCannonComponent>();
  }

  private bool CannonHasWarhead(Entity<OrbitalCannonComponent> cannon, out EntityUid warhead)
  {
    BaseContainer container;
    if (this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.WarheadContainer, out container) && container.ContainedEntities.Count > 0 && !this.EntityManager.IsQueuedForDeletion(container.ContainedEntities[0]))
    {
      warhead = container.ContainedEntities[0];
      return true;
    }
    warhead = new EntityUid();
    return false;
  }

  private bool CannonHasWarhead(Entity<OrbitalCannonComponent> cannon)
  {
    return this.CannonHasWarhead(cannon, out EntityUid _);
  }

  private int CannonGetFuel(Entity<OrbitalCannonComponent> cannon)
  {
    BaseContainer container;
    return !this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.FuelContainer, out container) ? 0 : container.ContainedEntities.Count;
  }

  private void CannonStatusChanged(Entity<OrbitalCannonComponent> cannon)
  {
    this._appearance.SetData((EntityUid) cannon, (Enum) OrbitalCannonVisuals.Base, (object) cannon.Comp.Status);
    OrbitalCannonChangedEvent args = new OrbitalCannonChangedEvent(cannon, this.CannonHasWarhead(cannon), this.CannonGetFuel(cannon));
    this.RaiseLocalEvent<OrbitalCannonChangedEvent>((EntityUid) cannon, ref args, true);
  }

  private bool TileHasIndestructibleWalls(EntityCoordinates coordinates)
  {
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(coordinates, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (this.HasComp<TagComponent>(uid) && this._tags.HasTag(uid, (ProtoId<TagPrototype>) "Wall") && !this.HasComp<DamageableComponent>(uid))
        return true;
    }
    return false;
  }

  public bool Fire(
    Entity<OrbitalCannonComponent> cannon,
    Vector2i fireCoordinates,
    EntityUid user,
    EntityUid squad)
  {
    if (this._net.IsClient || cannon.Comp.Status != OrbitalCannonStatus.Chambered)
      return false;
    TimeSpan curTime = this._timing.CurTime;
    if (cannon.Comp.LastFireAt.HasValue)
    {
      TimeSpan timeSpan = curTime;
      TimeSpan? lastFireAt = cannon.Comp.LastFireAt;
      TimeSpan fireCooldown = cannon.Comp.FireCooldown;
      TimeSpan? nullable = lastFireAt.HasValue ? new TimeSpan?(lastFireAt.GetValueOrDefault() + fireCooldown) : new TimeSpan?();
      if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        return false;
    }
    BaseContainer container1;
    if (!this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.WarheadContainer, out container1) || container1.ContainedEntities.Count == 0)
    {
      this._popup.PopupCursor("The orbital cannon has no ammo chambered.", user, PopupType.LargeCaution);
      return false;
    }
    MapCoordinates mapCoordinates;
    if (!this._rmcPlanet.TryPlanetToCoordinates(fireCoordinates, out mapCoordinates))
    {
      this._popup.PopupCursor("The target zone appears to be out of bounds. Please check coordinates.", user, PopupType.LargeCaution);
      return false;
    }
    ContentTileDefinition def;
    if (!this._rmcMap.TryGetTileDef(mapCoordinates, out def) || def.ID == "Space")
    {
      this._popup.PopupCursor("The target zone appears to be out of bounds. Please check coordinates.", user, PopupType.LargeCaution);
      return false;
    }
    bool roofed;
    if (!this._area.CanOrbitalBombard(this._transform.ToCoordinates(mapCoordinates), out roofed))
    {
      if (roofed)
      {
        this._popup.PopupCursor("The target zone has strong biological protection. The orbital strike cannot reach here.", user, PopupType.LargeCaution);
        return false;
      }
      this._popup.PopupCursor("The target zone is deep underground. The orbital strike cannot reach here.", user, PopupType.LargeCaution);
      return false;
    }
    this._popup.PopupCursor("Orbital bombardment request accepted. Orbital cannons are now calibrating.", PopupType.Large);
    EntityUid containedEntity = container1.ContainedEntities[0];
    int num1 = 0;
    BaseContainer container2;
    if (this._container.TryGetContainer((EntityUid) cannon, cannon.Comp.FuelContainer, out container2))
    {
      int count = container2.ContainedEntities.Count;
      string warheadProto = this.Prototype(containedEntity)?.ID;
      WarheadFuelRequirement? element;
      if (cannon.Comp.FuelRequirements.TryFirstOrNull<WarheadFuelRequirement>((Func<WarheadFuelRequirement, bool>) (f => f.Warhead.Id == warheadProto), out element))
        num1 = Math.Abs(count - element.Value.Fuel);
    }
    int num2 = num1 + 1;
    int num3 = num2 * this._random.Next(-3, 3);
    int num4 = num2 * this._random.Next(-3, 3);
    Vector2i vector2i = Vector2i.op_Addition(fireCoordinates, new Vector2i(num3, num4));
    OrbitalCannonFiringComponent cannonFiringComponent = this.EnsureComp<OrbitalCannonFiringComponent>((EntityUid) cannon);
    cannonFiringComponent.Coordinates = vector2i;
    cannonFiringComponent.WarheadName = this.Name(containedEntity);
    cannonFiringComponent.Squad = squad;
    cannonFiringComponent.StartedAt = curTime;
    OrbitalCannonWarheadComponent comp;
    if (this.TryComp<OrbitalCannonWarheadComponent>(containedEntity, out comp))
    {
      cannonFiringComponent.FirstWarningRange = comp.FirstWarningRange;
      cannonFiringComponent.SecondWarningRange = comp.SecondWarningRange;
      cannonFiringComponent.ThirdWarningRange = comp.ThirdWarningRange;
      if (comp.IntelPointsAwarded > 0 && this._net.IsServer)
        this._intel.AddPoints(comp.IntelPointsAwarded);
    }
    this.Dirty((EntityUid) cannon, (IComponent) cannonFiringComponent);
    this._popup.PopupCursor("Orbital bombardment launched!", user);
    string str = $"{this.ToPrettyString((Entity<MetaDataComponent>) user)} launched orbital bombardment at {fireCoordinates} for squad {this.ToPrettyString((Entity<MetaDataComponent>) squad)}, misfuel: {num1}, final coords: {vector2i}";
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(0, 1);
    logStringHandler.AppendFormatted(str);
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCOrbitalBombardment, ref local);
    OrbitalCannonLaunchEvent message = new OrbitalCannonLaunchEvent(cannon.Comp.FireCooldown + cannonFiringComponent.ImpactDelay);
    this.RaiseLocalEvent<OrbitalCannonLaunchEvent>(ref message);
    return true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<OrbitalCannonFiringComponent, OrbitalCannonComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<OrbitalCannonFiringComponent, OrbitalCannonComponent>();
    EntityUid uid1;
    OrbitalCannonFiringComponent comp1_1;
    OrbitalCannonComponent comp2;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2))
    {
      MapCoordinates mapCoordinates;
      if (!this._rmcPlanet.TryPlanetToCoordinates(comp1_1.Coordinates, out mapCoordinates))
      {
        this.RemCompDeferred<OrbitalCannonFiringComponent>(uid1);
      }
      else
      {
        if (!comp1_1.Alerted && curTime > comp1_1.StartedAt + comp1_1.AlertDelay)
        {
          comp1_1.Alerted = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          Filter filter = Filter.BroadcastMap(mapCoordinates.MapId).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => !this.HasComp<SquadMemberComponent>(e) && !this.HasComp<GhostComponent>(e)));
          this._audio.PlayGlobal(comp2.GroundAlertSound, filter, true);
          string str1 = "[font size=16][color=red]Orbital bombardment launch command detected![/color][/font]";
          this._rmcChat.ChatMessageToMany(str1, str1, filter, ChatChannel.Radio);
          Robust.Shared.Prototypes.EntityPrototype areaPrototype;
          if (this._area.TryGetArea(mapCoordinates, out Entity<AreaComponent>? _, out areaPrototype))
          {
            string str2 = $"[color=red]Launch command informs {comp1_1.WarheadName}. Estimated impact area: {areaPrototype.Name}[/color]";
            this._rmcChat.ChatMessageToMany(str2, str2, filter, ChatChannel.Radio);
          }
        }
        if (!comp1_1.BegunFire && curTime > comp1_1.StartedAt + comp1_1.BeginFireDelay)
        {
          comp1_1.BegunFire = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          Filter filter = Filter.BroadcastMap(this._transform.GetMapId((Entity<TransformComponent>) uid1));
          this._rmcCameraShake.ShakeCamera(filter, 10, 1);
          string str = "[color=red]The deck of the UNS Almayer shudders as the orbital cannons open fire on the colony.[/color]";
          this._rmcChat.ChatMessageToMany(str, str, filter, ChatChannel.Radio);
          this._marineAnnounce.AnnounceSquad("WARNING! Ballistic trans-atmospheric launch detected! Get outside of Danger Close!", comp1_1.Squad);
        }
        if (!comp1_1.Fired && curTime > comp1_1.StartedAt + comp1_1.FireDelay)
        {
          comp1_1.Fired = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          this._audio.PlayPvs(comp2.FireSound, uid1);
          EntityCoordinates coordinates = this._transform.ToCoordinates(mapCoordinates);
          this._audio.PlayPvs(comp2.TravelSound, coordinates, new AudioParams?(AudioParams.Default.WithMaxDistance(75f)));
          this._mortar.PopupWarning(mapCoordinates, (float) comp1_1.FirstWarningRange, (LocId) "rmc-ob-warning-one", (LocId) "rmc-ob-warning-above-one", true);
        }
        if (!comp1_1.WarnedOne && curTime > comp1_1.StartedAt + comp1_1.WarnOneDelay)
        {
          comp1_1.WarnedOne = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          this._mortar.PopupWarning(mapCoordinates, (float) comp1_1.SecondWarningRange, (LocId) "rmc-ob-warning-two", (LocId) "rmc-ob-warning-above-two", true);
        }
        if (!comp1_1.WarnedTwo && curTime > comp1_1.StartedAt + comp1_1.WarnTwoDelay)
        {
          comp1_1.WarnedTwo = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          this._mortar.PopupWarning(mapCoordinates, (float) comp1_1.ThirdWarningRange, (LocId) "rmc-ob-warning-three", (LocId) "rmc-ob-warning-above-three", true);
        }
        if (!comp1_1.AegisBoomed && curTime > comp1_1.StartedAt + comp1_1.AegisBoomDelay)
        {
          comp1_1.AegisBoomed = true;
          this.Dirty(uid1, (IComponent) comp1_1);
          EntityUid warhead;
          OrbitalCannonWarheadComponent comp;
          if (this.CannonHasWarhead((Entity<OrbitalCannonComponent>) (uid1, comp2), out warhead) && this.TryComp<OrbitalCannonWarheadComponent>(warhead, out comp) && comp.IsAegis)
          {
            EntityCoordinates coordinates = this._transform.ToCoordinates(mapCoordinates);
            (EntityUid Entity, AudioComponent Component)? nullable = this._audio.PlayPvs(comp2.AegisBoomSound, coordinates, new AudioParams?(AudioParams.Default.WithMaxDistance(300f)));
            if (nullable.HasValue)
              this._rmcPvs.AddGlobalOverride(nullable.Value.Entity);
          }
        }
        if (!comp1_1.Impacted && curTime > comp1_1.StartedAt + comp1_1.ImpactDelay)
        {
          comp1_1.Impacted = true;
          comp2.Status = OrbitalCannonStatus.Unloaded;
          comp2.LastFireAt = new TimeSpan?(curTime);
          this.Dirty(uid1, (IComponent) comp2);
          Entity<OrbitalCannonComponent> entity = new Entity<OrbitalCannonComponent>(uid1, comp2);
          int fuel = this.CannonGetFuel(entity);
          EntityUid warhead;
          if (this.CannonHasWarhead(entity, out warhead))
          {
            OrbitalBombardmentFireEvent args = new OrbitalBombardmentFireEvent(entity, warhead, fuel, mapCoordinates);
            this.RaiseLocalEvent<OrbitalBombardmentFireEvent>(warhead, ref args);
          }
          this.CannonStatusChanged(entity);
          this.RemCompDeferred<OrbitalCannonFiringComponent>(uid1);
          BaseContainer container1;
          if (this._container.TryGetContainer(uid1, comp2.FuelContainer, out container1))
            this._container.CleanContainer(container1);
          BaseContainer container2;
          if (this._container.TryGetContainer(uid1, comp2.WarheadContainer, out container2))
            this._container.CleanContainer(container2);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<OrbitalCannonExplosionComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<OrbitalCannonExplosionComponent>();
    EntityUid uid2;
    OrbitalCannonExplosionComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!comp1_2.Laser)
      {
        comp1_2.Laser = true;
        this.Spawn((string) OrbitalCannonSystem.OrbitalTargetMarker, this._transform.GetMapCoordinates(uid2), rotation: new Angle());
      }
      if (comp1_2.Current == 0 && comp1_2.LastAt == new TimeSpan())
      {
        comp1_2.LastAt = curTime;
        this.Dirty(uid2, (IComponent) comp1_2);
      }
      if (comp1_2.Current >= comp1_2.Steps.Count)
      {
        this.QueueDel(new EntityUid?(uid2));
      }
      else
      {
        OrbitalCannonExplosion step = comp1_2.Steps[comp1_2.Current];
        if (curTime >= comp1_2.LastAt + step.Delay)
        {
          if (step.Times <= 1)
          {
            ++comp1_2.Current;
            this.Dirty(uid2, (IComponent) comp1_2);
          }
          else if (!(curTime < comp1_2.LastStepAt + step.DelayPer))
          {
            ++comp1_2.Step;
            comp1_2.LastStepAt = curTime;
            if (comp1_2.Step >= step.Times)
            {
              ++comp1_2.Current;
              comp1_2.Step = 0;
              comp1_2.LastStepAt = new TimeSpan();
              this.Dirty(uid2, (IComponent) comp1_2);
            }
          }
          else
            continue;
          for (int index = 0; index < step.TimesPer; ++index)
          {
            MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid2);
            EntityCoordinates entityCoordinates = this._transform.GetMoverCoordinates(uid2);
            if (step.Spread > 0)
            {
              Vector2 vector2 = this._random.NextVector2((float) -step.Spread, (float) step.Spread);
              mapCoordinates = mapCoordinates.Offset(vector2);
              entityCoordinates = entityCoordinates.Offset(vector2);
            }
            if (!step.CheckProtectionPer || this._area.CanOrbitalBombard(entityCoordinates, out bool _))
            {
              if (step.ExplosionEffect.HasValue)
                this._rmcExplosion.TryDoEffect((Entity<CMExplosionEffectComponent>) this.Spawn((string) step.ExplosionEffect.Value, mapCoordinates, rotation: new Angle()));
              ProtoId<ExplosionPrototype>? type = step.Type;
              if (type.HasValue)
              {
                ProtoId<ExplosionPrototype> valueOrDefault = type.GetValueOrDefault();
                this._rmcExplosion.QueueExplosion(mapCoordinates, (string) valueOrDefault, step.Total, step.Slope, step.Max, new EntityUid?(uid2), canCreateVacuum: false);
              }
              EntProtoId? fire = step.Fire;
              if (fire.HasValue)
              {
                EntProtoId valueOrDefault = fire.GetValueOrDefault();
                if (step.FireRange > 0)
                  this._rmcFlammable.SpawnFireDiamond(valueOrDefault, entityCoordinates, step.FireRange);
              }
            }
          }
        }
      }
    }
  }
}
