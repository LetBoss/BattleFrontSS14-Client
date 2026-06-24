using Content.Client.Eui;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.NPC;

public sealed class NPCEui : BaseEui
{
	private NPCWindow? _window = new NPCWindow();

	public override void Opened()
	{
		base.Opened();
		_window = new NPCWindow();
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).OnClose += OnClosed;
	}

	private void OnClosed()
	{
		SendMessage(new CloseEuiMessage());
	}

	public override void Closed()
	{
		base.Closed();
		NPCWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		_window = null;
	}
}
