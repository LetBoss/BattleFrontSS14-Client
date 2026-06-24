// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasAnalyzerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasAnalyzerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<GasAnalyzerWindow>((BoundUserInterface) this);
    ((BaseWindow) this._window).OnClose += new Action(((BoundUserInterface) this).Close);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    if (this._window == null || !(message is GasAnalyzerComponent.GasAnalyzerUserMessage msg))
      return;
    this._window.Populate(msg);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
