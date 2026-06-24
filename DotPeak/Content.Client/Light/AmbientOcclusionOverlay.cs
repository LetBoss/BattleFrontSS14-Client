// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.AmbientOcclusionOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Client.Graphics;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Light;

public sealed class AmbientOcclusionOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> UnshadedShader = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
  private static readonly ProtoId<ShaderPrototype> StencilMaskShader = ProtoId<ShaderPrototype>.op_Implicit("StencilMask");
  private static readonly ProtoId<ShaderPrototype> StencilEqualDrawShader = ProtoId<ShaderPrototype>.op_Implicit("StencilEqualDraw");
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IConfigurationManager _cfgManager;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPrototypeManager _proto;
  private IRenderTexture? _aoTarget;
  private IRenderTexture? _aoBlurBuffer;
  private IRenderTexture? _aoStencilTarget;

  public virtual OverlaySpace Space => (OverlaySpace) 64 /*0x40*/;

  public AmbientOcclusionOverlay()
  {
    IoCManager.InjectDependencies<AmbientOcclusionOverlay>(this);
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    IClydeViewport viewport = args.Viewport;
    MapId mapId = args.MapId;
    Box2Rotated worldBounds = args.WorldBounds;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Color color = Color.FromHex((ReadOnlySpan<char>) this._cfgManager.GetCVar<string>(CCVars.AmbientOcclusionColor), new Color?());
    float distance = this._cfgManager.GetCVar<float>(CCVars.AmbientOcclusionDistance);
    IRenderTexture renderTarget = viewport.RenderTarget;
    Vector2 vector2 = Vector2i.op_Implicit(((IRenderTarget) renderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 scale = viewport.RenderScale / (Vector2.One / vector2);
    SharedMapSystem maps = this._entManager.System<SharedMapSystem>();
    EntityLookupSystem lookups = this._entManager.System<EntityLookupSystem>();
    OccluderSystem query = this._entManager.System<OccluderSystem>();
    SharedTransformSystem xformSystem = this._entManager.System<SharedTransformSystem>();
    TurfSystem turfSystem = this._entManager.System<TurfSystem>();
    Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
    Vector2i? size1 = this._aoTarget?.Texture.Size;
    Vector2i size2 = ((IRenderTarget) renderTarget).Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size2) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._aoTarget)?.Dispose();
      this._aoTarget = this._clyde.CreateRenderTarget(((IRenderTarget) renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "ambient-occlusion-target");
    }
    size1 = this._aoBlurBuffer?.Texture.Size;
    Vector2i size3 = ((IRenderTarget) renderTarget).Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size3) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._aoBlurBuffer)?.Dispose();
      this._aoBlurBuffer = this._clyde.CreateRenderTarget(((IRenderTarget) renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "ambient-occlusion-blur-target");
    }
    size1 = this._aoStencilTarget?.Texture.Size;
    Vector2i size4 = ((IRenderTarget) renderTarget).Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size4) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._aoStencilTarget)?.Dispose();
      this._aoStencilTarget = this._clyde.CreateRenderTarget(((IRenderTarget) renderTarget).Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "ambient-occlusion-stencil-target");
    }
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) this._aoTarget, (Action) (() =>
    {
      ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(AmbientOcclusionOverlay.UnshadedShader).Instance());
      Matrix3x2 worldToLocalMatrix = ((IRenderTarget) this._aoTarget).GetWorldToLocalMatrix(viewport.Eye, scale);
      foreach (ComponentTreeEntry<OccluderComponent> componentTreeEntry in ((ComponentTreeSystem<OccluderTreeComponent, OccluderComponent>) query).QueryAabb(mapId, worldBounds, true))
      {
        Matrix3x2 matrix3x2 = Matrix3x2.Multiply(xformSystem.GetWorldMatrix(componentTreeEntry.Transform), worldToLocalMatrix);
        ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
        worldHandle.DrawRect(((Box2) ref Box2.UnitCentered).Enlarged(distance / 32f), Color.White, true);
      }
    }), new Color?(Color.Transparent));
    this._clyde.BlurRenderTarget(viewport, (IRenderTarget) this._aoTarget, (IRenderTarget) this._aoBlurBuffer, viewport.Eye, 14f);
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) this._aoStencilTarget, (Action) (() =>
    {
      ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(AmbientOcclusionOverlay.UnshadedShader).Instance());
      List<Entity<MapGridComponent>> entityList = new List<Entity<MapGridComponent>>();
      this._mapManager.FindGridsIntersecting(mapId, worldBounds, ref entityList, false, true);
      using (List<Entity<MapGridComponent>>.Enumerator enumerator = entityList.GetEnumerator())
      {
label_6:
        while (enumerator.MoveNext())
        {
          Entity<MapGridComponent> current = enumerator.Current;
          Matrix3x2 matrix3x2 = Matrix3x2.Multiply(xformSystem.GetWorldMatrix(current.Owner), invMatrix);
          SharedMapSystem.TilesEnumerator tilesEnumerator = maps.GetTilesEnumerator(current.Owner, current.Comp, worldBounds, true, (Predicate<TileRef>) null);
          ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
          while (true)
          {
            TileRef tile;
            do
            {
              if (!((SharedMapSystem.TilesEnumerator) ref tilesEnumerator).MoveNext(ref tile))
                goto label_6;
            }
            while (turfSystem.IsSpace(tile));
            worldHandle.DrawRect(lookups.GetLocalBounds(tile, current.Comp.TileSize), Color.White, true);
          }
        }
      }
    }), new Color?(Color.Transparent));
    ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(AmbientOcclusionOverlay.StencilMaskShader).Instance());
    worldHandle.DrawTextureRect(this._aoStencilTarget.Texture, ref worldBounds, new Color?());
    ((DrawingHandleBase) worldHandle).UseShader(this._proto.Index<ShaderPrototype>(AmbientOcclusionOverlay.StencilEqualDrawShader).Instance());
    worldHandle.DrawTextureRect(this._aoTarget.Texture, ref worldBounds, new Color?(color));
    DrawingHandleWorld worldHandle1 = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) worldHandle1).SetTransform(ref local);
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).UseShader((ShaderInstance) null);
  }
}
