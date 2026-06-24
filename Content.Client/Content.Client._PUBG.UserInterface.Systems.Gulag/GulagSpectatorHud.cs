using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagSpectatorHud : PanelContainer
{
	private Label _fighter1Label;

	private Label _vsLabel;

	private Label _fighter2Label;

	private Label _timerLabel;

	private Label _queueLabel;

	public GulagSpectatorHud()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#000000DD", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#6c757d", (Color?)null),
			BorderThickness = new Thickness(0f, 0f, 0f, 3f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 15f);
		((PanelContainer)this).PanelOverride = (StyleBox)(object)val;
		((Control)this).MinWidth = 400f;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalAlignment = (HAlignment)2
		};
		Label val3 = new Label
		{
			Text = "\ud83d\udc41 ГУЛАГ - РЕЖИМ НАБЛЮДЕНИЯ \ud83d\udc41",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#6c757d", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 15,
			HorizontalAlignment = (HAlignment)2
		};
		_fighter1Label = new Label
		{
			Text = "Fighter1",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF0000", (Color?)null)
		};
		_vsLabel = new Label
		{
			Text = "VS",
			FontColorOverride = Color.White
		};
		_fighter2Label = new Label
		{
			Text = "Fighter2",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#0000FF", (Color?)null)
		};
		((Control)val4).AddChild((Control)(object)_fighter1Label);
		((Control)val4).AddChild((Control)(object)_vsLabel);
		((Control)val4).AddChild((Control)(object)_fighter2Label);
		_timerLabel = new Label
		{
			Text = "00:60",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		_queueLabel = new Label
		{
			Text = "В очереди: 0",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#AAAAAA", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val2).AddChild((Control)(object)_timerLabel);
		((Control)val2).AddChild((Control)(object)_queueLabel);
		((Control)this).AddChild((Control)(object)val2);
	}

	public void UpdateFight(string fighter1Username, string fighter1Rank, string fighter2Username, string fighter2Rank, float timeRemaining, int queueCount)
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		_fighter1Label.Text = fighter1Username + " (#" + fighter1Rank + ")";
		_fighter2Label.Text = fighter2Username + " (#" + fighter2Rank + ")";
		int value = (int)timeRemaining / 60;
		int value2 = (int)timeRemaining % 60;
		_timerLabel.Text = $"{value:D2}:{value2:D2}";
		_queueLabel.Text = $"В очереди: {queueCount}";
		if (timeRemaining <= 10f)
		{
			_timerLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#DC3545", (Color?)null);
		}
		else
		{
			_timerLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null);
		}
	}
}
