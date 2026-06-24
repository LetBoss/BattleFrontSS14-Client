// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.HighDivider
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public sealed class HighDivider : Control
{
  public HighDivider()
  {
    Control.OrderedChildCollection children = this.Children;
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).StyleClasses.Add(nameof (HighDivider));
    children.Add((Control) panelContainer);
  }
}
