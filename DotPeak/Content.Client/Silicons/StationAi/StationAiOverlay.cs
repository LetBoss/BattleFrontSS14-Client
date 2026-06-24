// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.StationAi.StationAiOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Silicons.StationAi;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Silicons.StationAi;

public sealed class StationAiOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> CameraStaticShader = ProtoId<ShaderPrototype>.op_Implicit("CameraStatic");
  private static readonly ProtoId<ShaderPrototype> StencilMaskShader = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");
  private static readonly ProtoId<ShaderPrototype> StencilDrawShader = ProtoId<ShaderPrototype>.op_Implicit("StencilDraw");
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _proto;
  private readonly HashSet<Vector2i> _visibleTiles = new HashSet<Vector2i>();
  private IRenderTexture? _staticTexture;
  private IRenderTexture? _stencilTexture;
  private float _updateRate = 0.0333333351f;
  private float _accumulator;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public StationAiOverlay() => IoCManager.InjectDependencies<StationAiOverlay>(this);

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    Vector2i? size1 = this._stencilTexture?.Texture.Size;
    Vector2i size2 = args.Viewport.Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size2) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._staticTexture)?.Dispose();
      ((IDisposable) this._stencilTexture)?.Dispose();
      this._stencilTexture = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "station-ai-stencil");
      this._staticTexture = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "station-ai-static");
    }
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    TransformComponent transformComponent;
    this._entManager.TryGetComponent<TransformComponent>(((ISharedPlayerManager) this._player).LocalEntity, ref transformComponent);
    EntityUid entityUid1 = (EntityUid?) transformComponent?.GridUid ?? EntityUid.Invalid;
    MapGridComponent grid;
    this._entManager.TryGetComponent<MapGridComponent>(entityUid1, ref grid);
    BroadphaseComponent broadphaseComponent;
    this._entManager.TryGetComponent<BroadphaseComponent>(entityUid1, ref broadphaseComponent);
    Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
    this._accumulator -= (float) this._timing.FrameTime.TotalSeconds;
    if (grid != null && broadphaseComponent != null)
    {
      EntityLookupSystem lookups = this._entManager.System<EntityLookupSystem>();
      SharedTransformSystem sharedTransformSystem = this._entManager.System<SharedTransformSystem>();
      if ((double) this._accumulator <= 0.0)
      {
        this._accumulator = MathF.Max(0.0f, this._accumulator + this._updateRate);
        this._visibleTiles.Clear();
        this._entManager.System<StationAiVisionSystem>().GetView(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((entityUid1, broadphaseComponent, grid)), worldBounds, this._visibleTiles);
      }
      EntityUid entityUid2 = entityUid1;
      Matrix3x2 matty = Matrix3x2.Multiply(sharedTransformSystem.GetWorldMatrix(entityUid2), invMatrix);
      ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._stencilTexture, (Action) (() =>
      {
        ((DrawingHandleBase) worldHandle).SetTransform(ref matty);
        foreach (Vector2i visibleTile in this._visibleTiles)
          worldHandle.DrawRect(lookups.GetLocalBounds(visibleTile, grid.TileSize), Color.White, true);
      }), new Color?(Color.Transparent));
      ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._staticTexture, (Action) (() =>
      {
        ((DrawingHandleBase) worldHandle).SetTransform(ref invMatrix);
        ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(StationAiOverlay.CameraStaticShader).Instance());
        worldHandle.DrawRect(ref worldBounds, Color.White, true);
      }), new Color?(Color.Black));
    }
    else
    {
      ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._stencilTexture, (Action) (() => { }), new Color?(Color.Transparent));
      ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._staticTexture, (Action) (() =>
      {
        DrawingHandleWorld drawingHandleWorld = worldHandle;
        Matrix3x2 identity = Matrix3x2.Identity;
        ref Matrix3x2 local = ref identity;
        ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
        worldHandle.DrawRect(ref worldBounds, Color.Black, true);
      }), new Color?(Color.Black));
    }
    ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(StationAiOverlay.StencilMaskShader).Instance());
    worldHandle.DrawTextureRect(this._stencilTexture.Texture, ref worldBounds, new Color?());
    ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(StationAiOverlay.StencilDrawShader).Instance());
    worldHandle.DrawTextureRect(this._staticTexture.Texture, ref worldBounds, new Color?());
    DrawingHandleWorld drawingHandleWorld1 = worldHandle;
    Matrix3x2 identity1 = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity1;
    ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local1);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
