using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Power;

public sealed class PowerMonitoringButton : Button
{
	public BoxContainer MainContainer;

	public TextureRect TextureRect;

	public Label NameLocalized;

	public ProgressBar BatteryLevel;

	public PanelContainer BackgroundPanel;

	public Label BatteryPercentage;

	public Label PowerValue;

	public PowerMonitoringButton()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0153: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		((Control)this).Margin = new Thickness(0f, 1f, 0f, 1f);
		MainContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			SetHeight = 32f
		};
		((Control)this).AddChild((Control)(object)MainContainer);
		TextureRect = new TextureRect
		{
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			SetSize = new Vector2(32f, 32f),
			Margin = new Thickness(0f, 0f, 5f, 0f)
		};
		((Control)MainContainer).AddChild((Control)(object)TextureRect);
		NameLocalized = new Label
		{
			HorizontalExpand = true,
			ClipText = true
		};
		((Control)MainContainer).AddChild((Control)(object)NameLocalized);
		BatteryLevel = new ProgressBar
		{
			SetWidth = 47f,
			SetHeight = 20f,
			Margin = new Thickness(15f, 0f, 0f, 0f),
			MaxValue = 1f,
			Visible = false,
			BackgroundStyleBoxOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.Black
			}
		};
		((Control)MainContainer).AddChild((Control)(object)BatteryLevel);
		BackgroundPanel = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = new Color(0f, 0f, 0f, 0.9f)
			},
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			HorizontalExpand = true,
			VerticalExpand = true,
			SetSize = new Vector2(43f, 16f)
		};
		((Control)BatteryLevel).AddChild((Control)(object)BackgroundPanel);
		BatteryPercentage = new Label
		{
			VerticalAlignment = (VAlignment)2,
			HorizontalAlignment = (HAlignment)2,
			Align = (AlignMode)1,
			SetWidth = 45f,
			MinWidth = 20f,
			Margin = new Thickness(10f, -4f, 10f, 0f),
			ClipText = true,
			Visible = false
		};
		((Control)BackgroundPanel).AddChild((Control)(object)BatteryPercentage);
		PowerValue = new Label
		{
			HorizontalAlignment = (HAlignment)3,
			Align = (AlignMode)2,
			SetWidth = 80f,
			Margin = new Thickness(10f, 0f, 0f, 0f),
			ClipText = true
		};
		((Control)MainContainer).AddChild((Control)(object)PowerValue);
	}
}
