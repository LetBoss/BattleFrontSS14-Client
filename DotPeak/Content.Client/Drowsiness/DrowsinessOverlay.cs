// Decompiled with JetBrains decompiler
// Type: Content.Client.Drowsiness.DrowsinessOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Drowsiness;
using Content.Shared.StatusEffectNew;
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
namespace Content.Client.Drowsiness;

public sealed class DrowsinessOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Drowsiness");
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
  private readonly ShaderInstance _drowsinessShader;
  public float CurrentPower;
  private const float PowerDivisor = 250f;
  private const float Intensity = 0.2f;
  private float _visualScale;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public DrowsinessOverlay()
  {
    IoCManager.InjectDependencies<DrowsinessOverlay>(this);
    this._statusEffects = this._sysMan.GetEntitySystem<SharedStatusEffectsSystem>();
    this._drowsinessShader = this._prototypeManager.Index<ShaderPrototype>(DrowsinessOverlay.Shader).InstanceUnique();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    TimeSpan? endTime;
    if (!localEntity.HasValue || !this._statusEffects.TryGetEffectsEndTimeWithComp<DrowsinessStatusEffectComponent>(localEntity, out endTime))
      return;
    endTime.GetValueOrDefault();
    if (!endTime.HasValue)
      endTime = new TimeSpan?(TimeSpan.MaxValue);
    TimeSpan? nullable = endTime;
    TimeSpan curTime = this._timing.CurTime;
    float totalSeconds = (float) (nullable.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - curTime) : new TimeSpan?()).Value.TotalSeconds;
    this.CurrentPower += (float) (8.0 * (0.5 * (double) totalSeconds - (double) this.CurrentPower) * (double) ((FrameEventArgs) ref args).DeltaSeconds / ((double) totalSeconds + 1.0));
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    if (!this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) || args.Viewport.Eye != eyeComponent.Eye)
      return false;
    this._visualScale = Math.Clamp(this.CurrentPower / 250f, 0.0f, 1f);
    return (double) this._visualScale > 0.0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._drowsinessShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._drowsinessShader.SetParameter("Strength", this._visualScale * 0.2f);
    ((DrawingHandleBase) worldHandle).UseShader(this._drowsinessShader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
