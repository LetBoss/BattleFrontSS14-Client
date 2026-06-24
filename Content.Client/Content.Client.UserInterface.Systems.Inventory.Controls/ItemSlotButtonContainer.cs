using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public sealed class ItemSlotButtonContainer : ItemSlotUIContainer<SlotControl>
{
	private readonly InventoryUIController _inventoryController;

	private string _slotGroup = "";

	private static readonly string[] PubgMainHotbarOrder = new string[5] { "pocket1", "pocket2", "suitstorage", "headArmor", "outerArmor" };

	public string SlotGroup
	{
		get
		{
			return _slotGroup;
		}
		set
		{
			_inventoryController.RemoveSlotGroup(SlotGroup);
			_slotGroup = value;
			_inventoryController.RegisterSlotGroupContainer(this);
		}
	}

	public ItemSlotButtonContainer()
	{
		_inventoryController = ((Control)this).UserInterfaceManager.GetUIController<InventoryUIController>();
	}

	public unsafe void ReorderPubgMainHotbar()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (_slotGroup != "MainHotbar" || Buttons.Count == 0)
		{
			return;
		}
		List<SlotControl> list = new List<SlotControl>(Buttons.Count);
		string[] pubgMainHotbarOrder = PubgMainHotbarOrder;
		foreach (string key in pubgMainHotbarOrder)
		{
			if (Buttons.TryGetValue(key, out SlotControl value))
			{
				list.Add(value);
			}
		}
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is SlotControl item && !list.Contains(item))
				{
					list.Add(item);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		List<SlotControl> list2 = new List<SlotControl>(Buttons.Count);
		enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is SlotControl item2)
				{
					list2.Add(item2);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		foreach (SlotControl item3 in list2)
		{
			((Control)this).RemoveChild((Control)(object)item3);
		}
		foreach (SlotControl item4 in list)
		{
			((Control)this).AddChild((Control)(object)item4);
		}
	}
}
