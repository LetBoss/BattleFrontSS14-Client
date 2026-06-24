using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.UserInterface.Controls;
using Content.Client.VendingMachines.UI;
using Content.Shared.VendingMachines;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.ViewVariables;

namespace Content.Client.VendingMachines;

public sealed class VendingMachineBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
	[ViewVariables]
	private VendingMachineMenu? _menu;

	[ViewVariables]
	private List<VendingMachineInventoryEntry> _cachedInventory = new List<VendingMachineInventoryEntry>();

	protected override void Open()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<VendingMachineMenu>((BoundUserInterface)(object)this);
		_menu.Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		_menu.OnItemSelected += OnItemSelected;
		Refresh();
	}

	public void Refresh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		VendingMachineComponent vendingMachineComponent = default(VendingMachineComponent);
		bool enabled = base.EntMan.TryGetComponent<VendingMachineComponent>(((BoundUserInterface)this).Owner, ref vendingMachineComponent) && !vendingMachineComponent.Ejecting;
		VendingMachineSystem vendingMachineSystem = base.EntMan.System<VendingMachineSystem>();
		_cachedInventory = vendingMachineSystem.GetAllInventory(((BoundUserInterface)this).Owner);
		_menu?.Populate(_cachedInventory, enabled);
	}

	public void UpdateAmounts()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		VendingMachineComponent vendingMachineComponent = default(VendingMachineComponent);
		bool enabled = base.EntMan.TryGetComponent<VendingMachineComponent>(((BoundUserInterface)this).Owner, ref vendingMachineComponent) && !vendingMachineComponent.Ejecting;
		VendingMachineSystem vendingMachineSystem = base.EntMan.System<VendingMachineSystem>();
		_cachedInventory = vendingMachineSystem.GetAllInventory(((BoundUserInterface)this).Owner);
		_menu?.UpdateAmounts(_cachedInventory, enabled);
	}

	private void OnItemSelected(GUIBoundKeyEventArgs args, ListData data)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) && data is VendorItemsListData { ItemIndex: var itemIndex } && _cachedInventory.Count != 0)
		{
			VendingMachineInventoryEntry vendingMachineInventoryEntry = _cachedInventory.ElementAtOrDefault(itemIndex);
			if (vendingMachineInventoryEntry != null)
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new VendingMachineEjectMessage(vendingMachineInventoryEntry.Type, vendingMachineInventoryEntry.ID));
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing && _menu != null)
		{
			_menu.OnItemSelected -= OnItemSelected;
			((BaseWindow)_menu).OnClose -= base.Close;
			((Control)_menu).Orphan();
		}
	}
}
