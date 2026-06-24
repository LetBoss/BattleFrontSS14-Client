// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Entrenching.BarricadeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.Construction;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Entrenching;

public sealed class BarricadeSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private RMCConstructionSystem _rmcConstruction;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private ITileDefinitionManager _tiles;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private UseDelaySystem _useDelay;
  private Robust.Shared.GameObjects.EntityQuery<BarricadeComponent> _barricadeQuery;

  public override void Initialize()
  {
    this._barricadeQuery = this.GetEntityQuery<BarricadeComponent>();
    this.SubscribeLocalEvent<EntrenchingToolComponent, AfterInteractEvent>(new EntityEventRefHandler<EntrenchingToolComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<EntrenchingToolComponent, EntrenchingToolDoAfterEvent>(new EntityEventRefHandler<EntrenchingToolComponent, EntrenchingToolDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<EntrenchingToolComponent, ItemToggledEvent>(new EntityEventRefHandler<EntrenchingToolComponent, ItemToggledEvent>(this.OnItemToggled));
    this.SubscribeLocalEvent<EntrenchingToolComponent, SandbagFillDoAfterEvent>(new EntityEventRefHandler<EntrenchingToolComponent, SandbagFillDoAfterEvent>(this.OnSandbagFillDoAfter));
    this.SubscribeLocalEvent<EntrenchingToolComponent, SandbagDismantleDoAfterEvent>(new EntityEventRefHandler<EntrenchingToolComponent, SandbagDismantleDoAfterEvent>(this.OnSandbagDismantleDoAfter));
    this.SubscribeLocalEvent<EmptySandbagComponent, InteractUsingEvent>(new EntityEventRefHandler<EmptySandbagComponent, InteractUsingEvent>(this.OnEmptyInteractUsing));
    this.SubscribeLocalEvent<FullSandbagComponent, ActivateInWorldEvent>(new EntityEventRefHandler<FullSandbagComponent, ActivateInWorldEvent>(this.OnFullActivateInWorld));
    this.SubscribeLocalEvent<FullSandbagComponent, AfterInteractEvent>(new EntityEventRefHandler<FullSandbagComponent, AfterInteractEvent>(this.OnFullAfterInteract));
    this.SubscribeLocalEvent<FullSandbagComponent, SandbagBuildDoAfterEvent>(new EntityEventRefHandler<FullSandbagComponent, SandbagBuildDoAfterEvent>(this.OnFullBuildDoAfter));
  }

  private void OnAfterInteract(Entity<EntrenchingToolComponent> tool, ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    if (this.HasComp<BarricadeSandbagComponent>(args.Target))
    {
      this.DismantleSandbagBaricade(tool, ref args);
      args.Handled = true;
    }
    else
    {
      this.StartDigging(tool, args.User, args.ClickLocation);
      args.Handled = true;
    }
  }

  private void DismantleSandbagBaricade(
    Entity<EntrenchingToolComponent> tool,
    ref AfterInteractEvent args)
  {
    ItemToggleComponent comp;
    if (this.TryComp<ItemToggleComponent>((EntityUid) tool, out comp) && !comp.Activated)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-entrenching-dismantle"), args.User, new EntityUid?(args.User));
    SandbagDismantleDoAfterEvent @event = new SandbagDismantleDoAfterEvent(this.GetNetCoordinates(args.ClickLocation));
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, tool.Comp.DigDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) tool), args.Target, new EntityUid?((EntityUid) tool))
    {
      BreakOnMove = true
    });
  }

  private void OnSandbagDismantleDoAfter(
    Entity<EntrenchingToolComponent> tool,
    ref SandbagDismantleDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    BarricadeSandbagComponent comp1;
    if (this._net.IsClient || !this.TryComp<BarricadeSandbagComponent>(args.Target, out comp1))
      return;
    EntityUid uid = this.Spawn((string) comp1.Material, this.GetCoordinates(args.Coordinates));
    int amount = comp1.MaxMaterial;
    FullSandbagComponent comp2;
    if (amount <= 0 && this.TryComp<FullSandbagComponent>(uid, out comp2))
      amount = comp2.StackRequired;
    DamageableComponent comp3;
    if (this.TryComp<DamageableComponent>(args.Target, out comp3))
      amount -= Math.Max((int) comp3.TotalDamage / comp1.MaterialLossDamageInterval - 1, 0);
    BarbedComponent comp4;
    if (this.TryComp<BarbedComponent>(args.Target, out comp4) && comp4.IsBarbed)
      this.Spawn((string) comp4.Spawn, this.GetCoordinates(args.Coordinates));
    this.Del(args.Target);
    if (amount <= 0)
    {
      this.Del(new EntityUid?(uid));
    }
    else
    {
      StackComponent comp5;
      if (!this.TryComp<StackComponent>(uid, out comp5))
        return;
      this._stack.SetCount(uid, amount, comp5);
    }
  }

  private void OnDoAfter(
    Entity<EntrenchingToolComponent> tool,
    ref EntrenchingToolDoAfterEvent args)
  {
    if (args.Handled)
      return;
    if (args.Cancelled)
    {
      this._popup.PopupClient(this.Loc.GetString("cm-entrenching-stop-digging"), args.User, new EntityUid?(args.User));
    }
    else
    {
      EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
      if (!this.CanDig(tool, args.User, coordinates, false, out Entity<MapGridComponent> _, out TileRef _))
        return;
      args.Handled = true;
      tool.Comp.TotalLayers = tool.Comp.LayersPerDig;
      this.Dirty<EntrenchingToolComponent>(tool);
      using (HashSet<Entity<EmptySandbagComponent>>.Enumerator enumerator = this._lookup.GetEntitiesInRange<EmptySandbagComponent>(this._transform.GetMoverCoordinates(args.User), 1.5f).GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        Entity<EmptySandbagComponent> current = enumerator.Current;
        SandbagFillDoAfterEvent @event = new SandbagFillDoAfterEvent();
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, tool.Comp.FillDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) tool), new EntityUid?((EntityUid) current), new EntityUid?((EntityUid) tool))
        {
          BreakOnMove = true
        });
        this._popup.PopupClient(this.Loc.GetString("cm-entrenching-begin-filling"), args.User, new EntityUid?(args.User));
      }
    }
  }

  private void OnItemToggled(Entity<EntrenchingToolComponent> tool, ref ItemToggledEvent args)
  {
    tool.Comp.TotalLayers = 0;
    this.Dirty<EntrenchingToolComponent>(tool);
  }

  private void OnSandbagFillDoAfter(
    Entity<EntrenchingToolComponent> tool,
    ref SandbagFillDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    HashSet<Entity<EmptySandbagComponent>> entitiesInRange = this._lookup.GetEntitiesInRange<EmptySandbagComponent>(this._transform.GetMoverCoordinates(args.User), 1.5f);
    bool flag = false;
    foreach (Entity<EmptySandbagComponent> entity in entitiesInRange)
    {
      if (flag)
      {
        args.Repeat = true;
        break;
      }
      flag = true;
      this.Fill(tool, entity, args.User, tool.Comp.TotalLayers);
      if (!this.TerminatingOrDeleted((EntityUid) entity))
      {
        args.Repeat = true;
        break;
      }
    }
    if (tool.Comp.TotalLayers > 0)
      return;
    args.Repeat = false;
    this.StartDigging(tool, args.User, tool.Comp.LastDigLocation);
  }

  private void OnEmptyInteractUsing(
    Entity<EmptySandbagComponent> empty,
    ref InteractUsingEvent args)
  {
    EntrenchingToolComponent comp;
    if (this._net.IsClient || args.Handled || !this.TryComp<EntrenchingToolComponent>(args.Used, out comp))
      return;
    args.Handled = true;
    Entity<EntrenchingToolComponent> entity = new Entity<EntrenchingToolComponent>(args.Used, comp);
    SandbagFillDoAfterEvent @event = new SandbagFillDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, entity.Comp.FillDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) entity), new EntityUid?((EntityUid) empty), new EntityUid?((EntityUid) entity))
    {
      BreakOnMove = true
    });
    this._popup.PopupClient(this.Loc.GetString("cm-entrenching-begin-filling"), args.User, new EntityUid?(args.User));
  }

  private void OnFullActivateInWorld(
    Entity<FullSandbagComponent> full,
    ref ActivateInWorldEvent args)
  {
    TransformComponent comp;
    if (args.Handled || !this.TryComp(args.User, out comp))
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(args.User, comp);
    Angle localRotation = comp.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    bool handled;
    if (!this.Build(full, args.User, moverCoordinates, cardinalDir, out handled))
      return;
    args.Handled = handled;
  }

  private void OnFullAfterInteract(Entity<FullSandbagComponent> full, ref AfterInteractEvent args)
  {
    TransformComponent comp;
    if (args.Handled || !args.CanReach || !this.TryComp(args.User, out comp))
      return;
    Angle localRotation = comp.LocalRotation;
    Direction cardinalDir = ((Angle) ref localRotation).GetCardinalDir();
    bool handled;
    if (!this.Build(full, args.User, args.ClickLocation, cardinalDir, out handled))
      return;
    args.Handled = handled;
  }

  private void OnFullBuildDoAfter(
    Entity<FullSandbagComponent> full,
    ref SandbagBuildDoAfterEvent args)
  {
    if (this._net.IsClient || args.Cancelled || args.Handled)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    EntityUid uid;
    MapGridComponent grid;
    TileRef? tile;
    if (!this._mapManager.TryFindGridAt(this._transform.ToMapCoordinates(coordinates), out uid, out grid) || !this._interaction.InRangeUnobstructed((EntityUid) full, coordinates) || !this._turf.TryGetTileRef(coordinates, out tile) || !this.CanBuild(full, (Entity<MapGridComponent>) (uid, grid), args.User, tile.Value, args.Direction))
      return;
    if (full.Comp.StackRequired > 1)
    {
      int count = this._stack.GetCount((EntityUid) full);
      if (count < full.Comp.StackRequired)
        return;
      StackComponent comp;
      if (this.TryComp<StackComponent>((EntityUid) full, out comp))
        this._stack.SetCount((EntityUid) full, count - full.Comp.StackRequired, comp);
      else
        this.QueueDel(new EntityUid?((EntityUid) full));
    }
    this._transform.SetLocalRotation(this.SpawnAtPosition((string) full.Comp.Builds, coordinates), DirectionExtensions.ToAngle(args.Direction));
    args.Handled = true;
  }

  private bool StartDigging(
    Entity<EntrenchingToolComponent> tool,
    EntityUid user,
    EntityCoordinates clicked)
  {
    Entity<MapGridComponent> grid;
    TileRef tileRef;
    if (!this.CanDig(tool, user, clicked, true, out grid, out tileRef))
      return false;
    EntityCoordinates local = this._mapSystem.GridTileToLocal((EntityUid) grid, (MapGridComponent) grid, tileRef.GridIndices);
    tool.Comp.LastDigLocation = local;
    this.Dirty<EntrenchingToolComponent>(tool);
    EntrenchingToolDoAfterEvent toolDoAfterEvent = new EntrenchingToolDoAfterEvent(this.GetNetCoordinates(local));
    EntityManager entityManager = this.EntityManager;
    EntityUid user1 = user;
    TimeSpan digDelay = tool.Comp.DigDelay;
    EntrenchingToolDoAfterEvent @event = toolDoAfterEvent;
    EntityUid? eventTarget = new EntityUid?((EntityUid) tool);
    EntityUid? nullable = new EntityUid?((EntityUid) tool);
    EntityUid? target = new EntityUid?();
    EntityUid? used = nullable;
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user1, digDelay, (DoAfterEvent) @event, eventTarget, target, used)
    {
      BreakOnMove = true,
      NeedHand = true,
      BreakOnHandChange = true
    });
    this._popup.PopupClient(this.Loc.GetString("cm-entrenching-start-digging"), user, new EntityUid?(user));
    this._audio.PlayPredicted(tool.Comp.DigSound, user, new EntityUid?(user));
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) tool, out comp))
      this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) tool, comp));
    return true;
  }

  private bool Fill(
    Entity<EntrenchingToolComponent> tool,
    Entity<EmptySandbagComponent> empty,
    EntityUid user,
    int amount)
  {
    if (tool.Comp.TotalLayers < amount)
      return false;
    int val1 = amount;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) empty);
    StackComponent comp;
    if (this.TryComp<StackComponent>((EntityUid) empty, out comp))
    {
      int count = this._stack.GetCount((EntityUid) empty, comp);
      int amount1 = Math.Min(val1, count);
      this._stack.SetCount((EntityUid) empty, count - amount1, comp);
      if (this._net.IsServer)
      {
        EntityUid uid = this.Spawn((string) empty.Comp.Filled, moverCoordinates);
        StackComponent component = this.EnsureComp<StackComponent>(uid);
        this._stack.SetCount(uid, amount1, component);
      }
    }
    else if (this._net.IsServer)
      this.Del(new EntityUid?((EntityUid) empty));
    tool.Comp.TotalLayers -= amount;
    this.Dirty<EntrenchingToolComponent>(tool);
    this._audio.PlayPredicted(tool.Comp.FillSound, user, new EntityUid?(user));
    return true;
  }

  private bool Build(
    Entity<FullSandbagComponent> full,
    EntityUid user,
    EntityCoordinates coordinates,
    Direction direction,
    out bool handled)
  {
    handled = false;
    EntityUid uid;
    MapGridComponent grid;
    TileRef? tile;
    if (!this._mapManager.TryFindGridAt(this._transform.ToMapCoordinates(coordinates), out uid, out grid) || !this._turf.TryGetTileRef(coordinates, out tile))
      return false;
    handled = true;
    if (!this.CanBuild(full, (Entity<MapGridComponent>) (uid, grid), user, tile.Value, direction))
      return false;
    SandbagBuildDoAfterEvent @event = new SandbagBuildDoAfterEvent(this.GetNetCoordinates(coordinates), direction);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, full.Comp.BuildDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) full), new EntityUid?((EntityUid) full))
    {
      BreakOnMove = true
    });
    return true;
  }

  private bool CanDig(
    Entity<EntrenchingToolComponent> tool,
    EntityUid user,
    EntityCoordinates coordinates,
    bool checkUseDelay,
    out Entity<MapGridComponent> grid,
    out TileRef tileRef)
  {
    grid = new Entity<MapGridComponent>();
    tileRef = new TileRef();
    UseDelayComponent comp1;
    ItemToggleComponent comp2;
    EntityUid uid;
    MapGridComponent grid1;
    if (checkUseDelay && this.TryComp<UseDelayComponent>((EntityUid) tool, out comp1) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) tool, comp1)) || !this._interaction.InRangeUnobstructed(user, coordinates) || this.TryComp<ItemToggleComponent>((EntityUid) tool, out comp2) && !comp2.Activated || !this._mapManager.TryFindGridAt(this._transform.ToMapCoordinates(coordinates), out uid, out grid1))
      return false;
    tileRef = this._mapSystem.GetTileRef(uid, grid1, coordinates);
    if (!((ContentTileDefinition) this._tiles[tileRef.Tile.TypeId]).CanDig || !this.TileSolidAndNotBlocked(tileRef))
      return false;
    grid = (Entity<MapGridComponent>) (uid, grid1);
    return true;
  }

  private bool TileSolidAndNotBlocked(TileRef tile)
  {
    return !this._turf.IsSpace(tile) && this._turf.GetContentTileDefinition(tile).Sturdy && !this._turf.IsTileBlocked(tile, CollisionGroup.Impassable);
  }

  private bool CanBuild(
    Entity<FullSandbagComponent> full,
    Entity<MapGridComponent> grid,
    EntityUid user,
    TileRef tile,
    Direction direction)
  {
    StackComponent comp1;
    if (!this.TryComp<StackComponent>((EntityUid) full, out comp1) || comp1.Count < 5)
      return false;
    EntityCoordinates entityCoordinates = new EntityCoordinates(tile.GridUid, (float) tile.X, (float) tile.Y).Offset(grid.Comp.TileSizeHalfVector);
    CollisionGroup collisionMask = CollisionGroup.TableMask | CollisionGroup.InteractImpassable;
    bool isClient = this._net.IsClient;
    if (!this._interaction.InRangeUnobstructed(user, entityCoordinates, collisionMask: collisionMask, popup: isClient) || !this.TileSolidAndNotBlocked(tile))
      return false;
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator((EntityUid) grid, (MapGridComponent) grid, tile.GridIndices);
    EntityUid? uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      TransformComponent comp2;
      if (this.HasComp<BarricadeComponent>(uid) && this.TryComp(uid, out comp2))
      {
        Angle localRotation = comp2.LocalRotation;
        if (((Angle) ref localRotation).GetCardinalDir() == direction)
          return false;
      }
    }
    EntityPrototype prototype;
    string prototypeName = this._prototype.TryIndex(full.Comp.Builds, out prototype) ? prototype.Name : this.Name((EntityUid) full);
    string popup;
    if (this._rmcConstruction.CanBuildAt(entityCoordinates, prototypeName, out popup, direction: (Direction) -1))
      return true;
    if (isClient)
      this._popup.PopupClient(popup, user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  public bool HasBarricadeFacing(EntityCoordinates coordinates, Direction direction)
  {
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        Vector2i pos = this._mapSystem.TileIndicesFor(valueOrDefault, comp, coordinates);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, pos);
        EntityUid? uid;
        while (entitiesEnumerator.MoveNext(out uid))
        {
          if (this._barricadeQuery.HasComp(uid))
          {
            Angle worldRotation = this._transform.GetWorldRotation(uid.Value);
            if (((Angle) ref worldRotation).GetCardinalDir() == direction)
              return true;
          }
        }
        return false;
      }
    }
    return false;
  }

  public bool HasBarricadeNearbyPopup(
    Entity<MapGridComponent> grid,
    EntityUid user,
    EntityCoordinates coordinates,
    int range)
  {
    Vector2i tile = this._mapSystem.LocalToTile((EntityUid) grid, (MapGridComponent) grid, coordinates);
    Box2 localAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref localAABB).\u002Ector((float) (tile.X - range), (float) (tile.Y - range), (float) (tile.X + range), (float) (tile.Y + range));
    foreach (EntityUid localAnchoredEntity in this._mapSystem.GetLocalAnchoredEntities((EntityUid) grid, (MapGridComponent) grid, localAABB))
    {
      if (this.HasComp<BarricadeComponent>(localAnchoredEntity))
      {
        this._popup.PopupClient(this.Loc.GetString("barricade-anchored-too-close", ("barricade", (object) localAnchoredEntity)), user, new EntityUid?(user), PopupType.SmallCaution);
        return true;
      }
    }
    return false;
  }
}
