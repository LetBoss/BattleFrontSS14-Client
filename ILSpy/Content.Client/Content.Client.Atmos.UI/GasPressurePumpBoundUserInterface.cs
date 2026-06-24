using System;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasPressurePumpBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GasPressurePumpWindow? _window;

	public GasPressurePumpBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasPressurePumpWindow>((BoundUserInterface)(object)this);
		_window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
		_window.PumpOutputPressureChanged += OnPumpOutputPressurePressed;
		((BoundUserInterface)this).Update();
	}

	public override void Update()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			_window.Title = Identity.Name(((BoundUserInterface)this).Owner, base.EntMan);
			GasPressurePumpComponent gasPressurePumpComponent = default(GasPressurePumpComponent);
			if (base.EntMan.TryGetComponent<GasPressurePumpComponent>(((BoundUserInterface)this).Owner, ref gasPressurePumpComponent))
			{
				_window.SetPumpStatus(gasPressurePumpComponent.Enabled);
				_window.MaxPressure = gasPressurePumpComponent.MaxTargetPressure;
				_window.SetOutputPressure(gasPressurePumpComponent.TargetPressure);
			}
		}
	}

	private void OnToggleStatusButtonPressed()
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasPressurePumpToggleStatusMessage(_window.PumpStatus));
		}
	}

	private void OnPumpOutputPressurePressed(float value)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasPressurePumpChangeOutputPressureMessage(value));
	}
}
