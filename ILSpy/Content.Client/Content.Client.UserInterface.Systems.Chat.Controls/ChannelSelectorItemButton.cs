using Content.Shared.Chat;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorItemButton : Button
{
	public readonly ChatSelectChannel Channel;

	public bool IsHidden => ((Control)this).Parent == null;

	public ChannelSelectorItemButton(ChatSelectChannel selector)
	{
		Channel = selector;
		((Control)this).AddStyleClass("chatSelectorOptionButton");
		((Button)this).Text = ChannelSelectorButton.ChannelSelectorName(selector);
		char c = ChatUIController.ChannelPrefixes[selector];
		if (c != 0)
		{
			((Button)this).Text = Loc.GetString("hud-chatbox-select-name-prefixed", new(string, object)[2]
			{
				("name", ((Button)this).Text),
				("prefix", c)
			});
		}
	}
}
