// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Teleporter;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Buckle.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
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
    this.SubscribeLocalEvent<VehicleEnterComponent, ActivateInWorldEvent>(new EntityEventRefHandler<VehicleEnterComponent, ActivateInWorldEvent>(this.OnVehicleEnterActivate));
    this.SubscribeLocalEvent<VehicleEnterComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleEnterComponent, ComponentShutdown>(this.OnVehicleEnterShutdown));
    this.SubscribeLocalEvent<VehicleExitComponent, ActivateInWorldEvent>(new EntityEventRefHandler<VehicleExitComponent, ActivateInWorldEvent>(this.OnVehicleExitActivate));
    this.SubscribeLocalEvent<VehicleEnterComponent, VehicleEnterDoAfterEvent>(new EntityEventRefHandler<VehicleEnterComponent, VehicleEnterDoAfterEvent>(this.OnVehicleEnterDoAfter));
    this.SubscribeLocalEvent<VehicleExitComponent, VehicleExitDoAfterEvent>(new EntityEventRefHandler<VehicleExitComponent, VehicleExitDoAfterEvent>(this.OnVehicleExitDoAfter));
    this.SubscribeLocalEvent<VehicleDriverSeatComponent, StrapAttemptEvent>(new EntityEventRefHandler<VehicleDriverSeatComponent, StrapAttemptEvent>(this.OnDriverSeatStrapAttempt));
    this.SubscribeLocalEvent<VehicleDriverSeatComponent, StrappedEvent>(new EntityEventRefHandler<VehicleDriverSeatComponent, StrappedEvent>(this.OnDriverSeatStrapped));
    this.SubscribeLocalEvent<VehicleDriverSeatComponent, UnstrappedEvent>(new EntityEventRefHandler<VehicleDriverSeatComponent, UnstrappedEvent>(this.OnDriverSeatUnstrapped));
    this.SubscribeLocalEvent<VehicleOperatorComponent, OnVehicleEnteredEvent>(new EntityEventRefHandler<VehicleOperatorComponent, OnVehicleEnteredEvent>(this.OnVehicleOperatorEntered));
    this.SubscribeLocalEvent<VehicleOperatorComponent, OnVehicleExitedEvent>(new EntityEventRefHandler<VehicleOperatorComponent, OnVehicleExitedEvent>(this.OnVehicleOperatorExited));
    this.SubscribeLocalEvent<VehicleInteriorOccupantComponent, ComponentStartup>(new EntityEventRefHandler<VehicleInteriorOccupantComponent, ComponentStartup>(this.OnOccupantStartup));
    this.SubscribeLocalEvent<VehicleInteriorOccupantComponent, ComponentRemove>(new EntityEventRefHandler<VehicleInteriorOccupantComponent, ComponentRemove>(this.OnOccupantRemove));
    this.SubscribeLocalEvent<VehicleInteriorOccupantComponent, MapUidChangedEvent>(new EntityEventRefHandler<VehicleInteriorOccupantComponent, MapUidChangedEvent>(this.OnOccupantMapChanged));
    this.SubscribeLocalEvent<VehicleInteriorOccupantComponent, MetaFlagRemoveAttemptEvent>(new EntityEventRefHandler<VehicleInteriorOccupantComponent, MetaFlagRemoveAttemptEvent>(this.OnOccupantMetaFlagRemoveAttempt));
    this.SubscribeLocalEvent<HardpointIntegrityComponent, VehicleCanRunEvent>(new EntityEventRefHandler<HardpointIntegrityComponent, VehicleCanRunEvent>(this.OnFrameVehicleCanRun));
    this.SubscribeLocalEvent<RMCConstructionAttemptEvent>(new EntityEventRefHandler<RMCConstructionAttemptEvent>(this.OnConstructionAttempt));
  }

  private void OnVehicleEnterActivate(
    Entity<VehicleEnterComponent> ent,
    ref ActivateInWorldEvent args)
  {
    if (this._net.IsClient)
      return;
    if (this.IsEntryBlockedByLock(ent.Owner, args.User))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-locked"), args.User, args.User, PopupType.SmallCaution);
      args.Handled = true;
    }
    else
    {
      if (args.Handled)
        return;
      int entryIndex;
      if (!this.TryFindEntry(ent, args.User, out entryIndex))
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-use-doorway"), args.User, args.User);
      }
      else
      {
        VehicleInteriorComponent interiorComponent = this.EnsureComp<VehicleInteriorComponent>(ent.Owner);
        if (!interiorComponent.EntryLocks.Add(entryIndex))
        {
          this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-busy"), args.User, args.User);
        }
        else
        {
          EntityManager entityManager = this.EntityManager;
          EntityUid user = args.User;
          double enterDoAfter = (double) ent.Comp.EnterDoAfter;
          VehicleEnterDoAfterEvent @event = new VehicleEnterDoAfterEvent();
          @event.EntryIndex = entryIndex;
          EntityUid? eventTarget = new EntityUid?(ent.Owner);
          EntityUid? target = new EntityUid?();
          EntityUid? used = new EntityUid?();
          if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, (float) enterDoAfter, (DoAfterEvent) @event, eventTarget, target, used)
          {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = false
          }))
            interiorComponent.EntryLocks.Remove(entryIndex);
          else
            args.Handled = true;
        }
      }
    }
  }

  private bool TryEnter(Entity<VehicleEnterComponent> ent, EntityUid user, int entryIndex = -1)
  {
    if (this.IsEntryBlockedByLock(ent.Owner, user))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-locked"), user, user, PopupType.SmallCaution);
      return false;
    }
    VehicleInteriorComponent interior;
    if (!this.EnsureInterior(ent, out interior))
      return false;
    this.PruneTrackedOccupants(ent.Owner, interior);
    bool isXeno = this.HasComp<XenoComponent>(user);
    if (isXeno)
    {
      if (ent.Comp.MaxXenos > 0 && !interior.Xenos.Contains(user) && this.CountLivingOccupants(interior.Xenos) >= ent.Comp.MaxXenos)
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-xeno-full"), user, user);
        return false;
      }
    }
    else if (ent.Comp.MaxPassengers > 0 && !interior.Passengers.Contains(user) && this.CountLivingOccupants(interior.Passengers) >= ent.Comp.MaxPassengers)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-passenger-full"), user, user);
      return false;
    }
    EntityCoordinates entry = interior.Entry;
    if (entryIndex >= 0 && entryIndex < ent.Comp.EntryPoints.Count)
    {
      Vector2? interiorCoords = ent.Comp.EntryPoints[entryIndex].InteriorCoords;
      if (interiorCoords.HasValue)
      {
        Vector2 valueOrDefault = interiorCoords.GetValueOrDefault();
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(new EntityCoordinates(interior.Grid.IsValid() ? interior.Grid : interior.EntryParent, valueOrDefault));
        this._rmcTeleporter.HandlePulling(user, mapCoordinates);
        this.TrackOccupant(user, ent.Owner, isXeno);
        return true;
      }
    }
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(entry);
    this._rmcTeleporter.HandlePulling(user, mapCoordinates1);
    this.TrackOccupant(user, ent.Owner, isXeno);
    return true;
  }

  private bool EnsureInterior(
    Entity<VehicleEnterComponent> ent,
    [NotNullWhen(true)] out VehicleInteriorComponent? interior)
  {
    if (this.TryComp<VehicleInteriorComponent>(ent.Owner, out interior) && interior.MapId != MapId.Nullspace && this._mapManager.MapExists(new MapId?(interior.MapId)))
      return true;
    interior = (VehicleInteriorComponent) null;
    if (this._net.IsClient)
      return false;
    interior = this.EnsureComp<VehicleInteriorComponent>(ent.Owner);
    DeserializationOptions deserializationOptions = new DeserializationOptions()
    {
      InitializeMaps = true
    };
    Entity<MapComponent>? map;
    if (!this._mapLoader.TryLoadMap(ent.Comp.InteriorPath, out map, out HashSet<Entity<MapGridComponent>> _, new DeserializationOptions?(deserializationOptions), rot: new Angle()))
    {
      this.Log.Error($"[VehicleEnter] Failed to load interior for {this.ToPrettyString((Entity<MetaDataComponent>) ent.Owner)} at {ent.Comp.InteriorPath}");
      return false;
    }
    if (!map.HasValue)
      return false;
    Entity<MapComponent> valueOrDefault = map.GetValueOrDefault();
    MapId mapId = valueOrDefault.Comp.MapId;
    EntityUid owner = valueOrDefault.Owner;
    EntityUid entityId = valueOrDefault.Owner;
    EntityUid entityUid = EntityUid.Invalid;
    Robust.Shared.GameObjects.EntityQueryEnumerator<MapGridComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
    EntityUid uid;
    TransformComponent comp2_1;
    while (entityQueryEnumerator1.MoveNext(out uid, out MapGridComponent _, out comp2_1))
    {
      if (!(comp2_1.MapID != mapId))
      {
        entityId = uid;
        entityUid = uid;
        break;
      }
    }
    EntityCoordinates entityCoordinates = new EntityCoordinates(entityId, Vector2.Zero);
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleExitComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<VehicleExitComponent, TransformComponent>();
    TransformComponent comp2_2;
    while (entityQueryEnumerator2.MoveNext(out EntityUid _, out VehicleExitComponent _, out comp2_2))
    {
      if (!(comp2_2.MapID != mapId))
      {
        entityCoordinates = comp2_2.Coordinates;
        entityId = comp2_2.ParentUid.IsValid() ? comp2_2.ParentUid : entityId;
        break;
      }
    }
    interior.Map = owner;
    interior.MapId = mapId;
    interior.Entry = entityCoordinates;
    interior.EntryParent = entityId;
    interior.Grid = entityUid;
    interior.Passengers.Clear();
    interior.Xenos.Clear();
    this.EnsureComp<VehicleInteriorLinkComponent>(owner).Vehicle = ent.Owner;
    return true;
  }

  private void OnVehicleEnterShutdown(Entity<VehicleEnterComponent> ent, ref ComponentShutdown args)
  {
    this.CleanupInterior(ent.Owner);
  }

  private void CleanupInterior(EntityUid vehicle)
  {
    VehicleInteriorComponent comp1;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out comp1))
      return;
    foreach (EntityUid uid in new List<EntityUid>((IEnumerable<EntityUid>) comp1.Passengers))
    {
      VehicleInteriorOccupantComponent comp2;
      if (this.TryComp<VehicleInteriorOccupantComponent>(uid, out comp2) && comp2.Vehicle == vehicle)
        this.RemComp<VehicleInteriorOccupantComponent>(uid);
    }
    foreach (EntityUid uid in new List<EntityUid>((IEnumerable<EntityUid>) comp1.Xenos))
    {
      VehicleInteriorOccupantComponent comp3;
      if (this.TryComp<VehicleInteriorOccupantComponent>(uid, out comp3) && comp3.Vehicle == vehicle)
        this.RemComp<VehicleInteriorOccupantComponent>(uid);
    }
    VehicleInteriorLinkComponent comp4;
    if (comp1.Map.IsValid() && this.EntityManager.EntityExists(comp1.Map) && this.TryComp<VehicleInteriorLinkComponent>(comp1.Map, out comp4) && comp4.Vehicle == vehicle)
      this.RemComp<VehicleInteriorLinkComponent>(comp1.Map);
    this.RemComp<VehicleInteriorComponent>(vehicle);
    if (this._net.IsClient)
      return;
    if (comp1.MapId != MapId.Nullspace && this._mapManager.MapExists(new MapId?(comp1.MapId)))
    {
      this._mapManager.DeleteMap(comp1.MapId);
    }
    else
    {
      if (!comp1.Map.IsValid() || !this.EntityManager.EntityExists(comp1.Map))
        return;
      this.Del(new EntityUid?(comp1.Map));
    }
  }

  private void OnVehicleExitActivate(
    Entity<VehicleExitComponent> ent,
    ref ActivateInWorldEvent args)
  {
    if (this._net.IsClient || args.Handled)
      return;
    if (ent.Comp.PendingExit)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-exit-busy"), args.User, args.User);
    }
    else
    {
      TransformComponent comp1;
      EntityUid? vehicle;
      if (!this.TryComp((EntityUid) ent, out comp1) || comp1.MapID == MapId.Nullspace || !this.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
        return;
      EntityUid valueOrDefault = vehicle.GetValueOrDefault();
      VehicleEnterComponent comp2;
      if (!this.TryComp<VehicleEnterComponent>(valueOrDefault, out comp2))
        return;
      if (this.IsExitBlockedByLock(valueOrDefault, args.User))
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-locked"), args.User, args.User, PopupType.SmallCaution);
        args.Handled = true;
      }
      else
      {
        ent.Comp.PendingExit = true;
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp2.ExitDoAfter, (DoAfterEvent) new VehicleExitDoAfterEvent(), new EntityUid?(ent.Owner))
        {
          BreakOnMove = true
        }))
          ent.Comp.PendingExit = false;
        else
          args.Handled = true;
      }
    }
  }

  private bool TryFindEntry(Entity<VehicleEnterComponent> ent, EntityUid user, out int entryIndex)
  {
    entryIndex = -1;
    if (ent.Comp.EntryPoints.Count == 0)
      return true;
    HardpointIntegrityComponent comp;
    bool flag = this.TryComp<HardpointIntegrityComponent>(ent.Owner, out comp) && comp.BypassEntryOnZero && (double) comp.Integrity <= 0.0;
    TransformComponent component1 = this.Transform(ent.Owner);
    TransformComponent component2 = this.Transform(user);
    if (component1.MapID != component2.MapID || component1.MapID == MapId.Nullspace)
      return false;
    Vector2 worldPosition = this._transform.GetWorldPosition(component1);
    Vector2 vector2_1 = this._transform.GetWorldPosition(component2) - worldPosition;
    Angle angle = Angle.op_UnaryNegation(component1.LocalRotation);
    Vector2 vector2_2 = ((Angle) ref angle).RotateVec(ref vector2_1);
    if (flag)
    {
      float num1 = float.MaxValue;
      int num2 = -1;
      for (int index = 0; index < ent.Comp.EntryPoints.Count; ++index)
      {
        VehicleEntryPoint entryPoint = ent.Comp.EntryPoints[index];
        float num3 = (vector2_2 - entryPoint.Offset).LengthSquared();
        if ((double) num3 < (double) num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
      if (num2 < 0)
        return false;
      entryIndex = num2;
      return true;
    }
    for (int index = 0; index < ent.Comp.EntryPoints.Count; ++index)
    {
      VehicleEntryPoint entryPoint = ent.Comp.EntryPoints[index];
      if ((double) (vector2_2 - entryPoint.Offset).Length() <= (double) entryPoint.Radius)
      {
        entryIndex = index;
        return true;
      }
    }
    return false;
  }

  private void OnVehicleEnterDoAfter(
    Entity<VehicleEnterComponent> ent,
    ref VehicleEnterDoAfterEvent args)
  {
    VehicleInteriorComponent comp;
    if (this.TryComp<VehicleInteriorComponent>(ent.Owner, out comp))
      comp.EntryLocks.Remove(args.EntryIndex);
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = this.TryEnter(ent, args.User, args.EntryIndex);
  }

  private bool TryExit(Entity<VehicleExitComponent> ent, EntityUid user)
  {
    TransformComponent comp1;
    EntityUid? vehicle;
    if (!this.TryComp((EntityUid) ent, out comp1) || comp1.MapID == MapId.Nullspace || !this.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      return false;
    EntityUid valueOrDefault = vehicle.GetValueOrDefault();
    VehicleEnterComponent comp2;
    if (!this.TryComp<VehicleEnterComponent>(valueOrDefault, out comp2))
      return false;
    if (this.IsExitBlockedByLock(valueOrDefault, user))
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-vehicle-enter-locked"), user, user, PopupType.SmallCaution);
      return false;
    }
    TransformComponent transformComponent = this.Transform(valueOrDefault);
    EntityUid? nullable = new EntityUid?(transformComponent.ParentUid);
    if (!nullable.HasValue || !nullable.Value.IsValid())
      nullable = transformComponent.MapUid;
    if (!nullable.HasValue || !nullable.Value.IsValid())
      return false;
    int entryIndex = ent.Comp.EntryIndex;
    Vector2 vector2_1 = entryIndex < 0 || entryIndex >= comp2.EntryPoints.Count ? comp2.ExitOffset : comp2.EntryPoints[entryIndex].Offset;
    Angle localRotation = transformComponent.LocalRotation;
    Vector2 vector2_2 = ((Angle) ref localRotation).RotateVec(ref vector2_1);
    Vector2 position = transformComponent.LocalPosition + vector2_2;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(new EntityCoordinates(nullable.Value, position));
    this._rmcTeleporter.HandlePulling(user, mapCoordinates);
    this.UntrackOccupant(user, valueOrDefault);
    return true;
  }

  private void OnVehicleExitDoAfter(
    Entity<VehicleExitComponent> ent,
    ref VehicleExitDoAfterEvent args)
  {
    ent.Comp.PendingExit = false;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = this.TryExit(ent, args.User);
  }

  private void OnOccupantStartup(
    Entity<VehicleInteriorOccupantComponent> ent,
    ref ComponentStartup args)
  {
    this._meta.AddFlag((EntityUid) ent, MetaDataFlags.ExtraTransformEvents);
  }

  private void OnOccupantRemove(
    Entity<VehicleInteriorOccupantComponent> ent,
    ref ComponentRemove args)
  {
    this._meta.RemoveFlag((EntityUid) ent, MetaDataFlags.ExtraTransformEvents);
    if (ent.Comp.Vehicle.IsValid())
      this.UnregisterTrackedOccupant(ent.Comp.Vehicle, ent.Owner, ent.Comp.IsXeno);
    if (this._net.IsClient)
      return;
    this.RemCompDeferred<RMCVehicleInteriorOccupantComponent>(ent.Owner);
  }

  private void OnOccupantMapChanged(
    Entity<VehicleInteriorOccupantComponent> ent,
    ref MapUidChangedEvent args)
  {
    if (ent.Comp.Vehicle == EntityUid.Invalid)
      return;
    VehicleInteriorComponent comp;
    if (this.TryComp<VehicleInteriorComponent>(ent.Comp.Vehicle, out comp))
    {
      MapId? newMapId = args.NewMapId;
      MapId mapId = comp.MapId;
      if ((newMapId.HasValue ? (newMapId.GetValueOrDefault() == mapId ? 1 : 0) : 0) != 0)
      {
        this.RegisterTrackedOccupant(ent.Comp.Vehicle, ent.Owner, ent.Comp.IsXeno, comp);
        return;
      }
    }
    this.RemCompDeferred<VehicleInteriorOccupantComponent>(ent.Owner);
  }

  private void OnOccupantMetaFlagRemoveAttempt(
    Entity<VehicleInteriorOccupantComponent> ent,
    ref MetaFlagRemoveAttemptEvent args)
  {
    if ((args.ToRemove & MetaDataFlags.ExtraTransformEvents) == MetaDataFlags.None || ent.Comp.LifeStage > ComponentLifeStage.Running)
      return;
    args.ToRemove &= ~MetaDataFlags.ExtraTransformEvents;
  }

  private void TrackOccupant(EntityUid user, EntityUid vehicle, bool isXeno)
  {
    VehicleInteriorOccupantComponent occupantComponent = this.EnsureComp<VehicleInteriorOccupantComponent>(user);
    if (occupantComponent.Vehicle.IsValid() && occupantComponent.Vehicle != vehicle)
      this.UnregisterTrackedOccupant(occupantComponent.Vehicle, user, occupantComponent.IsXeno);
    occupantComponent.Vehicle = vehicle;
    occupantComponent.IsXeno = isXeno;
    this.RegisterTrackedOccupant(vehicle, user, isXeno);
    this.SetInteriorOccupantVehicle(user, vehicle);
  }

  private void SetInteriorOccupantVehicle(EntityUid user, EntityUid vehicle)
  {
    if (this._net.IsClient)
      return;
    RMCVehicleInteriorOccupantComponent occupantComponent = this.EnsureComp<RMCVehicleInteriorOccupantComponent>(user);
    EntityUid? vehicle1 = occupantComponent.Vehicle;
    EntityUid entityUid = vehicle;
    if ((vehicle1.HasValue ? (vehicle1.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
      return;
    occupantComponent.Vehicle = new EntityUid?(vehicle);
    this.Dirty(user, (IComponent) occupantComponent);
  }

  private void UntrackOccupant(EntityUid user, EntityUid vehicle)
  {
    VehicleInteriorOccupantComponent comp;
    if (!this.TryComp<VehicleInteriorOccupantComponent>(user, out comp) || comp.Vehicle != vehicle)
      this.UnregisterTrackedOccupant(vehicle, user, this.HasComp<XenoComponent>(user));
    else
      this.RemComp<VehicleInteriorOccupantComponent>(user);
  }

  private void RegisterTrackedOccupant(
    EntityUid vehicle,
    EntityUid user,
    bool isXeno,
    VehicleInteriorComponent? interior = null)
  {
    if (!this.Resolve<VehicleInteriorComponent>(vehicle, ref interior, false))
      return;
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

  private void UnregisterTrackedOccupant(EntityUid vehicle, EntityUid user, bool isXeno)
  {
    VehicleInteriorComponent comp;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out comp))
      return;
    if (isXeno)
      comp.Xenos.Remove(user);
    else
      comp.Passengers.Remove(user);
  }

  private void PruneTrackedOccupants(EntityUid vehicle, VehicleInteriorComponent interior)
  {
    foreach (EntityUid uid in new List<EntityUid>((IEnumerable<EntityUid>) interior.Passengers))
    {
      VehicleInteriorOccupantComponent comp;
      if (!this.TryComp<VehicleInteriorOccupantComponent>(uid, out comp) || !(comp.Vehicle == vehicle) || comp.IsXeno || !(this._transform.GetMapId((Entity<TransformComponent>) uid) == interior.MapId))
        interior.Passengers.Remove(uid);
    }
    foreach (EntityUid uid in new List<EntityUid>((IEnumerable<EntityUid>) interior.Xenos))
    {
      VehicleInteriorOccupantComponent comp;
      if (!this.TryComp<VehicleInteriorOccupantComponent>(uid, out comp) || !(comp.Vehicle == vehicle) || !comp.IsXeno || !(this._transform.GetMapId((Entity<TransformComponent>) uid) == interior.MapId))
        interior.Xenos.Remove(uid);
    }
  }

  private int CountLivingOccupants(HashSet<EntityUid> occupants)
  {
    int num = 0;
    foreach (EntityUid occupant in occupants)
    {
      if (!this._mobState.IsDead(occupant))
        ++num;
    }
    return num;
  }

  private void OnDriverSeatStrapAttempt(
    Entity<VehicleDriverSeatComponent> ent,
    ref StrapAttemptEvent args)
  {
    if (args.Cancelled || this._skills.HasSkills((Entity<SkillsComponent>) args.Buckle.Owner, ent.Comp.Skills) || !args.Popup)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-skills-cant-operate", ("target", (object) ent)), (EntityUid) args.Buckle, args.User);
  }

  private void OnDriverSeatStrapped(Entity<VehicleDriverSeatComponent> ent, ref StrappedEvent args)
  {
    EntityUid? vehicle;
    VehicleComponent comp;
    if (this._net.IsClient || !this.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !this.TryComp<VehicleComponent>(vehicle, out comp))
      return;
    this._vehicles.TrySetOperator((Entity<VehicleComponent>) (vehicle.Value, comp), new EntityUid?(args.Buckle.Owner));
    this.EnsureComp<VehicleOperatorComponent>(args.Buckle.Owner);
    this._vehicleLock.EnableLockAction(args.Buckle.Owner, vehicle.Value);
  }

  private void OnDriverSeatUnstrapped(
    Entity<VehicleDriverSeatComponent> ent,
    ref UnstrappedEvent args)
  {
    EntityUid? vehicle;
    VehicleComponent comp;
    if (this._net.IsClient || !this.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !this.TryComp<VehicleComponent>(vehicle, out comp))
      return;
    this._vehicleLock.DisableLockAction(args.Buckle.Owner, vehicle.Value);
    EntityUid? nullable = comp.Operator;
    EntityUid owner = args.Buckle.Owner;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0)
      return;
    this._vehicles.TryRemoveOperator((Entity<VehicleComponent>) (vehicle.Value, comp));
    if (this.IsOperatingOtherVehicle(args.Buckle.Owner))
      return;
    this.RemCompDeferred<VehicleOperatorComponent>(args.Buckle.Owner);
  }

  private void OnVehicleOperatorEntered(
    Entity<VehicleOperatorComponent> ent,
    ref OnVehicleEnteredEvent args)
  {
    if (this._net.IsClient || !this.HasComp<VehicleEnterComponent>(args.Vehicle.Owner))
      return;
    this._eye.SetTarget(ent.Owner, new EntityUid?(args.Vehicle.Owner));
    this._viewToggle.EnableViewToggle(ent.Owner, args.Vehicle.Owner, args.Vehicle.Owner, new EntityUid?(), true);
  }

  private void OnVehicleOperatorExited(
    Entity<VehicleOperatorComponent> ent,
    ref OnVehicleExitedEvent args)
  {
    EyeComponent comp;
    if (this._net.IsClient || !this.TryComp<EyeComponent>((EntityUid) ent, out comp))
      return;
    this._viewToggle.DisableViewToggle(ent.Owner, args.Vehicle.Owner);
    EntityUid? nullable1 = comp.Target;
    EntityUid owner1 = args.Vehicle.Owner;
    if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() != owner1 ? 1 : 0) : 1) != 0)
      return;
    SharedEyeSystem eye = this._eye;
    EntityUid owner2 = ent.Owner;
    nullable1 = new EntityUid?();
    EntityUid? nullable2 = nullable1;
    EyeComponent eyeComponent = comp;
    eye.SetTarget(owner2, nullable2, eyeComponent);
  }

  private bool IsOperatingOtherVehicle(EntityUid entity)
  {
    BuckleComponent comp;
    return this.TryComp<BuckleComponent>(entity, out comp) && comp.BuckledTo.HasValue && this.HasComp<VehicleDriverSeatComponent>(comp.BuckledTo);
  }

  private void OnFrameVehicleCanRun(
    Entity<HardpointIntegrityComponent> ent,
    ref VehicleCanRunEvent args)
  {
    if (!args.CanRun || (double) ent.Comp.Integrity > 0.0)
      return;
    args.CanRun = false;
  }

  private void OnConstructionAttempt(ref RMCConstructionAttemptEvent ev)
  {
    if (ev.Cancelled || this._net.IsClient || !this.TryGetVehicleFromInterior(ev.Location.EntityId, out EntityUid? _))
      return;
    ev.Cancelled = true;
    ev.Popup = this.Loc.GetString("construction-system-inside-container");
  }

  private bool IsEntryBlockedByLock(EntityUid vehicle, EntityUid user)
  {
    VehicleLockComponent comp;
    return this.TryComp<VehicleLockComponent>(vehicle, out comp) && comp.Locked && !this.CanBypassLockWithDestroyedFrame(vehicle, user);
  }

  private bool IsExitBlockedByLock(EntityUid vehicle, EntityUid user)
  {
    VehicleLockComponent comp;
    return this.TryComp<VehicleLockComponent>(vehicle, out comp) && comp.Locked && !this.CanBypassLockWithDestroyedFrame(vehicle, user);
  }

  private bool CanBypassLockWithDestroyedFrame(EntityUid vehicle, EntityUid user)
  {
    HardpointIntegrityComponent comp;
    return this.HasComp<XenoComponent>(user) && this.TryComp<HardpointIntegrityComponent>(vehicle, out comp) && comp.BypassEntryOnZero && (double) comp.Integrity <= 0.0;
  }

  public bool TryGetVehicleFromInterior(EntityUid interiorEntity, out EntityUid? vehicle)
  {
    vehicle = new EntityUid?();
    MapId mapId = this._transform.GetMapId((Entity<TransformComponent>) interiorEntity);
    VehicleInteriorLinkComponent comp;
    if (mapId == MapId.Nullspace || !this._mapManager.MapExists(new MapId?(mapId)) || !this.TryComp<VehicleInteriorLinkComponent>(this._mapManager.GetMapEntityId(mapId), out comp) || this.Deleted(comp.Vehicle))
      return false;
    vehicle = new EntityUid?(comp.Vehicle);
    return true;
  }

  public bool TryResolveControlledVehicle(EntityUid user, out EntityUid vehicle)
  {
    vehicle = EntityUid.Invalid;
    VehicleOperatorComponent comp;
    if (this.TryComp<VehicleOperatorComponent>(user, out comp))
    {
      EntityUid? vehicle1 = comp.Vehicle;
      if (vehicle1.HasValue)
      {
        EntityUid valueOrDefault = vehicle1.GetValueOrDefault();
        if (this.EntityManager.EntityExists(valueOrDefault))
        {
          vehicle = valueOrDefault;
          return true;
        }
      }
    }
    EntityUid? vehicle2;
    if (this.TryGetVehicleFromInterior(user, out vehicle2) && vehicle2.HasValue)
    {
      EntityUid valueOrDefault = vehicle2.GetValueOrDefault();
      if (this.EntityManager.EntityExists(valueOrDefault))
      {
        vehicle = valueOrDefault;
        return true;
      }
    }
    return false;
  }

  public bool TryGetInteriorMapId(EntityUid vehicle, out MapId mapId)
  {
    mapId = MapId.Nullspace;
    VehicleInteriorComponent comp;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out comp))
      return false;
    mapId = comp.MapId;
    return mapId != MapId.Nullspace;
  }

  public bool TryGetInteriorEntryCoordinates(EntityUid vehicle, out EntityCoordinates coordinates)
  {
    coordinates = EntityCoordinates.Invalid;
    VehicleInteriorComponent comp;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out comp))
      return false;
    coordinates = comp.Entry;
    return coordinates.IsValid((IEntityManager) this.EntityManager);
  }

  public bool TryEnsureInteriorEntry(EntityUid vehicle, out EntityCoordinates coordinates)
  {
    coordinates = EntityCoordinates.Invalid;
    VehicleInteriorComponent interiorComponent;
    VehicleEnterComponent comp;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out interiorComponent) && (!this.TryComp<VehicleEnterComponent>(vehicle, out comp) || !this.EnsureInterior((Entity<VehicleEnterComponent>) (vehicle, comp), out interiorComponent)))
      return false;
    coordinates = interiorComponent.Entry;
    return coordinates.IsValid((IEntityManager) this.EntityManager);
  }

  public bool TryGetInteriorGrid(EntityUid vehicle, out EntityUid grid)
  {
    grid = EntityUid.Invalid;
    VehicleInteriorComponent comp;
    if (!this.TryComp<VehicleInteriorComponent>(vehicle, out comp) || !comp.Grid.IsValid() || !this.Exists(comp.Grid))
      return false;
    grid = comp.Grid;
    return true;
  }

  public bool TryExitFromInterior(EntityUid user, EntityUid vehicle)
  {
    VehicleEnterComponent comp;
    if (!this.TryComp<VehicleEnterComponent>(vehicle, out comp) || this.IsExitBlockedByLock(vehicle, user))
      return false;
    TransformComponent transformComponent = this.Transform(vehicle);
    EntityUid? nullable = new EntityUid?(transformComponent.ParentUid);
    if (!nullable.HasValue || !nullable.Value.IsValid())
      nullable = transformComponent.MapUid;
    if (!nullable.HasValue || !nullable.Value.IsValid())
      return false;
    Angle localRotation = transformComponent.LocalRotation;
    Vector2 vector2 = ((Angle) ref localRotation).RotateVec(ref comp.ExitOffset);
    Vector2 position = transformComponent.LocalPosition + vector2;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(new EntityCoordinates(nullable.Value, position));
    this._rmcTeleporter.HandlePulling(user, mapCoordinates);
    this.UntrackOccupant(user, vehicle);
    this.RemCompDeferred<RMCVehicleInteriorOccupantComponent>(user);
    return true;
  }

  public bool TryGetDisplayEntity(EntityUid entity, out EntityUid displayEntity)
  {
    displayEntity = entity;
    RMCVehicleInteriorOccupantComponent comp;
    if (this.TryComp<RMCVehicleInteriorOccupantComponent>(entity, out comp))
    {
      EntityUid? vehicle = comp.Vehicle;
      if (vehicle.HasValue)
      {
        EntityUid valueOrDefault = vehicle.GetValueOrDefault();
        if (this.Exists(valueOrDefault))
        {
          displayEntity = valueOrDefault;
          return true;
        }
      }
    }
    EntityUid? vehicle1;
    if (this.TryGetVehicleFromInterior(entity, out vehicle1) && vehicle1.HasValue)
    {
      EntityUid valueOrDefault = vehicle1.GetValueOrDefault();
      if (this.Exists(valueOrDefault))
      {
        displayEntity = valueOrDefault;
        return true;
      }
    }
    return this.Exists(entity);
  }

  public bool TryGetDisplayMapCoordinates(EntityUid entity, out MapCoordinates coordinates)
  {
    coordinates = MapCoordinates.Nullspace;
    EntityUid displayEntity;
    if (!this.TryGetDisplayEntity(entity, out displayEntity))
      return false;
    coordinates = this._transform.GetMapCoordinates(displayEntity);
    return coordinates.MapId != MapId.Nullspace;
  }

  public bool TryGetDisplayMapId(EntityUid entity, out MapId mapId)
  {
    mapId = MapId.Nullspace;
    MapCoordinates coordinates;
    if (!this.TryGetDisplayMapCoordinates(entity, out coordinates))
      return false;
    mapId = coordinates.MapId;
    return mapId != MapId.Nullspace;
  }
}
