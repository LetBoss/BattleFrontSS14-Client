using Content.Client.GameTicking.Managers;
using Content.Shared.GameTicking;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.RoundEnd;

public sealed class RoundEndSummaryUIController : UIController, IOnSystemLoaded<ClientGameTicker>
{
	[Dependency]
	private IInputManager _input;

	private RoundEndSummaryWindow? _window;

	private void ToggleScoreboardWindow(ICommonSession? session = null)
	{
		if (_window != null)
		{
			if (((BaseWindow)_window).IsOpen)
			{
				((BaseWindow)_window).Close();
				return;
			}
			((BaseWindow)_window).OpenCenteredRight();
			((BaseWindow)_window).MoveToFront();
		}
	}

	public void OpenRoundEndSummaryWindow(RoundEndMessageEvent message)
	{
		if (_window?.RoundId != message.RoundId)
		{
			_window = new RoundEndSummaryWindow(message.GamemodeTitle, message.RoundEndText, message.RoundDuration, message.RoundId, message.AllPlayersEndInfo, base.EntityManager);
		}
	}

	public void OnSystemLoaded(ClientGameTicker system)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		_input.SetInputCommand(ContentKeyFunctions.ToggleRoundEndSummaryWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(ToggleScoreboardWindow), (StateInputCmdDelegate)null, true, true));
	}
}
