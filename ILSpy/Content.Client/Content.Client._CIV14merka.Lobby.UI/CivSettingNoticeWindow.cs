using System;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Lobby.UI;

public sealed class CivSettingNoticeWindow : DefaultWindow
{
	public event Action<bool>? ChoiceMade;

	public CivSettingNoticeWindow(string title, string message, string enableText, string disableText)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		((DefaultWindow)this).Title = title;
		((Control)this).MinSize = new Vector2(480f, 220f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 12,
			Margin = new Thickness(12f)
		};
		RichTextLabel val2 = new RichTextLabel
		{
			HorizontalExpand = true
		};
		((Control)val2).SetWidth = 456f;
		val2.SetMessage(message, (Color?)null);
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		Button val4 = new Button
		{
			Text = enableText,
			HorizontalExpand = true,
			StyleClasses = { "ButtonColorGreen" }
		};
		((BaseButton)val4).OnPressed += delegate
		{
			this.ChoiceMade?.Invoke(obj: true);
			((BaseWindow)this).Close();
		};
		Button val5 = new Button
		{
			Text = disableText,
			HorizontalExpand = true
		};
		((BaseButton)val5).OnPressed += delegate
		{
			this.ChoiceMade?.Invoke(obj: false);
			((BaseWindow)this).Close();
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		((DefaultWindow)this).Contents.AddChild((Control)(object)val);
	}
}
