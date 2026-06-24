// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.NightVision.HalfNightVisionBrightnessOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

#nullable disable
namespace Content.Client._RMC14.NightVision;

public sealed class HalfNightVisionBrightnessOverlay : Overlay
{
  public virtual OverlaySpace Space => (OverlaySpace) 512 /*0x0200*/;

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!(args.DrawingHandle is DrawingHandleWorld drawingHandle))
      return;
    Box2 worldAabb = args.WorldAABB;
    Color color;
    // ISSUE: explicit constructor call
    ((Color) ref color).\u002Ector(0.45f, 0.45f, 0.45f, 1f);
    drawingHandle.DrawRect(worldAabb, color, true);
  }
}
