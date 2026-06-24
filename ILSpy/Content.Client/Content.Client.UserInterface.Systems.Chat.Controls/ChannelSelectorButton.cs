using System;
using System.Numerics;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorButton : ChatPopupButton<ChannelSelectorPopup>
{
	private const int SelectorDropdownOffset = 38;

	public ChatSelectChannel SelectedChannel { get; private set; }

	public event Action<ChatSelectChannel>? OnChannelSelect;

	public ChannelSelectorButton()
	{
		((Control)this).Name = "ChannelSelector";
		Popup.Selected += OnChannelSelected;
		ChatSelectChannel? firstChannel = Popup.FirstChannel;
		if (firstChannel.HasValue)
		{
			ChatSelectChannel valueOrDefault = firstChannel.GetValueOrDefault();
			Select(valueOrDefault);
		}
	}

	protected override UIBox2 GetPopupPosition()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float x = ((Control)this).GlobalPosition.X;
		float y = ((Control)this).GlobalPosition.Y + ((Control)this).Height;
		Vector2 vector = new Vector2(x, y);
		UIBox2 sizeBox = ((Control)this).SizeBox;
		return UIBox2.FromDimensions(vector, new Vector2(((UIBox2)(ref sizeBox)).Width, 38f));
	}

	private void OnChannelSelected(ChatSelectChannel channel)
	{
		Select(channel);
	}

	public void Select(ChatSelectChannel channel)
	{
		if (((Control)Popup).Visible)
		{
			((Popup)Popup).Close();
		}
		if (SelectedChannel != channel)
		{
			SelectedChannel = channel;
			this.OnChannelSelect?.Invoke(channel);
		}
	}

	public static string ChannelSelectorName(ChatSelectChannel channel)
	{
		return Loc.GetString($"hud-chatbox-select-channel-{channel}");
	}

	public Color ChannelSelectColor(ChatSelectChannel channel)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(channel switch
		{
			ChatSelectChannel.Radio => Color.LimeGreen, 
			ChatSelectChannel.Party => Color.Gold, 
			ChatSelectChannel.MiniGame => Color.DeepSkyBlue, 
			ChatSelectChannel.Lobby => Color.LightGreen, 
			ChatSelectChannel.LOOC => Color.MediumTurquoise, 
			ChatSelectChannel.OOC => Color.LightSkyBlue, 
			ChatSelectChannel.Dead => Color.MediumPurple, 
			ChatSelectChannel.Admin => Color.HotPink, 
			ChatSelectChannel.Mentor => Color.Orange, 
			_ => Color.DarkGray, 
		});
	}

	public void UpdateChannelSelectButton(ChatSelectChannel channel, RadioChannelPrototype? radio, string? textOverride = null, Color? colorOverride = null)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((Button)this).Text = textOverride ?? ((radio != null) ? Loc.GetString(LocId.op_Implicit(radio.Name)) : ChannelSelectorName(channel));
		((Control)this).Modulate = (Color)(((_003F?)colorOverride) ?? radio?.Color ?? ChannelSelectColor(channel));
	}
}
