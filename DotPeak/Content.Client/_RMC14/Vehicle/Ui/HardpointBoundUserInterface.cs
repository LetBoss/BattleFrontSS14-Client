// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Ui.HardpointBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class HardpointBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private HardpointMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = new HardpointMenu();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._menu.VehicleEntity = new EntityUid?(this.Owner);
    MetaDataComponent metaDataComponent;
    if (this.EntMan.GetEntityQuery<MetaDataComponent>().TryGetComponent(this.Owner, ref metaDataComponent))
      this._menu.Title = metaDataComponent.EntityName;
    this._menu.OnRemove += (Action<string>) (slotId => this.SendMessage((BoundUserInterfaceMessage) new HardpointRemoveMessage(slotId)));
    this._menu.OpenCentered();
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    if (this._menu != null)
      this._menu.OnClose -= new Action(((BoundUserInterface) this).Close);
    ((Control) this._menu)?.Dispose();
    this._menu = (HardpointMenu) null;
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is HardpointBoundUserInterfaceState userInterfaceState))
      return;
    this._menu?.Update((IReadOnlyList<HardpointUiEntry>) userInterfaceState.Hardpoints, userInterfaceState.FrameIntegrity, userInterfaceState.FrameMaxIntegrity, userInterfaceState.HasFrameIntegrity, userInterfaceState.Error);
  }
}
