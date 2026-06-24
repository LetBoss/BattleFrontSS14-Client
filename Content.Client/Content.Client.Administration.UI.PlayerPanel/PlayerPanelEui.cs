using Content.Client.Administration.Managers;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Administration.UI.PlayerPanel;

public sealed class PlayerPanelEui : BaseEui
{
	[Dependency]
	private IClientConsoleHost _console;

	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IClipboardManager _clipboard;

	private PlayerPanel PlayerPanel { get; }

	public PlayerPanelEui()
	{
		PlayerPanel = new PlayerPanel(_admin);
		PlayerPanel.OnUsernameCopy += delegate(string username)
		{
			_clipboard.SetText(username);
		};
		PlayerPanel.OnOpenNotes += delegate(NetUserId? id)
		{
			((IConsoleHost)_console).ExecuteCommand($"adminnotes \"{id}\"");
		};
		PlayerPanel.OnKick += delegate(string? username)
		{
			((IConsoleHost)_console).ExecuteCommand("kick \"" + username + "\"");
		};
		PlayerPanel.OnOpenBanPanel += delegate(NetUserId? id)
		{
			((IConsoleHost)_console).ExecuteCommand($"banpanel \"{id}\"");
		};
		PlayerPanel.OnOpenBans += delegate(NetUserId? id)
		{
			((IConsoleHost)_console).ExecuteCommand($"banlist \"{id}\"");
		};
		PlayerPanel.OnAhelp += delegate(NetUserId? id)
		{
			((IConsoleHost)_console).ExecuteCommand($"openahelp \"{id}\"");
		};
		PlayerPanel.OnWhitelistToggle += delegate(NetUserId? id, bool whitelisted)
		{
			((IConsoleHost)_console).ExecuteCommand(whitelisted ? $"whitelistremove \"{id}\"" : $"whitelistadd \"{id}\"");
		};
		PlayerPanel.OnFreezeAndMuteToggle += delegate
		{
			SendMessage(new PlayerPanelFreezeMessage(mute: true));
		};
		PlayerPanel.OnFreeze += delegate
		{
			SendMessage(new PlayerPanelFreezeMessage());
		};
		PlayerPanel.OnLogs += delegate
		{
			SendMessage(new PlayerPanelLogsMessage());
		};
		PlayerPanel.OnRejuvenate += delegate
		{
			SendMessage(new PlayerPanelRejuvenationMessage());
		};
		PlayerPanel.OnDelete += delegate
		{
			SendMessage(new PlayerPanelDeleteMessage());
		};
		PlayerPanel.OnFollow += delegate
		{
			SendMessage(new PlayerPanelFollowMessage());
		};
		((BaseWindow)PlayerPanel).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void Opened()
	{
		((BaseWindow)PlayerPanel).OpenCentered();
	}

	public override void Closed()
	{
		((BaseWindow)PlayerPanel).Close();
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (state is PlayerPanelEuiState playerPanelEuiState)
		{
			PlayerPanel.TargetPlayer = playerPanelEuiState.Guid;
			PlayerPanel.TargetUsername = playerPanelEuiState.Username;
			PlayerPanel.SetUsername(playerPanelEuiState.Username);
			PlayerPanel.SetPlaytime(playerPanelEuiState.Playtime);
			PlayerPanel.SetBans(playerPanelEuiState.TotalBans, playerPanelEuiState.TotalRoleBans);
			PlayerPanel.SetNotes(playerPanelEuiState.TotalNotes);
			PlayerPanel.SetWhitelisted(playerPanelEuiState.Whitelisted);
			PlayerPanel.SetSharedConnections(playerPanelEuiState.SharedConnections);
			PlayerPanel.SetFrozen(playerPanelEuiState.CanFreeze, playerPanelEuiState.Frozen);
			PlayerPanel.SetAhelp(playerPanelEuiState.CanAhelp);
			PlayerPanel.SetButtons();
		}
	}
}
