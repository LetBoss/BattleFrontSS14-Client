// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.BlackAndWhiteOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client.Overlays;

public sealed class BlackAndWhiteOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("GreyscaleFullscreen");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly ShaderInstance _greyscaleShader;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public BlackAndWhiteOverlay()
  {
    IoCManager.InjectDependencies<BlackAndWhiteOverlay>(this);
    this._greyscaleShader = this._prototypeManager.Index<ShaderPrototype>(BlackAndWhiteOverlay.Shader).InstanceUnique();
    this.ZIndex = new int?(10);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    this._greyscaleShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
    ((DrawingHandleBase) worldHandle).UseShader(this._greyscaleShader);
    worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
