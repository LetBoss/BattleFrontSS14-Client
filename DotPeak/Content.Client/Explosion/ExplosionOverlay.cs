// Decompiled with JetBrains decompiler
// Type: Content.Client.Explosion.ExplosionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Explosion;

public sealed class ExplosionOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IPrototypeManager _proto;
  private readonly SharedTransformSystem _transformSystem;
  private SharedAppearanceSystem _appearance;
  private ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public ExplosionOverlay(SharedAppearanceSystem appearanceSystem)
  {
    IoCManager.InjectDependencies<ExplosionOverlay>(this);
    this._shader = this._proto.Index<ShaderPrototype>(ExplosionOverlay.UnshadedShader).Instance();
    this._transformSystem = this._entMan.System<SharedTransformSystem>();
    this._appearance = appearanceSystem;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    EntityQuery<TransformComponent> entityQuery = this._entMan.GetEntityQuery<TransformComponent>();
    EntityQueryEnumerator<ExplosionVisualsComponent, ExplosionVisualsTexturesComponent> entityQueryEnumerator = this._entMan.EntityQueryEnumerator<ExplosionVisualsComponent, ExplosionVisualsTexturesComponent>();
    EntityUid entityUid;
    ExplosionVisualsComponent visuals;
    ExplosionVisualsTexturesComponent textures;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visuals, ref textures))
    {
      int num;
      if (!MapId.op_Inequality(visuals.Epicenter.MapId, args.MapId) && this._appearance.TryGetData<int>(entityUid, (Enum) ExplosionAppearanceData.Progress, ref num, (AppearanceComponent) null))
      {
        num = Math.Min(num, visuals.Intensity.Count - 1);
        this.DrawExplosion(worldHandle, args.WorldBounds, visuals, num, entityQuery, textures);
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private void DrawExplosion(
    DrawingHandleWorld drawHandle,
    Box2Rotated worldBounds,
    ExplosionVisualsComponent visuals,
    int index,
    EntityQuery<TransformComponent> xforms,
    ExplosionVisualsTexturesComponent textures)
  {
    foreach ((EntityUid key, Dictionary<int, List<Vector2i>> tileSets) in visuals.Tiles)
    {
      MapGridComponent mapGridComponent;
      if (this._entMan.TryGetComponent<MapGridComponent>(key, ref mapGridComponent))
      {
        (Vector2, Angle, Matrix3x2, Matrix3x2) rotationMatrixWithInv = this._transformSystem.GetWorldPositionRotationMatrixWithInv(xforms.GetComponent(key), xforms);
        Matrix3x2 matrix3x2 = rotationMatrixWithInv.Item3;
        Box2 box2 = Matrix3Helpers.TransformBox(rotationMatrixWithInv.Item4, ref worldBounds);
        Box2 gridBounds = ((Box2) ref box2).Enlarged((float) ((int) mapGridComponent.TileSize * 2));
        ((DrawingHandleBase) drawHandle).SetTransform(ref matrix3x2);
        this.DrawTiles(drawHandle, gridBounds, index, tileSets, visuals, mapGridComponent.TileSize, textures);
      }
    }
    if (visuals.SpaceTiles == null)
      return;
    Matrix3x2 result;
    Matrix3x2.Invert(visuals.SpaceMatrix, out result);
    Box2 box2_1 = Matrix3Helpers.TransformBox(result, ref worldBounds);
    Box2 gridBounds1 = ((Box2) ref box2_1).Enlarged(2f);
    ((DrawingHandleBase) drawHandle).SetTransform(ref visuals.SpaceMatrix);
    this.DrawTiles(drawHandle, gridBounds1, index, visuals.SpaceTiles, visuals, visuals.SpaceTileSize, textures);
  }

  private void DrawTiles(
    DrawingHandleWorld drawHandle,
    Box2 gridBounds,
    int index,
    Dictionary<int, List<Vector2i>> tileSets,
    ExplosionVisualsComponent visuals,
    ushort tileSize,
    ExplosionVisualsTexturesComponent textures)
  {
    for (int index1 = 0; index1 <= index; ++index1)
    {
      List<Vector2i> vector2iList;
      if (tileSets.TryGetValue(index1, out vector2iList))
      {
        int index2 = (int) Math.Min(visuals.Intensity[index1] / textures.IntensityPerState, (float) (textures.FireFrames.Count - 1));
        Texture[] fireFrame = textures.FireFrames[index2];
        foreach (Vector2i vector2i in vector2iList)
        {
          Vector2 vector2 = (Vector2i.op_Implicit(vector2i) + Vector2Helpers.Half) * (float) tileSize;
          if (((Box2) ref gridBounds).Contains(vector2, true))
          {
            Texture texture = RandomExtensions.Pick<Texture>(this._robustRandom, (IReadOnlyList<Texture>) fireFrame);
            drawHandle.DrawTextureRect(texture, Box2.CenteredAround(vector2, new Vector2((float) tileSize, (float) tileSize)), textures.FireColor);
          }
        }
      }
    }
  }
}
