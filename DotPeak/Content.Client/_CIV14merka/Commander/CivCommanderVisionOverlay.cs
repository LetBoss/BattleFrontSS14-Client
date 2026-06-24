// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderVisionOverlay : Overlay
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly CivCommanderVisionSystem _system;
  private readonly SharedTransformSystem _xform;
  private ShaderInstance? _maskShader;
  private IRenderTexture? _maskTarget;
  private readonly List<(Box2 Bounds, Color Color)> _tileBatch = new List<(Box2, Color)>(4096 /*0x1000*/);
  private const float SeenMaskValue = 0.45f;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public CivCommanderVisionOverlay(CivCommanderVisionSystem system)
  {
    IoCManager.InjectDependencies<CivCommanderVisionOverlay>(this);
    this._system = system;
    this._xform = this._entities.System<SharedTransformSystem>();
    this.InitializeShaders();
    this.ZIndex = new int?(240 /*0xF0*/);
  }

  private void InitializeShaders()
  {
    this._maskShader?.Dispose();
    this._maskShader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("CivCommanderVisionMask")).InstanceUnique();
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    ((IDisposable) this._maskTarget)?.Dispose();
    this._maskShader?.Dispose();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    if (!this._system.Active || this.ScreenTexture == null)
      return false;
    if (this._maskShader == null)
      this.InitializeShaders();
    Vector2i size = args.Viewport.Size;
    if (this._maskTarget == null || Vector2i.op_Inequality(((IRenderTarget) this._maskTarget).Size, size))
    {
      ((IDisposable) this._maskTarget)?.Dispose();
      this._maskTarget = this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "civ-commander-vision-mask");
    }
    return this._maskTarget != null && this._maskShader != null;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._system.Active || MapId.op_Equality(args.MapId, MapId.Nullspace) || this.ScreenTexture == null || this._maskTarget == null || this._maskShader == null || args.Viewport.Eye == null)
      return;
    DrawingHandleWorld handle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    Matrix3x2 inverseMatrix = args.Viewport.GetWorldToLocalMatrix();
    MapId mapId = args.MapId;
    ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._maskTarget, (Action) (() =>
    {
      ((DrawingHandleBase) handle).SetTransform(ref inverseMatrix);
      handle.DrawRect(ref worldBounds, Color.Black, true);
      EntityQueryEnumerator<MapGridComponent, TransformComponent> entityQueryEnumerator = this._entities.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
      EntityUid key;
      MapGridComponent grid;
      TransformComponent transformComponent;
      while (entityQueryEnumerator.MoveNext(ref key, ref grid, ref transformComponent))
      {
        Dictionary<Vector2i, byte[]> dictionary;
        if (!MapId.op_Inequality(transformComponent.MapID, mapId) && this._system.GridChunks.TryGetValue(key, out dictionary))
        {
          Matrix3x2 matrix3x2 = Matrix3x2.Multiply(this._xform.GetWorldMatrix(key), inverseMatrix);
          ((DrawingHandleBase) handle).SetTransform(ref matrix3x2);
          this._tileBatch.Clear();
          foreach ((Vector2i vector2i, byte[] states) in dictionary)
            this.CollectVisibleChunk(grid, vector2i, states);
          foreach ((Box2, Color) valueTuple in this._tileBatch)
            handle.DrawRect(valueTuple.Item1, valueTuple.Item2, true);
        }
      }
    }), new Color?(Color.Transparent));
    this._maskShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._maskShader.SetParameter("MASK_TEXTURE", this._maskTarget.Texture);
    this._maskShader.SetParameter("shadowOpacity", 0.85f);
    ((DrawingHandleBase) handle).SetTransform(ref inverseMatrix);
    ((DrawingHandleBase) handle).UseShader(this._maskShader);
    handle.DrawRect(ref worldBounds, Color.White, true);
    ((DrawingHandleBase) handle).UseShader((ShaderInstance) null);
    DrawingHandleWorld drawingHandleWorld = handle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private void CollectVisibleChunk(MapGridComponent grid, Vector2i chunkIndex, byte[] states)
  {
    if (states.Length != 256 /*0x0100*/)
      return;
    int num = 16 /*0x10*/;
    ushort tileSize = grid.TileSize;
    int originX = chunkIndex.X * num;
    int originY = chunkIndex.Y * num;
    for (int y = 0; y < num; ++y)
    {
      int startX1 = -1;
      int startX2 = -1;
      for (int index1 = 0; index1 <= num; ++index1)
      {
        CivCommanderVisionTileState commanderVisionTileState;
        if (index1 < num)
        {
          int index2 = y * num + index1;
          commanderVisionTileState = (CivCommanderVisionTileState) states[index2];
        }
        else
          commanderVisionTileState = CivCommanderVisionTileState.Unseen;
        switch (commanderVisionTileState)
        {
          case CivCommanderVisionTileState.Seen:
            if (startX1 >= 0)
            {
              this.FlushRow(originX, originY, y, startX1, index1 - 1, (float) tileSize, true);
              startX1 = -1;
            }
            if (startX2 < 0)
            {
              startX2 = index1;
              break;
            }
            break;
          case CivCommanderVisionTileState.Visible:
            if (startX2 >= 0)
            {
              this.FlushRow(originX, originY, y, startX2, index1 - 1, (float) tileSize, false);
              startX2 = -1;
            }
            if (startX1 < 0)
            {
              startX1 = index1;
              break;
            }
            break;
          default:
            if (startX1 >= 0)
            {
              this.FlushRow(originX, originY, y, startX1, index1 - 1, (float) tileSize, true);
              startX1 = -1;
            }
            if (startX2 >= 0)
            {
              this.FlushRow(originX, originY, y, startX2, index1 - 1, (float) tileSize, false);
              startX2 = -1;
              break;
            }
            break;
        }
      }
    }
  }

  private void FlushRow(
    int originX,
    int originY,
    int y,
    int startX,
    int endX,
    float tileSize,
    bool visible)
  {
    float num1 = (float) (originX + startX) * tileSize;
    float num2 = (float) (originX + endX + 1) * tileSize;
    float num3 = (float) (originY + y) * tileSize;
    float num4 = num3 + tileSize;
    Color color = visible ? Color.White : new Color(0.45f, 0.45f, 0.45f, 1f);
    this._tileBatch.Add((new Box2(num1, num3, num2, num4), color));
  }
}
