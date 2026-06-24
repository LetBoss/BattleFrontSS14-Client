// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.TileEmissionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Light;

public sealed class TileEmissionOverlay : Overlay
{
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IOverlayManager _overlay;
  private SharedMapSystem _mapSystem;
  private SharedTransformSystem _xformSystem;
  private readonly EntityLookupSystem _lookup;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly HashSet<Entity<TileEmissionComponent>> _entities = new HashSet<Entity<TileEmissionComponent>>();
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();
  public const int ContentZIndex = -8;

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public TileEmissionOverlay(IEntityManager entManager)
  {
    IoCManager.InjectDependencies<TileEmissionOverlay>(this);
    this._lookup = entManager.System<EntityLookupSystem>();
    this._mapSystem = entManager.System<SharedMapSystem>();
    this._xformSystem = entManager.System<SharedTransformSystem>();
    this._xformQuery = entManager.GetEntityQuery<TransformComponent>();
    this.ZIndex = new int?(-8);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.Viewport.Eye == null)
      return;
    MapId mapId = args.MapId;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    BeforeLightTargetOverlay overlay = this._overlay.GetOverlay<BeforeLightTargetOverlay>();
    Box2Rotated bounds = overlay.EnlargedBounds;
    IRenderTexture target = overlay.EnlargedLightTarget;
    IClydeViewport viewport = args.Viewport;
    this._grids.Clear();
    this._mapManager.FindGridsIntersecting(mapId, bounds, ref this._grids, true, true);
    if (this._grids.Count == 0)
      return;
    Vector2 vector2 = Vector2i.op_Implicit(((IRenderTarget) viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 scale = viewport.RenderScale / (Vector2.One / vector2);
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) target, (Action) (() =>
    {
      Matrix3x2 worldToLocalMatrix = ((IRenderTarget) target).GetWorldToLocalMatrix(viewport.Eye, scale);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Box2 box2 = Matrix3Helpers.TransformBox(this._xformSystem.GetInvWorldMatrix(Entity<MapGridComponent>.op_Implicit(grid)), ref bounds);
        this._entities.Clear();
        this._lookup.GetLocalEntitiesIntersecting<TileEmissionComponent>(grid.Owner, box2, this._entities, (LookupFlags) 110);
        if (this._entities.Count != 0)
        {
          Matrix3x2 worldMatrix = this._xformSystem.GetWorldMatrix(grid.Owner);
          foreach (Entity<TileEmissionComponent> entity in this._entities)
          {
            TransformComponent transformComponent = this._xformQuery.Comp(Entity<TileEmissionComponent>.op_Implicit(entity));
            Vector2i tile = this._mapSystem.LocalToTile(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), transformComponent.Coordinates);
            Matrix3x2 matrix3x2 = Matrix3x2.Multiply(worldMatrix, worldToLocalMatrix);
            ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
            Box2 localBounds = this._lookup.GetLocalBounds(tile, grid.Comp.TileSize);
            worldHandle.DrawRect(((Box2) ref localBounds).Enlarged(entity.Comp.Range), entity.Comp.Color, true);
          }
        }
      }
    }), new Color?());
  }
}
