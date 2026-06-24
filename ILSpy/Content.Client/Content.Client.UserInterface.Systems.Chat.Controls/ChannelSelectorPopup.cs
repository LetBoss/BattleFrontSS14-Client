using System;
using System.Collections.Generic;
using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorPopup : Popup
{
	public static readonly ChatSelectChannel[] ChannelSelectorOrder = new ChatSelectChannel[12]
	{
		ChatSelectChannel.Local,
		ChatSelectChannel.Whisper,
		ChatSelectChannel.Emotes,
		ChatSelectChannel.Radio,
		ChatSelectChannel.Party,
		ChatSelectChannel.MiniGame,
		ChatSelectChannel.Lobby,
		ChatSelectChannel.LOOC,
		ChatSelectChannel.OOC,
		ChatSelectChannel.Dead,
		ChatSelectChannel.Admin,
		ChatSelectChannel.Mentor
	};

	private readonly BoxContainer _channelSelectorHBox;

	private readonly Dictionary<ChatSelectChannel, ChannelSelectorItemButton> _selectorStates = new Dictionary<ChatSelectChannel, ChannelSelectorItemButton>();

	private readonly ChatUIController _chatUIController;

	public ChatSelectChannel? FirstChannel
	{
		get
		{
			foreach (ChannelSelectorItemButton value in _selectorStates.Values)
			{
				if (!value.IsHidden)
				{
					return value.Channel;
				}
			}
			return null;
		}
	}

	public event Action<ChatSelectChannel>? Selected;

	public ChannelSelectorPopup()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		_channelSelectorHBox = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 1
		};
		_chatUIController = ((Control)this).UserInterfaceManager.GetUIController<ChatUIController>();
		_chatUIController.SelectableChannelsChanged += SetChannels;
		SetChannels(_chatUIController.SelectableChannels);
		((Control)this).AddChild((Control)(object)_channelSelectorHBox);
	}

	private bool IsPreferredAvailable()
	{
		ChatSelectChannel key = _chatUIController.MapLocalIfGhost(_chatUIController.GetPreferredChannel());
		if (_selectorStates.TryGetValue(key, out ChannelSelectorItemButton value))
		{
			return !value.IsHidden;
		}
		return false;
	}

	public void SetChannels(ChatSelectChannel channels)
	{
		bool flag = IsPreferredAvailable();
		((Control)_channelSelectorHBox).RemoveAllChildren();
		ChatSelectChannel[] channelSelectorOrder = ChannelSelectorOrder;
		foreach (ChatSelectChannel chatSelectChannel in channelSelectorOrder)
		{
			if (!_selectorStates.TryGetValue(chatSelectChannel, out ChannelSelectorItemButton value))
			{
				value = new ChannelSelectorItemButton(chatSelectChannel);
				_selectorStates.Add(chatSelectChannel, value);
				((BaseButton)value).OnPressed += OnSelectorPressed;
			}
			if ((channels & chatSelectChannel) == 0)
			{
				if ((object)((Control)value).Parent == _channelSelectorHBox)
				{
					((Control)_channelSelectorHBox).RemoveChild((Control)(object)value);
				}
			}
			else if (value.IsHidden)
			{
				((Control)_channelSelectorHBox).AddChild((Control)(object)value);
			}
		}
		bool flag2 = IsPreferredAvailable();
		if (!flag && flag2)
		{
			Select(_chatUIController.GetPreferredChannel());
		}
		else if (flag && !flag2)
		{
			Select(ChatSelectChannel.OOC);
		}
	}

	private void OnSelectorPressed(ButtonEventArgs args)
	{
		ChannelSelectorItemButton channelSelectorItemButton = (ChannelSelectorItemButton)(object)args.Button;
		Select(channelSelectorItemButton.Channel);
	}

	private void Select(ChatSelectChannel channel)
	{
		this.Selected?.Invoke(channel);
	}

	[Obsolete]
	protected override void Dispose(bool disposing)
	{
		((Control)this).Dispose(disposing);
		if (disposing)
		{
			_chatUIController.SelectableChannelsChanged -= SetChannels;
		}
	}
}
