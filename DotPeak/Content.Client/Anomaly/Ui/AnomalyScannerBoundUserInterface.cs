// Decompiled with JetBrains decompiler
// Type: Content.Client.Anomaly.Ui.AnomalyScannerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Anomaly;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Anomaly.Ui;

public sealed class AnomalyScannerBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private AnomalyScannerMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = new AnomalyScannerMenu();
    this._menu.OpenCentered();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is AnomalyScannerUserInterfaceState userInterfaceState) || this._menu == null)
      return;
    this._menu.LastMessage = userInterfaceState.Message;
    this._menu.NextPulseTime = userInterfaceState.NextPulseTime;
    this._menu.UpdateMenu();
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
  }
}
