// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Ui.ShellSelectionBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle.Weapons;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class ShellSelectionBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private ShellSelectionWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = new ShellSelectionWindow();
    this._window.OnClose += new Action(((BoundUserInterface) this).Close);
    this._window.OnShellSelected += new Action<string>(this.OnShellSelected);
    this._window.OnCancel += new Action(this.OnCancel);
    this._window.OpenCentered();
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    if (this._window != null)
    {
      this._window.OnClose -= new Action(((BoundUserInterface) this).Close);
      this._window.OnShellSelected -= new Action<string>(this.OnShellSelected);
      this._window.OnCancel -= new Action(this.OnCancel);
    }
    ((Control) this._window)?.Dispose();
    this._window = (ShellSelectionWindow) null;
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ShellSelectionUiState state1))
      return;
    this._window?.UpdateState(state1);
  }

  private void OnShellSelected(string protoId)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ShellTypeSelectMessage(EntProtoId.op_Implicit(protoId)));
    this.Close();
  }

  private void OnCancel()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ShellSelectionCancelMessage());
    this.Close();
  }
}
