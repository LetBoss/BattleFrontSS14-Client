using System;
using Content.Shared.Anomaly;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client.Anomaly.Ui;

public sealed class AnomalyScannerBoundUserInterface : BoundUserInterface
{
	private AnomalyScannerMenu? _menu;

	public AnomalyScannerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = new AnomalyScannerMenu();
		((BaseWindow)_menu).OpenCentered();
		((BaseWindow)_menu).OnClose += base.Close;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is AnomalyScannerUserInterfaceState anomalyScannerUserInterfaceState && _menu != null)
		{
			_menu.LastMessage = anomalyScannerUserInterfaceState.Message;
			_menu.NextPulseTime = anomalyScannerUserInterfaceState.NextPulseTime;
			_menu.UpdateMenu();
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			AnomalyScannerMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
		}
	}
}
