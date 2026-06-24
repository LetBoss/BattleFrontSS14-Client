using System;
using System.Collections.Generic;
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

namespace Content.Client.UserInterface.Systems.Hotbar;

public sealed class HotbarUIController : UIController
{
	private InventoryUIController? _inventory;

	private HandsUIController? _hands;

	private StorageUIController? _storage;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
	}

	private void OnScreenLoad()
	{
		ReloadHotbar();
	}

	public void Setup(HandsContainer handsContainer)
	{
		_inventory = base.UIManager.GetUIController<InventoryUIController>();
		_hands = base.UIManager.GetUIController<HandsUIController>();
		_storage = base.UIManager.GetUIController<StorageUIController>();
		_hands.RegisterHandContainer(handsContainer);
	}

	public void ReloadHotbar()
	{
		if (base.UIManager.ActiveScreen == null)
		{
			return;
		}
		HotbarGui widget = base.UIManager.ActiveScreen.GetWidget<HotbarGui>();
		if (widget != null)
		{
			foreach (ItemSlotButtonContainer allItemSlotContainer in GetAllItemSlotContainers((Control)(object)widget))
			{
				allItemSlotContainer.SlotGroup = allItemSlotContainer.SlotGroup;
			}
		}
		_hands?.ReloadHands();
		_inventory?.ReloadSlots();
		InventoryGui widget2 = base.UIManager.ActiveScreen.GetWidget<InventoryGui>();
		if (widget2 == null)
		{
			return;
		}
		foreach (ItemSlotButtonContainer allItemSlotContainer2 in GetAllItemSlotContainers((Control)(object)widget2))
		{
			allItemSlotContainer2.SlotGroup = allItemSlotContainer2.SlotGroup;
		}
		_inventory?.RegisterInventoryBarContainer(widget2.InventoryHotbar);
	}

	private unsafe static IEnumerable<ItemSlotButtonContainer> GetAllItemSlotContainers(Control gui)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		List<ItemSlotButtonContainer> list = new List<ItemSlotButtonContainer>();
		Enumerator enumerator = gui.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				if (current is ItemSlotButtonContainer item)
				{
					list.Add(item);
				}
				list.AddRange(GetAllItemSlotContainers(current));
			}
			return list;
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
