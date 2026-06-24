using System;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagFightHud : PanelContainer
{
	private Label _opponentLabel;

	private Label _timerLabel;

	public GulagFightHud()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#000000DD", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#DC3545", (Color?)null),
			BorderThickness = new Thickness(0f, 0f, 0f, 3f)
		};
		((StyleBox)val).SetContentMarginOverride((Margin)15, 15f);
		((PanelContainer)this).PanelOverride = (StyleBox)(object)val;
		((Control)this).MinWidth = 350f;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 10,
			HorizontalAlignment = (HAlignment)2
		};
		Label val3 = new Label
		{
			Text = "⚔ БОЙ В ГУЛАГ ⚔",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#DC3545", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		_opponentLabel = new Label
		{
			Text = "Противник: -",
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2
		};
		_timerLabel = new Label
		{
			Text = "00:60",
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFA500", (Color?)null),
			HorizontalAlignment = (HAlignment)2
		};
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)_opponentLabel);
		((Control)val2).AddChild((Control)(object)_timerLabel);
		((Control)this).AddChild((Control)(object)val2);
	}

	public void UpdateFight(string opponentUsername, string opponentRank, float timeRemaining)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		_opponentLabel.Text = $"Противник: {opponentUsername} (#{opponentRank})";
		int value = (int)timeRemaining / 60;
		int value2 = (int)timeRemaining % 60;
		_timerLabel.Text = $"{value:D2}:{value2:D2}";
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
