// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Ui.VehicleWeaponsBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Ui;

public sealed class VehicleWeaponsBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private VehicleWeaponsMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = new VehicleWeaponsMenu();
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._menu.Title = string.Empty;
    this._menu.OnSelect += (Action<NetEntity?>) (mountedEntity => this.SendMessage((BoundUserInterfaceMessage) new VehicleWeaponsSelectMessage(mountedEntity)));
    this._menu.OnToggleStabilization += (Action<bool>) (enabled => this.SendMessage((BoundUserInterfaceMessage) new VehicleWeaponsStabilizationMessage(enabled)));
    this._menu.OnToggleAutoTurret += (Action<bool>) (enabled => this.SendMessage((BoundUserInterfaceMessage) new VehicleWeaponsAutoModeMessage(enabled)));
    this._menu.OpenCenteredAt(new Vector2(0.7f, 0.05f));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    if (this._menu != null)
      this._menu.OnClose -= new Action(((BoundUserInterface) this).Close);
    ((Control) this._menu)?.Dispose();
    this._menu = (VehicleWeaponsMenu) null;
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is VehicleWeaponsUiState vehicleWeaponsUiState))
      return;
    this._menu?.Update(vehicleWeaponsUiState.Vehicle, (IReadOnlyList<VehicleWeaponsUiEntry>) vehicleWeaponsUiState.Hardpoints, vehicleWeaponsUiState.CanToggleStabilization, vehicleWeaponsUiState.StabilizationEnabled, vehicleWeaponsUiState.CanToggleAuto, vehicleWeaponsUiState.AutoEnabled);
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    base.ReceiveMessage(message);
    if (!(message is VehicleWeaponsCooldownFeedbackMessage cooldownFeedbackMessage))
      return;
    this._menu?.FlashCooldownFeedback(cooldownFeedbackMessage.RemainingSeconds);
  }
}
