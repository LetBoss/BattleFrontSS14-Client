// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.BeforeLightTargetOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Light;

public sealed class BeforeLightTargetOverlay : Overlay
{
  [Dependency]
  private IClyde _clyde;
  public IRenderTexture EnlargedLightTarget;
  public Box2Rotated EnlargedBounds;
  private float _skirting = 2f;
  public const int ContentZIndex = -10;

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public BeforeLightTargetOverlay()
  {
    IoCManager.InjectDependencies<BeforeLightTargetOverlay>(this);
    this.ZIndex = new int?(-10);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    Vector2i vector2i = Vector2i.op_Addition(((IRenderTarget) args.Viewport.LightRenderTarget).Size, (int) ((double) this._skirting * 32.0));
    this.EnlargedBounds = ((Box2Rotated) ref args.WorldBounds).Enlarged(this._skirting / 2f);
    IRenderTexture enlargedLightTarget = this.EnlargedLightTarget;
    if ((enlargedLightTarget != null ? (Vector2i.op_Inequality(((IRenderTarget) enlargedLightTarget).Size, vector2i) ? 1 : 0) : 1) != 0)
      this.EnlargedLightTarget = this._clyde.CreateRenderTarget(vector2i, new RenderTargetFormatParameters((RenderTargetColorFormat) 1, false), new TextureSampleParameters?(), "enlarged-light-copy");
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) this.EnlargedLightTarget, (Action) (() => { }), new Color?(this._clyde.GetClearColor(args.MapUid)));
  }
}
