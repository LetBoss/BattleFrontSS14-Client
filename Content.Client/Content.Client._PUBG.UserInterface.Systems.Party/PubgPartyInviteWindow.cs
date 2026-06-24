using System;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Party;

public sealed class PubgPartyInviteWindow : DefaultWindow
{
	private readonly RichTextLabel _body;

	private readonly Label _timer;

	public Button AcceptButton { get; }

	public Button DeclineButton { get; }

	public PubgPartyInviteWindow()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("pubg-party-invite-title");
		_body = new RichTextLabel
		{
			HorizontalExpand = true
		};
		_timer = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#BBBBBB", (Color?)null)
		};
		AcceptButton = new Button
		{
			Text = Loc.GetString("pubg-party-invite-accept")
		};
		DeclineButton = new Button
		{
			Text = Loc.GetString("pubg-party-invite-decline")
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		((Control)val).AddChild((Control)(object)AcceptButton);
		((Control)val).AddChild(new Control
		{
			MinSize = new Vector2(16f, 0f)
		});
		((Control)val).AddChild((Control)(object)DeclineButton);
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 12,
			Margin = new Thickness(12f)
		};
		((Control)val2).AddChild((Control)(object)_body);
		((Control)val2).AddChild((Control)(object)_timer);
		((Control)val2).AddChild((Control)(object)val);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val2);
	}

	public void SetInviter(string inviterName)
	{
		_body.Text = Loc.GetString("pubg-party-invite-message", new(string, object)[1] { ("name", inviterName) });
	}

	public void SetTimerSeconds(int seconds)
	{
		_timer.Text = Loc.GetString("pubg-party-invite-timer", new(string, object)[1] { ("seconds", seconds) });
	}

	public void SetExpired()
	{
		_timer.Text = Loc.GetString("pubg-party-invite-expired");
	}
}
