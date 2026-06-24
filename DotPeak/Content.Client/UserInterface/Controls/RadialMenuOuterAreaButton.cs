// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuOuterAreaButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using System.Numerics;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public sealed class RadialMenuOuterAreaButton : RadialMenuTextureButtonBase
{
  public float OuterRadius { get; set; }

  public Vector2? ParentCenter { get; set; }

  protected virtual bool HasPoint(Vector2 point)
  {
    return !this.ParentCenter.HasValue ? ((Control) this).HasPoint(point) : (double) (point + ((Control) this).Position - this.ParentCenter.Value).LengthSquared() > (double) (this.OuterRadius * this.OuterRadius);
  }
}
