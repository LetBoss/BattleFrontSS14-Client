using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Robust.Client.UserInterface;

namespace Content.Client.Replay.UI;

public sealed class ReplayGhostState : ReplaySpectateEntityState
{
	protected override void Startup()
	{
		base.Startup();
		UIScreen activeScreen = UserInterfaceManager.ActiveScreen;
		if (activeScreen != null)
		{
			activeScreen.ShowWidget<GhostGui>(false);
			activeScreen.ShowWidget<ActionsBar>(false);
			activeScreen.ShowWidget<AlertsUI>(false);
			activeScreen.ShowWidget<HotbarGui>(false);
		}
	}

	protected override void Shutdown()
	{
		UIScreen activeScreen = UserInterfaceManager.ActiveScreen;
		if (activeScreen != null)
		{
			activeScreen.ShowWidget<GhostGui>(true);
			activeScreen.ShowWidget<ActionsBar>(true);
			activeScreen.ShowWidget<AlertsUI>(true);
			activeScreen.ShowWidget<HotbarGui>(true);
		}
		base.Shutdown();
	}
}
