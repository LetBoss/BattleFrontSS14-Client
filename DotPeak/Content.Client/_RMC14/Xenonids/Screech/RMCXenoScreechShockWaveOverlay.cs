// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Screech.RMCXenoScreechShockWaveOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Screech;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Screech;

public sealed class RMCXenoScreechShockWaveOverlay : Overlay, IEntityEventSubscriber
{
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private SharedTransformSystem? _xformSystem;
  private readonly ShaderInstance _shader;
  private Vector2 _position;
  private float _waveStrength;
  private float _waveSpeed;
  private float _downScale;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public RMCXenoScreechShockWaveOverlay()
  {
    IoCManager.InjectDependencies<RMCXenoScreechShockWaveOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCXenoScreechShockWave")).Instance().Duplicate();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EntityUid entityUid;
    RMCXenoScreechShockWaveComponent shockWaveComponent;
    TransformComponent transformComponent;
    if (args.Viewport.Eye == null || this._xformSystem == null && !this._entMan.TrySystem<SharedTransformSystem>(ref this._xformSystem) || !this._entMan.EntityQueryEnumerator<RMCXenoScreechShockWaveComponent, TransformComponent>().MoveNext(ref entityUid, ref shockWaveComponent, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, args.MapId))
      return false;
    Vector2 worldPosition = this._xformSystem.GetWorldPosition(entityUid);
    Vector2 local = args.Viewport.WorldToLocal(worldPosition);
    local.Y = (float) (1.0 - (double) local.Y / (double) args.Viewport.Size.Y);
    local.X /= (float) args.Viewport.Size.X;
    this._position = local;
    this._waveStrength = shockWaveComponent.WaveStrength;
    this._waveSpeed = shockWaveComponent.WaveSpeed;
    this._downScale = shockWaveComponent.DownScale;
    return true;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null || args.Viewport.Eye == null)
      return;
    this._shader?.SetParameter("position", this._position);
    this._shader?.SetParameter("waveSpeed", this._waveSpeed);
    this._shader?.SetParameter("downScale", this._downScale);
    this._shader?.SetParameter("waveStrength", this._waveStrength);
    this._shader?.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
