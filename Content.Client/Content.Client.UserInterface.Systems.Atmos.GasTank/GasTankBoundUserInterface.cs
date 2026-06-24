using System;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Systems.Atmos.GasTank;

public sealed class GasTankBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GasTankWindow? _window;

	public GasTankBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	public void SetOutputPressure(float value)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasTankSetPressureMessage
		{
			Pressure = value
		});
	}

	public void ToggleInternals()
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasTankToggleInternalsMessage());
	}

	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasTankWindow>((BoundUserInterface)(object)this);
		_window.Entity = ((BoundUserInterface)this).Owner;
		_window.SetTitle(base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName);
		_window.OnOutputPressure += SetOutputPressure;
		_window.OnToggleInternals += ToggleInternals;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		GasTankComponent gasTankComponent = default(GasTankComponent);
		if (base.EntMan.TryGetComponent<GasTankComponent>(((BoundUserInterface)this).Owner, ref gasTankComponent))
		{
			bool canConnectInternals = base.EntMan.System<SharedGasTankSystem>().CanConnectToInternals(Entity<GasTankComponent>.op_Implicit((((BoundUserInterface)this).Owner, gasTankComponent)));
			_window?.Update(canConnectInternals, gasTankComponent.IsConnected, gasTankComponent.OutputPressure);
		}
		if (state is GasTankBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		GasTankWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}
}
