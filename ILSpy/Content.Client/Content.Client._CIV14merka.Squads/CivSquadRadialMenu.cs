using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Squads;

public sealed class CivSquadRadialMenu : RadialMenu
{
	private readonly RadialContainer _options;

	private readonly PanelContainer _infoPanel;

	private readonly RichTextLabel _titleLabel;

	private readonly RichTextLabel _descriptionLabel;

	private string _defaultTitle = string.Empty;

	private string _defaultDescription = string.Empty;

	public event Action<CivSquadRadialAction>? OnActionSelected;

	public CivSquadRadialMenu()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		((Control)this).MinSize = new Vector2(560f, 560f);
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		base.CloseButtonStyleClass = "RadialMenuCloseButton";
		base.BackButtonStyleClass = "RadialMenuBackButton";
		RadialContainer radialContainer = new RadialContainer();
		((Control)radialContainer).HorizontalExpand = true;
		((Control)radialContainer).VerticalExpand = true;
		radialContainer.InitialRadius = 148f;
		radialContainer.InnerRadiusMultiplier = 0.62f;
		radialContainer.OuterRadiusMultiplier = 1.55f;
		radialContainer.ReserveSpaceForHiddenChildren = false;
		radialContainer.RadialAlignment = RadialContainer.RAlignment.Clockwise;
		_options = radialContainer;
		((Control)this).AddChild((Control)(object)_options);
		_infoPanel = new PanelContainer
		{
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)3,
			Margin = new Thickness(0f, 0f, 0f, 34f),
			MinSize = new Vector2(276f, 92f),
			MouseFilter = (MouseFilterMode)2
		};
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			SeparationOverride = 2,
			HorizontalExpand = true,
			VerticalExpand = true,
			MouseFilter = (MouseFilterMode)2
		};
		_titleLabel = new RichTextLabel
		{
			HorizontalExpand = true,
			MouseFilter = (MouseFilterMode)2
		};
		_descriptionLabel = new RichTextLabel
		{
			HorizontalExpand = true,
			MouseFilter = (MouseFilterMode)2
		};
		((Control)val).AddChild((Control)(object)_titleLabel);
		((Control)val).AddChild((Control)(object)_descriptionLabel);
		((Control)_infoPanel).AddChild((Control)(object)val);
		((Control)this).AddChild((Control)(object)_infoPanel);
	}

	public void SetOptions(int teamId, string title, string description, IReadOnlyList<CivSquadRadialOption> options)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		((Control)_options).RemoveAllChildren();
		Color accent = GetTeamAccent(teamId);
		_infoPanel.PanelOverride = (StyleBox)(object)CreateInfoStyle(accent);
		_defaultTitle = title;
		_defaultDescription = description;
		SetInfoText(_defaultTitle, _defaultDescription, accent);
		foreach (CivSquadRadialOption option in options)
		{
			CivSquadRadialButton civSquadRadialButton = new CivSquadRadialButton(option.Action, option.Title, option.Description, option.AccentColor);
			((Control)civSquadRadialButton).ToolTip = option.Tooltip;
			CivSquadRadialButton civSquadRadialButton2 = civSquadRadialButton;
			((BaseButton)civSquadRadialButton2).OnPressed += delegate
			{
				this.OnActionSelected?.Invoke(option.Action);
			};
			((Control)civSquadRadialButton2).OnMouseEntered += delegate
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				SetInfoText(option.Title, option.Description, option.AccentColor);
			};
			((Control)civSquadRadialButton2).OnMouseExited += delegate
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				SetInfoText(_defaultTitle, _defaultDescription, accent);
			};
			((Control)_options).AddChild((Control)(object)civSquadRadialButton2);
		}
	}

	public bool TryGetHoveredAction(Control? hovered, out CivSquadRadialAction action)
	{
		if (hovered != null)
		{
			foreach (Control selfAndLogicalAncestor in LogicalExtensions.GetSelfAndLogicalAncestors(hovered))
			{
				if (selfAndLogicalAncestor is CivSquadRadialButton civSquadRadialButton)
				{
					action = civSquadRadialButton.Action;
					return true;
				}
			}
		}
		action = CivSquadRadialAction.Attack;
		return false;
	}

	private void SetInfoText(string title, string description, Color accent)
	{
		string value = FormattedMessage.EscapeText(title);
		string text = FormattedMessage.EscapeText(description);
		_titleLabel.Text = $"[font size=16][color={((Color)(ref accent)).ToHex()}][bold]{value}[/bold][/color][/font]";
		_descriptionLabel.Text = "[font size=11][color=#D4DEE8]" + text + "[/color][/font]";
	}

	private static StyleBoxFlat CreateInfoStyle(Color accent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		StyleBoxFlat val = new StyleBoxFlat();
		Color val2 = Color.FromHex((ReadOnlySpan<char>)"#0F1821", (Color?)null);
		val.BackgroundColor = ((Color)(ref val2)).WithAlpha(0.94f);
		val.BorderColor = ((Color)(ref accent)).WithAlpha(0.85f);
		val.BorderThickness = new Thickness(2f);
		((StyleBox)val).SetContentMarginOverride((Margin)8, 12f);
		((StyleBox)val).SetContentMarginOverride((Margin)1, 10f);
		((StyleBox)val).SetContentMarginOverride((Margin)4, 12f);
		((StyleBox)val).SetContentMarginOverride((Margin)2, 10f);
		return val;
	}

	private static Color GetTeamAccent(int teamId)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(teamId switch
		{
			1 => Color.FromHex((ReadOnlySpan<char>)"#5EA7FF", (Color?)null), 
			2 => Color.FromHex((ReadOnlySpan<char>)"#FF6F63", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#D8B775", (Color?)null), 
		});
	}
}
