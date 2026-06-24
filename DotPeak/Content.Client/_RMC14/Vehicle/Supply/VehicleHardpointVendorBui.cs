// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.Supply.VehicleHardpointVendorBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleHardpointVendorBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private VehicleHardpointVendorWindow? _window;
  private string? _selectedVehicleId;
  private string? _selectedHardpointId;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<VehicleHardpointVendorWindow>((BoundUserInterface) this);
    MetaDataComponent metaDataComponent;
    if (this.EntMan.TryGetComponent<MetaDataComponent>(this.Owner, ref metaDataComponent))
      this._window.Title = metaDataComponent.EntityName;
    this._window.VehicleList.OnItemSelected += new Action<ItemList.ItemListSelectedEventArgs>(this.OnVehicleSelected);
    this._window.HardpointList.OnItemSelected += new Action<ItemList.ItemListSelectedEventArgs>(this.OnHardpointSelected);
    ((BaseButton) this._window.PrintButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.PrintSelected());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is VehicleHardpointVendorBuiState state1) || this._window == null)
      return;
    this.UpdateVehicleList(state1);
    this.UpdateHardpointList(state1);
  }

  private void UpdateVehicleList(VehicleHardpointVendorBuiState state)
  {
    if (this._window == null)
      return;
    this._window.VehicleList.Clear();
    foreach (VehicleSupplyEntryState vehicle in state.Vehicles)
      this._window.VehicleList.Add(new ItemList.Item(this._window.VehicleList)
      {
        Text = vehicle.Name,
        Metadata = (object) vehicle.Id
      });
    this._selectedVehicleId = state.SelectedVehicle;
  }

  private void UpdateHardpointList(VehicleHardpointVendorBuiState state)
  {
    if (this._window == null)
      return;
    this._window.HardpointList.Clear();
    foreach (VehicleSupplyEntryState hardpoint in state.Hardpoints)
      this._window.HardpointList.Add(new ItemList.Item(this._window.HardpointList)
      {
        Text = hardpoint.Name,
        Metadata = (object) hardpoint.Id
      });
    if (this._selectedHardpointId != null && !this.HasHardpoint(this._selectedHardpointId))
      this._selectedHardpointId = (string) null;
    ((BaseButton) this._window.PrintButton).Disabled = this._selectedHardpointId == null;
  }

  private bool HasHardpoint(string hardpointId)
  {
    if (this._window == null)
      return false;
    foreach (ItemList.Item hardpoint in this._window.HardpointList)
    {
      if (hardpoint.Metadata is string metadata && metadata == hardpointId)
        return true;
    }
    return false;
  }

  private void OnVehicleSelected(ItemList.ItemListSelectedEventArgs args)
  {
    if (!(((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Metadata is string metadata))
      return;
    this._selectedVehicleId = metadata;
    this._selectedHardpointId = (string) null;
    this.SendMessage((BoundUserInterfaceMessage) new VehicleHardpointVendorSelectMsg(metadata));
  }

  private void OnHardpointSelected(ItemList.ItemListSelectedEventArgs args)
  {
    if (!(((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Metadata is string metadata))
      return;
    this._selectedHardpointId = metadata;
    if (this._window == null)
      return;
    ((BaseButton) this._window.PrintButton).Disabled = false;
  }

  private void PrintSelected()
  {
    if (this._selectedHardpointId == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new VehicleHardpointVendorPrintMsg(this._selectedHardpointId));
  }
}
