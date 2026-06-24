using System;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasFilterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private const float MaxTransferRate = 200f;

	[ViewVariables]
	private GasFilterWindow? _window;

	public GasFilterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		AtmosphereSystem atmosphereSystem = base.EntMan.System<AtmosphereSystem>();
		_window = BoundUserInterfaceExt.CreateWindow<GasFilterWindow>((BoundUserInterface)(object)this);
		_window.PopulateGasList(atmosphereSystem.Gases);
		_window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
		_window.FilterTransferRateChanged += OnFilterTransferRatePressed;
		_window.SelectGasPressed += OnSelectGasPressed;
	}

	private void OnToggleStatusButtonPressed()
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasFilterToggleStatusMessage(_window.FilterStatus));
		}
	}

	private void OnFilterTransferRatePressed(string value)
	{
		float result;
		float rate = (UserInputParser.TryFloat(value, out result) ? result : 0f);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasFilterChangeRateMessage(rate));
	}

	private void OnSelectGasPressed()
	{
		if (_window != null)
		{
			int result;
			if (_window.SelectedGas == null)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasFilterSelectGasMessage(null));
			}
			else if (int.TryParse(_window.SelectedGas, out result))
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasFilterSelectGasMessage(result));
			}
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is GasFilterBoundUserInterfaceState gasFilterBoundUserInterfaceState)
		{
			((DefaultWindow)_window).Title = gasFilterBoundUserInterfaceState.FilterLabel;
			_window.SetFilterStatus(gasFilterBoundUserInterfaceState.Enabled);
			_window.SetTransferRate(gasFilterBoundUserInterfaceState.TransferRate);
			if (gasFilterBoundUserInterfaceState.FilteredGas.HasValue)
			{
				GasPrototype gas = base.EntMan.System<AtmosphereSystem>().GetGas(gasFilterBoundUserInterfaceState.FilteredGas.Value);
				string name = Loc.GetString(gas.Name);
				_window.SetGasFiltered(gas.ID, name);
			}
			else
			{
				_window.SetGasFiltered(null, Loc.GetString("comp-gas-filter-ui-filter-gas-none"));
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			GasFilterWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
