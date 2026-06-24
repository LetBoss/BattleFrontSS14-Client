using System;
using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client._CIV14merka.Squads;

public sealed class CivSquadRadialButton : RadialMenuTextureButtonWithSector
{
	private string _title = string.Empty;

	private string _description = string.Empty;

	private readonly Color _accentColor;

	private RichTextLabel? _label;

	public CivSquadRadialAction Action { get; }

	public CivSquadRadialButton(CivSquadRadialAction action, string title, string description, Color accentColor)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		Action = action;
		_title = title;
		_description = description;
		_accentColor = accentColor;
		((Control)this).SetSize = new Vector2(136f, 86f);
		base.DrawBorder = true;
		base.DrawBackground = true;
		Color val = Color.FromHex((ReadOnlySpan<char>)"#101923", (Color?)null);
		base.BackgroundColor = ((Color)(ref val)).WithAlpha(0.94f);
		base.HoverBackgroundColor = ((Color)(ref accentColor)).WithAlpha(0.26f);
		base.BorderColor = ((Color)(ref accentColor)).WithAlpha(0.45f);
		base.HoverBorderColor = accentColor;
		base.SeparatorColor = ((Color)(ref accentColor)).WithAlpha(0.28f);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		_label = new RichTextLabel
		{
			HorizontalExpand = true,
			VerticalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Margin = new Thickness(10f, 8f),
			MouseFilter = (MouseFilterMode)2
		};
		((Control)this).AddChild((Control)(object)_label);
		UpdateLabel();
	}

	protected override void DrawModeChanged()
	{
		((TextureButton)this).DrawModeChanged();
		UpdateLabel();
	}

	private void UpdateLabel()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (_label != null)
		{
			DrawModeEnum drawMode = ((BaseButton)this).DrawMode;
			bool flag = drawMode - 1 <= 1;
			string value = (flag ? "#FFFFFF" : ((Color)(ref _accentColor)).ToHex());
			string value2 = (flag ? "#F3F7FB" : "#C5D0DB");
			_label.Text = $"[font size=13][color={value}][bold]{FormattedMessage.EscapeText(_title)}[/bold][/color][/font]\n[font size=10][color={value2}]{FormattedMessage.EscapeText(_description)}[/color][/font]";
		}
	}
}
