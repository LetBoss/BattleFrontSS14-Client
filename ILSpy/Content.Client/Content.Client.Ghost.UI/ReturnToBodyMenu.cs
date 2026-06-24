using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.Ghost.UI;

public sealed class ReturnToBodyMenu : DefaultWindow
{
	public readonly Button DenyButton;

	public readonly Button AcceptButton;

	public ReturnToBodyMenu()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_0096: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00e4: Expected O, but got Unknown
		//IL_00ef: Expected O, but got Unknown
		//IL_00f4: Expected O, but got Unknown
		//IL_00f9: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("ghost-return-to-body-title");
		Control contents = ((DefaultWindow)this).Contents;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		OrderedChildCollection children = ((Control)val).Children;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val2).Children.Add((Control)new Label
		{
			Text = Loc.GetString("ghost-return-to-body-text")
		});
		OrderedChildCollection children2 = ((Control)val2).Children;
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Align = (AlignMode)1
		};
		OrderedChildCollection children3 = ((Control)val3).Children;
		Button val4 = new Button
		{
			Text = Loc.GetString("accept-cloning-window-accept-button")
		};
		Button val5 = val4;
		AcceptButton = val4;
		children3.Add((Control)(object)val5);
		((Control)val3).Children.Add(new Control
		{
			MinSize = new Vector2(20f, 0f)
		});
		OrderedChildCollection children4 = ((Control)val3).Children;
		Button val6 = new Button
		{
			Text = Loc.GetString("accept-cloning-window-deny-button")
		};
		val5 = val6;
		DenyButton = val6;
		children4.Add((Control)(object)val5);
		children2.Add((Control)val3);
		children.Add((Control)val2);
		contents.AddChild((Control)val);
	}
}
