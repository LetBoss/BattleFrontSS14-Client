using System;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Communications;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.Communications.UI;

public sealed class CommunicationsConsoleBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IConfigurationManager _cfg;

	[ViewVariables]
	private CommunicationsConsoleMenu? _menu;

	public CommunicationsConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<CommunicationsConsoleMenu>((BoundUserInterface)(object)this);
		_menu.OnAnnounce += AnnounceButtonPressed;
		_menu.OnBroadcast += BroadcastButtonPressed;
		_menu.OnAlertLevel += AlertLevelSelected;
		_menu.OnEmergencyLevel += EmergencyShuttleButtonPressed;
	}

	public void AlertLevelSelected(string level)
	{
		if (_menu.AlertLevelSelectable)
		{
			_menu.CurrentLevel = level;
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CommunicationsConsoleSelectAlertLevelMessage(level));
		}
	}

	public void EmergencyShuttleButtonPressed()
	{
		if (_menu.CountdownStarted)
		{
			RecallShuttle();
		}
		else
		{
			CallShuttle();
		}
	}

	public void AnnounceButtonPressed(string message)
	{
		int cVar = _cfg.GetCVar<int>(CCVars.ChatMaxAnnouncementLength);
		string message2 = SharedChatSystem.SanitizeAnnouncement(message, cVar);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CommunicationsConsoleAnnounceMessage(message2));
	}

	public void BroadcastButtonPressed(string message)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CommunicationsConsoleBroadcastMessage(message));
	}

	public void CallShuttle()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CommunicationsConsoleCallEmergencyShuttleMessage());
	}

	public void RecallShuttle()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CommunicationsConsoleRecallEmergencyShuttleMessage());
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CommunicationsConsoleInterfaceState communicationsConsoleInterfaceState && _menu != null)
		{
			_menu.CanAnnounce = communicationsConsoleInterfaceState.CanAnnounce;
			_menu.CanBroadcast = communicationsConsoleInterfaceState.CanBroadcast;
			_menu.CanCall = communicationsConsoleInterfaceState.CanCall;
			_menu.CountdownStarted = communicationsConsoleInterfaceState.CountdownStarted;
			_menu.AlertLevelSelectable = communicationsConsoleInterfaceState.AlertLevels != null && !float.IsNaN(communicationsConsoleInterfaceState.CurrentAlertDelay) && communicationsConsoleInterfaceState.CurrentAlertDelay <= 0f;
			_menu.CurrentLevel = communicationsConsoleInterfaceState.CurrentAlert;
			_menu.CountdownEnd = communicationsConsoleInterfaceState.ExpectedCountdownEnd;
			_menu.UpdateCountdown();
			_menu.UpdateAlertLevels(communicationsConsoleInterfaceState.AlertLevels, _menu.CurrentLevel);
			((BaseButton)_menu.AlertLevelButton).Disabled = !_menu.AlertLevelSelectable;
			((BaseButton)_menu.EmergencyShuttleButton).Disabled = !_menu.CanCall;
			((BaseButton)_menu.AnnounceButton).Disabled = !_menu.CanAnnounce;
			((BaseButton)_menu.BroadcastButton).Disabled = !_menu.CanBroadcast;
		}
	}
}
