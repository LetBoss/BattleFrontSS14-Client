using System;
using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleHardpointVendorBui : BoundUserInterface
{
	private VehicleHardpointVendorWindow? _window;

	private string? _selectedVehicleId;

	private string? _selectedHardpointId;

	public VehicleHardpointVendorBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<VehicleHardpointVendorWindow>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.TryGetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner, ref val))
		{
			((DefaultWindow)_window).Title = val.EntityName;
		}
		_window.VehicleList.OnItemSelected += OnVehicleSelected;
		_window.HardpointList.OnItemSelected += OnHardpointSelected;
		((BaseButton)_window.PrintButton).OnPressed += delegate
		{
			PrintSelected();
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is VehicleHardpointVendorBuiState state2 && _window != null)
		{
			UpdateVehicleList(state2);
			UpdateHardpointList(state2);
		}
	}

	private void UpdateVehicleList(VehicleHardpointVendorBuiState state)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (_window == null)
		{
			return;
		}
		_window.VehicleList.Clear();
		foreach (VehicleSupplyEntryState vehicle in state.Vehicles)
		{
			Item val = new Item(_window.VehicleList)
			{
				Text = vehicle.Name,
				Metadata = vehicle.Id
			};
			_window.VehicleList.Add(val);
		}
		_selectedVehicleId = state.SelectedVehicle;
	}

	private void UpdateHardpointList(VehicleHardpointVendorBuiState state)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (_window == null)
		{
			return;
		}
		_window.HardpointList.Clear();
		foreach (VehicleSupplyEntryState hardpoint in state.Hardpoints)
		{
			Item val = new Item(_window.HardpointList)
			{
				Text = hardpoint.Name,
				Metadata = hardpoint.Id
			};
			_window.HardpointList.Add(val);
		}
		if (_selectedHardpointId != null && !HasHardpoint(_selectedHardpointId))
		{
			_selectedHardpointId = null;
		}
		((BaseButton)_window.PrintButton).Disabled = _selectedHardpointId == null;
	}

	private bool HasHardpoint(string hardpointId)
	{
		if (_window == null)
		{
			return false;
		}
		foreach (Item hardpoint in _window.HardpointList)
		{
			if (hardpoint.Metadata is string text && text == hardpointId)
			{
				return true;
			}
		}
		return false;
	}

	private void OnVehicleSelected(ItemListSelectedEventArgs args)
	{
		if (((ItemListEventArgs)args).ItemList[args.ItemIndex].Metadata is string text)
		{
			_selectedVehicleId = text;
			_selectedHardpointId = null;
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleHardpointVendorSelectMsg(text));
		}
	}

	private void OnHardpointSelected(ItemListSelectedEventArgs args)
	{
		if (((ItemListEventArgs)args).ItemList[args.ItemIndex].Metadata is string selectedHardpointId)
		{
			_selectedHardpointId = selectedHardpointId;
			if (_window != null)
			{
				((BaseButton)_window.PrintButton).Disabled = false;
			}
		}
	}

	private void PrintSelected()
	{
		if (_selectedHardpointId != null)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleHardpointVendorPrintMsg(_selectedHardpointId));
		}
	}
}
