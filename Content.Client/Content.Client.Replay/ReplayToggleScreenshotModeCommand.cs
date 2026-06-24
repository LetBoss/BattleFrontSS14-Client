using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared.Chat;
using Robust.Client.Replays.Commands;
using Robust.Client.Replays.UI;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Replay;

public sealed class ReplayToggleScreenshotModeCommand : BaseReplayCommand
{
	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private ContentReplayPlaybackManager _replayManager;

	public override string Command => "replay_toggle_screenshot_mode";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		UIScreen activeScreen = _userInterfaceManager.ActiveScreen;
		if (activeScreen == null)
		{
			return;
		}
		_replayManager.IsScreenshotMode = !_replayManager.IsScreenshotMode;
		bool isScreenshotMode = _replayManager.IsScreenshotMode;
		activeScreen.ShowWidget<ReplayControlWidget>(isScreenshotMode);
		foreach (ChatBox chat in _userInterfaceManager.GetUIController<ChatUIController>().Chats)
		{
			((Control)chat.ChatInput).Visible = !isScreenshotMode;
			if (!isScreenshotMode)
			{
				chat.ChatInput.ChannelSelector.Select(ChatSelectChannel.Local);
			}
		}
	}
}
