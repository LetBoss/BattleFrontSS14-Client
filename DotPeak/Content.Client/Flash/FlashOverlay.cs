// Decompiled with JetBrains decompiler
// Type: Content.Client.Flash.FlashOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Flash;
using Content.Shared.Flash.Components;
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
namespace Content.Client.Flash;

public sealed class FlashOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> FlashedEffectShader = ProtoId<ShaderPrototype>.op_Implicit("FlashedEffect");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IGameTiming _timing;
  private readonly SharedFlashSystem _flash;
  private readonly StatusEffectsSystem _statusSys;
  private readonly ShaderInstance _shader;
  public float PercentComplete;
  public Texture? ScreenshotTexture;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public FlashOverlay()
  {
    IoCManager.InjectDependencies<FlashOverlay>(this);
    this._shader = this._prototypeManager.Index<ShaderPrototype>(FlashOverlay.FlashedEffectShader).InstanceUnique();
    this._flash = this._entityManager.System<SharedFlashSystem>();
    this._statusSys = this._entityManager.System<StatusEffectsSystem>();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    StatusEffectsComponent status;
    (TimeSpan, TimeSpan)? time;
    if (!localEntity.HasValue || !this._entityManager.HasComponent<FlashedComponent>(localEntity) || !this._entityManager.TryGetComponent<StatusEffectsComponent>(localEntity, ref status) || !this._statusSys.TryGetTime(localEntity.Value, ProtoId<StatusEffectPrototype>.op_Implicit(this._flash.FlashedKey), out time, status))
      return;
    TimeSpan curTime = this._timing.CurTime;
    float totalSeconds = (float) (time.Value.Item2 - time.Value.Item1).TotalSeconds;
    TimeSpan timeSpan = time.Value.Item1;
    this.PercentComplete = (float) (curTime - timeSpan).TotalSeconds / totalSeconds;
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    return this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) && args.Viewport.Eye == eyeComponent.Eye && (double) this.PercentComplete < 1.0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.RequestScreenTexture && this.ScreenTexture != null)
    {
      this.ScreenshotTexture = this.ScreenTexture;
      this.RequestScreenTexture = false;
    }
    if (this.ScreenshotTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._shader.SetParameter("percentComplete", this.PercentComplete);
    ((DrawingHandleBase) worldHandle).UseShader(this._shader);
    worldHandle.DrawTextureRectRegion(this.ScreenshotTexture, ref args.WorldBounds, new Color?(), new UIBox2?());
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  protected virtual void DisposeBehavior()
  {
    base.DisposeBehavior();
    this.ScreenshotTexture = (Texture) null;
  }
}
