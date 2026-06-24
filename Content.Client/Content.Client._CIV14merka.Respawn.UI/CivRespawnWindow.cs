using System;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Respawn.UI;

public sealed class CivRespawnWindow : DefaultWindow
{
	public event Action? AcceptPressed;

	public event Action? DeclinePressed;

	public CivRespawnWindow()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		((DefaultWindow)this).Title = Loc.GetString("civ-ui-respawn-title");
		((Control)this).MinSize = new Vector2(440f, 180f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 8,
			Margin = new Thickness(8f)
		};
		Label val2 = new Label
		{
			HorizontalExpand = true,
			Text = Loc.GetString("civ-ui-respawn-prompt")
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		Button val4 = new Button
		{
			Text = Loc.GetString("civ-ui-respawn-yes"),
			HorizontalExpand = true
		};
		((BaseButton)val4).OnPressed += delegate
		{
			this.AcceptPressed?.Invoke();
		};
		Button val5 = new Button
		{
			Text = Loc.GetString("civ-ui-respawn-no"),
			HorizontalExpand = true
		};
		((BaseButton)val5).OnPressed += delegate
		{
			this.DeclinePressed?.Invoke();
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}
}
