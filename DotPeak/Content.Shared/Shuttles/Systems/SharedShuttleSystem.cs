// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Systems.SharedShuttleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.UI.MapObjects;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Shuttles.Systems;

public abstract class SharedShuttleSystem : EntitySystem
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  protected FixtureSystem Fixtures;
  [Dependency]
  protected SharedMapSystem Maps;
  [Dependency]
  protected SharedPhysicsSystem Physics;
  [Dependency]
  protected SharedTransformSystem XformSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  public const float FTLRange = 256f;
  public const float FTLBufferRange = 8f;
  public const float TileDensityMultiplier = 0.5f;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FixturesComponent, GridFixtureChangeEvent>(new ComponentEventHandler<FixturesComponent, GridFixtureChangeEvent>(this.OnGridFixtureChange));
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
  }

  private void OnGridFixtureChange(
    EntityUid uid,
    FixturesComponent manager,
    GridFixtureChangeEvent args)
  {
    foreach (KeyValuePair<string, Fixture> newFixture in args.NewFixtures)
    {
      this.Physics.SetDensity(uid, newFixture.Key, newFixture.Value, 0.5f, false, manager);
      this.Fixtures.SetRestitution(uid, newFixture.Key, newFixture.Value, 0.1f, false, manager);
    }
  }

  public bool CanFTLTo(EntityUid shuttleUid, MapId targetMap, EntityUid consoleUid)
  {
    EntityUid mapOrInvalid = this.Maps.GetMapOrInvalid(new MapId?(targetMap));
    if (this._xformQuery.GetComponent(shuttleUid).MapID == targetMap)
      return true;
    FTLDestinationComponent comp1;
    if (!this.TryComp<FTLDestinationComponent>(mapOrInvalid, out comp1) || !comp1.Enabled)
      return false;
    if (comp1.RequireCoordinateDisk)
    {
      ItemSlotsComponent comp2;
      ItemSlot itemSlot;
      if (!this.TryComp<ItemSlotsComponent>(consoleUid, out comp2) || !this._itemSlots.TryGetSlot(consoleUid, SharedShuttleConsoleComponent.DiskSlotName, out itemSlot, comp2) || !itemSlot.HasItem)
        return false;
      EntityUid? nullable = itemSlot.Item;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        if (valueOrDefault.Valid)
        {
          ShuttleDestinationCoordinatesComponent component = (ShuttleDestinationCoordinatesComponent) null;
          if (!this.Resolve<ShuttleDestinationCoordinatesComponent>(valueOrDefault, ref component))
            return false;
          EntityUid? destination = component.Destination;
          FTLDestinationComponent comp3;
          if (!destination.HasValue || !this.TryComp<FTLDestinationComponent>(destination.Value, out comp3) || comp3 != comp1)
            return false;
          goto label_14;
        }
      }
      return false;
    }
