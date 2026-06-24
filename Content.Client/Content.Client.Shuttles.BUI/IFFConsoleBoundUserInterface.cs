using System;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Shuttles.BUI;

public sealed class IFFConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private IFFConsoleWindow? _window;

	public IFFConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<IFFConsoleWindow>((BoundUserInterface)(object)this);
		_window.ShowIFF += SendIFFMessage;
		_window.ShowVessel += SendVesselMessage;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is IFFConsoleBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	private void SendIFFMessage(bool obj)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new IFFShowIFFMessage
		{
			Show = obj
		});
	}

	private void SendVesselMessage(bool obj)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new IFFShowVesselMessage
		{
			Show = obj
		});
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			IFFConsoleWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).Close();
			}
			_window = null;
		}
	}
}
