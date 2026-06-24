// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Explosion.RMCExplosionShockWaveOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Explosion.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Explosion;

public sealed class RMCExplosionShockWaveOverlay : Overlay, IEntityEventSubscriber
{
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private SharedTransformSystem? _xformSystem;
  private readonly ShaderInstance _shader;
  public const int MaxCount = 10;
  private readonly Vector2[] _positions = new Vector2[10];
  private readonly float[] _falloffPower = new float[10];
  private readonly float[] _sharpness = new float[10];
  private readonly float[] _width = new float[10];
  private int _count;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public RMCExplosionShockWaveOverlay()
  {
    IoCManager.InjectDependencies<RMCExplosionShockWaveOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCShockWave")).Instance().Duplicate();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    if (args.Viewport.Eye == null || this._xformSystem == null && !this._entMan.TrySystem<SharedTransformSystem>(ref this._xformSystem))
      return false;
    EntityQueryEnumerator<RMCExplosionShockWaveComponent, TransformComponent> entityQueryEnumerator = this._entMan.EntityQueryEnumerator<RMCExplosionShockWaveComponent, TransformComponent>();
    this._count = 0;
    EntityUid entityUid;
    RMCExplosionShockWaveComponent shockWaveComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref shockWaveComponent, ref transformComponent))
    {
      if (!MapId.op_Inequality(transformComponent.MapID, args.MapId))
      {
        Vector2 worldPosition = this._xformSystem.GetWorldPosition(entityUid);
        Vector2 local = args.Viewport.WorldToLocal(worldPosition);
        local.Y = (float) (1.0 - (double) local.Y / (double) args.Viewport.Size.Y);
        local.X /= (float) args.Viewport.Size.X;
        this._positions[this._count] = local;
        this._falloffPower[this._count] = shockWaveComponent.FalloffPower;
        this._sharpness[this._count] = shockWaveComponent.Sharpness;
        this._width[this._count] = shockWaveComponent.Width;
        ++this._count;
        if (this._count == 10)
          break;
      }
    }
    return this._count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null || args.Viewport.Eye == null)
      return;
    this._shader?.SetParameter("renderScale", args.Viewport.RenderScale * args.Viewport.Eye.Scale);
    this._shader?.SetParameter("count", this._count);
    this._shader?.SetParameter("position", this._positions);
    this._shader?.SetParameter("falloffPower", this._falloffPower);
    this._shader?.SetParameter("sharpness", this._sharpness);
    this._shader?.SetParameter("width", this._width);
    this._shader?.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
