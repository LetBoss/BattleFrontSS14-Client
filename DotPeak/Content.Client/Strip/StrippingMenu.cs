// Decompiled with JetBrains decompiler
// Type: Content.Client.Strip.StrippingMenu
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Strip;

public sealed class StrippingMenu : DefaultWindow
{
  public LayoutContainer InventoryContainer = new LayoutContainer();
  public LayoutContainer HandsContainer = new LayoutContainer();
  public BoxContainer SnareContainer = new BoxContainer();
  public bool Dirty = true;

  public event Action? OnDirty;

  public StrippingMenu()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).Margin = new Thickness(0.0f, 8f);
    BoxContainer boxContainer2 = boxContainer1;
    this.Contents.AddChild((Control) boxContainer2);
    ((Control) boxContainer2).AddChild((Control) this.SnareContainer);
    ((Control) boxContainer2).AddChild((Control) this.HandsContainer);
    ((Control) boxContainer2).AddChild((Control) this.InventoryContainer);
  }

  public void ClearButtons()
  {
    ((Control) this.InventoryContainer).DisposeAllChildren();
    ((Control) this.HandsContainer).DisposeAllChildren();
    ((Control) this.SnareContainer).DisposeAllChildren();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    if (!this.Dirty)
      return;
    this.Dirty = false;
    Action onDirty = this.OnDirty;
    if (onDirty == null)
      return;
    onDirty();
  }
}
