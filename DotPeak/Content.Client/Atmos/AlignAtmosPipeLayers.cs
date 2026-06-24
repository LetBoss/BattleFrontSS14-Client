// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.AlignAtmosPipeLayers
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Construction;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.Placement.Modes;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Atmos;

public sealed class AlignAtmosPipeLayers : SnapgridCenter
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IEyeManager _eyeManager;
  private readonly SharedMapSystem _mapSystem;
  private readonly SharedTransformSystem _transformSystem;
  private readonly SharedAtmosPipeLayersSystem _pipeLayersSystem;
  private readonly SpriteSystem _spriteSystem;
  private const float SearchBoxSize = 2f;
  private EntityCoordinates _unalignedMouseCoords;
  private const float MouseDeadzoneRadius = 0.25f;
  private Color _guideColor = new Color(0.0f, 0.0f, 0.5785f, 1f);
  private const float GuideRadius = 0.1f;
  private const float GuideOffset = 0.21875f;

  public AlignAtmosPipeLayers(PlacementManager pMan)
    : base(pMan)
  {
    IoCManager.InjectDependencies<AlignAtmosPipeLayers>(this);
    this._mapSystem = this._entityManager.System<SharedMapSystem>();
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
    this._pipeLayersSystem = this._entityManager.System<SharedAtmosPipeLayersSystem>();
    this._spriteSystem = this._entityManager.System<SpriteSystem>();
  }

  public virtual void Render(in OverlayDrawArgs args)
  {
    EntityUid? grid = this._entityManager.System<SharedTransformSystem>().GetGrid(((PlacementMode) this).MouseCoords);
    if (!grid.HasValue || this.Grid == null)
      return;
    if (((PlacementMode) this).pManager.PlacementType == null)
    {
      Angle worldRotation = this._transformSystem.GetWorldRotation(grid.Value);
      Vector2 world = this._mapSystem.LocalToWorld(grid.Value, this.Grid, ((PlacementMode) this).MouseCoords.Position);
      Angle angle = Angle.op_Addition(Angle.op_Addition(this._eyeManager.CurrentEye.Rotation, worldRotation), Angle.op_Implicit(Math.PI / 2.0));
      Direction cardinalDir = ((Angle) ref angle).GetCardinalDir();
      float num = cardinalDir == 4 || cardinalDir == null ? -1f : 1f;
      ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawCircle(world, 0.1f, this._guideColor, true);
      DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
      Vector2 vector2_1 = world;
      ref Angle local1 = ref worldRotation;
      Vector2 vector2_2 = new Vector2(num * (7f / 32f), 7f / 32f);
      ref Vector2 local2 = ref vector2_2;
      Vector2 vector2_3 = ((Angle) ref local1).RotateVec(ref local2);
      Vector2 vector2_4 = vector2_1 + vector2_3;
      Color guideColor1 = this._guideColor;
      ((DrawingHandleBase) worldHandle1).DrawCircle(vector2_4, 0.1f, guideColor1, true);
      DrawingHandleWorld worldHandle2 = ((OverlayDrawArgs) ref args).WorldHandle;
      Vector2 vector2_5 = world;
      ref Angle local3 = ref worldRotation;
      Vector2 vector2_6 = new Vector2(num * (7f / 32f), 7f / 32f);
      ref Vector2 local4 = ref vector2_6;
      Vector2 vector2_7 = ((Angle) ref local3).RotateVec(ref local4);
      Vector2 vector2_8 = vector2_5 - vector2_7;
      Color guideColor2 = this._guideColor;
      ((DrawingHandleBase) worldHandle2).DrawCircle(vector2_8, 0.1f, guideColor2, true);
    }
    base.Render(ref args);
  }

  public virtual void AlignPlacementMode(ScreenCoordinates mouseScreen)
  {
    this._unalignedMouseCoords = ((PlacementMode) this).ScreenToCursorGrid(mouseScreen);
    base.AlignPlacementMode(mouseScreen);
    if (((PlacementMode) this).pManager.PlacementType != null)
      return;
    ((PlacementMode) this).MouseCoords = CoordinatesExtensions.AlignWithClosestGridTile(this._unalignedMouseCoords, 2f, this._entityManager, this._mapManager);
    EntityUid? grid = this._transformSystem.GetGrid(((PlacementMode) this).MouseCoords);
    MapGridComponent mapGridComponent;
    if (!this._entityManager.TryGetComponent<MapGridComponent>(grid, ref mapGridComponent))
      return;
    Angle worldRotation = this._transformSystem.GetWorldRotation(grid.Value);
    ((PlacementMode) this).CurrentTile = this._mapSystem.GetTileRef(grid.Value, mapGridComponent, ((PlacementMode) this).MouseCoords);
    float tileSize = (float) mapGridComponent.TileSize;
    ((PlacementMode) this).GridDistancing = tileSize;
    EntityUid entityId = ((PlacementMode) this).MouseCoords.EntityId;
    TileRef currentTile1 = ((PlacementMode) this).CurrentTile;
    double x = (double) ((TileRef) ref currentTile1).X + (double) tileSize / 2.0 + (double) ((PlacementMode) this).pManager.PlacementOffset.X;
    TileRef currentTile2 = ((PlacementMode) this).CurrentTile;
    double y = (double) ((TileRef) ref currentTile2).Y + (double) tileSize / 2.0 + (double) ((PlacementMode) this).pManager.PlacementOffset.Y;
    Vector2 vector2_1 = new Vector2((float) x, (float) y);
    ((PlacementMode) this).MouseCoords = new EntityCoordinates(entityId, vector2_1);
    Vector2 vector2_2 = this._unalignedMouseCoords.Position - ((PlacementMode) this).MouseCoords.Position;
    AtmosPipeLayer layer = AtmosPipeLayer.Primary;
    if ((double) vector2_2.Length() > 0.25)
    {
      Angle angle = Angle.op_Addition(Angle.op_Addition(Angle.op_Addition(new Angle(vector2_2), this._eyeManager.CurrentEye.Rotation), worldRotation), Angle.op_Implicit(Math.PI / 2.0));
      Direction cardinalDir = ((Angle) ref angle).GetCardinalDir();
      layer = cardinalDir == 4 || cardinalDir == 2 ? AtmosPipeLayer.Secondary : AtmosPipeLayer.Tertiary;
    }
    if (((PlacementMode) this).pManager.Hijack != null)
      this.UpdateHijackedPlacer(layer, mouseScreen);
    else
      this.UpdatePlacer(layer);
  }

  private void UpdateHijackedPlacer(AtmosPipeLayer layer, ScreenCoordinates mouseScreen)
  {
    ConstructionSystem constructionSystem = ((PlacementMode) this).pManager.Hijack is ConstructionPlacementHijack hijack1 ? hijack1.CurrentConstructionSystem : (ConstructionSystem) null;
    ProtoId<ConstructionPrototype>[] alternativePrototypes = ((PlacementMode) this).pManager.Hijack is ConstructionPlacementHijack hijack2 ? hijack2.CurrentPrototype?.AlternativePrototypes : (ProtoId<ConstructionPrototype>[]) null;
    ConstructionPrototype prototype;
    if (constructionSystem == null || alternativePrototypes == null || layer >= (AtmosPipeLayer) alternativePrototypes.Length || !this._protoManager.TryIndex<ConstructionPrototype>(alternativePrototypes[(int) layer], ref prototype))
      return;
    if (prototype.Type != ConstructionType.Structure)
    {
      ((PlacementMode) this).pManager.Clear();
    }
    else
    {
      if (prototype.ID == (((PlacementMode) this).pManager.Hijack is ConstructionPlacementHijack hijack3 ? hijack3.CurrentPrototype?.ID : (string) null))
        return;
      PlacementManager pManager = ((PlacementMode) this).pManager;
      PlacementInformation placementInformation = new PlacementInformation();
      placementInformation.IsTile = false;
      placementInformation.PlacementOption = prototype.PlacementMode;
      ConstructionPlacementHijack constructionPlacementHijack = new ConstructionPlacementHijack(constructionSystem, prototype);
      pManager.BeginPlacing(placementInformation, (PlacementHijack) constructionPlacementHijack);
      if (((PlacementMode) this).pManager.CurrentMode is AlignAtmosPipeLayers currentMode)
        currentMode.RefreshGrid(mouseScreen);
      constructionSystem.GetGuide(prototype);
    }
  }

  private void UpdatePlacer(AtmosPipeLayer layer)
  {
    EntityPrototype entityPrototype1;
    AtmosPipeLayersComponent component;
    EntProtoId proto;
    EntityPrototype entityPrototype2;
    if (((PlacementMode) this).pManager.CurrentPermission?.EntityType == null || !this._protoManager.TryIndex<EntityPrototype>(((PlacementMode) this).pManager.CurrentPermission.EntityType, ref entityPrototype1) || !entityPrototype1.TryGetComponent<AtmosPipeLayersComponent>(ref component, this._entityManager.ComponentFactory) || !this._pipeLayersSystem.TryGetAlternativePrototype(component, layer, out proto) || !this._protoManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(proto), ref entityPrototype2))
      return;
    ((PlacementMode) this).pManager.CurrentPermission.EntityType = EntProtoId.op_Implicit(proto);
    SpriteComponent spriteComponent;
    if (!entityPrototype2.TryGetComponent<SpriteComponent>(ref spriteComponent, this._entityManager.ComponentFactory))
      return;
    List<IDirectionalTextureProvider> idirectionalTextureProviderList = new List<IDirectionalTextureProvider>();
    foreach (ISpriteLayer allLayer in spriteComponent.AllLayers)
    {
      RSI actualRsi = allLayer.ActualRsi;
      int num;
      if (actualRsi == null)
      {
        num = 0;
      }
      else
      {
        ResPath path = actualRsi.Path;
        num = 1;
      }
      if (num != 0 && allLayer.RsiState.Name != null)
        idirectionalTextureProviderList.Add((IDirectionalTextureProvider) this._spriteSystem.RsiStateLike((SpriteSpecifier) new SpriteSpecifier.Rsi(allLayer.ActualRsi.Path, allLayer.RsiState.Name)));
    }
    ((PlacementMode) this).pManager.CurrentTextures = idirectionalTextureProviderList;
  }

  private void RefreshGrid(ScreenCoordinates mouseScreen) => base.AlignPlacementMode(mouseScreen);
}
