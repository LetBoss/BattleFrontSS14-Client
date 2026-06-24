// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Consoles.AtmosAlertsComputerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.Consoles;

public sealed class AtmosAlertsComputerBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private AtmosAlertsComputerWindow? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = new AtmosAlertsComputerWindow(this, new EntityUid?(this.Owner));
    this._menu.OpenCentered();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    AtmosAlertsComputerBoundInterfaceState boundInterfaceState = (AtmosAlertsComputerBoundInterfaceState) state;
    TransformComponent transformComponent;
    this.EntMan.TryGetComponent<TransformComponent>(this.Owner, ref transformComponent);
    this._menu?.UpdateUI(transformComponent?.Coordinates, boundInterfaceState.AirAlarms, boundInterfaceState.FireAlarms, boundInterfaceState.FocusData);
  }

  public void SendFocusChangeMessage(NetEntity? netEntity)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AtmosAlertsComputerFocusChangeMessage(netEntity));
  }

  public void SendDeviceSilencedMessage(NetEntity netEntity, bool silenceDevice)
  {
    this.SendMessage((BoundUserInterfaceMessage) new AtmosAlertsComputerDeviceSilencedMessage(netEntity, silenceDevice));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
  }
}
