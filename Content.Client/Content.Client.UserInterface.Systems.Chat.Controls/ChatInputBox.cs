using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

[Virtual]
public class ChatInputBox : PanelContainer
{
	public readonly ChannelSelectorButton ChannelSelector;

	public readonly HistoryLineEdit Input;

	public readonly ChannelFilterButton FilterButton;

	protected readonly BoxContainer Container;

	protected ChatChannel ActiveChannel { get; private set; } = ChatChannel.Local;

	public ChatInputBox()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		Container = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 4
		};
		((Control)this).AddChild((Control)(object)Container);
		ChannelSelectorButton channelSelectorButton = new ChannelSelectorButton();
		((Control)channelSelectorButton).Name = "ChannelSelector";
		((BaseButton)channelSelectorButton).ToggleMode = true;
		((Control)channelSelectorButton).StyleClasses.Add("chatSelectorOptionButton");
		((Control)channelSelectorButton).MinWidth = 75f;
		ChannelSelector = channelSelectorButton;
		((Control)Container).AddChild((Control)(object)ChannelSelector);
		Input = new HistoryLineEdit
		{
			Name = "Input",
			PlaceHolder = GetChatboxInfoPlaceholder(),
			HorizontalExpand = true,
			StyleClasses = { "chatLineEdit" }
		};
		((Control)Container).AddChild((Control)(object)Input);
		ChannelFilterButton channelFilterButton = new ChannelFilterButton();
		((Control)channelFilterButton).Name = "FilterButton";
		((Control)channelFilterButton).StyleClasses.Add("chatFilterOptionButton");
		FilterButton = channelFilterButton;
		((Control)Container).AddChild((Control)(object)FilterButton);
		((Control)this).AddStyleClass("ChatSubPanel");
		ChannelSelector.OnChannelSelect += UpdateActiveChannel;
	}

	private void UpdateActiveChannel(ChatSelectChannel selectedChannel)
	{
		ActiveChannel = (ChatChannel)selectedChannel;
	}

	private static string GetChatboxInfoPlaceholder()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		bool num = BoundKeyHelper.IsBound(ContentKeyFunctions.FocusChat);
		bool flag = BoundKeyHelper.IsBound(ContentKeyFunctions.CycleChatChannelForward);
		if (num)
		{
			if (flag)
			{
				return Loc.GetString("hud-chatbox-info", new(string, object)[2]
				{
					("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)),
					("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))
				});
			}
			return Loc.GetString("hud-chatbox-info-talk", new(string, object)[1] { ("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)) });
		}
		if (flag)
		{
			return Loc.GetString("hud-chatbox-info-cycle", new(string, object)[1] { ("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward)) });
		}
		return Loc.GetString("hud-chatbox-info-unbound");
	}
}
