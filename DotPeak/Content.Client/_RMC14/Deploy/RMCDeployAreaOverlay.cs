// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Deploy.RMCDeployAreaOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

#nullable disable
namespace Content.Client._RMC14.Deploy;

public sealed class RMCDeployAreaOverlay : Overlay
{
  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public Box2 Box { get; set; }

  public Color Color { get; set; }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    Color color1 = this.Color;
    Color color2 = ((Color) ref color1).WithAlpha(0.5f);
    ((OverlayDrawArgs) ref args).WorldHandle.DrawRect(this.Box, color2, true);
  }
}
