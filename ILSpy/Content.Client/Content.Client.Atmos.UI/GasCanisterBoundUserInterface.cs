using System;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.IdentityManagement;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasCanisterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GasCanisterWindow? _window;

	public GasCanisterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasCanisterWindow>((BoundUserInterface)(object)this);
		_window.ReleaseValveCloseButtonPressed += OnReleaseValveClosePressed;
		_window.ReleaseValveOpenButtonPressed += OnReleaseValveOpenPressed;
		_window.ReleasePressureSet += OnReleasePressureSet;
		_window.TankEjectButtonPressed += OnTankEjectPressed;
	}

	private void OnTankEjectPressed()
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasCanisterHoldingTankEjectMessage());
	}

	private void OnReleasePressureSet(float value)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasCanisterChangeReleasePressureMessage(value));
	}

	private void OnReleaseValveOpenPressed()
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasCanisterChangeReleaseValveMessage(valve: true));
	}

	private void OnReleaseValveClosePressed()
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasCanisterChangeReleaseValveMessage(valve: false));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		GasCanisterComponent gasCanisterComponent = default(GasCanisterComponent);
		if (_window != null && state is GasCanisterBoundUserInterfaceState gasCanisterBoundUserInterfaceState && base.EntMan.TryGetComponent<GasCanisterComponent>(((BoundUserInterface)this).Owner, ref gasCanisterComponent))
		{
			IdentityEntity identityEntity = Identity.Name(((BoundUserInterface)this).Owner, base.EntMan);
			string tankLabel = (gasCanisterComponent.GasTankSlot.Item.HasValue ? Identity.Name(gasCanisterComponent.GasTankSlot.Item.Value, base.EntMan).Name : null);
			_window.SetCanisterLabel(identityEntity);
			_window.SetCanisterPressure(gasCanisterBoundUserInterfaceState.CanisterPressure);
			_window.SetPortStatus(gasCanisterBoundUserInterfaceState.PortStatus);
			_window.SetTankLabel(tankLabel);
			_window.SetTankPressure(gasCanisterBoundUserInterfaceState.TankPressure);
			_window.SetReleasePressureRange(gasCanisterComponent.MinReleasePressure, gasCanisterComponent.MaxReleasePressure);
			_window.SetReleasePressure(gasCanisterComponent.ReleasePressure);
			_window.SetReleaseValve(gasCanisterComponent.ReleaseValve);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			GasCanisterWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
