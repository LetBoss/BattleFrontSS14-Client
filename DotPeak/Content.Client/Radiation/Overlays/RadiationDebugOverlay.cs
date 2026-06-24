// Decompiled with JetBrains decompiler
// Type: Content.Client.Radiation.Overlays.RadiationDebugOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Radiation.Systems;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Radiation.Overlays;

public sealed class RadiationDebugOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entityManager;
  private readonly SharedMapSystem _mapSystem;
  private readonly RadiationSystem _radiation;
  private readonly Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  public RadiationDebugOverlay()
  {
    IoCManager.InjectDependencies<RadiationDebugOverlay>(this);
    this._radiation = this._entityManager.System<RadiationSystem>();
    this._mapSystem = this._entityManager.System<SharedMapSystem>();
    this._font = (Font) new VectorFont(IoCManager.Resolve<IResourceCache>().GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    OverlaySpace space = args.Space;
    if (space != 2)
    {
      if (space != 4)
        return;
      this.DrawWorld(in args);
    }
    else
    {
      this.DrawScreenRays(args);
      this.DrawScreenResistance(args);
    }
  }

  private void DrawScreenRays(OverlayDrawArgs args)
  {
    List<DebugRadiationRay> rays = this._radiation.Rays;
    if (rays == null || args.ViewportControl == null)
      return;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    foreach (DebugRadiationRay debugRadiationRay in rays)
    {
      if (!MapId.op_Inequality(debugRadiationRay.MapId, args.MapId))
      {
        if (debugRadiationRay.ReachedDestination)
        {
          Vector2 screen = args.ViewportControl.WorldToScreen(debugRadiationRay.Destination);
          screenHandle.DrawString(this._font, screen, (ReadOnlySpan<char>) debugRadiationRay.Rads.ToString("F2"), 2f, Color.White);
        }
        foreach ((NetEntity key, List<(Vector2i, float)> tupleList) in debugRadiationRay.Blockers)
        {
          EntityUid entity = this._entityManager.GetEntity(key);
          MapGridComponent mapGridComponent;
          if (this._entityManager.TryGetComponent<MapGridComponent>(entity, ref mapGridComponent))
          {
            foreach ((Vector2i vector2i, float num) in tupleList)
            {
              Vector2 worldPos = this._mapSystem.GridTileToWorldPos(entity, mapGridComponent, vector2i);
              Vector2 screen = args.ViewportControl.WorldToScreen(worldPos);
              screenHandle.DrawString(this._font, screen, (ReadOnlySpan<char>) num.ToString("F2"), 1.5f, Color.White);
            }
          }
        }
      }
    }
  }

  private void DrawScreenResistance(OverlayDrawArgs args)
  {
    Dictionary<NetEntity, Dictionary<Vector2i, float>> resistanceGrids = this._radiation.ResistanceGrids;
    if (resistanceGrids == null || args.ViewportControl == null)
      return;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    foreach ((NetEntity key1, Dictionary<Vector2i, float> dictionary) in resistanceGrids)
    {
      EntityUid entity = this._entityManager.GetEntity(key1);
      MapGridComponent mapGridComponent;
      TransformComponent transformComponent;
      if (this._entityManager.TryGetComponent<MapGridComponent>(entity, ref mapGridComponent) && (!entityQuery.TryGetComponent(entity, ref transformComponent) || !MapId.op_Inequality(transformComponent.MapID, args.MapId)))
      {
        Vector2 vector2_1 = new Vector2((float) mapGridComponent.TileSize, (float) -mapGridComponent.TileSize) * 0.25f;
        foreach ((Vector2i key2, float num) in dictionary)
        {
          Vector2 vector2_2 = this._mapSystem.GridTileToLocal(entity, mapGridComponent, key2).Position + vector2_1;
          Vector2 world = this._mapSystem.LocalToWorld(entity, mapGridComponent, vector2_2);
          Vector2 screen = args.ViewportControl.WorldToScreen(world);
          screenHandle.DrawString(this._font, screen, num.ToString("F2"), Color.White);
        }
      }
    }
  }

  private void DrawWorld(in OverlayDrawArgs args)
  {
    List<DebugRadiationRay> rays = this._radiation.Rays;
    if (rays == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    foreach (DebugRadiationRay debugRadiationRay in rays)
    {
      if (!MapId.op_Inequality(debugRadiationRay.MapId, args.MapId))
      {
        if (debugRadiationRay.ReachedDestination)
        {
          ((DrawingHandleBase) worldHandle).DrawLine(debugRadiationRay.Source, debugRadiationRay.Destination, Color.Red);
        }
        else
        {
          foreach ((NetEntity key, List<(Vector2i, float)> source) in debugRadiationRay.Blockers)
          {
            EntityUid entity = this._entityManager.GetEntity(key);
            MapGridComponent mapGridComponent;
            if (this._entityManager.TryGetComponent<MapGridComponent>(entity, ref mapGridComponent))
            {
              Vector2i vector2i = source.Last<(Vector2i, float)>().Item1;
              Vector2 worldPos = this._mapSystem.GridTileToWorldPos(entity, mapGridComponent, vector2i);
              ((DrawingHandleBase) worldHandle).DrawLine(debugRadiationRay.Source, worldPos, Color.Red);
            }
          }
        }
      }
    }
  }
}
