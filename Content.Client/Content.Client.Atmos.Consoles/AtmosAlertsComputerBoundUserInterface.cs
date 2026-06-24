using System;
using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.Consoles;

public sealed class AtmosAlertsComputerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private AtmosAlertsComputerWindow? _menu;

	public AtmosAlertsComputerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = new AtmosAlertsComputerWindow(this, ((BoundUserInterface)this).Owner);
		((BaseWindow)_menu).OpenCentered();
		((BaseWindow)_menu).OnClose += base.Close;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		AtmosAlertsComputerBoundInterfaceState atmosAlertsComputerBoundInterfaceState = (AtmosAlertsComputerBoundInterfaceState)(object)state;
		TransformComponent val = default(TransformComponent);
		base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val);
		_menu?.UpdateUI((val != null) ? new EntityCoordinates?(val.Coordinates) : ((EntityCoordinates?)null), atmosAlertsComputerBoundInterfaceState.AirAlarms, atmosAlertsComputerBoundInterfaceState.FireAlarms, atmosAlertsComputerBoundInterfaceState.FocusData);
	}

	public void SendFocusChangeMessage(NetEntity? netEntity)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AtmosAlertsComputerFocusChangeMessage(netEntity));
	}

	public void SendDeviceSilencedMessage(NetEntity netEntity, bool silenceDevice)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AtmosAlertsComputerDeviceSilencedMessage(netEntity, silenceDevice));
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			AtmosAlertsComputerWindow? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
