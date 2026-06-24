using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelFilterCheckbox : CheckBox
{
	public readonly ChatChannel Channel;

	public bool IsHidden => ((Control)this).Parent == null;

	public ChannelFilterCheckbox(ChatChannel channel)
	{
		Channel = channel;
		((CheckBox)this).Text = Loc.GetString($"hud-chatbox-channel-{Channel}");
	}

	private void UpdateText(int? unread)
	{
		string text = Loc.GetString($"hud-chatbox-channel-{Channel}");
		if (unread > 0)
		{
			text = text + " (" + ((unread > 9) ? "9+" : ((object)unread))?.ToString() + ")";
		}
		((CheckBox)this).Text = text;
	}

	public void UpdateUnreadCount(int? unread)
	{
		UpdateText(unread);
	}
}
