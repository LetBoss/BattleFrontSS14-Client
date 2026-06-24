// Decompiled with JetBrains decompiler
// Type: Content.Shared.Tiles.FloorTileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Tiles;

public sealed class FloorTileSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private ITileDefinitionManager _tileDefinitionManager;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStackSystem _stackSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TileSystem _tile;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private TurfSystem _turf;
  private static readonly Vector2 CheckRange = new Vector2(1f, 1f);
  private readonly HashSet<EntityUid> _turfCheck = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FloorTileComponent, AfterInteractEvent>(new ComponentEventHandler<FloorTileComponent, AfterInteractEvent>(this.OnAfterInteract));
  }

  private void OnAfterInteract(
    EntityUid uid,
    FloorTileComponent component,
    AfterInteractEvent args)
  {
    StackComponent comp1;
    if (!args.CanReach || args.Handled || !this.TryComp<StackComponent>(uid, out comp1) || component.OutputTiles == null)
      return;
    EntityCoordinates entityCoordinates = args.ClickLocation.AlignWithClosestGridTile();
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(entityCoordinates);
    if (mapCoordinates1.MapId == MapId.Nullspace)
      return;
    Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> entityQuery1 = this.GetEntityQuery<PhysicsComponent>();
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> entityQuery2 = this.GetEntityQuery<TransformComponent>();
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(entityCoordinates);
    (bool, EntityUid) state = (true, entityCoordinates.EntityId);
    this._mapManager.FindGridsIntersecting<(bool, EntityUid)>(mapCoordinates2.MapId, new Box2(mapCoordinates2.Position - FloorTileSystem.CheckRange, mapCoordinates2.Position + FloorTileSystem.CheckRange), ref state, (GridCallback<(bool, EntityUid)>) ((EntityUid entityUid, MapGridComponent grid, ref (bool weh, EntityUid EntityId) tuple) =>
    {
      if (tuple.Item2 == entityUid)
        return true;
      tuple.Item1 = false;
      return false;
    }));
    if (!state.Item1)
    {
      if (!this._netManager.IsClient || !this._timing.IsFirstTimePredicted)
        return;
      this._popup.PopupEntity(this.Loc.GetString("invalid-floor-placement"), args.User);
    }
    else
    {
      Vector2 vector2 = this._transform.ToMapCoordinates(entityQuery2.GetComponent(args.User).Coordinates).Position - mapCoordinates2.Position;
      bool flag = false;
      if ((double) vector2.LengthSquared() > 0.01)
      {
        CollisionRay ray = new CollisionRay(mapCoordinates2.Position, Vector2Helpers.Normalized(vector2), 2);
        flag = !this._physics.IntersectRay(mapCoordinates1.MapId, ray, vector2.Length()).Any<RayCastResults>();
      }
      TileRef? tile;
      if (!flag && this._turf.TryGetTileRef(entityCoordinates, out tile))
      {
        this._turfCheck.Clear();
        this._lookup.GetEntitiesInTile(tile.Value, this._turfCheck);
        foreach (EntityUid uid1 in this._turfCheck)
        {
          PhysicsComponent component1;
          if (entityQuery1.TryGetComponent(uid1, out component1) && component1.BodyType == BodyType.Static && component1.Hard && (component1.CollisionLayer & 2) != 0)
            return;
        }
      }
      MapGridComponent comp2;
      this.TryComp<MapGridComponent>(entityCoordinates.EntityId, out comp2);
      foreach (string outputTile in component.OutputTiles)
      {
        ContentTileDefinition tileDef = (ContentTileDefinition) this._tileDefinitionManager[outputTile];
        if (comp2 != null)
        {
          EntityUid entityId = entityCoordinates.EntityId;
          TileRef tileRef = this._map.GetTileRef(entityId, comp2, entityCoordinates);
          string reason;
          if (!this.CanPlaceTile(entityId, comp2, tileRef.GridIndices, out reason))
          {
            this._popup.PopupClient(reason, args.User, new EntityUid?(args.User));
            break;
          }
          ContentTileDefinition contentTileDefinition = (ContentTileDefinition) this._tileDefinitionManager[tileRef.Tile.TypeId];
          if (this.HasBaseTurf(tileDef, contentTileDefinition.ID) && this._stackSystem.Use(uid, 1, comp1))
          {
            this.PlaceAt(args.User, entityId, comp2, entityCoordinates, tileDef.TileId, component.PlaceTileSound);
            args.Handled = true;
            break;
          }
        }
        else if (this.HasBaseTurf(tileDef, "Space") && this._stackSystem.Use(uid, 1, comp1))
        {
          args.Handled = true;
          if (this._netManager.IsClient)
            break;
          Entity<MapGridComponent> gridEntity = this._mapManager.CreateGridEntity(mapCoordinates1.MapId);
          TransformComponent transformComponent = this.Transform((EntityUid) gridEntity);
          this._transform.SetWorldPosition((Entity<TransformComponent>) ((EntityUid) gridEntity, transformComponent), mapCoordinates1.Position);
          EntityCoordinates location = new EntityCoordinates((EntityUid) gridEntity, Vector2.Zero);
          this.PlaceAt(args.User, (EntityUid) gridEntity, gridEntity.Comp, location, this._tileDefinitionManager[component.OutputTiles[0]].TileId, component.PlaceTileSound, (float) gridEntity.Comp.TileSize / 2f);
          break;
        }
      }
    }
  }

  public bool HasBaseTurf(ContentTileDefinition tileDef, string baseTurf)
  {
    return tileDef.BaseTurf == baseTurf;
  }

  private void PlaceAt(
    EntityUid user,
    EntityUid gridUid,
    MapGridComponent mapGrid,
    EntityCoordinates location,
    ushort tileId,
    SoundSpecifier placeSound,
    float offset = 0.0f)
  {
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(18, 4);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "actor", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" placed tile ");
    logStringHandler.AppendFormatted(this._tileDefinitionManager[(int) tileId].Name);
    logStringHandler.AppendLiteral(" at ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) gridUid), "ToPrettyString(gridUid)");
    logStringHandler.AppendLiteral(" ");
    logStringHandler.AppendFormatted<EntityCoordinates>(location, nameof (location));
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Tile, LogImpact.Low, ref local);
    Random random = new Random((int) this._timing.CurTick.Value);
    byte variant = this._tile.PickVariant((ContentTileDefinition) this._tileDefinitionManager[(int) tileId], random);
    this._map.SetTile(gridUid, mapGrid, location.Offset(new Vector2(offset, offset)), new Tile((int) tileId, variant: variant));
    this._audio.PlayPredicted(placeSound, location, new EntityUid?(user));
  }

  public bool CanPlaceTile(
    EntityUid gridUid,
    MapGridComponent component,
    Vector2i gridIndices,
    [NotNullWhen(false)] out string? reason)
  {
    FloorTileAttemptEvent args = new FloorTileAttemptEvent(gridIndices);
    this.RaiseLocalEvent<FloorTileAttemptEvent>(gridUid, ref args);
    if (args.Cancelled)
    {
      reason = this.Loc.GetString("invalid-floor-placement");
      return false;
    }
    reason = (string) null;
    return true;
  }
}
