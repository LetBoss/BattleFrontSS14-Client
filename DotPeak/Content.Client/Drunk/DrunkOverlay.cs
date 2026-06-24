// Decompiled with JetBrains decompiler
// Type: Content.Client.Drunk.DrunkOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Drunk;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Drunk;

public sealed class DrunkOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Drunk");
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntitySystemManager _sysMan;
  [Dependency]
  private IGameTiming _timing;
  private readonly ShaderInstance _drunkShader;
  public float CurrentBoozePower;
  private const float VisualThreshold = 10f;
  private const float PowerDivisor = 250f;
  private float _visualScale;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public DrunkOverlay()
  {
    IoCManager.InjectDependencies<DrunkOverlay>(this);
    this._drunkShader = this._prototypeManager.Index<ShaderPrototype>(DrunkOverlay.Shader).InstanceUnique();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    StatusEffectsComponent status;
    (TimeSpan, TimeSpan)? time;
    if (!localEntity.HasValue || !this._entityManager.HasComponent<DrunkComponent>(localEntity) || !this._entityManager.TryGetComponent<StatusEffectsComponent>(localEntity, ref status) || !this._sysMan.GetEntitySystem<StatusEffectsSystem>().TryGetTime(localEntity.Value, ProtoId<StatusEffectPrototype>.op_Implicit(SharedDrunkSystem.DrunkKey), out time, status))
      return;
    TimeSpan curTime = this._timing.CurTime;
    float totalSeconds = (float) (time.Value.Item2 - curTime).TotalSeconds;
    this.CurrentBoozePower += (float) (8.0 * (0.5 * (double) totalSeconds - (double) this.CurrentBoozePower) * (double) ((FrameEventArgs) ref args).DeltaSeconds / ((double) totalSeconds + 1.0));
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    if (!this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) || args.Viewport.Eye != eyeComponent.Eye)
      return false;
    this._visualScale = this.BoozePowerToVisual(this.CurrentBoozePower);
    return (double) this._visualScale > 0.0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._drunkShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._drunkShader.SetParameter("boozePower", this._visualScale);
    ((DrawingHandleBase) worldHandle).UseShader(this._drunkShader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private float BoozePowerToVisual(float boozePower)
  {
    return (double) boozePower < 50.0 ? 0.0f : Math.Clamp((float) (((double) boozePower - 10.0) / 250.0), 0.0f, 1f);
  }
}
