using Content.Client._PUBG.Leaderboard;
using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LeaderboardCommand : IConsoleCommand
{
	private LeaderboardWindow? _window;

	public string Command => "leaderboard";

	public string Description => "дуфвукищфкв";

	public string Help => "Usage: leaderboard";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (_window == null)
		{
			_window = new LeaderboardWindow();
			((BaseWindow)_window).OnClose += delegate
			{
				_window = null;
			};
		}
		if (((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).Close();
		}
		else
		{
			((BaseWindow)_window).OpenCentered();
		}
	}
}
