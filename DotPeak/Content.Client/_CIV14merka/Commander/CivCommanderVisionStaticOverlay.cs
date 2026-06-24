// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderVisionStaticOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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

public sealed class CivCommanderVisionStaticOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> CommanderStaticShader = ProtoId<ShaderPrototype>.op_Implicit("CivCommanderStatic");
  private static readonly ProtoId<ShaderPrototype> StencilMaskShader = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");
  private static readonly ProtoId<ShaderPrototype> StencilDrawShader = ProtoId<ShaderPrototype>.op_Implicit("StencilDraw");
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entities;
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly CivCommanderVisionSystem _system;
  private readonly EntityLookupSystem _lookup;
  private readonly SharedTransformSystem _xform;
  private IRenderTexture? _staticTexture;
  private IRenderTexture? _stencilTexture;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public CivCommanderVisionStaticOverlay(CivCommanderVisionSystem system)
  {
    IoCManager.InjectDependencies<CivCommanderVisionStaticOverlay>(this);
    this._system = system;
    this._lookup = this._entities.System<EntityLookupSystem>();
    this._xform = this._entities.System<SharedTransformSystem>();
    this.ZIndex = new int?(241);
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    ((IDisposable) this._staticTexture)?.Dispose();
    ((IDisposable) this._stencilTexture)?.Dispose();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this._system.Active || MapId.op_Equality(args.MapId, MapId.Nullspace))
      return;
    Vector2i? size1 = this._stencilTexture?.Texture.Size;
    Vector2i size2 = args.Viewport.Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size2) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._staticTexture)?.Dispose();
      ((IDisposable) this._stencilTexture)?.Dispose();
      this._stencilTexture = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "civ-commander-static-stencil");
      this._staticTexture = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "civ-commander-static");
    }
    if (this._stencilTexture == null || this._staticTexture == null)
      return;
    DrawingHandleWorld handle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    Matrix3x2 inverseMatrix = args.Viewport.GetWorldToLocalMatrix();
    MapId mapId = args.MapId;
    ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._stencilTexture, (Action) (() =>
    {
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
          foreach ((Vector2i vector2i, byte[] states) in dictionary)
            this.DrawKnownChunk(handle, grid, vector2i, states);
        }
      }
    }), new Color?(Color.Transparent));
    ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._staticTexture, (Action) (() =>
    {
      ((DrawingHandleBase) handle).SetTransform(ref inverseMatrix);
      ((DrawingHandleBase) handle).UseShader(this._prototype.Index<ShaderPrototype>(CivCommanderVisionStaticOverlay.CommanderStaticShader).Instance());
      handle.DrawRect(ref worldBounds, Color.White, true);
      ((DrawingHandleBase) handle).UseShader((ShaderInstance) null);
    }), new Color?(Color.Black));
    ((DrawingHandleBase) handle).UseShader(this._prototype.Index<ShaderPrototype>(CivCommanderVisionStaticOverlay.StencilMaskShader).Instance());
    handle.DrawTextureRect(this._stencilTexture.Texture, ref worldBounds, new Color?());
    ((DrawingHandleBase) handle).UseShader(this._prototype.Index<ShaderPrototype>(CivCommanderVisionStaticOverlay.StencilDrawShader).Instance());
    handle.DrawTextureRect(this._staticTexture.Texture, ref worldBounds, new Color?());
    DrawingHandleWorld drawingHandleWorld = handle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    ((DrawingHandleBase) handle).UseShader((ShaderInstance) null);
  }

  private void DrawKnownChunk(
    DrawingHandleWorld handle,
    MapGridComponent grid,
    Vector2i chunkIndex,
    byte[] states)
  {
    if (states.Length != 256 /*0x0100*/)
      return;
    int num1 = chunkIndex.X * 16 /*0x10*/;
    int num2 = chunkIndex.Y * 16 /*0x10*/;
    for (int index1 = 0; index1 < 16 /*0x10*/; ++index1)
    {
      for (int index2 = 0; index2 < 16 /*0x10*/; ++index2)
      {
        int index3 = index1 * 16 /*0x10*/ + index2;
        if (states[index3] != (byte) 0)
        {
          Vector2i vector2i;
          // ISSUE: explicit constructor call
          ((Vector2i) ref vector2i).\u002Ector(num1 + index2, num2 + index1);
          Box2 localBounds = this._lookup.GetLocalBounds(vector2i, grid.TileSize);
          handle.DrawRect(localBounds, Color.White, true);
        }
      }
    }
  }
}
