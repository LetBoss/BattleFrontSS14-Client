// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Hotbar.HotbarUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Hands;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.Inventory;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Client.UserInterface.Systems.Inventory.Widgets;
using Content.Client.UserInterface.Systems.Storage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Hotbar;

public sealed class HotbarUIController : UIController
{
  private InventoryUIController? _inventory;
  private HandsUIController? _hands;
  private StorageUIController? _storage;

  public virtual void Initialize()
  {
    base.Initialize();
    this.UIManager.GetUIController<GameplayStateLoadController>().OnScreenLoad += new Action(this.OnScreenLoad);
  }

  private void OnScreenLoad() => this.ReloadHotbar();

  public void Setup(HandsContainer handsContainer)
  {
    this._inventory = this.UIManager.GetUIController<InventoryUIController>();
    this._hands = this.UIManager.GetUIController<HandsUIController>();
    this._storage = this.UIManager.GetUIController<StorageUIController>();
    this._hands.RegisterHandContainer(handsContainer);
  }

  public void ReloadHotbar()
  {
    if (this.UIManager.ActiveScreen == null)
      return;
    HotbarGui widget1 = this.UIManager.ActiveScreen.GetWidget<HotbarGui>();
    if (widget1 != null)
    {
      foreach (ItemSlotButtonContainer itemSlotContainer in HotbarUIController.GetAllItemSlotContainers((Control) widget1))
        itemSlotContainer.SlotGroup = itemSlotContainer.SlotGroup;
    }
    this._hands?.ReloadHands();
    this._inventory?.ReloadSlots();
    InventoryGui widget2 = this.UIManager.ActiveScreen.GetWidget<InventoryGui>();
    if (widget2 == null)
      return;
    foreach (ItemSlotButtonContainer itemSlotContainer in HotbarUIController.GetAllItemSlotContainers((Control) widget2))
      itemSlotContainer.SlotGroup = itemSlotContainer.SlotGroup;
    this._inventory?.RegisterInventoryBarContainer(widget2.InventoryHotbar);
  }

  private static IEnumerable<ItemSlotButtonContainer> GetAllItemSlotContainers(Control gui)
  {
    List<ItemSlotButtonContainer> itemSlotContainers = new List<ItemSlotButtonContainer>();
    foreach (Control child in gui.Children)
    {
      if (child is ItemSlotButtonContainer slotButtonContainer)
        itemSlotContainers.Add(slotButtonContainer);
      itemSlotContainers.AddRange(HotbarUIController.GetAllItemSlotContainers(child));
    }
    return (IEnumerable<ItemSlotButtonContainer>) itemSlotContainers;
  }
}
