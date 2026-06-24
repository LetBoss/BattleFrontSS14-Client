// Decompiled with JetBrains decompiler
// Type: Content.Client.Xenoarchaeology.Ui.NodeScannerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Xenoarchaeology.Ui;

public sealed class NodeScannerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private NodeScannerDisplay? _scannerDisplay;

  protected virtual void Open()
  {
    base.Open();
    this._scannerDisplay = BoundUserInterfaceExt.CreateWindow<NodeScannerDisplay>((BoundUserInterface) this);
    this._scannerDisplay.SetOwner(this.Owner);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._scannerDisplay)?.Orphan();
  }
}
