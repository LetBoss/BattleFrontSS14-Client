// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.HSpacer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public sealed class HSpacer : Control
{
  public float Spacing
  {
    get => this.MinHeight;
    set => this.MinHeight = value;
  }

  public HSpacer() => this.MinHeight = this.Spacing;

  public HSpacer(float height = 5f)
  {
    this.Spacing = height;
    this.MinHeight = height;
  }
}
