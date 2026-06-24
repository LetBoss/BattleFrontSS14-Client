// Decompiled with JetBrains decompiler
// Type: Content.Client.RCD.AlignRCDConstruction
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.Hands.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.RCD.Components;
using Content.Shared.RCD.Systems;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System.Numerics;

#nullable enable
namespace Content.Client.RCD;

public sealed class AlignRCDConstruction : PlacementMode
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IMapManager _mapManager;
  private readonly SharedMapSystem _mapSystem;
  private readonly HandsSystem _handsSystem;
  private readonly RCDSystem _rcdSystem;
  private readonly SharedTransformSystem _transformSystem;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IStateManager _stateManager;
  private const float SearchBoxSize = 2f;
  private const float PlaceColorBaseAlpha = 0.5f;
  private EntityCoordinates _unalignedMouseCoords;

  public AlignRCDConstruction(PlacementManager pMan)
    : base(pMan)
  {
    IoCManager.InjectDependencies<AlignRCDConstruction>(this);
    this._mapSystem = this._entityManager.System<SharedMapSystem>();
    this._handsSystem = this._entityManager.System<HandsSystem>();
    this._rcdSystem = this._entityManager.System<RCDSystem>();
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
    Color validPlaceColor = this.ValidPlaceColor;
    this.ValidPlaceColor = ((Color) ref validPlaceColor).WithAlpha(0.5f);
  }

  public virtual void AlignPlacementMode(ScreenCoordinates mouseScreen)
  {
    this._unalignedMouseCoords = this.ScreenToCursorGrid(mouseScreen);
    this.MouseCoords = CoordinatesExtensions.AlignWithClosestGridTile(this._unalignedMouseCoords, 2f, this._entityManager, this._mapManager);
    EntityUid? grid = this._transformSystem.GetGrid(this.MouseCoords);
    MapGridComponent mapGridComponent;
    if (!this._entityManager.TryGetComponent<MapGridComponent>(grid, ref mapGridComponent))
      return;
    this.CurrentTile = this._mapSystem.GetTileRef(grid.Value, mapGridComponent, this.MouseCoords);
    float tileSize = (float) mapGridComponent.TileSize;
    this.GridDistancing = tileSize;
    if (this.pManager.CurrentPermission.IsTile)
    {
      EntityUid entityId = this.MouseCoords.EntityId;
      TileRef currentTile1 = this.CurrentTile;
      double x = (double) ((TileRef) ref currentTile1).X + (double) tileSize / 2.0;
      TileRef currentTile2 = this.CurrentTile;
      double y = (double) ((TileRef) ref currentTile2).Y + (double) tileSize / 2.0;
      Vector2 vector2 = new Vector2((float) x, (float) y);
      this.MouseCoords = new EntityCoordinates(entityId, vector2);
    }
    else
    {
      EntityUid entityId = this.MouseCoords.EntityId;
      TileRef currentTile3 = this.CurrentTile;
      double x = (double) ((TileRef) ref currentTile3).X + (double) tileSize / 2.0 + (double) this.pManager.PlacementOffset.X;
      TileRef currentTile4 = this.CurrentTile;
      double y = (double) ((TileRef) ref currentTile4).Y + (double) tileSize / 2.0 + (double) this.pManager.PlacementOffset.Y;
      Vector2 vector2 = new Vector2((float) x, (float) y);
      this.MouseCoords = new EntityCoordinates(entityId, vector2);
    }
  }

  public virtual bool IsValidPosition(EntityCoordinates position)
  {
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    TransformComponent transformComponent;
    if (!this._entityManager.TryGetComponent<TransformComponent>(attachedEntity, ref transformComponent))
      return false;
    if (!this._transformSystem.InRange(transformComponent.Coordinates, position, 1.5f))
    {
      Color invalidPlaceColor = this.InvalidPlaceColor;
      this.InvalidPlaceColor = ((Color) ref invalidPlaceColor).WithAlpha((byte) 0);
      return false;
    }
    Color invalidPlaceColor1 = this.InvalidPlaceColor;
    this.InvalidPlaceColor = ((Color) ref invalidPlaceColor1).WithAlpha(0.5f);
    EntityUid? nullable;
    RCDComponent component;
    if (!this._handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit(attachedEntity.Value), out nullable) || !this._entityManager.TryGetComponent<RCDComponent>(nullable, ref component))
      return false;
    EntityUid? grid = this._transformSystem.GetGrid(position);
    MapGridComponent mapGrid;
    if (!this._entityManager.TryGetComponent<MapGridComponent>(grid, ref mapGrid))
      return false;
    TileRef tileRef = this._mapSystem.GetTileRef(grid.Value, mapGrid, position);
    Vector2i position1 = this._mapSystem.TileIndicesFor(grid.Value, mapGrid, position);
    if (!(this._stateManager.CurrentState is GameplayStateBase currentState))
      return false;
    EntityUid? clickedEntity = currentState.GetClickedEntity(this._transformSystem.ToMapCoordinates(this._unalignedMouseCoords, true));
    return this._rcdSystem.IsRCDOperationStillValid(nullable.Value, component, grid.Value, mapGrid, tileRef, position1, clickedEntity, attachedEntity.Value, false);
  }
}
