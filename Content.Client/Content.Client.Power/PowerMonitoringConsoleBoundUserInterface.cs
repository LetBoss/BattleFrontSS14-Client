using System;
using Content.Shared.Power;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power;

public sealed class PowerMonitoringConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private PowerMonitoringWindow? _menu;

	public PowerMonitoringConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<PowerMonitoringWindow>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.SendPowerMonitoringConsoleMessageAction += SendPowerMonitoringConsoleMessage;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		PowerMonitoringConsoleBoundInterfaceState powerMonitoringConsoleBoundInterfaceState = (PowerMonitoringConsoleBoundInterfaceState)(object)state;
		TransformComponent val = default(TransformComponent);
		base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val);
		_menu?.ShowEntites(powerMonitoringConsoleBoundInterfaceState.TotalSources, powerMonitoringConsoleBoundInterfaceState.TotalBatteryUsage, powerMonitoringConsoleBoundInterfaceState.TotalLoads, powerMonitoringConsoleBoundInterfaceState.AllEntries, powerMonitoringConsoleBoundInterfaceState.FocusSources, powerMonitoringConsoleBoundInterfaceState.FocusLoads, (val != null) ? new EntityCoordinates?(val.Coordinates) : ((EntityCoordinates?)null));
	}

	public void SendPowerMonitoringConsoleMessage(NetEntity? netEntity, PowerMonitoringConsoleGroup group)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PowerMonitoringConsoleMessage(netEntity, group));
	}
}
