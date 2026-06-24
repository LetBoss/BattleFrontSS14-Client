// Decompiled with JetBrains decompiler
// Type: Content.Client.Drugs.RainbowOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Content.Shared.Drugs;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Drugs;

public sealed class RainbowOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Rainbow");
  [Dependency]
  private IConfigurationManager _config;
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
  private readonly SharedStatusEffectsSystem _statusEffects;
  private readonly ShaderInstance _rainbowShader;
  public float Intoxication;
  public float TimeTicker;
  public float Phase;
  private const float VisualThreshold = 10f;
  private const float PowerDivisor = 250f;
  private float _timeScale;
  private float _warpScale;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  private float EffectScale
  {
    get => Math.Clamp((float) (((double) this.Intoxication - 10.0) / 250.0), 0.0f, 1f);
  }

  public RainbowOverlay()
  {
    IoCManager.InjectDependencies<RainbowOverlay>(this);
    this._statusEffects = this._sysMan.GetEntitySystem<SharedStatusEffectsSystem>();
    this._rainbowShader = this._prototypeManager.Index<ShaderPrototype>(RainbowOverlay.Shader).InstanceUnique();
    this._config.OnValueChanged<bool>(CCVars.ReducedMotion, new Action<bool>(this.OnReducedMotionChanged), true);
  }

  private void OnReducedMotionChanged(bool reducedMotion)
  {
    this._timeScale = reducedMotion ? 0.0f : 1f;
    this._warpScale = reducedMotion ? 0.0f : 1f;
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    TimeSpan? endTime;
    if (!localEntity.HasValue || !this._statusEffects.TryGetEffectsEndTimeWithComp<SeeingRainbowsStatusEffectComponent>(localEntity, out endTime))
      return;
    endTime.GetValueOrDefault();
    if (!endTime.HasValue)
      endTime = new TimeSpan?(TimeSpan.MaxValue);
    TimeSpan? nullable = endTime;
    TimeSpan curTime = this._timing.CurTime;
    float totalSeconds = (float) (nullable.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - curTime) : new TimeSpan?()).Value.TotalSeconds;
    this.TimeTicker += ((FrameEventArgs) ref args).DeltaSeconds;
    if ((double) totalSeconds - (double) this.TimeTicker > (double) totalSeconds / 16.0)
      this.Intoxication += (float) (((double) totalSeconds - (double) this.Intoxication) * (double) ((FrameEventArgs) ref args).DeltaSeconds / 16.0);
    else
      this.Intoxication -= this.Intoxication / (totalSeconds - this.TimeTicker) * ((FrameEventArgs) ref args).DeltaSeconds;
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    return this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) && args.Viewport.Eye == eyeComponent.Eye && (double) this.EffectScale > 0.0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._rainbowShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._rainbowShader.SetParameter("colorScale", this.EffectScale);
    this._rainbowShader.SetParameter("timeScale", this._timeScale);
    this._rainbowShader.SetParameter("warpScale", this._warpScale * this.EffectScale);
    this._rainbowShader.SetParameter("phase", this.Phase);
    ((DrawingHandleBase) worldHandle).UseShader(this._rainbowShader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
