// Decompiled with JetBrains decompiler
// Type: Content.Client.VendingMachines.VendingMachineBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Client.VendingMachines.UI;
using Content.Shared.VendingMachines;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.VendingMachines;

public sealed class VendingMachineBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private VendingMachineMenu? _menu;
  [Robust.Shared.ViewVariables.ViewVariables]
  private List<VendingMachineInventoryEntry> _cachedInventory = new List<VendingMachineInventoryEntry>();

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<VendingMachineMenu>((BoundUserInterface) this);
    this._menu.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    this._menu.OnItemSelected += new Action<GUIBoundKeyEventArgs, ListData>(this.OnItemSelected);
    this.Refresh();
  }

  public void Refresh()
  {
    VendingMachineComponent machineComponent;
    bool enabled = this.EntMan.TryGetComponent<VendingMachineComponent>(this.Owner, ref machineComponent) && !machineComponent.Ejecting;
    this._cachedInventory = this.EntMan.System<VendingMachineSystem>().GetAllInventory(this.Owner);
    this._menu?.Populate(this._cachedInventory, enabled);
  }

  public void UpdateAmounts()
  {
    VendingMachineComponent machineComponent;
    bool enabled = this.EntMan.TryGetComponent<VendingMachineComponent>(this.Owner, ref machineComponent) && !machineComponent.Ejecting;
    this._cachedInventory = this.EntMan.System<VendingMachineSystem>().GetAllInventory(this.Owner);
    this._menu?.UpdateAmounts(this._cachedInventory, enabled);
  }

  private void OnItemSelected(GUIBoundKeyEventArgs args, ListData data)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    VendorItemsListData vendorItemsListData = data as VendorItemsListData;
    if ((object) vendorItemsListData == null)
      return;
    int itemIndex = vendorItemsListData.ItemIndex;
    if (this._cachedInventory.Count == 0)
      return;
    VendingMachineInventoryEntry machineInventoryEntry = this._cachedInventory.ElementAtOrDefault<VendingMachineInventoryEntry>(itemIndex);
    if (machineInventoryEntry == null)
      return;
    this.SendPredictedMessage((BoundUserInterfaceMessage) new VendingMachineEjectMessage(machineInventoryEntry.Type, machineInventoryEntry.ID));
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing || this._menu == null)
      return;
    this._menu.OnItemSelected -= new Action<GUIBoundKeyEventArgs, ListData>(this.OnItemSelected);
    this._menu.OnClose -= new Action(((BoundUserInterface) this).Close);
    ((Control) this._menu).Orphan();
  }
}
