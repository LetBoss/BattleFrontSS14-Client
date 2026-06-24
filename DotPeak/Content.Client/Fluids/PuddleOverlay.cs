// Decompiled with JetBrains decompiler
// Type: Content.Client.Fluids.PuddleOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.Fluids;

public sealed class PuddleOverlay : Overlay
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IEntitySystemManager _entitySystemManager;
  private readonly PuddleDebugOverlaySystem _debugOverlaySystem;
  private readonly SharedTransformSystem _transformSystem;
  private readonly Color _heavyPuddle = new Color((byte) 0, byte.MaxValue, byte.MaxValue, (byte) 50);
  private readonly Color _mediumPuddle = new Color((byte) 0, (byte) 150, byte.MaxValue, (byte) 50);
  private readonly Color _lightPuddle = new Color((byte) 0, (byte) 50, byte.MaxValue, (byte) 50);
  private readonly Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 6;

  public PuddleOverlay()
  {
    IoCManager.InjectDependencies<PuddleOverlay>(this);
    this._debugOverlaySystem = this._entitySystemManager.GetEntitySystem<PuddleDebugOverlaySystem>();
    this._font = (Font) new VectorFont(IoCManager.Resolve<IResourceCache>().GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
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
      this.DrawScreen(in args);
  }

  private void DrawWorld(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    foreach (EntityUid key in this._debugOverlaySystem.TileData.Keys)
    {
      MapGridComponent mapGridComponent;
      if (this._entityManager.TryGetComponent<MapGridComponent>(key, ref mapGridComponent))
      {
        (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = this._transformSystem.GetWorldPositionRotationMatrixWithInv(entityQuery.GetComponent(key), entityQuery);
        Matrix3x2 matrix3x2 = rotationMatrixWithInv.Item3;
        Box2 box2_1 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref args.WorldBounds);
        Box2 box2_2 = ((Box2) ref box2_1).Enlarged((float) ((int) mapGridComponent.TileSize * 2));
        ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
        foreach (PuddleDebugOverlayData debugOverlayData in this._debugOverlaySystem.GetData(key))
        {
          Vector2 vector2 = (Vector2i.op_Implicit(debugOverlayData.Pos) + Vector2Helpers.Half) * (float) mapGridComponent.TileSize;
          if (((Box2) ref box2_2).Contains(vector2, true))
          {
            Box2 box2_3 = ((Box2) ref Box2.UnitCentered).Translated(vector2);
            worldHandle.DrawRect(box2_3, Color.Blue, false);
            worldHandle.DrawRect(box2_3, this.ColorMap(debugOverlayData.CurrentVolume), true);
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private void DrawScreen(in OverlayDrawArgs args)
  {
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    foreach (EntityUid key in this._debugOverlaySystem.TileData.Keys)
    {
      MapGridComponent mapGridComponent;
      if (this._entityManager.TryGetComponent<MapGridComponent>(key, ref mapGridComponent))
      {
        (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = this._transformSystem.GetWorldPositionRotationMatrixWithInv(entityQuery.GetComponent(key), entityQuery);
        Matrix3x2 matrix = rotationMatrixWithInv.Item3;
        Box2 box2_1 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref args.WorldBounds);
        Box2 box2_2 = ((Box2) ref box2_1).Enlarged((float) ((int) mapGridComponent.TileSize * 2));
        foreach (PuddleDebugOverlayData debugOverlayData in this._debugOverlaySystem.GetData(key))
        {
          Vector2 position = (Vector2i.op_Implicit(debugOverlayData.Pos) + Vector2Helpers.Half) * (float) mapGridComponent.TileSize;
          if (((Box2) ref box2_2).Contains(position, true))
          {
            Vector2 screen = this._eyeManager.WorldToScreen(Vector2.Transform(position, matrix));
            screenHandle.DrawString(this._font, screen, debugOverlayData.CurrentVolume.ToString(), Color.White);
          }
        }
      }
    }
  }

  private Color ColorMap(FixedPoint2 intensity)
  {
    FixedPoint2 fixedPoint2 = (FixedPoint2) 1 - intensity / FixedPoint2.New(20f);
    return !(fixedPoint2 < (FixedPoint2) 0.5f) ? Color.InterpolateBetween(this._lightPuddle, this._mediumPuddle, (float) (((double) fixedPoint2.Float() - 0.5) * 2.0)) : Color.InterpolateBetween(this._mediumPuddle, this._heavyPuddle, fixedPoint2.Float() * 2f);
  }
}
