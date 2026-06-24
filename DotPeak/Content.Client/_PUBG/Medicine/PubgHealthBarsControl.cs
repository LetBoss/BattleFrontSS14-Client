// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Medicine.PubgHealthBarsControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Medicine;

public sealed class PubgHealthBarsControl : Control
{
  [Dependency]
  private readonly IOverlayManager _overlays;
  private const float BarHeight = 30f;
  private const float ResourceBarHeight = 18f;
  private const float Padding = 10f;

  public PubgHealthBarsControl() => IoCManager.InjectDependencies<PubgHealthBarsControl>(this);

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    PubgHealthOverlay pubgHealthOverlay;
    if (!this._overlays.TryGetOverlay<PubgHealthOverlay>(ref pubgHealthOverlay) || pubgHealthOverlay == null)
      return;
    float barWidth = MathF.Max(120f, (float) this.PixelSize.X - 20f);
    float barY = (float) ((double) this.PixelSize.Y - 30.0 - 10.0);
    pubgHealthOverlay.DrawBars(handle, 10f, barY, barWidth, 30f, 18f, true);
  }
}
