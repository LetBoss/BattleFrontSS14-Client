using System;
using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.Consoles;

public sealed class AtmosMonitoringConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private AtmosMonitoringConsoleWindow? _menu;

	public AtmosMonitoringConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = new AtmosMonitoringConsoleWindow(this, ((BoundUserInterface)this).Owner);
		((BaseWindow)_menu).OpenCentered();
		((BaseWindow)_menu).OnClose += base.Close;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (state is AtmosMonitoringConsoleBoundInterfaceState atmosMonitoringConsoleBoundInterfaceState)
		{
			TransformComponent val = default(TransformComponent);
			base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val);
			_menu?.UpdateUI((val != null) ? new EntityCoordinates?(val.Coordinates) : ((EntityCoordinates?)null), atmosMonitoringConsoleBoundInterfaceState.AtmosNetworks);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			AtmosMonitoringConsoleWindow? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
