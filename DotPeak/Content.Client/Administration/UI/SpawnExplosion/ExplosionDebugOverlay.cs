// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.SpawnExplosion.ExplosionDebugOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Administration.UI.SpawnExplosion;

public sealed class ExplosionDebugOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IEyeManager _eyeManager;
  public Dictionary<int, List<Vector2i>>? SpaceTiles;
  public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();
  public List<float> Intensity = new List<float>();
  public float TotalIntensity;
  public float Slope;
  public ushort SpaceTileSize;
  public Matrix3x2 SpaceMatrix;
  public MapId Map;
  private readonly Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  public ExplosionDebugOverlay()
  {
    IoCManager.InjectDependencies<ExplosionDebugOverlay>(this);
    this._font = (Font) new VectorFont(IoCManager.Resolve<IResourceCache>().GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    MapId map = this.Map;
    MapId? mapId = args.Viewport.Eye?.Position.MapId;
    if ((mapId.HasValue ? (MapId.op_Inequality(map, mapId.GetValueOrDefault()) ? 1 : 0) : 1) != 0 || this.Tiles.Count == 0 && this.SpaceTiles == null)
      return;
    OverlaySpace space = args.Space;
    if (space != 2)
    {
      if (space != 4)
        return;
      this.DrawWorld(in args);
    }
    else
      this.DrawScreen(args);
  }

  private void DrawScreen(OverlayDrawArgs args)
  {
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    TransformSystem transformSystem = this._entityManager.System<TransformSystem>();
    foreach ((EntityUid key, Dictionary<int, List<Vector2i>> tileSets) in this.Tiles)
    {
      MapGridComponent mapGridComponent;
      if (this._entityManager.TryGetComponent<MapGridComponent>(key, ref mapGridComponent))
      {
        TransformComponent component = entityQuery.GetComponent(key);
        (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = ((SharedTransformSystem) transformSystem).GetWorldPositionRotationMatrixWithInv(component, entityQuery);
        Matrix3x2 transform = rotationMatrixWithInv.Item3;
        Box2 box2 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref args.WorldBounds);
        Box2 gridBounds = ((Box2) ref box2).Enlarged((float) ((int) mapGridComponent.TileSize * 2));
        this.DrawText(screenHandle, gridBounds, transform, tileSets, mapGridComponent.TileSize);
      }
    }
    if (this.SpaceTiles == null)
      return;
    Matrix3x2 result;
    Matrix3x2.Invert(this.SpaceMatrix, out result);
    Box2 gridBounds1 = Matrix3Helpers.TransformBox(result, ref args.WorldBounds);
    this.DrawText(screenHandle, gridBounds1, this.SpaceMatrix, this.SpaceTiles, this.SpaceTileSize);
  }

  private void DrawText(
    DrawingHandleScreen handle,
    Box2 gridBounds,
    Matrix3x2 transform,
    Dictionary<int, List<Vector2i>> tileSets,
    ushort tileSize)
  {
    for (int index = 1; index < this.Intensity.Count; ++index)
    {
      List<Vector2i> vector2iList;
      if (tileSets.TryGetValue(index, out vector2iList))
      {
        foreach (Vector2i vector2i in vector2iList)
        {
          Vector2 position = (Vector2i.op_Implicit(vector2i) + Vector2Helpers.Half) * (float) tileSize;
          if (((Box2) ref gridBounds).Contains(position, true))
          {
            Vector2 screen = this._eyeManager.WorldToScreen(Vector2.Transform(position, transform));
            Vector2 vector2 = (double) this.Intensity[index] <= 9.0 ? screen + new Vector2(-8f, -8f) : screen + new Vector2(-12f, -8f);
            handle.DrawString(this._font, vector2, this.Intensity[index].ToString("F2"));
          }
        }
      }
    }
    List<Vector2i> source;
    if (!tileSets.TryGetValue(0, out source))
      return;
    Vector2 vector2_1 = this._eyeManager.WorldToScreen(Vector2.Transform((Vector2i.op_Implicit(source.First<Vector2i>()) + Vector2Helpers.Half) * (float) tileSize, transform)) + new Vector2(-24f, -24f);
    string str = $"{this.Intensity[0]:F2}\nΣ={this.TotalIntensity:F1}\nΔ={this.Slope:F1}";
    handle.DrawString(this._font, vector2_1, str);
  }

  private void DrawWorld(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    TransformSystem transformSystem = this._entityManager.System<TransformSystem>();
    foreach ((EntityUid key, Dictionary<int, List<Vector2i>> tileSets) in this.Tiles)
    {
      MapGridComponent mapGridComponent;
      if (this._entityManager.TryGetComponent<MapGridComponent>(key, ref mapGridComponent))
      {
        TransformComponent component = entityQuery.GetComponent(key);
        (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = ((SharedTransformSystem) transformSystem).GetWorldPositionRotationMatrixWithInv(component, entityQuery);
        Matrix3x2 matrix3x2 = rotationMatrixWithInv.Item3;
        Box2 box2 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref args.WorldBounds);
        Box2 gridBounds = ((Box2) ref box2).Enlarged((float) ((int) mapGridComponent.TileSize * 2));
        ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
        this.DrawTiles(worldHandle, gridBounds, tileSets, this.SpaceTileSize);
      }
    }
    if (this.SpaceTiles == null)
      return;
    Matrix3x2 result;
    Matrix3x2.Invert(this.SpaceMatrix, out result);
    Box2 box2_1 = Matrix3Helpers.TransformBox(result, ref args.WorldBounds);
    Box2 gridBounds1 = ((Box2) ref box2_1).Enlarged(2f);
    ((DrawingHandleBase) worldHandle).SetTransform(ref this.SpaceMatrix);
    this.DrawTiles(worldHandle, gridBounds1, this.SpaceTiles, this.SpaceTileSize);
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private void DrawTiles(
    DrawingHandleWorld handle,
    Box2 gridBounds,
    Dictionary<int, List<Vector2i>> tileSets,
    ushort tileSize)
  {
    for (int index = 0; index < this.Intensity.Count; ++index)
    {
      Color color1 = this.ColorMap(this.Intensity[index]);
      Color color2 = color1;
      color2.A = 0.2f;
      List<Vector2i> vector2iList;
      if (tileSets.TryGetValue(index, out vector2iList))
      {
        foreach (Vector2i vector2i in vector2iList)
        {
          Vector2 vector2 = (Vector2i.op_Implicit(vector2i) + Vector2Helpers.Half) * (float) tileSize;
          if (((Box2) ref gridBounds).Contains(vector2, true))
          {
            Box2 box2 = ((Box2) ref Box2.UnitCentered).Translated(vector2);
            handle.DrawRect(box2, color1, false);
            handle.DrawRect(box2, color2, true);
          }
        }
      }
    }
  }

  private Color ColorMap(float intensity)
  {
    float num = (float) (1.0 - (double) intensity / (double) this.Intensity[0]);
    return (double) num >= 0.5 ? Color.InterpolateBetween(Color.Orange, Color.Yellow, (float) (((double) num - 0.5) * 2.0)) : Color.InterpolateBetween(Color.Red, Color.Orange, num * 2f);
  }
}
