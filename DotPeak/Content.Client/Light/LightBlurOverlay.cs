// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.LightBlurOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Light;

public sealed class LightBlurOverlay : Overlay
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IOverlayManager _overlay;
  public const int ContentZIndex = -7;
  private IRenderTarget? _blurTarget;

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public LightBlurOverlay()
  {
    IoCManager.InjectDependencies<LightBlurOverlay>(this);
    this.ZIndex = new int?(-7);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (args.Viewport.Eye == null)
      return;
    BeforeLightTargetOverlay overlay = this._overlay.GetOverlay<BeforeLightTargetOverlay>();
    Vector2i size = ((IRenderTarget) overlay.EnlargedLightTarget).Size;
    IRenderTarget blurTarget = this._blurTarget;
    if ((blurTarget != null ? (Vector2i.op_Inequality(blurTarget.Size, size) ? 1 : 0) : 1) != 0)
      this._blurTarget = (IRenderTarget) this._clyde.CreateRenderTarget(size, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "enlarged-light-blur");
    IRenderTexture enlargedLightTarget = overlay.EnlargedLightTarget;
    this._clyde.BlurRenderTarget(args.Viewport, (IRenderTarget) enlargedLightTarget, this._blurTarget, args.Viewport.Eye, 70f);
  }
}
