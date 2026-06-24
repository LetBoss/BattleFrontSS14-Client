using System;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Monitor.UI;

public sealed class AirAlarmBoundUserInterface : BoundUserInterface
{
	private AirAlarmWindow? _window;

	public AirAlarmBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<AirAlarmWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(((BoundUserInterface)this).Owner);
		_window.AtmosDeviceDataChanged += OnDeviceDataChanged;
		_window.AtmosDeviceDataCopied += OnDeviceDataCopied;
		_window.AtmosAlarmThresholdChanged += OnThresholdChanged;
		_window.AirAlarmModeChanged += OnAirAlarmModeChanged;
		_window.AutoModeChanged += OnAutoModeChanged;
		_window.ResyncAllRequested += ResyncAllDevices;
	}

	private void ResyncAllDevices()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmResyncAllDevicesMessage());
	}

	private void OnDeviceDataChanged(string address, IAtmosDeviceData data)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmUpdateDeviceDataMessage(address, data));
	}

	private void OnDeviceDataCopied(IAtmosDeviceData data)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmCopyDeviceDataMessage(data));
	}

	private void OnAirAlarmModeChanged(AirAlarmMode mode)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmUpdateAlarmModeMessage(mode));
	}

	private void OnAutoModeChanged(bool enabled)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmUpdateAutoModeMessage(enabled));
	}

	private void OnThresholdChanged(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AirAlarmUpdateAlarmThresholdMessage(address, type, threshold, gas));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is AirAlarmUIState state2 && _window != null)
		{
			_window.UpdateState(state2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			AirAlarmWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
