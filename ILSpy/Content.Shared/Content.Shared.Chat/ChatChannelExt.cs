using System;
using Robust.Shared.Localization;

namespace Content.Shared.Chat;

public static class ChatChannelExt
{
	public static string GetString(this ChatChannel channel)
	{
		return channel switch
		{
			ChatChannel.OOC => Loc.GetString("chat-channel-humanized-ooc"), 
			ChatChannel.AdminChat => Loc.GetString("chat-channel-humanized-admin"), 
			ChatChannel.Party => Loc.GetString("chat-channel-humanized-party"), 
			ChatChannel.MiniGame => Loc.GetString("chat-channel-humanized-minigame"), 
			ChatChannel.Lobby => Loc.GetString("chat-channel-humanized-lobby"), 
			_ => throw new ArgumentOutOfRangeException("channel", channel, null), 
		};
	}
}
