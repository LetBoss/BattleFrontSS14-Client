using System;
using Content.Shared._PUBG.Party;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyHudRow : BoxContainer
{
	private readonly Label _nameLabel;

	private readonly ProgressBar _hpBar;

	private readonly Label _voiceLabel;

	public PubgPartyHudRow()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((BoxContainer)this).SeparationOverride = 2;
		_nameLabel = new Label
		{
			FontColorOverride = Color.White
		};
		_voiceLabel = new Label
		{
			FontColorOverride = Color.Gray
		};
		_hpBar = new ProgressBar
		{
			MinValue = 0f,
			MaxValue = 1f,
			Value = 1f,
			SetHeight = 8f,
			HorizontalExpand = true
		};
		((Control)this).AddChild((Control)(object)_nameLabel);
		((Control)this).AddChild((Control)(object)_voiceLabel);
		((Control)this).AddChild((Control)(object)_hpBar);
	}

	public void UpdateRow(PubgPartyMemberState member)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		_nameLabel.Text = Loc.GetString("pubg-party-member-ckey-with-level", new(string, object)[2]
		{
			("ckey", member.Username),
			("level", member.Level)
		});
		((Range)_hpBar).Value = Math.Clamp(member.HpPercent, 0f, 1f);
		((Control)_hpBar).Visible = !member.IsDead;
		_voiceLabel.Text = Loc.GetString(member.IsInVoice ? "pubg-party-voice-on" : "pubg-party-voice-off");
		_nameLabel.FontColorOverride = (member.IsDead ? Color.Gray : GetPartyColor(member.SlotIndex));
	}

	private static Color GetPartyColor(int slotIndex)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(slotIndex switch
		{
			1 => Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null), 
			2 => Color.FromHex((ReadOnlySpan<char>)"#ffeb3b", (Color?)null), 
			3 => Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null), 
		});
	}
}
