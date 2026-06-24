// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.MainViewport
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class MainViewport : UIWidget
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private ViewportManager _vpManager;

  public ScalingViewport Viewport { get; }

  public MainViewport()
  {
    IoCManager.InjectDependencies<MainViewport>(this);
    ScalingViewport scalingViewport = new ScalingViewport();
    scalingViewport.AlwaysRender = true;
    scalingViewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
    scalingViewport.MouseFilter = (Control.MouseFilterMode) 0;
    this.Viewport = scalingViewport;
    ((Control) this).AddChild((Control) this.Viewport);
  }

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    this._vpManager.AddViewport(this);
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this._vpManager.RemoveViewport(this);
  }

  public void UpdateCfg()
  {
    int num = this._cfg.GetCVar<bool>(CCVars.ViewportStretch) ? 1 : 0;
    bool cvar1 = this._cfg.GetCVar<bool>(CCVars.ViewportScaleRender);
    int cvar2 = this._cfg.GetCVar<int>(CCVars.ViewportFixedScaleFactor);
    bool cvar3 = this._cfg.GetCVar<bool>(CCVars.ViewportVerticalFit);
    if (num != 0)
    {
      int? nullable = this.CalcSnappingFactor();
      if (!nullable.HasValue)
      {
        this.Viewport.FixedStretchSize = new Vector2i?();
        this.Viewport.StretchMode = ScalingViewportStretchMode.Bilinear;
        this.Viewport.IgnoreDimension = cvar3 ? ScalingViewportIgnoreDimension.Horizontal : ScalingViewportIgnoreDimension.None;
        if (cvar1)
        {
          this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
          return;
        }
        this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
        this.Viewport.FixedRenderScale = 1;
        return;
      }
      cvar2 = nullable.Value;
    }
    this.Viewport.FixedStretchSize = new Vector2i?(Vector2i.op_Multiply(this.Viewport.ViewportSize, cvar2));
    this.Viewport.StretchMode = ScalingViewportStretchMode.Nearest;
    if (cvar1)
    {
      this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
      this.Viewport.FixedRenderScale = cvar2;
    }
    else
    {
      this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
      this.Viewport.FixedRenderScale = 1;
    }
  }

  private int? CalcSnappingFactor()
  {
    int cvar1 = this._cfg.GetCVar<int>(CCVars.ViewportSnapToleranceMargin);
    int cvar2 = this._cfg.GetCVar<int>(CCVars.ViewportSnapToleranceClip);
    bool cvar3 = this._cfg.GetCVar<bool>(CCVars.ViewportVerticalFit);
    for (int index = 1; index <= 10; ++index)
    {
      int toleranceMargin = index * cvar1;
      int toleranceClip = index * cvar2;
      Vector2 vector2 = Vector2i.op_Implicit(this.Viewport.ViewportSize) * (float) index;
      float num1;
      float num2;
      Vector2Helpers.Deconstruct(Vector2i.op_Implicit(((Control) this).PixelSize) - vector2, ref num1, ref num2);
      float a1 = num1;
      float a2 = num2;
      if (Fits(a1) | cvar3 && Fits(a2) || !cvar3 && Fits(a1) && Larger(a2) || Larger(a1) && Fits(a2))
        return new int?(index);

      bool Larger(float a) => (double) a > (double) toleranceMargin;

      bool Fits(float a)
      {
        return (double) a <= (double) toleranceMargin && (double) a >= (double) -toleranceClip;
      }
    }
    return new int?();
  }

  protected virtual void Resized()
  {
    ((Control) this).Resized();
    this.UpdateCfg();
  }
}
