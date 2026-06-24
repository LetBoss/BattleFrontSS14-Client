using System.Collections.Generic;
using Content.Client._RMC14.UserInterface;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin.ChatBans;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Admin.ChatBans;

public sealed class RMCAdminChatBansListEui : BaseEui
{
	private RMCAdminChatBanListWindow? _window;

	private RMCAdminChatBanListState? _state;

	public override void Opened()
	{
		base.Opened();
		_window = new RMCAdminChatBanListWindow();
		Refresh();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		RMCAdminChatBanListWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is RMCAdminChatBanListState state2)
		{
			_state = state2;
			Refresh();
		}
	}

	private void Refresh()
	{
		if (_window == null || _state == null)
		{
			return;
		}
		((Control)_window.Container).DisposeAllChildren();
		foreach (ChatBan ban in _state.Bans)
		{
			RMCAdminChatBanRow rMCAdminChatBanRow = new RMCAdminChatBanRow();
			rMCAdminChatBanRow.TypeLabel.Text = FormatChatType(ban.Type);
			rMCAdminChatBanRow.ReasonLabel.Text = FormattedMessage.EscapeText(ban.Reason);
			rMCAdminChatBanRow.BannedAtLabel.Text = $"{ban.BannedAt:MM/dd/yyyy h:mm tt}";
			rMCAdminChatBanRow.ExpiresAtLabel.Text = ((!ban.ExpiresAt.HasValue) ? "rmc-chat-bans-list-permanent" : $"{ban.ExpiresAt:MM/dd/yyyy h:mm tt}");
			if (ban.UnbannedBy != null)
			{
				Label expiresAtLabel = rMCAdminChatBanRow.ExpiresAtLabel;
				expiresAtLabel.Text = expiresAtLabel.Text + "\n" + Loc.GetString("ban-list-unbanned", new(string, object)[1] { ("date", $"{ban.UnbannedAt:MM/dd/yyyy h:mm tt}") });
				Label expiresAtLabel2 = rMCAdminChatBanRow.ExpiresAtLabel;
				expiresAtLabel2.Text = expiresAtLabel2.Text + "\n" + Loc.GetString("ban-list-unbanned-by", new(string, object)[1] { ("unbanner", ban.UnbannedBy) });
				((Control)rMCAdminChatBanRow.PardonButton).Visible = false;
			}
			rMCAdminChatBanRow.PardonButton.OnPressed += delegate
			{
				SendMessage(new RMCAdminChatBanListPardonMsg(ban.Id));
			};
			((Control)_window.Container).AddChild((Control)(object)rMCAdminChatBanRow);
			((Control)_window.Container).AddChild((Control)(object)new BlueHorizontalSeparator());
		}
	}

	private static string FormatChatType(ChatType type)
	{
		List<string> list = new List<string>();
		if (type.HasFlag(ChatType.Dead))
		{
			list.Add(Loc.GetString("rmc-chat-bans-dead"));
		}
		if (type.HasFlag(ChatType.Looc))
		{
			list.Add(Loc.GetString("rmc-chat-bans-looc"));
		}
		if (type.HasFlag(ChatType.Ooc))
		{
			list.Add(Loc.GetString("rmc-chat-bans-ooc"));
		}
		if (type.HasFlag(ChatType.Local))
		{
			list.Add(Loc.GetString("rmc-chat-bans-local"));
		}
		if (type.HasFlag(ChatType.Whisper))
		{
			list.Add(Loc.GetString("rmc-chat-bans-whisper"));
		}
		if (type.HasFlag(ChatType.Radio))
		{
			list.Add(Loc.GetString("rmc-chat-bans-radio"));
		}
		if (type.HasFlag(ChatType.Emote))
		{
			list.Add(Loc.GetString("rmc-chat-bans-emote"));
		}
		if (type.HasFlag(ChatType.Party))
		{
			list.Add(Loc.GetString("rmc-chat-bans-party"));
		}
		if (type.HasFlag(ChatType.MiniGame))
		{
			list.Add(Loc.GetString("rmc-chat-bans-minigame"));
		}
		if (type.HasFlag(ChatType.Lobby))
		{
			list.Add(Loc.GetString("rmc-chat-bans-lobby"));
		}
		if (type.HasFlag(ChatType.Ahelp))
		{
			list.Add(Loc.GetString("rmc-chat-bans-ahelp"));
		}
		if (list.Count <= 0)
		{
			return type.ToString();
		}
		return string.Join(", ", list);
	}
}
