// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Inventory.Controls.ItemSlotButtonContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public sealed class ItemSlotButtonContainer : ItemSlotUIContainer<SlotControl>
{
  private readonly InventoryUIController _inventoryController;
  private string _slotGroup = "";
  private static readonly string[] PubgMainHotbarOrder = new string[5]
  {
    "pocket1",
    "pocket2",
    "suitstorage",
    "headArmor",
    "outerArmor"
  };

  public string SlotGroup
  {
    get => this._slotGroup;
    set
    {
      this._inventoryController.RemoveSlotGroup(this.SlotGroup);
      this._slotGroup = value;
      this._inventoryController.RegisterSlotGroupContainer(this);
    }
  }

  public ItemSlotButtonContainer()
  {
    this._inventoryController = ((Control) this).UserInterfaceManager.GetUIController<InventoryUIController>();
  }

  public void ReorderPubgMainHotbar()
  {
    if (this._slotGroup != "MainHotbar" || this.Buttons.Count == 0)
      return;
    List<SlotControl> slotControlList1 = new List<SlotControl>(this.Buttons.Count);
    foreach (string key in ItemSlotButtonContainer.PubgMainHotbarOrder)
    {
      SlotControl slotControl;
      if (this.Buttons.TryGetValue(key, out slotControl))
        slotControlList1.Add(slotControl);
    }
    foreach (Control child in ((Control) this).Children)
    {
      if (child is SlotControl slotControl && !slotControlList1.Contains(slotControl))
        slotControlList1.Add(slotControl);
    }
    List<SlotControl> slotControlList2 = new List<SlotControl>(this.Buttons.Count);
    foreach (Control child in ((Control) this).Children)
    {
      if (child is SlotControl slotControl)
        slotControlList2.Add(slotControl);
    }
    foreach (Control control in slotControlList2)
      ((Control) this).RemoveChild(control);
    foreach (Control control in slotControlList1)
      ((Control) this).AddChild(control);
  }
}
