// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.AfterLightTargetOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Light;

public sealed class AfterLightTargetOverlay : Overlay
{
  [Dependency]
  private IOverlayManager _overlay;
  public const int ContentZIndex = -6;

  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  public AfterLightTargetOverlay()
  {
    IoCManager.InjectDependencies<AfterLightTargetOverlay>(this);
    this.ZIndex = new int?(-6);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    IClydeViewport viewport = args.Viewport;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    if (viewport.Eye == null)
      return;
    BeforeLightTargetOverlay lightOverlay = this._overlay.GetOverlay<BeforeLightTargetOverlay>();
    Box2Rotated bounds = args.WorldBounds;
    Vector2 vector2_1 = Vector2i.op_Implicit(((IRenderTarget) viewport.LightRenderTarget).Size) / Vector2i.op_Implicit(viewport.Size);
    Vector2 vector2_2 = viewport.RenderScale / (Vector2.One / vector2_1);
    Matrix3x2 localMatrix = ((IRenderTarget) viewport.LightRenderTarget).GetWorldToLocalMatrix(viewport.Eye, vector2_2);
    Vector2i halfDiff = Vector2i.op_Division(Vector2i.op_Subtraction(((IRenderTarget) lightOverlay.EnlargedLightTarget).Size, ((IRenderTarget) viewport.LightRenderTarget).Size), 2);
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).RenderInRenderTarget((IRenderTarget) viewport.LightRenderTarget, (Action) (() =>
    {
      UIBox2i uiBox2i;
      // ISSUE: explicit constructor call
      ((UIBox2i) ref uiBox2i).\u002Ector(halfDiff.X, halfDiff.Y, ((IRenderTarget) viewport.LightRenderTarget).Size.X + halfDiff.X, ((IRenderTarget) viewport.LightRenderTarget).Size.Y + halfDiff.Y);
      ((DrawingHandleBase) worldHandle).SetTransform(ref localMatrix);
      DrawingHandleWorld drawingHandleWorld = worldHandle;
      Texture texture = lightOverlay.EnlargedLightTarget.Texture;
      ref Box2Rotated local = ref bounds;
      UIBox2? nullable1 = new UIBox2?(UIBox2i.op_Implicit(uiBox2i));
      Color? nullable2 = new Color?();
      UIBox2? nullable3 = nullable1;
      drawingHandleWorld.DrawTextureRectRegion(texture, ref local, nullable2, nullable3);
    }), new Color?(Color.Transparent));
  }
}
