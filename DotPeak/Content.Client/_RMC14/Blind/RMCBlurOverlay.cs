// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Blind.RMCBlurOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.BlurredVision;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client._RMC14.Blind;

public sealed class RMCBlurOverlay : Overlay
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntityManager _entityManager;
  private readonly ShaderInstance _blurShader;
  private const float BlurAmount = 0.01f;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public RMCBlurOverlay(IEntityManager entManager)
  {
    IoCManager.InjectDependencies<RMCBlurOverlay>(this);
    this._blurShader = this._prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCBlurryVisionX")).InstanceUnique();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    EyeComponent eyeComponent;
    return this._entityManager.TryGetComponent<EyeComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity, ref eyeComponent) && this._entityManager.HasComponent<RMCBlindedComponent>(((ISharedPlayerManager) this._playerManager).LocalEntity) && args.Viewport.Eye == eyeComponent.Eye;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null || !((ISharedPlayerManager) this._playerManager).LocalEntity.HasValue)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._blurShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    this._blurShader.SetParameter("BLUR_AMOUNT", 0.01f);
    ((DrawingHandleBase) worldHandle).UseShader(this._blurShader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
