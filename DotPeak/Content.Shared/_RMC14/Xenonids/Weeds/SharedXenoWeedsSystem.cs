// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Weeds.SharedXenoWeedsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Communications;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids.Construction.FloorResin;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Climbing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Maps;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Weeds;

public abstract class SharedXenoWeedsSystem : EntitySystem
{
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private ITileDefinitionManager _tile;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<AffectableByWeedsComponent> _affectedQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoWeedsComponent> _weedsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ResinSlowdownModifierComponent> _slowResinQuery;
  private Robust.Shared.GameObjects.EntityQuery<ResinSpeedupModifierComponent> _fastResinQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;
  private Robust.Shared.GameObjects.EntityQuery<BlockWeedsComponent> _blockWeedsQuery;
  private Robust.Shared.GameObjects.EntityQuery<HiveMemberComponent> _hiveMemberQuery;

  public override void Initialize()
  {
    this._affectedQuery = this.GetEntityQuery<AffectableByWeedsComponent>();
    this._weedsQuery = this.GetEntityQuery<XenoWeedsComponent>();
    this._slowResinQuery = this.GetEntityQuery<ResinSlowdownModifierComponent>();
    this._fastResinQuery = this.GetEntityQuery<ResinSpeedupModifierComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this._blockWeedsQuery = this.GetEntityQuery<BlockWeedsComponent>();
    this._hiveMemberQuery = this.GetEntityQuery<HiveMemberComponent>();
    this.SubscribeLocalEvent<XenoWeedsComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<XenoWeedsComponent, AnchorStateChangedEvent>(this.OnWeedsAnchorChanged));
    this.SubscribeLocalEvent<XenoWeedsComponent, ComponentShutdown>(new EntityEventRefHandler<XenoWeedsComponent, ComponentShutdown>(this.OnModifierShutdown<XenoWeedsComponent>));
    this.SubscribeLocalEvent<XenoWeedsComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoWeedsComponent, EntityTerminatingEvent>(this.OnWeedsTerminating));
    this.SubscribeLocalEvent<XenoWeedsComponent, MapInitEvent>(new EntityEventRefHandler<XenoWeedsComponent, MapInitEvent>(this.OnWeedsMapInit));
    this.SubscribeLocalEvent<XenoWeedsComponent, StartCollideEvent>(new EntityEventRefHandler<XenoWeedsComponent, StartCollideEvent>(this.OnWeedsStartCollide));
    this.SubscribeLocalEvent<XenoWeedsComponent, EndCollideEvent>(new EntityEventRefHandler<XenoWeedsComponent, EndCollideEvent>(this.OnWeedsEndCollide));
    this.SubscribeLocalEvent<XenoWeedsComponent, ExaminedEvent>(new EntityEventRefHandler<XenoWeedsComponent, ExaminedEvent>(this.OnWeedsExamined));
    this.SubscribeLocalEvent<XenoWallWeedsComponent, ComponentRemove>(new EntityEventRefHandler<XenoWallWeedsComponent, ComponentRemove>(this.OnWallWeedsRemove<ComponentRemove>));
    this.SubscribeLocalEvent<XenoWallWeedsComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoWallWeedsComponent, EntityTerminatingEvent>(this.OnWallWeedsRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<XenoWeedableComponent, AnchorStateChangedEvent>(new EntityEventRefHandler<XenoWeedableComponent, AnchorStateChangedEvent>(this.OnWeedableAnchorStateChanged));
    this.SubscribeLocalEvent<XenoWeedableComponent, ComponentRemove>(new EntityEventRefHandler<XenoWeedableComponent, ComponentRemove>(this.OnWeedableRemove<ComponentRemove>));
    this.SubscribeLocalEvent<XenoWeedableComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoWeedableComponent, EntityTerminatingEvent>(this.OnWeedableRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DamageOffWeedsComponent, MapInitEvent>(new EntityEventRefHandler<DamageOffWeedsComponent, MapInitEvent>(this.OnDamageOffWeedsMapInit));
    this.SubscribeLocalEvent<AffectableByWeedsComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<AffectableByWeedsComponent, RefreshMovementSpeedModifiersEvent>(this.WeedsRefreshPassiveSpeed));
    this.SubscribeLocalEvent<AffectableByWeedsComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<AffectableByWeedsComponent, XenoOvipositorChangedEvent>(this.WeedsOvipositorChanged));
    this.SubscribeLocalEvent<XenoWeedsSpreadingComponent, MapInitEvent>(new EntityEventRefHandler<XenoWeedsSpreadingComponent, MapInitEvent>(this.OnSpreadingMapInit));
    this.SubscribeLocalEvent<ResinSlowdownModifierComponent, ComponentShutdown>(new EntityEventRefHandler<ResinSlowdownModifierComponent, ComponentShutdown>(this.OnModifierShutdown<ResinSlowdownModifierComponent>));
    this.SubscribeLocalEvent<ResinSlowdownModifierComponent, StartCollideEvent>(new EntityEventRefHandler<ResinSlowdownModifierComponent, StartCollideEvent>(this.OnResinSlowdownStartCollide));
    this.SubscribeLocalEvent<ResinSlowdownModifierComponent, EndCollideEvent>(new EntityEventRefHandler<ResinSlowdownModifierComponent, EndCollideEvent>(this.OnResinSlowdownEndCollide));
    this.SubscribeLocalEvent<ResinSpeedupModifierComponent, ComponentShutdown>(new EntityEventRefHandler<ResinSpeedupModifierComponent, ComponentShutdown>(this.OnModifierShutdown<ResinSpeedupModifierComponent>));
    this.SubscribeLocalEvent<ResinSpeedupModifierComponent, StartCollideEvent>(new EntityEventRefHandler<ResinSpeedupModifierComponent, StartCollideEvent>(this.OnResinSpeedupStartCollide));
    this.SubscribeLocalEvent<ResinSpeedupModifierComponent, EndCollideEvent>(new EntityEventRefHandler<ResinSpeedupModifierComponent, EndCollideEvent>(this.OnResinSpeedupEndCollide));
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
  }

  private void OnWeedsExamined(Entity<XenoWeedsComponent> weeds, ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner) || (double) weeds.Comp.FruitGrowthMultiplier == 1.0)
      return;
    using (args.PushGroup("XenoWeedsComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-weed-boost", ("percent", (object) (int) ((double) weeds.Comp.FruitGrowthMultiplier * 100.0))));
  }

  private void OnWeedsAnchorChanged(
    Entity<XenoWeedsComponent> weeds,
    ref AnchorStateChangedEvent args)
  {
    if (!this._net.IsServer || args.Anchored)
      return;
    this.QueueDel(new EntityUid?((EntityUid) weeds));
  }

  private void OnModifierShutdown<T>(Entity<T> ent, ref ComponentShutdown args) where T : IComponent
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out comp))
      return;
    this._toUpdate.UnionWith((IEnumerable<EntityUid>) this._physics.GetContactingEntities((EntityUid) ent, comp));
  }

  private void OnWeedsTerminating(Entity<XenoWeedsComponent> ent, ref EntityTerminatingEvent args)
  {
    if (!ent.Comp.IsSource)
    {
      XenoWeedsComponent component;
      if (this._weedsQuery.TryComp(ent.Comp.Source, out component))
      {
        component.Spread.Remove((EntityUid) ent);
        this.Dirty(ent.Comp.Source.Value, (IComponent) component);
      }
      foreach (EntityUid uid in ent.Comp.LocalWeeded)
      {
        if (!this.HasComp<CommunicationsTowerComponent>(uid))
          this._appearance.SetData(uid, (Enum) WeededEntityLayers.Layer, (object) false);
      }
    }
    else
    {
      foreach (EntityUid uid in ent.Comp.Spread)
      {
        if (!this.TerminatingOrDeleted(uid))
        {
          XenoWeedsComponent component;
          if (this._weedsQuery.TryComp(uid, out component))
          {
            component.Source = new EntityUid?();
            this.Dirty(uid, (IComponent) component);
          }
          this.EnsureComp<TimedDespawnComponent>(uid).Lifetime = (float) this._random.Next(ent.Comp.MinRandomDelete, ent.Comp.MaxRandomDelete).TotalSeconds;
        }
      }
      ent.Comp.Spread.Clear();
      this.Dirty<XenoWeedsComponent>(ent);
    }
  }

  private void OnWeedsMapInit(Entity<XenoWeedsComponent> ent, ref MapInitEvent args)
  {
    foreach (EntityUid uid in this._physics.GetEntitiesIntersectingBody((EntityUid) ent, 65))
    {
      AffectableByWeedsComponent component;
      if (this._affectedQuery.TryComp(uid, out component) && !component.OnXenoWeeds)
        this._toUpdate.Add(uid);
    }
  }

  private void OnWeedsStartCollide(Entity<XenoWeedsComponent> ent, ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || component.OnXenoWeeds)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnWeedsEndCollide(Entity<XenoWeedsComponent> ent, ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || !component.OnXenoWeeds)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnWallWeedsRemove<T>(Entity<XenoWallWeedsComponent> ent, ref T args)
  {
    XenoWeedsComponent comp;
    if (!this.TryComp<XenoWeedsComponent>(ent.Comp.Weeds, out comp))
      return;
    comp.Spread.Remove((EntityUid) ent);
    this.Dirty(ent.Comp.Weeds.Value, (IComponent) comp);
  }

  private void OnWeedableAnchorStateChanged(
    Entity<XenoWeedableComponent> weedable,
    ref AnchorStateChangedEvent args)
  {
    if (!this._net.IsServer || args.Anchored)
      return;
    this.QueueDel(weedable.Comp.Entity);
  }

  private void OnWeedableRemove<T>(Entity<XenoWeedableComponent> weedable, ref T args)
  {
    if (!this._net.IsServer || !weedable.Comp.Entity.HasValue)
      return;
    this.QueueDel(weedable.Comp.Entity);
  }

  private void OnDamageOffWeedsMapInit(
    Entity<DamageOffWeedsComponent> damage,
    ref MapInitEvent args)
  {
    damage.Comp.DamageAt = new TimeSpan?(this._timing.CurTime + damage.Comp.Every);
  }

  private void WeedsRefreshPassiveSpeed(
    Entity<AffectableByWeedsComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    PhysicsComponent comp1;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out comp1))
      return;
    float num1 = 0.0f;
    float num2 = 0.0f;
    bool flag1 = this._xenoQuery.HasComp((EntityUid) ent);
    HiveMemberComponent component1;
    this._hiveMemberQuery.TryComp((EntityUid) ent, out component1);
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    bool flag5 = false;
    int num3 = 0;
    int num4 = 0;
    this._intersecting.Clear();
    this._physics.GetContactingEntities((Entity<PhysicsComponent>) ((EntityUid) ent, comp1), this._intersecting);
    TransformComponent comp2;
    if (this.TryComp((EntityUid) ent, out comp2) && comp2.Anchored)
    {
      RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator((EntityUid) ent, facing: (DirectionFlag) 0);
      EntityUid uid;
      while (entitiesEnumerator.MoveNext(out uid))
        this._intersecting.Add(uid);
    }
    foreach (EntityUid entityUid in this._intersecting)
    {
      ResinSlowdownModifierComponent component2;
      if (this._slowResinQuery.TryComp(entityUid, out component2))
      {
        if (component1 == null || !this._hive.IsMember((Entity<HiveMemberComponent>) entityUid, component1.Hive))
        {
          if (this.HasComp<RMCArmorSpeedTierUserComponent>(entityUid))
            num2 += component2.OutsiderSpeedModifierArmor;
          else
            num2 += component2.OutsiderSpeedModifier;
          ++num3;
        }
        flag3 = true;
      }
      else
      {
        ResinSpeedupModifierComponent component3;
        if (this._fastResinQuery.TryComp(entityUid, out component3))
        {
          if (flag1 && component1 != null && this._hive.IsMember((Entity<HiveMemberComponent>) entityUid, component1.Hive))
          {
            num2 += component3.HiveSpeedModifier;
            ++num3;
          }
          flag4 = true;
        }
        else
        {
          XenoWeedsComponent component4;
          if (this._weedsQuery.TryComp(entityUid, out component4))
          {
            flag2 = true;
            if (flag1 && component1 != null && this._hive.IsMember((Entity<HiveMemberComponent>) entityUid, component1.Hive))
            {
              num1 += component4.SpeedMultiplierXeno;
              flag5 = true;
              ++num4;
            }
            else if (component1 == null || !this._hive.IsMember((Entity<HiveMemberComponent>) entityUid, component1.Hive))
            {
              if (this.HasComp<RMCArmorSpeedTierUserComponent>(entityUid))
                num1 += component4.SpeedMultiplierOutsiderArmor;
              else
                num1 += component4.SpeedMultiplierOutsider;
              ++num4;
            }
          }
        }
      }
    }
    if (!flag2 && this.Transform((EntityUid) ent).Anchored && this._rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(ent.Owner.ToCoordinates(), facing: (DirectionFlag) 0))
      flag2 = true;
    float num5 = 1f;
    if (num4 > 0)
      num1 /= (float) num4;
    if (num3 > 0)
      num2 /= (float) num3;
    if (((double) num1 > 1.0 || (double) num2 > 1.0) && num3 > 0 && num4 > 0)
      num5 = num1 * num2;
    else if (num3 > 0)
      num5 = num2;
    else if (num4 > 0)
      num5 = num1;
    args.ModifySpeed(num5, num5);
    ent.Comp.OnXenoWeeds = flag2;
    ent.Comp.OnFriendlyWeeds = flag5;
    ent.Comp.OnXenoSlowResin = flag3;
    ent.Comp.OnXenoFastResin = flag4;
    this.Dirty<AffectableByWeedsComponent>(ent);
  }

  private void WeedsOvipositorChanged(
    Entity<AffectableByWeedsComponent> ent,
    ref XenoOvipositorChangedEvent args)
  {
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp((EntityUid) ent, out component) || component.OnXenoSlowResin)
      return;
    this._toUpdate.Add((EntityUid) ent);
  }

  public bool HasWeedsNearby(
    Entity<MapGridComponent> grid,
    EntityCoordinates coordinates,
    int range = 5)
  {
    Vector2i tile = this._mapSystem.LocalToTile((EntityUid) grid, (MapGridComponent) grid, coordinates);
    Box2 localAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref localAABB).\u002Ector((float) (tile.X - range + 1), (float) (tile.Y - range + 1), (float) (tile.X + range), (float) (tile.Y + range));
    foreach (EntityUid localAnchoredEntity in this._mapSystem.GetLocalAnchoredEntities((EntityUid) grid, (MapGridComponent) grid, localAABB))
    {
      XenoWeedsComponent comp;
      if (this.TryComp<XenoWeedsComponent>(localAnchoredEntity, out comp) && comp.IsSource)
        return true;
    }
    return false;
  }

  public bool IsOnHiveWeeds(
    Entity<MapGridComponent> grid,
    EntityCoordinates coordinates,
    bool sourceOnly = false)
  {
    Entity<XenoWeedsComponent>? weedsOnFloor = this.GetWeedsOnFloor(grid, coordinates, sourceOnly);
    XenoWeedsComponent comp;
    EntityPrototype prototype;
    return this.TryComp<XenoWeedsComponent>(weedsOnFloor.HasValue ? new EntityUid?((EntityUid) weedsOnFloor.GetValueOrDefault()) : new EntityUid?(), out comp) && this._prototype.TryIndex(comp.Spawns, out prototype) && prototype.HasComponent<HiveWeedsComponent>();
  }

  public bool IsOnWeeds(
    Entity<MapGridComponent> grid,
    EntityCoordinates coordinates,
    bool sourceOnly = false)
  {
    return this.GetWeedsOnFloor(grid, coordinates, sourceOnly).HasValue;
  }

  public Entity<XenoWeedsComponent>? GetWeedsOnFloor(
    Entity<MapGridComponent> grid,
    EntityCoordinates coordinates,
    bool sourceOnly = false)
  {
    Vector2i tile = this._mapSystem.LocalToTile((EntityUid) grid, (MapGridComponent) grid, coordinates);
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, tile);
    EntityUid? uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      XenoWeedsComponent component;
      if (this._weedsQuery.TryComp(uid, out component) && (!sourceOnly || component.IsSource))
        return new Entity<XenoWeedsComponent>?((Entity<XenoWeedsComponent>) (uid.Value, component));
    }
    return new Entity<XenoWeedsComponent>?();
  }

  public EntityUid? GetWeedsOnFloor(EntityCoordinates coordinates, bool sourceOnly = false)
  {
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Entity<XenoWeedsComponent>? weedsOnFloor = this.GetWeedsOnFloor((Entity<MapGridComponent>) (valueOrDefault, comp), coordinates, sourceOnly);
        return !weedsOnFloor.HasValue ? new EntityUid?() : new EntityUid?((EntityUid) weedsOnFloor.GetValueOrDefault());
      }
    }
    return new EntityUid?();
  }

  public bool IsOnWeeds(Entity<TransformComponent?> entity)
  {
    if (!this.Resolve((EntityUid) entity, ref entity.Comp))
      return false;
    EntityCoordinates grid1 = this._rmcMap.SnapToGrid(this._transform.GetMoverCoordinates((EntityUid) entity, entity.Comp));
    EntityUid? grid2 = this._transform.GetGrid(grid1);
    if (grid2.HasValue)
    {
      EntityUid valueOrDefault = grid2.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
        return this.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), grid1);
    }
    return false;
  }

  public bool IsOnFriendlyWeeds(Entity<TransformComponent?> entity)
  {
    if (!this.Resolve((EntityUid) entity, ref entity.Comp))
      return false;
    EntityCoordinates grid1 = this._rmcMap.SnapToGrid(this._transform.GetMoverCoordinates((EntityUid) entity, entity.Comp));
    EntityUid? grid2 = this._transform.GetGrid(grid1);
    if (grid2.HasValue)
    {
      EntityUid valueOrDefault = grid2.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Entity<XenoWeedsComponent>? weedsOnFloor = this.GetWeedsOnFloor((Entity<MapGridComponent>) (valueOrDefault, comp), grid1);
        return weedsOnFloor.HasValue && this._hive.FromSameHive((Entity<HiveMemberComponent>) entity.Owner, (Entity<HiveMemberComponent>) weedsOnFloor.Value.Owner);
      }
    }
    return false;
  }

  private void OnResinSlowdownStartCollide(
    Entity<ResinSlowdownModifierComponent> ent,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || component.OnXenoSlowResin)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnResinSlowdownEndCollide(
    Entity<ResinSlowdownModifierComponent> ent,
    ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || !component.OnXenoSlowResin)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnResinSpeedupStartCollide(
    Entity<ResinSpeedupModifierComponent> ent,
    ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || component.OnXenoFastResin)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnResinSpeedupEndCollide(
    Entity<ResinSpeedupModifierComponent> ent,
    ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    AffectableByWeedsComponent component;
    if (!this._affectedQuery.TryComp(otherEntity, out component) || !component.OnXenoFastResin)
      return;
    this._toUpdate.Add(otherEntity);
  }

  private void OnSpreadingMapInit(Entity<XenoWeedsSpreadingComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.SpreadAt = this._timing.CurTime + ent.Comp.SpreadDelay;
    this.Dirty<XenoWeedsSpreadingComponent>(ent);
  }

  public bool CanSpreadWeedsPopup(
    Entity<MapGridComponent> grid,
    Vector2i tile,
    EntityUid? user,
    bool semiWeedable = false,
    bool source = false)
  {
    TileRef tile1;
    ITileDefinition definition;
    if (!this._mapSystem.TryGetTileRef((EntityUid) grid, (MapGridComponent) grid, tile, out tile1) || !this._tile.TryGetDefinition(tile1.Tile.TypeId, out definition) || definition.ID == "Space" || definition is ContentTileDefinition contentTileDefinition1 && !contentTileDefinition1.WeedsSpreadable && ((!(definition is ContentTileDefinition contentTileDefinition2) ? 0 : (contentTileDefinition2.SemiWeedable ? 1 : 0)) & (semiWeedable ? 1 : 0)) == 0)
    {
      GenericPopup();
      return false;
    }
    if (!this._area.CanResinPopup((Entity<MapGridComponent, AreaGridComponent>) ((EntityUid) grid, (MapGridComponent) grid, (AreaGridComponent) null), tile, user))
      return false;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, tile);
    EntityUid? uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (this._blockWeedsQuery.HasComp(uid) || source && this.HasComp<XenoResinHoleComponent>(uid))
        return false;
    }
    return true;

    void GenericPopup()
    {
      if (!user.HasValue)
        return;
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-construction-failed-weeds"), user.Value, new EntityUid?(user.Value), PopupType.SmallCaution);
    }
  }

  public bool CanPlaceWeedsPopup(
    EntityUid xeno,
    Entity<MapGridComponent> grid,
    EntityCoordinates coordinates,
    bool limitDistance)
  {
    Entity<XenoWeedsComponent> ent;
    if (this._rmcMap.HasAnchoredEntityEnumerator<XenoWeedsComponent>(coordinates, out ent, facing: (DirectionFlag) 0))
    {
      if (ent.Comp.IsSource)
      {
        this._popup.PopupClient("There's a pod here already!", (EntityUid) ent, new EntityUid?(xeno), PopupType.SmallCaution);
        return false;
      }
      if (ent.Comp.BlockOtherWeeds)
      {
        this._popup.PopupClient("These weeds are too strong to plant a node on!", (EntityUid) ent, new EntityUid?(xeno), PopupType.SmallCaution);
        return false;
      }
    }
    if (limitDistance && !this.HasWeedsNearby(grid, coordinates))
    {
      this._popup.PopupClient("We can only plant weed nodes near other weed nodes our hive owns!", xeno, new EntityUid?(xeno), PopupType.SmallCaution);
      return false;
    }
    foreach (EntityUid anchoredEntity in this._mapSystem.GetAnchoredEntities(grid, coordinates.ToVector2i((IEntityManager) this.EntityManager, this._map, this._transform)))
    {
      if ((this.HasComp<ClimbableComponent>(anchoredEntity) || this.HasComp<RMCReactorPoweredLightComponent>(anchoredEntity)) && !this.HasComp<BarricadeComponent>(anchoredEntity))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-weeds-blocked"), xeno, new EntityUid?(xeno), PopupType.SmallCaution);
        return false;
      }
    }
    return true;
  }

  public void UpdateQueued(EntityUid update)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers(update);
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach (EntityUid update in this._toUpdate)
        this.UpdateQueued(update);
    }
    finally
    {
      this._toUpdate.Clear();
    }
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DamageOffWeedsComponent, DamageableComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DamageOffWeedsComponent, DamageableComponent>();
    EntityUid uid;
    DamageOffWeedsComponent comp1;
    DamageableComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      AffectableByWeedsComponent comp;
      if (this.TryComp<AffectableByWeedsComponent>(uid, out comp) && comp.OnXenoWeeds || this.HasComp<InXenoTunnelComponent>(uid))
      {
        if (comp1.DamageAt.HasValue)
        {
          comp1.DamageAt = new TimeSpan?();
          this.Dirty(uid, (IComponent) comp1);
        }
      }
      else
      {
        if (!comp1.DamageAt.HasValue)
        {
          comp1.DamageAt = new TimeSpan?(curTime + comp1.Every);
          this.Dirty(uid, (IComponent) comp1);
        }
        TimeSpan timeSpan = curTime;
        TimeSpan? damageAt = comp1.DamageAt;
        if ((damageAt.HasValue ? (timeSpan < damageAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          comp1.DamageAt = new TimeSpan?(curTime + comp1.Every);
          BaseContainer container;
          if ((!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid, (TransformComponent) null), out container) || !this._xenoQuery.HasComp(container.Owner)) && (!comp1.RestingStopsDamage || !this.HasComp<XenoRestingComponent>(uid)))
            this._damageable.TryChangeDamage(new EntityUid?(uid), comp1.Damage, damageable: comp2);
        }
      }
    }
  }
}
