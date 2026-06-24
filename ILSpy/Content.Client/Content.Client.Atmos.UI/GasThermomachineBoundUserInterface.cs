using System;
using Content.Client.Power.EntitySystems;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Systems;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasThermomachineBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
	[ViewVariables]
	private GasThermomachineWindow? _window;

	[ViewVariables]
	private float _minTemp;

	[ViewVariables]
	private float _maxTemp;

	[ViewVariables]
	private bool _isHeater = true;

	protected override void Open()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasThermomachineWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.ToggleStatusButton).OnPressed += delegate
		{
			OnToggleStatusButtonPressed();
		};
		_window.TemperatureSpinbox.OnValueChanged += delegate
		{
			OnTemperatureChanged(_window.TemperatureSpinbox.Value);
		};
		_window.Entity = ((BoundUserInterface)this).Owner;
		((BoundUserInterface)this).Update();
	}

	private void OnToggleStatusButtonPressed()
	{
		if (_window != null)
		{
			_window.SetActive(!_window.Active);
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasThermomachineToggleMessage());
		}
	}

	private void OnTemperatureChanged(float value)
	{
		float num = 0f;
		num = ((!_isHeater) ? Math.Max(value, _minTemp) : Math.Min(value, _maxTemp));
		num = Math.Max(num, 2.7f);
		if (!MathHelper.CloseTo((double)num, (double)value, 0.09))
		{
			_window?.SetTemperature(num);
		}
		else
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasThermomachineChangeTemperatureMessage(num));
		}
	}

	public override void Update()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		GasThermoMachineComponent gasThermoMachineComponent = default(GasThermoMachineComponent);
		if (_window != null && base.EntMan.TryGetComponent<GasThermoMachineComponent>(((BoundUserInterface)this).Owner, ref gasThermoMachineComponent))
		{
			SharedGasThermoMachineSystem sharedGasThermoMachineSystem = base.EntMan.System<SharedGasThermoMachineSystem>();
			_minTemp = gasThermoMachineComponent.MinTemperature;
			_maxTemp = gasThermoMachineComponent.MaxTemperature;
			_isHeater = sharedGasThermoMachineSystem.IsHeater(gasThermoMachineComponent);
			_window.SetTemperature(gasThermoMachineComponent.TargetTemperature);
			PowerReceiverSystem powerReceiverSystem = base.EntMan.System<PowerReceiverSystem>();
			SharedApcPowerReceiverComponent component = null;
			powerReceiverSystem.ResolveApc(((BoundUserInterface)this).Owner, ref component);
			if (component != null)
			{
				_window.SetActive(!component.PowerDisabled);
			}
			GasThermomachineWindow? window = _window;
			string title = (_isHeater ? Loc.GetString("comp-gas-thermomachine-ui-title-heater") : Loc.GetString("comp-gas-thermomachine-ui-title-freezer"));
			window.Title = title;
		}
	}
}
