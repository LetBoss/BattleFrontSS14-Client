// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.FtlArrivalOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Shuttles.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Shuttles;

public sealed class FtlArrivalOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  private EntityLookupSystem _lookups;
  private SharedMapSystem _maps;
  private SharedTransformSystem _transforms;
  private SpriteSystem _sprites;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPrototypeManager _protos;
  private readonly HashSet<Entity<FtlVisualizerComponent>> _visualizers = new HashSet<Entity<FtlVisualizerComponent>>();
  private ShaderInstance _shader;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public FtlArrivalOverlay()
  {
    IoCManager.InjectDependencies<FtlArrivalOverlay>(this);
    this._lookups = this._entManager.System<EntityLookupSystem>();
    this._transforms = this._entManager.System<SharedTransformSystem>();
    this._maps = this._entManager.System<SharedMapSystem>();
    this._sprites = this._entManager.System<SpriteSystem>();
    this._shader = this._protos.Index<ShaderPrototype>(FtlArrivalOverlay.UnshadedShader).Instance();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    this._visualizers.Clear();
    this._lookups.GetEntitiesOnMap<FtlVisualizerComponent>(args.MapId, this._visualizers);
    return this._visualizers.Count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).UseShader(this._shader);
    using (HashSet<Entity<FtlVisualizerComponent>>.Enumerator enumerator = this._visualizers.GetEnumerator())
    {
label_6:
      while (enumerator.MoveNext())
      {
        EntityUid entityUid1;
        FtlVisualizerComponent visualizerComponent1;
        enumerator.Current.Deconstruct(ref entityUid1, ref visualizerComponent1);
        EntityUid entityUid2 = entityUid1;
        FtlVisualizerComponent visualizerComponent2 = visualizerComponent1;
        EntityUid grid = visualizerComponent2.Grid;
        MapGridComponent mapGridComponent;
        if (this._entManager.TryGetComponent<MapGridComponent>(grid, ref mapGridComponent))
        {
          Texture frame = this._sprites.GetFrame((SpriteSpecifier) visualizerComponent2.Sprite, TimeSpan.FromSeconds((double) visualizerComponent2.Elapsed), false);
          visualizerComponent2.Elapsed += (float) this._timing.FrameTime.TotalSeconds;
          (Vector2 _, Angle _, Matrix3x2 matrix3x2_1, Matrix3x2 matrix3x2_2) = this._transforms.GetWorldPositionRotationMatrixWithInv(entityUid2);
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).SetTransform(ref matrix3x2_1);
          ref Box2Rotated local = ref args.WorldBounds;
          Box2 box2 = Matrix3Helpers.TransformBox(matrix3x2_2, ref local);
          SharedMapSystem.TilesEnumerator localTilesEnumerator = this._maps.GetLocalTilesEnumerator(grid, mapGridComponent, box2, true, (Predicate<TileRef>) null);
          while (true)
          {
            TileRef tileRef;
            if (((SharedMapSystem.TilesEnumerator) ref localTilesEnumerator).MoveNext(ref tileRef))
            {
              Box2 localBounds = this._lookups.GetLocalBounds(tileRef, mapGridComponent.TileSize);
              ((OverlayDrawArgs) ref args).WorldHandle.DrawTextureRect(frame, localBounds, new Color?());
            }
            else
              goto label_6;
          }
        }
      }
    }
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).UseShader((ShaderInstance) null);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) worldHandle).SetTransform(ref local1);
  }
}
