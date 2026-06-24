// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuTextureButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButton : RadialMenuTextureButtonBase
{
  public Control? TargetLayer { get; set; }

  public string? TargetLayerControlName { get; set; }

  public RadialMenuTextureButton()
  {
    ((BaseButton) this).EnableAllKeybinds = true;
    ((BaseButton) this).OnButtonUp += new Action<BaseButton.ButtonEventArgs>(this.OnClicked);
  }

  private void OnClicked(BaseButton.ButtonEventArgs args)
  {
    if (this.TargetLayer == null && this.TargetLayerControlName == null)
      return;
    RadialMenu multiLayerContainer = this.FindParentMultiLayerContainer((Control) this);
    if (multiLayerContainer == null)
      return;
    if (this.TargetLayer != null)
      multiLayerContainer.TryToMoveToNewLayer(this.TargetLayer);
    else
      multiLayerContainer.TryToMoveToNewLayer(this.TargetLayerControlName);
  }

  private RadialMenu? FindParentMultiLayerContainer(Control control)
  {
    foreach (Control andLogicalAncestor in LogicalExtensions.GetSelfAndLogicalAncestors(control))
    {
      if (andLogicalAncestor is RadialMenu multiLayerContainer)
        return multiLayerContainer;
    }
    return (RadialMenu) null;
  }
}
