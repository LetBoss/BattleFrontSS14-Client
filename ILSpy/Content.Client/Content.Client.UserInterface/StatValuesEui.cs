using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.UserInterface;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.UserInterface;

public sealed class StatValuesEui : BaseEui
{
	private readonly StatsWindow _window;

	public StatValuesEui()
	{
		_window = new StatsWindow();
		((DefaultWindow)_window).Title = "Melee stats";
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).OnClose += Closed;
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		base.HandleMessage(msg);
		if (msg is StatValuesEuiMessage statValuesEuiMessage)
		{
			((DefaultWindow)_window).Title = statValuesEuiMessage.Title;
			_window.UpdateValues(statValuesEuiMessage.Headers, statValuesEuiMessage.Values);
		}
	}
}
