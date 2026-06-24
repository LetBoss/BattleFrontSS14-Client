// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Stun.DazedOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Stun;

public sealed class DazedOverlay : Overlay
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly ShaderInstance _vignetteShader;
  private float _outerFadeStart;
  private float _outerFadeEnd = 0.8f;
  private float _alpha = 1f;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public bool IsEnabled { get; set; }

  public DazedOverlay()
  {
    IoCManager.InjectDependencies<DazedOverlay>(this);
    this._vignetteShader = this._prototypeManager.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("GradientCircleMask")).InstanceUnique();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this.IsEnabled)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    Box2 worldAabb = args.WorldAABB;
    int width = ((UIBox2i) ref args.ViewportBounds).Width;
    this._vignetteShader.SetParameter("color", new Vector3(0.0f, 0.0f, 0.0f));
    this._vignetteShader.SetParameter("darknessAlphaOuter", this._alpha);
    this._vignetteShader.SetParameter("darknessAlphaInner", 0.0f);
    this._vignetteShader.SetParameter("innerCircleRadius", (float) ((double) this._outerFadeStart * (double) width * 0.5));
    this._vignetteShader.SetParameter("innerCircleMaxRadius", (float) ((double) this._outerFadeStart * (double) width * 0.5));
    this._vignetteShader.SetParameter("outerCircleRadius", (float) ((double) this._outerFadeEnd * (double) width * 0.5));
    this._vignetteShader.SetParameter("outerCircleMaxRadius", (float) ((double) this._outerFadeEnd * (double) width * 0.5));
    ((DrawingHandleBase) worldHandle).UseShader(this._vignetteShader);
    worldHandle.DrawRect(worldAabb, Color.White, true);
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }
}
