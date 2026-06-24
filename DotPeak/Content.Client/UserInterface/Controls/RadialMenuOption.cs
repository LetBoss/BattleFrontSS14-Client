// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuOption
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public abstract class RadialMenuOption
{
  public string? ToolTip { get; init; }

  public SpriteSpecifier? Sprite { get; init; }

  public Color? BackgroundColor { get; set; }

  public Color? HoverBackgroundColor { get; set; }
}
