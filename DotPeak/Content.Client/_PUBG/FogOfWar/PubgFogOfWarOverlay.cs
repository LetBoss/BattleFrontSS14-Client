// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.FogOfWar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarOverlay : Overlay
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  private readonly EntityLookupSystem _lookup;
  private readonly SharedTransformSystem _xform;
  private readonly PubgFogOfWarSystem _system;
  private readonly PubgFovModifierSystem _fovModifiers;
  private ShaderInstance _maskShader;
  private ShaderInstance _blurXShader;
  private ShaderInstance _blurYShader;
  private IRenderTexture? _blurPass;
  private IRenderTexture? _backBuffer;
  private IRenderTexture? _maskTarget;
  private IRenderTexture? _maskBlurTarget;
  private const float MaskBlurRadius = 6f;
  private const float SeenMaskValue = 0.5f;
  private const float ConeSoftness = 0.08f;
  private const float SafeZoneMeters = 0.65f;
  private const float SafeZoneSoftMeters = 0.2f;
  private const int RayCount = 256 /*0x0100*/;
  private const float FacingOffset = 0.0f;
  private const float BlurScale = 0.7f;
  private static readonly TimeSpan BlurUpdateInterval = TimeSpan.FromSeconds(0.01666666753590107);
  private TimeSpan _nextBlurUpdate;
  private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();
  private readonly List<Box2> _occluderBoxes = new List<Box2>();
  private readonly List<Vector2> _fanVertices = new List<Vector2>();

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public PubgFogOfWarOverlay(PubgFogOfWarSystem system)
  {
    IoCManager.InjectDependencies<PubgFogOfWarOverlay>(this);
    this._system = system;
    this._xform = this._entManager.System<SharedTransformSystem>();
    this._lookup = this._entManager.System<EntityLookupSystem>();
    this._fovModifiers = this._entManager.System<PubgFovModifierSystem>();
    this.InitializeShaders();
    this.ZIndex = new int?(100);
  }

  private void InitializeShaders()
  {
    this._maskShader?.Dispose();
    this._blurXShader?.Dispose();
    this._blurYShader?.Dispose();
    this._maskShader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("PubgVisionMask")).InstanceUnique();
    this._blurXShader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("BlurryVisionX")).InstanceUnique();
    this._blurYShader = this._prototype.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("BlurryVisionY")).InstanceUnique();
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    ((IDisposable) this._blurPass)?.Dispose();
    ((IDisposable) this._backBuffer)?.Dispose();
    ((IDisposable) this._maskTarget)?.Dispose();
    ((IDisposable) this._maskBlurTarget)?.Dispose();
    this._maskShader?.Dispose();
    this._blurXShader?.Dispose();
    this._blurYShader?.Dispose();
    this._maskShader = (ShaderInstance) null;
    this._blurXShader = (ShaderInstance) null;
    this._blurYShader = (ShaderInstance) null;
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    if (this._maskShader == null || this._blurXShader == null || this._blurYShader == null)
      this.InitializeShaders();
    if (!((ISharedPlayerManager) this._player).LocalEntity.HasValue || !this._system.Active)
      return false;
    Vector2i size = args.Viewport.Size;
    Vector2i vector2i = Vector2i.op_Explicit(Vector2i.op_Multiply(size, 0.7f));
    if (this._backBuffer == null || Vector2i.op_Inequality(((IRenderTarget) this._backBuffer).Size, vector2i))
    {
      ((IDisposable) this._backBuffer)?.Dispose();
      this._backBuffer = this._clyde.CreateRenderTarget(vector2i, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "pubg-fov-backbuffer");
      ((IDisposable) this._blurPass)?.Dispose();
      this._blurPass = this._clyde.CreateRenderTarget(vector2i, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "pubg-fov-blurpass");
    }
    if (this._maskTarget == null || Vector2i.op_Inequality(((IRenderTarget) this._maskTarget).Size, size))
    {
      ((IDisposable) this._maskTarget)?.Dispose();
      ((IDisposable) this._maskBlurTarget)?.Dispose();
      this._maskTarget = this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "pubg-fov-mask");
      this._maskBlurTarget = this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "pubg-fov-mask-blur");
    }
    return true;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid? playerEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    IEye eye = args.Viewport.Eye;
    if (!playerEntity.HasValue || MapId.op_Equality(args.MapId, MapId.Nullspace) || this.ScreenTexture == null || this._backBuffer == null || this._blurPass == null || this._maskTarget == null || this._maskBlurTarget == null || eye == null)
      return;
    DrawingHandleWorld handle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2Rotated worldBounds = args.WorldBounds;
    Matrix3x2 invMatrix = args.Viewport.GetWorldToLocalMatrix();
    if (this._timing.RealTime >= this._nextBlurUpdate)
    {
      Box2 blurBounds = new Box2(Vector2.Zero, Vector2i.op_Implicit(((IRenderTarget) this._blurPass).Size));
      ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._blurPass, (Action) (() =>
      {
        this._blurXShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
        ((DrawingHandleBase) handle).UseShader(this._blurXShader);
        handle.DrawRect(blurBounds, Color.White, true);
      }), new Color?(Color.Transparent));
      ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._backBuffer, (Action) (() =>
      {
        this._blurYShader.SetParameter("SCREEN_TEXTURE", this._blurPass.Texture);
        ((DrawingHandleBase) handle).UseShader(this._blurYShader);
        handle.DrawRect(blurBounds, Color.White, true);
      }), new Color?(Color.Transparent));
      this._nextBlurUpdate = this._timing.RealTime + PubgFogOfWarOverlay.BlurUpdateInterval;
    }
    MapId mapId = args.MapId;
    TransformComponent playerXform;
    this._entManager.TryGetComponent<TransformComponent>(playerEntity.Value, ref playerXform);
    PubgFogOfWarComponent visionComponent;
    this._entManager.TryGetComponent<PubgFogOfWarComponent>(playerEntity.Value, ref visionComponent);
    ((DrawingHandleBase) handle).RenderInRenderTarget((IRenderTarget) this._maskTarget, (Action) (() =>
    {
      ((DrawingHandleBase) handle).SetTransform(ref invMatrix);
      handle.DrawRect(ref worldBounds, new Color(0.5f, 0.5f, 0.5f, 1f), true);
      if (playerXform == null || visionComponent == null)
        return;
      this.BuildVisibleCone(playerEntity.Value, playerXform, visionComponent, visionComponent.DesiredViewAngle.HasValue ? visionComponent.CurrentAngle : this._xform.GetWorldRotation(playerEntity.Value), mapId, this._fanVertices);
      if (this._fanVertices.Count < 3)
        return;
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 2, this._fanVertices, Color.White);
    }), new Color?(Color.Transparent));
    this._clyde.BlurRenderTarget(args.Viewport, (IRenderTarget) this._maskTarget, (IRenderTarget) this._maskBlurTarget, eye, 6f);
    this._maskShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._maskShader.SetParameter("BLURRED_TEXTURE", this._backBuffer.Texture);
    this._maskShader.SetParameter("MASK_TEXTURE", this._maskBlurTarget.Texture);
    float num1 = 0.85f;
    float num2 = 120f;
    Angle angle1 = new Angle();
    if (visionComponent != null)
    {
      num1 = visionComponent.ConeOpacity;
      num2 = this._fovModifiers.GetEffectiveFov(playerEntity.Value, visionComponent);
      if (visionComponent.DesiredViewAngle.HasValue)
        angle1 = visionComponent.CurrentAngle;
    }
    if (playerXform == null)
      return;
    Vector2 worldPosition = this._xform.GetWorldPosition(playerXform);
    Vector2 local1 = args.Viewport.WorldToLocal(worldPosition);
    local1.Y = (float) args.Viewport.Size.Y - local1.Y;
    if (Angle.op_Equality(angle1, new Angle()))
      angle1 = this._xform.GetWorldRotation(playerEntity.Value);
    Angle angle2 = new Angle(angle1.Theta + 0.0);
    Vector2 worldVec = ((Angle) ref angle2).ToWorldVec();
    Vector2 local2 = args.Viewport.WorldToLocal(worldPosition + worldVec);
    local2.Y = (float) args.Viewport.Size.Y - local2.Y;
    Vector2 vector2_1 = local2 - local1;
    Vector2 vector2_2 = (double) vector2_1.LengthSquared() <= 1.0 / 1000.0 ? Vector2.UnitY : Vector2.Normalize(vector2_1);
    Vector2 local3 = args.Viewport.WorldToLocal(worldPosition + new Vector2(0.65f, 0.0f));
    local3.Y = (float) args.Viewport.Size.Y - local3.Y;
    float num3 = (local3 - local1).Length();
    Vector2 local4 = args.Viewport.WorldToLocal(worldPosition + new Vector2(0.849999964f, 0.0f));
    local4.Y = (float) args.Viewport.Size.Y - local4.Y;
    float num4 = MathF.Max(1f, (local4 - local1).Length() - num3);
    this._maskShader.SetParameter("shadowOpacity", num1);
    this._maskShader.SetParameter("playerScreen", local1);
    this._maskShader.SetParameter("viewDir", vector2_2);
    this._maskShader.SetParameter("fovCos", MathF.Cos(MathHelper.DegreesToRadians(num2 * 0.5f)));
    this._maskShader.SetParameter("coneSoftness", 0.08f);
    this._maskShader.SetParameter("safeRadius", num3);
    this._maskShader.SetParameter("safeSoftness", num4);
    ((DrawingHandleBase) handle).UseShader(this._maskShader);
    handle.DrawRect(ref worldBounds, Color.White, true);
    ((DrawingHandleBase) handle).UseShader((ShaderInstance) null);
  }

  private void BuildVisibleCone(
    EntityUid playerUid,
    TransformComponent playerXform,
    PubgFogOfWarComponent visionComponent,
    Angle viewAngle,
    MapId mapId,
    List<Vector2> vertices)
  {
    Vector2 worldPosition = this._xform.GetWorldPosition(playerXform);
    float num1 = MathF.Max(0.1f, visionComponent.Range);
    float effectiveFov = this._fovModifiers.GetEffectiveFov(playerUid, visionComponent);
    this._occluders.Clear();
    this._lookup.GetEntitiesInRange<OccluderComponent>(mapId, worldPosition, num1, this._occluders, (LookupFlags) 5);
    this._occluderBoxes.Clear();
    foreach (Entity<OccluderComponent> occluder in this._occluders)
    {
      EntityUid entityUid1;
      OccluderComponent occluderComponent1;
      occluder.Deconstruct(ref entityUid1, ref occluderComponent1);
      EntityUid entityUid2 = entityUid1;
      OccluderComponent occluderComponent2 = occluderComponent1;
      TransformComponent transformComponent;
      if (occluderComponent2.Enabled && this._entManager.TryGetComponent<TransformComponent>(entityUid2, ref transformComponent) && !MapId.op_Inequality(transformComponent.MapID, mapId))
        this._occluderBoxes.Add(Matrix3Helpers.TransformBox(this._xform.GetWorldMatrix(entityUid2), ref occluderComponent2.BoundingBox));
    }
    vertices.Clear();
    vertices.Add(worldPosition);
    double radians = (double) MathHelper.DegreesToRadians(effectiveFov * 0.5f);
    Angle angle1 = viewAngle;
    float num2 = (float) -radians;
    float num3 = (float) (radians * 2.0 / 256.0);
    for (int index1 = 0; index1 <= 256 /*0x0100*/; ++index1)
    {
      float num4 = num2 + num3 * (float) index1;
      Angle angle2 = new Angle(angle1.Theta + (double) num4);
      Vector2 worldVec = ((Angle) ref angle2).ToWorldVec();
      float num5 = num1;
      for (int index2 = 0; index2 < this._occluderBoxes.Count; ++index2)
      {
        float distance;
        if (PubgFogOfWarOverlay.RayAabb(worldPosition, worldVec, this._occluderBoxes[index2], out distance) && (double) distance >= 0.0 && (double) distance < (double) num5)
          num5 = distance;
      }
      vertices.Add(worldPosition + worldVec * num5);
    }
  }

  private static bool RayAabb(Vector2 origin, Vector2 dir, Box2 box, out float distance)
  {
    distance = 0.0f;
    float tmin = 0.0f;
    float tmax = float.PositiveInfinity;
    if (!PubgFogOfWarOverlay.RaySlab(origin.X, dir.X, box.Left, box.Right, ref tmin, ref tmax) || !PubgFogOfWarOverlay.RaySlab(origin.Y, dir.Y, box.Bottom, box.Top, ref tmin, ref tmax) || (double) tmax < 0.0)
      return false;
    distance = (double) tmin >= 0.0 ? tmin : tmax;
    return (double) distance >= 0.0;
  }

  private static bool RaySlab(
    float origin,
    float dir,
    float min,
    float max,
    ref float tmin,
    ref float tmax)
  {
    if ((double) MathF.Abs(dir) < 9.9999997473787516E-05)
      return (double) origin >= (double) min && (double) origin <= (double) max;
    float num1 = 1f / dir;
    float num2 = (min - origin) * num1;
    float num3 = (max - origin) * num1;
    if ((double) num2 > (double) num3)
    {
      double num4 = (double) num2;
      num2 = num3;
      num3 = (float) num4;
    }
    if ((double) num2 > (double) tmin)
      tmin = num2;
    if ((double) num3 < (double) tmax)
      tmax = num3;
    return (double) tmax >= (double) tmin;
  }
}