label_14:
    return !this.HasComp<FTLMapComponent>(mapOrInvalid) && this._whitelistSystem.IsWhitelistPassOrNull(comp1.Whitelist, shuttleUid);
  }

  public IEnumerable<(ShuttleExclusionObject Exclusion, MapCoordinates Coordinates)> GetExclusions(
    MapId mapId,
    List<ShuttleExclusionObject> exclusions)
  {
    SharedShuttleSystem sharedShuttleSystem = this;
    foreach (ShuttleExclusionObject exclusion in exclusions)
    {
      MapCoordinates mapCoordinates = sharedShuttleSystem.XformSystem.ToMapCoordinates(sharedShuttleSystem.GetCoordinates(exclusion.Coordinates));
      if (!(mapCoordinates.MapId != mapId))
        yield return (exclusion, mapCoordinates);
    }
  }

  public IEnumerable<(ShuttleBeaconObject Beacon, MapCoordinates Coordinates)> GetBeacons(
    MapId mapId,
    List<ShuttleBeaconObject> beacons)
  {
    SharedShuttleSystem sharedShuttleSystem = this;
    foreach (ShuttleBeaconObject beacon in beacons)
    {
      MapCoordinates mapCoordinates = sharedShuttleSystem.XformSystem.ToMapCoordinates(sharedShuttleSystem.GetCoordinates(beacon.Coordinates));
      if (!(mapCoordinates.MapId != mapId))
        yield return (beacon, mapCoordinates);
    }
  }

  public bool CanDraw(EntityUid gridUid, PhysicsComponent? physics = null, IFFComponent? iffComp = null)
  {
    if (!this.Resolve<PhysicsComponent>(gridUid, ref physics))
      return true;
    if (physics.BodyType != BodyType.Static && (double) physics.Mass < 10.0)
      return false;
    return !this.Resolve<IFFComponent>(gridUid, ref iffComp, false) || (iffComp.Flags & IFFFlags.Hide) == IFFFlags.None;
  }

  public bool IsBeaconMap(EntityUid mapUid)
  {
    FTLDestinationComponent comp;
    return this.TryComp<FTLDestinationComponent>(mapUid, out comp) && comp.BeaconsOnly;
  }

  public bool CanFTLBeacon(NetCoordinates nCoordinates)
  {
    return this.HasComp<MapComponent>(this.GetCoordinates(nCoordinates).EntityId);
  }

  public float GetFTLRange(EntityUid shuttleUid) => 256f;

  public float GetFTLBufferRange(EntityUid shuttleUid, MapGridComponent? grid = null)
  {
    if (!this._gridQuery.Resolve(shuttleUid, ref grid))
      return 0.0f;
    Box2 localAabb = grid.LocalAABB;
    return (float) ((double) ((Box2) ref localAabb).MaxDimension / 2.0 + 8.0);
  }

  public bool FTLFree(
    EntityUid shuttleUid,
    EntityCoordinates coordinates,
    Angle angle,
    List<ShuttleExclusionObject>? exclusionZones)
  {
    PhysicsComponent component1;
    TransformComponent component2;
    if (!this._physicsQuery.TryGetComponent(shuttleUid, out component1) || !this._xformQuery.TryGetComponent(shuttleUid, out component2))
      return false;
    this._grids.Clear();
    MapCoordinates mapCoordinates1 = this.XformSystem.ToMapCoordinates(coordinates);
    Vector2 gridPosition = this.Maps.GetGridPosition((Entity<PhysicsComponent, TransformComponent>) (shuttleUid, component1, component2));
    Vector2 position = mapCoordinates1.Position;
    Vector2 vector2 = position - gridPosition;
    if ((double) vector2.Length() > 256.0)
      return false;
    if (exclusionZones != null)
    {
      foreach (ShuttleExclusionObject exclusionZone in exclusionZones)
      {
        MapCoordinates mapCoordinates2 = this.XformSystem.ToMapCoordinates(this.GetCoordinates(exclusionZone.Coordinates));
        if (!(mapCoordinates2.MapId != mapCoordinates1.MapId))
        {
          vector2 = mapCoordinates1.Position - mapCoordinates2.Position;
          if ((double) vector2.Length() <= (double) exclusionZone.Range)
            return false;
        }
      }
    }
    PhysShapeCircle shape = new PhysShapeCircle(this.GetFTLBufferRange(shuttleUid) + 8f, position);
    this._mapManager.FindGridsIntersecting<PhysShapeCircle>(mapCoordinates1.MapId, shape, Robust.Shared.Physics.Transform.Empty, ref this._grids, includeMap: false);
    foreach (Entity<MapGridComponent> grid in this._grids)
    {
      if (!(grid.Owner == shuttleUid))
        return false;
    }
    return true;
  }

  protected virtual void UpdateIFFInterfaces(EntityUid gridUid, IFFComponent component)
  {
  }

  public Color GetIFFColor(EntityUid gridUid, bool self = false, IFFComponent? component = null)
  {
    if (self)
      return IFFComponent.SelfColor;
    return !this.Resolve<IFFComponent>(gridUid, ref component, false) ? IFFComponent.IFFColor : component.Color;
  }

  public string? GetIFFLabel(EntityUid gridUid, bool self = false, IFFComponent? component = null)
  {
    string entityName = this.MetaData(gridUid).EntityName;
    if (self)
      return entityName;
    if (this.Resolve<IFFComponent>(gridUid, ref component, false) && (component.Flags & (IFFFlags.HideLabel | IFFFlags.Hide)) != IFFFlags.None)
      return (string) null;
    return !string.IsNullOrEmpty(entityName) ? entityName : this.Loc.GetString("shuttle-console-unknown");
  }

  public void SetIFFColor(EntityUid gridUid, Color color, IFFComponent? component = null)
  {
    if (component == null)
      component = this.EnsureComp<IFFComponent>(gridUid);
    if (((Color) ref component.Color).Equals(color))
      return;
    component.Color = color;
    this.Dirty(gridUid, (IComponent) component);
    this.UpdateIFFInterfaces(gridUid, component);
  }

  public void AddIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
  {
    if (component == null)
      component = this.EnsureComp<IFFComponent>(gridUid);
    if ((component.Flags & flags) == flags)
      return;
    component.Flags |= flags;
    this.Dirty(gridUid, (IComponent) component);
    this.UpdateIFFInterfaces(gridUid, component);
  }

  public void RemoveIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
  {
    if (!this.Resolve<IFFComponent>(gridUid, ref component, false) || (component.Flags & flags) == IFFFlags.None)
      return;
    component.Flags &= ~flags;
    this.Dirty(gridUid, (IComponent) component);
    this.UpdateIFFInterfaces(gridUid, component);
  }
}
