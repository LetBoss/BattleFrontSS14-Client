using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivRosterWindow : FancyWindow
{
	private static readonly Color DarkBg = Color.FromHex((ReadOnlySpan<char>)"#13161A", (Color?)null);

	public CivRosterControl RosterControl { get; }

	public CivRosterWindow()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		base.Title = Loc.GetString("civ-lobby-roster-title");
		((Control)this).MinSize = new Vector2(1000f, 650f);
		((BaseWindow)this).Resizable = true;
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = DarkBg,
				ContentMarginLeftOverride = 10f,
				ContentMarginTopOverride = 10f,
				ContentMarginRightOverride = 10f,
				ContentMarginBottomOverride = 10f
			},
			HorizontalExpand = true,
			VerticalExpand = true
		};
		CivRosterControl civRosterControl = new CivRosterControl();
		((Control)civRosterControl).HorizontalExpand = true;
		((Control)civRosterControl).VerticalExpand = true;
		RosterControl = civRosterControl;
		((Control)val).AddChild((Control)(object)RosterControl);
		base.ContentsContainer.AddChild((Control)(object)val);
	}
}
