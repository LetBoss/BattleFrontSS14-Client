// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Construction.Tunnel.NameTunnelBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Construction.Tunnel;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Construction.Tunnel;

public sealed class NameTunnelBui(EntityUid owner, Enum key) : BoundUserInterface(owner, key)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private NameTunnelWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    NameTunnelWindow window = this._window;
    if (window != null && ((BaseWindow) window).IsOpen)
      return;
    this._window = BoundUserInterfaceExt.CreateWindow<NameTunnelWindow>((BoundUserInterface) this);
    LineEdit tunnelInput = this._window.TunnelName;
    ((BaseButton) this._window.SubmitButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      string tunnelName = tunnelInput.Text.Trim();
      if (tunnelName.Length == 0)
        return;
      this.SendMessage((BoundUserInterfaceMessage) new NameTunnelMessage(tunnelName));
    });
  }
}
