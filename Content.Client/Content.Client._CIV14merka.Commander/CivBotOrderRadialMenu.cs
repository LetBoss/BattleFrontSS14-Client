using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Commander;

public sealed class CivBotOrderRadialMenu : RadialMenu
{
	private readonly RadialContainer _options;

	private readonly PanelContainer _infoPanel;

	private readonly RichTextLabel _titleLabel;

	public event Action<CivBotOrderType>? OnOrderSelected;

	public CivBotOrderRadialMenu()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_018e: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		((Control)this).MinSize = new Vector2(420f, 420f);
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		base.CloseButtonStyleClass = "RadialMenuCloseButton";
		base.BackButtonStyleClass = "RadialMenuBackButton";
		RadialContainer radialContainer = new RadialContainer();
		((Control)radialContainer).HorizontalExpand = true;
		((Control)radialContainer).VerticalExpand = true;
		radialContainer.InitialRadius = 110f;
		radialContainer.InnerRadiusMultiplier = 0.62f;
		radialContainer.OuterRadiusMultiplier = 1.55f;
		radialContainer.ReserveSpaceForHiddenChildren = false;
		radialContainer.RadialAlignment = RadialContainer.RAlignment.Clockwise;
		_options = radialContainer;
		((Control)this).AddChild((Control)(object)_options);
		PanelContainer val = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)3,
			Margin = new Thickness(0f, 0f, 0f, 24f),
			MinSize = new Vector2(200f, 40f),
			MouseFilter = (MouseFilterMode)2
		};
		StyleBoxFlat val2 = new StyleBoxFlat();
		Color val3 = Color.FromHex((ReadOnlySpan<char>)"#0F1821", (Color?)null);
		val2.BackgroundColor = ((Color)(ref val3)).WithAlpha(0.94f);
		val3 = Color.FromHex((ReadOnlySpan<char>)"#D8B775", (Color?)null);
		val2.BorderColor = ((Color)(ref val3)).WithAlpha(0.85f);
		val2.BorderThickness = new Thickness(2f);
		((StyleBox)val2).ContentMarginLeftOverride = 12f;
		((StyleBox)val2).ContentMarginTopOverride = 8f;
		((StyleBox)val2).ContentMarginRightOverride = 12f;
		((StyleBox)val2).ContentMarginBottomOverride = 8f;
		val.PanelOverride = (StyleBox)val2;
		_infoPanel = val;
		_titleLabel = new RichTextLabel
		{
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			MouseFilter = (MouseFilterMode)2
		};
		((Control)_infoPanel).AddChild((Control)(object)_titleLabel);
		((Control)this).AddChild((Control)(object)_infoPanel);
		SetInfoText(Loc.GetString("civ-cmd-bot-radial-default"), null);
		BuildOptions();
	}

	private void BuildOptions()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		AddOption(CivBotOrderType.Move, Loc.GetString("civ-cmd-bot-radial-move-title"), Loc.GetString("civ-cmd-bot-radial-move-desc"), Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null));
		AddOption(CivBotOrderType.AttackMove, Loc.GetString("civ-cmd-bot-radial-attack-title"), Loc.GetString("civ-cmd-bot-radial-attack-desc"), Color.FromHex((ReadOnlySpan<char>)"#f44336", (Color?)null));
		AddOption(CivBotOrderType.HoldPosition, Loc.GetString("civ-cmd-bot-radial-hold-title"), Loc.GetString("civ-cmd-bot-radial-hold-desc"), Color.FromHex((ReadOnlySpan<char>)"#2196f3", (Color?)null));
		AddOption(CivBotOrderType.Follow, Loc.GetString("civ-cmd-bot-radial-follow-title"), Loc.GetString("civ-cmd-bot-radial-follow-desc"), Color.FromHex((ReadOnlySpan<char>)"#9c27b0", (Color?)null));
		AddOption(CivBotOrderType.Defend, Loc.GetString("civ-cmd-bot-radial-defend-title"), Loc.GetString("civ-cmd-bot-radial-defend-desc"), Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null));
		AddOption(CivBotOrderType.Patrol, Loc.GetString("civ-cmd-bot-radial-patrol-title"), Loc.GetString("civ-cmd-bot-radial-patrol-desc"), Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null));
	}

	private void AddOption(CivBotOrderType order, string title, string desc, Color accent)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		RadialMenuTextureButtonWithSector radialMenuTextureButtonWithSector = new RadialMenuTextureButtonWithSector();
		((Control)radialMenuTextureButtonWithSector).SetSize = new Vector2(100f, 64f);
		radialMenuTextureButtonWithSector.DrawBorder = true;
		radialMenuTextureButtonWithSector.DrawBackground = true;
		Color val = Color.FromHex((ReadOnlySpan<char>)"#101923", (Color?)null);
		radialMenuTextureButtonWithSector.BackgroundColor = ((Color)(ref val)).WithAlpha(0.94f);
		radialMenuTextureButtonWithSector.HoverBackgroundColor = ((Color)(ref accent)).WithAlpha(0.26f);
		radialMenuTextureButtonWithSector.BorderColor = ((Color)(ref accent)).WithAlpha(0.45f);
		radialMenuTextureButtonWithSector.HoverBorderColor = accent;
		radialMenuTextureButtonWithSector.SeparatorColor = ((Color)(ref accent)).WithAlpha(0.28f);
		((Control)radialMenuTextureButtonWithSector).MouseFilter = (MouseFilterMode)0;
		RadialMenuTextureButtonWithSector radialMenuTextureButtonWithSector2 = radialMenuTextureButtonWithSector;
		RichTextLabel val2 = new RichTextLabel
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(8f, 4f),
			MouseFilter = (MouseFilterMode)2
		};
		val2.Text = $"[font size=13][color={((Color)(ref accent)).ToHex()}][bold]{FormattedMessage.EscapeText(title)}[/bold][/color][/font]";
		((Control)radialMenuTextureButtonWithSector2).AddChild((Control)(object)val2);
		((BaseButton)radialMenuTextureButtonWithSector2).OnPressed += delegate
		{
			this.OnOrderSelected?.Invoke(order);
		};
		((Control)radialMenuTextureButtonWithSector2).OnMouseEntered += delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			SetInfoText(title, desc, accent);
		};
		((Control)radialMenuTextureButtonWithSector2).OnMouseExited += delegate
		{
			SetInfoText(Loc.GetString("civ-cmd-bot-radial-default"), null);
		};
		((Control)_options).AddChild((Control)(object)radialMenuTextureButtonWithSector2);
	}

	private void SetInfoText(string title, string? desc, Color? accent = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(((_003F?)accent) ?? Color.FromHex((ReadOnlySpan<char>)"#D8B775", (Color?)null));
		string value = FormattedMessage.EscapeText(title);
		string text = $"[font size=14][color={((Color)(ref val)).ToHex()}][bold]{value}[/bold][/color][/font]";
		if (desc != null)
		{
			text = text + "\n[font size=11][color=#aaaaaa]" + FormattedMessage.EscapeText(desc) + "[/color][/font]";
		}
		_titleLabel.Text = text;
	}
}
