using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._RMC14.Vehicle.Ui;
using Content.Shared._RMC14.Vehicle.Supply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Vehicle.Supply;

public sealed class VehicleSupplyBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
	private VehicleSupplyWindow? _window;

	private string? _selectedVehicleId;

	private bool _suppressEvents;

	private readonly List<string> _availableVehicleIds = new List<string>();

	private readonly Dictionary<string, int> _availableCounts = new Dictionary<string, int>();

	private readonly Dictionary<string, int> _selectedCopyIndices = new Dictionary<string, int>();

	private readonly Dictionary<string, HardpointButton> _selectButtons = new Dictionary<string, HardpointButton>();

	private readonly Dictionary<string, HardpointButton> _copyToggleButtons = new Dictionary<string, HardpointButton>();

	private readonly Dictionary<string, BoxContainer> _copyContainers = new Dictionary<string, BoxContainer>();

	private readonly Dictionary<string, List<HardpointButton>> _copyButtons = new Dictionary<string, List<HardpointButton>>();

	private readonly HashSet<string> _copyExpanded = new HashSet<string>();

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<VehicleSupplyWindow>((BoundUserInterface)(object)this);
		if (_window != null)
		{
			_window.Title = string.Empty;
			((BaseButton)_window.RaiseButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleSupplyLiftMsg(raise: true));
			};
			((BaseButton)_window.LowerButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleSupplyLiftMsg(raise: false));
			};
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is VehicleSupplyBuiState vehicleSupplyBuiState && _window != null)
		{
			_suppressEvents = true;
			UpdateStatus(vehicleSupplyBuiState);
			UpdateLists(vehicleSupplyBuiState);
			_window.SetPreview(vehicleSupplyBuiState.Preview);
			_suppressEvents = false;
		}
	}

	private void UpdateStatus(VehicleSupplyBuiState state)
	{
		if (_window != null)
		{
			string value = state.LiftMode?.ToString() ?? "No lift";
			string value2 = (string.IsNullOrWhiteSpace(state.ActiveVehicleId) ? "none" : state.ActiveVehicleId);
			string value3 = (state.Busy ? "busy" : "idle");
			_window.StatusLabel.Text = $"Lift: {value} | Status: {value3} | Active: {value2}";
			bool pulse = state.LiftMode == VehicleSupplyLiftMode.Raising;
			bool pulse2 = state.LiftMode == VehicleSupplyLiftMode.Lowering;
			_window.RaiseButton.Pulse = pulse;
			_window.LowerButton.Pulse = pulse2;
			_window.SetLiftActivity(state.LiftMode, state.Busy);
		}
	}

	private void UpdateLists(VehicleSupplyBuiState state)
	{
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Expected O, but got Unknown
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Expected O, but got Unknown
		if (_window == null)
		{
			return;
		}
		_availableVehicleIds.Clear();
		_availableCounts.Clear();
		((Control)_window.AvailableRows).DisposeAllChildren();
		_selectButtons.Clear();
		_copyToggleButtons.Clear();
		_copyContainers.Clear();
		_copyButtons.Clear();
		if (state.Available.Count == 0)
		{
			_selectedVehicleId = null;
			return;
		}
		_selectedVehicleId = state.SelectedVehicleId;
		bool flag = false;
		if (!string.IsNullOrWhiteSpace(_selectedVehicleId))
		{
			foreach (VehicleSupplyEntryState item in state.Available)
			{
				if (item.Id == _selectedVehicleId)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag && state.Available.Count > 0)
		{
			_selectedVehicleId = state.Available[0].Id;
		}
		foreach (VehicleSupplyEntryState item2 in state.Available)
		{
			string labelText = ((item2.Count > 1) ? $"{item2.Name} x{item2.Count}" : item2.Name);
			_availableVehicleIds.Add(item2.Id);
			_availableCounts[item2.Id] = item2.Count;
			BoxContainer val = new BoxContainer
			{
				Orientation = (LayoutOrientation)0,
				SeparationOverride = 6,
				HorizontalExpand = true
			};
			HardpointButton obj = new HardpointButton
			{
				LabelText = labelText
			};
			((Control)obj).HorizontalExpand = true;
			HardpointButton hardpointButton = obj;
			string vehicleId = item2.Id;
			((BaseButton)hardpointButton).OnPressed += delegate
			{
				if (!_suppressEvents)
				{
					SelectVehicle(vehicleId, _selectedCopyIndices.TryGetValue(vehicleId, out var value4) ? value4 : 0);
				}
			};
			ApplySelectionStyle(hardpointButton, _selectedVehicleId == vehicleId);
			((Control)val).AddChild((Control)(object)hardpointButton);
			_selectButtons[vehicleId] = hardpointButton;
			if (item2.Count > 1)
			{
				HardpointButton obj2 = new HardpointButton
				{
					LabelText = (_copyExpanded.Contains(vehicleId) ? "Copies v" : "Copies >")
				};
				((Control)obj2).MinSize = new Vector2(110f, 0f);
				HardpointButton hardpointButton2 = obj2;
				BoxContainer val2 = new BoxContainer
				{
					Orientation = (LayoutOrientation)1,
					Margin = new Thickness(12f, 0f, 0f, 0f),
					HorizontalExpand = true,
					Visible = _copyExpanded.Contains(vehicleId)
				};
				for (int num = 0; num < item2.Count; num++)
				{
					int copyIndex = num;
					HardpointButton obj3 = new HardpointButton
					{
						LabelText = $"    #{num + 1}"
					};
					((Control)obj3).HorizontalExpand = true;
					HardpointButton hardpointButton3 = obj3;
					((BaseButton)hardpointButton3).OnPressed += delegate
					{
						if (!_suppressEvents)
						{
							_selectedCopyIndices[vehicleId] = copyIndex;
							UpdateCopySelection(vehicleId);
							SelectVehicle(vehicleId, copyIndex);
						}
					};
					((Control)val2).AddChild((Control)(object)hardpointButton3);
					if (!_copyButtons.TryGetValue(vehicleId, out List<HardpointButton> value))
					{
						value = new List<HardpointButton>();
						_copyButtons[vehicleId] = value;
					}
					value.Add(hardpointButton3);
				}
				((Control)val).AddChild((Control)(object)hardpointButton2);
				_copyToggleButtons[vehicleId] = hardpointButton2;
				_copyContainers[vehicleId] = val2;
				((BaseButton)hardpointButton2).OnPressed += delegate
				{
					if (!_suppressEvents)
					{
						if (_copyExpanded.Contains(vehicleId))
						{
							_copyExpanded.Remove(vehicleId);
						}
						else
						{
							_copyExpanded.Add(vehicleId);
						}
						UpdateCopyExpanded(vehicleId);
					}
				};
			}
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 2,
				HorizontalExpand = true
			};
			((Control)val3).AddChild((Control)(object)val);
			if (_copyContainers.TryGetValue(vehicleId, out BoxContainer value2))
			{
				((Control)val3).AddChild((Control)(object)value2);
			}
			((Control)_window.AvailableRows).AddChild((Control)(object)val3);
		}
		foreach (var (text2, num3) in _availableCounts)
		{
			if (num3 > 1)
			{
				int value3;
				if (text2 == _selectedVehicleId)
				{
					_selectedCopyIndices[text2] = state.SelectedCopyIndex;
				}
				else if (!_selectedCopyIndices.TryGetValue(text2, out value3) || value3 < 0 || value3 >= num3)
				{
					_selectedCopyIndices[text2] = 0;
				}
				UpdateCopySelection(text2);
				UpdateCopyExpanded(text2);
			}
		}
	}

	private void SelectVehicle(string vehicleId, int copyIndex)
	{
		if (_selectedVehicleId == vehicleId)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleSupplySelectMsg(vehicleId, copyIndex));
			return;
		}
		_selectedVehicleId = vehicleId;
		UpdateSelectionVisuals();
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new VehicleSupplySelectMsg(vehicleId, copyIndex));
	}

	private void UpdateSelectionVisuals()
	{
		foreach (var (text2, button) in _selectButtons)
		{
			ApplySelectionStyle(button, text2 == _selectedVehicleId);
		}
	}

	private void UpdateCopySelection(string vehicleId)
	{
		if (_copyButtons.TryGetValue(vehicleId, out List<HardpointButton> value))
		{
			if (!_selectedCopyIndices.TryGetValue(vehicleId, out var value2))
			{
				value2 = 0;
			}
			for (int i = 0; i < value.Count; i++)
			{
				ApplySelectionStyle(value[i], i == value2);
			}
		}
	}

	private void UpdateCopyExpanded(string vehicleId)
	{
		if (_copyContainers.TryGetValue(vehicleId, out BoxContainer value) && _copyToggleButtons.TryGetValue(vehicleId, out HardpointButton value2))
		{
			bool flag = (((Control)value).Visible = _copyExpanded.Contains(vehicleId));
			value2.LabelText = (flag ? "Copies v" : "Copies >");
		}
	}

	private static void ApplySelectionStyle(HardpointButton button, bool selected)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		button.Selected = selected;
		button.SelectedColor = HardpointButton.DefaultUnhoveredColor;
		button.UnhoveredColor = Color.FromHex((ReadOnlySpan<char>)"#1A3D5C", (Color?)null);
		button.HoveredColor = HardpointButton.DefaultHoveredColor;
		button.DisabledColor = HardpointButton.DefaultDisabledColor;
		button.TextColor = (selected ? HardpointButton.DefaultTextColor : HardpointButton.DefaultUnselectedTextColor);
		button.DisabledTextColor = HardpointButton.DefaultDisabledTextColor;
		button.RefreshStyle();
	}
}
