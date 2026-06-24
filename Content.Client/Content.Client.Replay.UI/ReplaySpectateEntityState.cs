using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.Replays.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;

namespace Content.Client.Replay.UI;

[Virtual]
public class ReplaySpectateEntityState : GameplayState
{
	[Dependency]
	private ContentReplayPlaybackManager _replayManager;

	protected override void Startup()
	{
		base.Startup();
		UIScreen activeScreen = UserInterfaceManager.ActiveScreen;
		if (activeScreen == null)
		{
			return;
		}
		activeScreen.ShowWidget<GameTopMenuBar>(false);
		ReplayControlWidget orAddWidget = activeScreen.GetOrAddWidget<ReplayControlWidget>();
		LayoutContainer.SetAnchorAndMarginPreset((Control)(object)orAddWidget, (LayoutPreset)0, (LayoutPresetMode)0, 10);
		((Control)orAddWidget).Visible = !_replayManager.IsScreenshotMode;
		foreach (ChatBox chat in UserInterfaceManager.GetUIController<ChatUIController>().Chats)
		{
			((Control)chat.ChatInput).Visible = _replayManager.IsScreenshotMode;
		}
	}

	protected override void Shutdown()
	{
		UIScreen activeScreen = UserInterfaceManager.ActiveScreen;
		if (activeScreen != null)
		{
			activeScreen.RemoveWidget<ReplayControlWidget>();
			activeScreen.ShowWidget<GameTopMenuBar>(true);
		}
		foreach (ChatBox chat in UserInterfaceManager.GetUIController<ChatUIController>().Chats)
		{
			((Control)chat.ChatInput).Visible = true;
		}
		base.Shutdown();
	}
}
