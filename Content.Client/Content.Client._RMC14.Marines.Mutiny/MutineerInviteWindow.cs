using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutineerInviteWindow : DefaultWindow
{
	public Button DenyButton { get; }

	public Button AcceptButton { get; }

	public MutineerInviteWindow()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("mutineer-invite-title");
		AcceptButton = new Button
		{
			Text = Loc.GetString("mutineer-invite-accept")
		};
		DenyButton = new Button
		{
			Text = Loc.GetString("mutineer-invite-deny")
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val).AddChild((Control)new RichTextLabel
		{
			Text = "You are being asked to join a mutiny.",
			VerticalExpand = true,
			VerticalAlignment = (VAlignment)2
		});
		((Control)val).AddChild((Control)new RichTextLabel
		{
			Text = "Read the Mutinies and Riots guidelines (Core Rules -> \"Mutinies, Riots\").",
			VerticalExpand = true,
			VerticalAlignment = (VAlignment)2
		});
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Align = (AlignMode)1
		};
		((Control)val2).AddChild((Control)(object)AcceptButton);
		((Control)val2).AddChild(new Control
		{
			MinSize = new Vector2(20f, 0f)
		});
		((Control)val2).AddChild((Control)(object)DenyButton);
		((Control)val).AddChild((Control)(object)val2);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}
}
