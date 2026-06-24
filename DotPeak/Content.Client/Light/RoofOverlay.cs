// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.RoofOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Light;

public sealed class RoofOverlay : Overlay
{
  private const int LeakDepth = 2;
  private static readonly Vector2i[] LeakDirs = new Vector2i[4]
  {
    new Vector2i(0, -1),
    new Vector2i(1, 0),
    new Vector2i(0, 1),
    new Vector2i(-1, 0)
  };
  private readonly IEntityManager _entManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IOverlayManager _overlay;
  private readonly EntityLookupSystem _lookup;
  private readonly SharedMapSystem _mapSystem;
  private readonly SharedRoofSystem _roof;
  private readonly SharedTransformSystem _xformSystem;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();
  private readonly Queue<(Vector2i Tile, int Depth)> _open = new Queue<(Vector2i, int)>();
  private readonly Dictionary<Vector2i, int> _leaks = new Dictionary<Vector2i, int>();
  private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();
  public const int ContentZIndex = -9;

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public RoofOverlay(IEntityManager entManager)
  {
    this._entManager = entManager;
    IoCManager.InjectDependencies<RoofOverlay>(this);
    this._lookup = this._entManager.System<EntityLookupSystem>();
    this._mapSystem = this._entManager.System<SharedMapSystem>();
    this._roof = this._entManager.System<SharedRoofSystem>();
    this._xformSystem = this._entManager.System<SharedTransformSystem>();
    this.ZIndex = new int?(-9);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.Viewport.Eye == null || !this._entManager.HasComponent<MapLightComponent>(args.MapUid))
      return;
    IClydeViewport viewport = args.Viewport;
    IEye eye = args.Viewport.Eye;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    BeforeLightTargetOverlay overlay = this._overlay.GetOverlay<BeforeLightTargetOverlay>();
    Box2Rotated bounds = overlay.EnlargedBounds;
    IRenderTexture target = overlay.EnlargedLightTarget;
    this._grids.Clear();
    this._mapManager.FindGridsIntersecting(args.MapId, bounds, ref this._grids, true, true);
    Vector2 vector2 = Vector2i.op_Implicit(((IRenderTarget) viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 scale = viewport.RenderScale / (Vector2.One / vector2);
    ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) target, (Action) (() =>
    {
      Matrix3x2 worldToLocalMatrix = ((IRenderTarget) target).GetWorldToLocalMatrix(eye, scale);
      for (int index = 0; index < this._grids.Count; ++index)
      {
        Entity<MapGridComponent> grid = this._grids[index];
        ImplicitRoofComponent implicitRoofComponent;
        if (this._entManager.TryGetComponent<ImplicitRoofComponent>(grid.Owner, ref implicitRoofComponent))
        {
          Matrix3x2 matrix3x2 = Matrix3x2.Multiply(this._xformSystem.GetWorldMatrix(grid.Owner), worldToLocalMatrix);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
          SharedMapSystem.TilesEnumerator tilesEnumerator = this._mapSystem.GetTilesEnumerator(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), bounds, true, (Predicate<TileRef>) null);
          Color color = implicitRoofComponent.Color;
          TileRef tileRef;
          while (((SharedMapSystem.TilesEnumerator) ref tilesEnumerator).MoveNext(ref tileRef))
            worldHandle.DrawRect(this._lookup.GetLocalBounds(tileRef, grid.Comp.TileSize), color, true);
          this._grids.RemoveAt(index);
          --index;
        }
      }
    }), new Color?());
    ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) target, (Action) (() =>
    {
      Matrix3x2 worldToLocalMatrix = ((IRenderTarget) target).GetWorldToLocalMatrix(eye, scale);
      using (List<Entity<MapGridComponent>>.Enumerator enumerator = this._grids.GetEnumerator())
      {
label_9:
        while (enumerator.MoveNext())
        {
          Entity<MapGridComponent> current = enumerator.Current;
          RoofComponent roofComponent;
          if (this._entManager.TryGetComponent<RoofComponent>(current.Owner, ref roofComponent))
          {
            Matrix3x2 matrix3x2 = Matrix3x2.Multiply(this._xformSystem.GetWorldMatrix(current.Owner), worldToLocalMatrix);
            ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
            SharedMapSystem.TilesEnumerator tilesEnumerator = this._mapSystem.GetTilesEnumerator(current.Owner, Entity<MapGridComponent>.op_Implicit(current), bounds, true, (Predicate<TileRef>) null);
            (EntityUid, MapGridComponent, RoofComponent) valueTuple = (current.Owner, current.Comp, roofComponent);
            this.FillLeaks(Entity<MapGridComponent, RoofComponent>.op_Implicit(valueTuple), bounds);
            while (true)
            {
              TileRef tileRef;
              Color? color1;
              do
              {
                if (((SharedMapSystem.TilesEnumerator) ref tilesEnumerator).MoveNext(ref tileRef))
                  color1 = this._roof.GetColor(Entity<MapGridComponent, RoofComponent>.op_Implicit(valueTuple), tileRef.GridIndices);
                else
                  goto label_9;
              }
              while (!color1.HasValue);
              int depth;
              if (this._leaks.TryGetValue(tileRef.GridIndices, out depth))
              {
                ref Color? local = ref color1;
                Color color2 = color1.Value;
                Color color3 = ((Color) ref color2).WithAlpha(color1.Value.A * RoofOverlay.GetLeakAlpha(depth));
                local = new Color?(color3);
              }
              worldHandle.DrawRect(this._lookup.GetLocalBounds(tileRef, current.Comp.TileSize), color1.Value, true);
            }
          }
        }
      }
    }), new Color?());
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local1);
  }

  private void FillLeaks(Entity<MapGridComponent, RoofComponent> roof, Box2Rotated bounds)
  {
    this._open.Clear();
    this._leaks.Clear();
    SharedMapSystem.TilesEnumerator tilesEnumerator = this._mapSystem.GetTilesEnumerator(roof.Owner, roof.Comp1, bounds, true, (Predicate<TileRef>) null);
    TileRef tileRef;
    Color? color;
    while (((SharedMapSystem.TilesEnumerator) ref tilesEnumerator).MoveNext(ref tileRef))
    {
      Vector2i gridIndices = tileRef.GridIndices;
      color = this._roof.GetColor(roof, gridIndices);
      if (!color.HasValue && !this.BlocksLeak(roof.Owner, gridIndices))
      {
        foreach (Vector2i leakDir in RoofOverlay.LeakDirs)
        {
          Vector2i vector2i = Vector2i.op_Addition(gridIndices, leakDir);
          if (!this._leaks.ContainsKey(vector2i))
          {
            color = this._roof.GetColor(roof, vector2i);
            if (color.HasValue && !this.BlocksLeak(roof.Owner, vector2i))
              this._open.Enqueue((vector2i, 1));
          }
        }
      }
    }
    (Vector2i Tile, int Depth) result;
    while (this._open.TryDequeue(out result))
    {
      if (!this._leaks.ContainsKey(result.Tile))
      {
        this._leaks[result.Tile] = result.Depth;
        if (result.Depth < 2)
        {
          foreach (Vector2i leakDir in RoofOverlay.LeakDirs)
          {
            Vector2i vector2i = Vector2i.op_Addition(result.Tile, leakDir);
            if (!this._leaks.ContainsKey(vector2i))
            {
              color = this._roof.GetColor(roof, vector2i);
              if (color.HasValue && !this.BlocksLeak(roof.Owner, vector2i))
                this._open.Enqueue((vector2i, result.Depth + 1));
            }
          }
        }
      }
    }
  }

  private bool BlocksLeak(EntityUid gridUid, Vector2i tile)
  {
    this._occluders.Clear();
    this._lookup.GetLocalEntitiesIntersecting<OccluderComponent>(gridUid, tile, this._occluders, -0.04f, (LookupFlags) 110, (MapGridComponent) null);
    foreach (Entity<OccluderComponent> occluder in this._occluders)
    {
      if (occluder.Comp.Enabled)
        return true;
    }
    return false;
  }

  private static float GetLeakAlpha(int depth)
  {
    float leakAlpha;
    switch (depth)
    {
      case 1:
        leakAlpha = 0.15f;
        break;
      case 2:
        leakAlpha = 0.4f;
        break;
      default:
        leakAlpha = 0.55f;
        break;
    }
    return leakAlpha;
  }
}
