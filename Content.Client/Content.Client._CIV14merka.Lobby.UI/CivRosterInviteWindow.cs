using System;
using System.Numerics;
using Content.Shared._CIV14merka;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterInviteWindow : DefaultWindow
{
	private readonly Label _label;

	public event Action? AcceptPressed;

	public event Action? DeclinePressed;

	public CivRosterInviteWindow()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("civ-lobby-invite-title");
		((Control)this).MinSize = new Vector2(420f, 180f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(8f)
		};
		_label = new Label
		{
			HorizontalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		Button val3 = new Button
		{
			Text = Loc.GetString("civ-lobby-invite-accept"),
			HorizontalExpand = true
		};
		((BaseButton)val3).OnPressed += delegate
		{
			this.AcceptPressed?.Invoke();
		};
		Button val4 = new Button
		{
			Text = Loc.GetString("civ-lobby-invite-decline"),
			HorizontalExpand = true
		};
		((BaseButton)val4).OnPressed += delegate
		{
			this.DeclinePressed?.Invoke();
		};
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)_label);
		((Control)val).AddChild((Control)(object)val2);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}

	public void UpdateInvite(CivRosterInvitePromptEvent msg)
	{
		_label.Text = Loc.GetString("civ-lobby-invite-message", new(string, object)[3]
		{
			("inviter", msg.InviterName),
			("team", msg.TeamName),
			("squad", msg.SquadId)
		});
	}
}
