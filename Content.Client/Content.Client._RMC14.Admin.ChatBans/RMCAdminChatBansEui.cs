using System;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin.ChatBans;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Admin.ChatBans;

public sealed class RMCAdminChatBansEui : BaseEui
{
	private RMCAdminChatBanWindow? _window;

	private string? _prefilledTarget;

	public override void Opened()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		base.Opened();
		_window = new RMCAdminChatBanWindow();
		if (!string.IsNullOrWhiteSpace(_prefilledTarget))
		{
			_window.PlayerEdit.Text = _prefilledTarget;
		}
		_window.ReasonEdit.Placeholder = (Node)new Leaf(Loc.GetString("rmc-chat-bans-reason-placeholder"));
		((BaseButton)_window.SubmitButton).OnPressed += delegate
		{
			ChatType chatType = ChatType.None;
			if (((BaseButton)_window.DeadButton).Pressed)
			{
				chatType |= ChatType.Dead;
			}
			if (((BaseButton)_window.LoocButton).Pressed)
			{
				chatType |= ChatType.Looc;
			}
			if (((BaseButton)_window.OocButton).Pressed)
			{
				chatType |= ChatType.Ooc;
			}
			if (((BaseButton)_window.LocalButton).Pressed)
			{
				chatType |= ChatType.Local;
			}
			if (((BaseButton)_window.WhisperButton).Pressed)
			{
				chatType |= ChatType.Whisper;
			}
			if (((BaseButton)_window.RadioButton).Pressed)
			{
				chatType |= ChatType.Radio;
			}
			if (((BaseButton)_window.EmoteButton).Pressed)
			{
				chatType |= ChatType.Emote;
			}
			if (((BaseButton)_window.PartyButton).Pressed)
			{
				chatType |= ChatType.Party;
			}
			if (((BaseButton)_window.MiniGameButton).Pressed)
			{
				chatType |= ChatType.MiniGame;
			}
			if (((BaseButton)_window.LobbyButton).Pressed)
			{
				chatType |= ChatType.Lobby;
			}
			if (((BaseButton)_window.AhelpButton).Pressed)
			{
				chatType |= ChatType.Ahelp;
			}
			if (!double.TryParse(_window.TimeEdit.Text, out var result))
			{
				result = 0.0;
			}
			result *= (double)_window.Multiplier;
			TimeSpan value = TimeSpan.FromMinutes(result);
			string reason = Rope.Collapse(_window.ReasonEdit.TextRope);
			RMCAdminChatBanAddMsg msg = new RMCAdminChatBanAddMsg(_window.PlayerEdit.Text, chatType, (result == 0.0) ? ((TimeSpan?)null) : new TimeSpan?(value), reason);
			SendMessage(msg);
		};
		((BaseWindow)_window).OpenCentered();
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (state is RMCAdminChatBanState rMCAdminChatBanState)
		{
			_prefilledTarget = rMCAdminChatBanState.Target;
			if (_window != null && !string.IsNullOrWhiteSpace(rMCAdminChatBanState.Target))
			{
				_window.PlayerEdit.Text = rMCAdminChatBanState.Target;
			}
		}
	}

	public override void Closed()
	{
		base.Closed();
		RMCAdminChatBanWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
	}

	public override void HandleMessage(EuiMessageBase msg)
	{
		base.HandleMessage(msg);
		if (_window != null)
		{
			RichTextLabel errorLabel = _window.ErrorLabel;
			string text = ((!(msg is RMCAdminChatBanAddErrorMsg rMCAdminChatBanAddErrorMsg)) ? _window.ErrorLabel.Text : rMCAdminChatBanAddErrorMsg.Message);
			errorLabel.Text = text;
		}
	}
}
