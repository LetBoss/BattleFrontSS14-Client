// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.SunShadowOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light.Components;
using Robust.Client.Graphics;
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

public sealed class SunShadowOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> MixShader = ProtoId<ShaderPrototype>.op_Implicit("Mix");
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPrototypeManager _protoManager;
  private readonly EntityLookupSystem _lookup;
  private readonly SharedTransformSystem _xformSys;
  private readonly HashSet<Entity<SunShadowCastComponent>> _shadows = new HashSet<Entity<SunShadowCastComponent>>();
  private IRenderTexture? _blurTarget;
  private IRenderTexture? _target;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public SunShadowOverlay()
  {
    IoCManager.InjectDependencies<SunShadowOverlay>(this);
    this._xformSys = this._entManager.System<SharedTransformSystem>();
    this._lookup = this._entManager.System<EntityLookupSystem>();
    this.ZIndex = new int?(-5);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    IClydeViewport viewport = args.Viewport;
    IEye eye = viewport.Eye;
    if (eye == null)
      return;
    this._grids.Clear();
    this._mapManager.FindGridsIntersecting(args.MapId, ((Box2Rotated) ref args.WorldBounds).Enlarged(5f), ref this._grids, false, true);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    MapId mapId = args.MapId;
    Box2Rotated worldBounds = args.WorldBounds;
    Vector2i size = ((IRenderTarget) viewport.LightRenderTarget).Size;
    IRenderTexture target = this._target;
    if ((target != null ? (Vector2i.op_Inequality(((IRenderTarget) target).Size, size) ? 1 : 0) : 1) != 0)
    {
      this._target = this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "sun-shadow-target");
      IRenderTexture blurTarget = this._blurTarget;
      if ((blurTarget != null ? (Vector2i.op_Inequality(((IRenderTarget) blurTarget).Size, size) ? 1 : 0) : 1) != 0)
        this._blurTarget = this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "sun-shadow-blur");
    }
    Vector2 vector2 = Vector2i.op_Implicit(((IRenderTarget) viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 scale = viewport.RenderScale / (Vector2.One / vector2);
    foreach (Entity<MapGridComponent> grid in this._grids)
    {
      SunShadowComponent sunShadowComponent;
      if (this._entManager.TryGetComponent<SunShadowComponent>(grid.Owner, ref sunShadowComponent))
      {
        Vector2 direction = sunShadowComponent.Direction;
        float alpha = Math.Clamp(sunShadowComponent.Alpha, 0.0f, 1f);
        if (!direction.Equals(Vector2.Zero) && (double) alpha != 0.0)
        {
          Box2Rotated expandedBounds = ((Box2Rotated) ref worldBounds).Enlarged(direction.Length() + 0.01f);
          this._shadows.Clear();
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) this._target, (Action) (() =>
          {
            Matrix3x2 worldToLocalMatrix = ((IRenderTarget) this._target).GetWorldToLocalMatrix(eye, scale);
            Vector2[] destinationArray = new Vector2[16 /*0x10*/];
            this._lookup.GetEntitiesIntersecting<SunShadowCastComponent>(mapId, expandedBounds, this._shadows, (LookupFlags) 110);
            foreach (Entity<SunShadowCastComponent> shadow in this._shadows)
            {
              (Vector2 position2, Angle angle2) = this._xformSys.GetWorldPositionRotation(this._entManager.GetComponent<TransformComponent>(shadow.Owner));
              Matrix3x2 matrix3x2 = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(position2), worldToLocalMatrix);
              int length = shadow.Comp.Points.Length;
              Array.Copy((Array) shadow.Comp.Points, (Array) destinationArray, length);
              for (int index = 0; index < length; ++index)
              {
                destinationArray[index] = ((Angle) ref angle2).RotateVec(ref destinationArray[index]);
                destinationArray[length + index] = destinationArray[index] + direction;
              }
              Span<Vector2> points = PhysicsHull.ComputePoints((ReadOnlySpan<Vector2>) destinationArray, length * 2);
              ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
              ((DrawingHandleBase) worldHandle).DrawPrimitives((DrawPrimitiveTopology) 2, (ReadOnlySpan<Vector2>) points, Color.White);
            }
          }), new Color?(Color.Transparent));
          this._clyde.BlurRenderTarget(viewport, (IRenderTarget) this._target, (IRenderTarget) this._blurTarget, eye, 1f);
          ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) viewport.LightRenderTarget, (Action) (() =>
          {
            Matrix3x2 worldToLocalMatrix = ((IRenderTarget) viewport.LightRenderTarget).GetWorldToLocalMatrix(eye, scale);
            ((DrawingHandleBase) worldHandle).SetTransform(ref worldToLocalMatrix);
            ((DrawingHandleBase) worldHandle).UseShader(this._protoManager.Index<ShaderPrototype>(SunShadowOverlay.MixShader).Instance());
            DrawingHandleWorld drawingHandleWorld = worldHandle;
            Texture texture = this._target.Texture;
            ref Box2Rotated local = ref worldBounds;
            Color black = Color.Black;
            Color? nullable = new Color?(((Color) ref black).WithAlpha(alpha));
            drawingHandleWorld.DrawTextureRect(texture, ref local, nullable);
          }), new Color?());
        }
      }
    }
  }
}
