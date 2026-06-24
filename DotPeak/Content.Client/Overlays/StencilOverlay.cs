// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.StencilOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax;
using Content.Client.Weather;
using Content.Shared.Light.Components;
using Content.Shared.Salvage;
using Content.Shared.Weather;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
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
namespace Content.Client.Overlays;

public sealed class StencilOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> CircleShader = ProtoId<ShaderPrototype>.op_Implicit("WorldGradientCircle");
  private static readonly ProtoId<ShaderPrototype> StencilMask = ProtoId<ShaderPrototype>.op_Implicit(nameof (StencilMask));
  private static readonly ProtoId<ShaderPrototype> StencilDraw = ProtoId<ShaderPrototype>.op_Implicit(nameof (StencilDraw));
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private IPrototypeManager _protoManager;
  private readonly ParallaxSystem _parallax;
  private readonly SharedTransformSystem _transform;
  private readonly SharedMapSystem _map;
  private readonly SpriteSystem _sprite;
  private readonly WeatherSystem _weather;
  private IRenderTexture? _blep;
  private readonly ShaderInstance _shader;
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public StencilOverlay(
    ParallaxSystem parallax,
    SharedTransformSystem transform,
    SharedMapSystem map,
    SpriteSystem sprite,
    WeatherSystem weather)
  {
    this.ZIndex = new int?(1);
    this._parallax = parallax;
    this._transform = transform;
    this._map = map;
    this._sprite = sprite;
    this._weather = weather;
    IoCManager.InjectDependencies<StencilOverlay>(this);
    this._shader = this._protoManager.Index<ShaderPrototype>(StencilOverlay.CircleShader).InstanceUnique();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityUid mapOrInvalid = this._map.GetMapOrInvalid(new MapId?(args.MapId));
    Matrix3x2 worldToLocalMatrix = args.Viewport.GetWorldToLocalMatrix();
    Vector2i? size1 = this._blep?.Texture.Size;
    Vector2i size2 = args.Viewport.Size;
    if ((size1.HasValue ? (Vector2i.op_Inequality(size1.GetValueOrDefault(), size2) ? 1 : 0) : 1) != 0)
    {
      ((IDisposable) this._blep)?.Dispose();
      this._blep = this._clyde.CreateRenderTarget(args.Viewport.Size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "weather-stencil");
    }
    WeatherComponent weatherComponent;
    if (this._entManager.TryGetComponent<WeatherComponent>(mapOrInvalid, ref weatherComponent))
    {
      foreach ((ProtoId<WeatherPrototype> key, WeatherData component) in weatherComponent.Weather)
      {
        WeatherPrototype weatherProto;
        if (this._protoManager.TryIndex<WeatherPrototype>(key, ref weatherProto))
        {
          float percent = this._weather.GetPercent(component, mapOrInvalid);
          this.DrawWeather(in args, weatherProto, percent, worldToLocalMatrix);
        }
      }
    }
    RestrictedRangeComponent rangeComp;
    if (this._entManager.TryGetComponent<RestrictedRangeComponent>(mapOrInvalid, ref rangeComp))
      this.DrawRestrictedRange(in args, rangeComp, worldToLocalMatrix);
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).UseShader((ShaderInstance) null);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) worldHandle).SetTransform(ref local);
  }

  private void DrawRestrictedRange(
    in OverlayDrawArgs args,
    RestrictedRangeComponent rangeComp,
    Matrix3x2 invMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    float x1 = args.Viewport.RenderScale.X;
    IEye eye1 = args.Viewport.Eye;
    float x2 = (eye1 != null ? eye1.Zoom : Vector2.One).X;
    double num1 = (double) MathF.Min(10f, rangeComp.Range);
    Vector2 vector2 = Vector2.Transform(rangeComp.Origin, invMatrix);
    int y = args.Viewport.Size.Y;
    float num2 = (float) ((double) rangeComp.Range * (double) x1 / (double) x2 * 32.0);
    double num3 = (double) x1;
    float num4 = (float) (num1 * num3 / (double) x2 * 32.0);
    float num5 = num2 - num4;
    this._shader.SetParameter("position", new Vector2(vector2.X, (float) y - vector2.Y));
    this._shader.SetParameter("maxRange", num2);
    this._shader.SetParameter("minRange", num5);
    this._shader.SetParameter("bufferRange", num4);
    this._shader.SetParameter("gradient", 0.8f);
    Box2 worldAabb = args.WorldAABB;
    Box2Rotated worldBounds = args.WorldBounds;
    IEye eye2 = args.Viewport.Eye;
    Vector2 position = eye2 != null ? eye2.Position.Position : Vector2.Zero;
    Box2 localAABB = Matrix3Helpers.TransformBox(invMatrix, ref worldAabb);
    ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._blep, (Action) (() =>
    {
      ((DrawingHandleBase) worldHandle).UseShader(this._shader);
      worldHandle.DrawRect(localAABB, Color.White, true);
    }), new Color?(Color.Transparent));
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
    ((DrawingHandleBase) worldHandle).UseShader(this._protoManager.Index<ShaderPrototype>(StencilOverlay.StencilMask).Instance());
    worldHandle.DrawTextureRect(this._blep.Texture, ref worldBounds, new Color?());
    TimeSpan realTime = this._timing.RealTime;
    Texture frame = this._sprite.GetFrame((SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Parallaxes/noise.png")), realTime, true);
    ((DrawingHandleBase) worldHandle).UseShader(this._protoManager.Index<ShaderPrototype>(StencilOverlay.StencilDraw).Instance());
    this._parallax.DrawParallax(worldHandle, worldAabb, frame, realTime, position, new Vector2(0.5f, 0.0f));
  }

  private void DrawWeather(
    in OverlayDrawArgs args,
    WeatherPrototype weatherProto,
    float alpha,
    Matrix3x2 invMatrix)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    MapId mapId = args.MapId;
    Box2 worldAABB = args.WorldAABB;
    Box2Rotated worldBounds = args.WorldBounds;
    IEye eye = args.Viewport.Eye;
    Vector2 vector2 = eye != null ? eye.Position.Position : Vector2.Zero;
    ((DrawingHandleBase) worldHandle).RenderInRenderTarget((IRenderTarget) this._blep, (Action) (() =>
    {
      EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
      this._grids.Clear();
      this._mapManager.FindGridsIntersecting(mapId, worldAABB, ref this._grids, false, true);
      foreach (Entity<MapGridComponent> grid in this._grids)
      {
        Matrix3x2 matrix3x2 = Matrix3x2.Multiply(this._transform.GetWorldMatrix(Entity<MapGridComponent>.op_Implicit(grid), entityQuery), invMatrix);
        ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2);
        RoofComponent roofComp;
        this._entManager.TryGetComponent<RoofComponent>(grid.Owner, ref roofComp);
        foreach (TileRef tileRef in this._map.GetTilesIntersecting(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), worldAABB, true, (Predicate<TileRef>) null))
        {
          if (!this._weather.CanWeatherAffect(grid.Owner, Entity<MapGridComponent>.op_Implicit(grid), tileRef, roofComp))
          {
            Box2 box2;
            // ISSUE: explicit constructor call
            ((Box2) ref box2).\u002Ector(Vector2i.op_Implicit(Vector2i.op_Multiply(tileRef.GridIndices, (int) grid.Comp.TileSize)), Vector2i.op_Implicit(Vector2i.op_Multiply(Vector2i.op_Addition(tileRef.GridIndices, Vector2i.One), (int) grid.Comp.TileSize)));
            worldHandle.DrawRect(box2, Color.White, true);
          }
        }
      }
    }), new Color?(Color.Transparent));
    DrawingHandleWorld drawingHandleWorld1 = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local1 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld1).SetTransform(ref local1);
    ((DrawingHandleBase) worldHandle).UseShader(this._protoManager.Index<ShaderPrototype>(StencilOverlay.StencilMask).Instance());
    worldHandle.DrawTextureRect(this._blep.Texture, ref worldBounds, new Color?());
    TimeSpan realTime = this._timing.RealTime;
    Texture frame = this._sprite.GetFrame(weatherProto.Sprite, realTime, true);
    ((DrawingHandleBase) worldHandle).UseShader(this._protoManager.Index<ShaderPrototype>(StencilOverlay.StencilDraw).Instance());
    ParallaxSystem parallax = this._parallax;
    DrawingHandleWorld worldHandle1 = worldHandle;
    Box2 worldAABB1 = worldAABB;
    Texture sprite = frame;
    TimeSpan curTime = realTime;
    Vector2 position = vector2;
    Vector2 zero = Vector2.Zero;
    Color color = weatherProto.Color ?? Color.White;
    Color? modulate = new Color?(((Color) ref color).WithAlpha(alpha));
    parallax.DrawParallax(worldHandle1, worldAABB1, sprite, curTime, position, zero, modulate: modulate);
    DrawingHandleWorld drawingHandleWorld2 = worldHandle;
    identity = Matrix3x2.Identity;
    ref Matrix3x2 local2 = ref identity;
    ((DrawingHandleBase) drawingHandleWorld2).SetTransform(ref local2);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
