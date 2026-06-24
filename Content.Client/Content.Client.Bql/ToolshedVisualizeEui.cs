using Content.Client.Eui;
using Content.Shared.Bql;
using Content.Shared.Eui;
using Robust.Client.Console;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Bql;

public sealed class ToolshedVisualizeEui : BaseEui
{
	private readonly ToolshedVisualizeWindow _window;

	public ToolshedVisualizeEui()
	{
		_window = new ToolshedVisualizeWindow(IoCManager.Resolve<IClientConsoleHost>(), IoCManager.Resolve<ILocalizationManager>());
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is ToolshedVisualizeEuiState toolshedVisualizeEuiState)
		{
			_window.Update(toolshedVisualizeEuiState.Entities);
		}
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).Close();
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}
}
