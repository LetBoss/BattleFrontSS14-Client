using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Guidebook.Richtext;

public sealed class ColorBox : PanelContainer, IDocumentTag
{
	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		control = (Control?)(object)this;
		if (args.TryGetValue("Margin", out string value))
		{
			((Control)this).Margin = new Thickness(float.Parse(value));
		}
		if (args.TryGetValue("HorizontalAlignment", out string value2))
		{
			((Control)this).HorizontalAlignment = Enum.Parse<HAlignment>(value2);
		}
		else
		{
			((Control)this).HorizontalAlignment = (HAlignment)0;
		}
		if (args.TryGetValue("VerticalAlignment", out string value3))
		{
			((Control)this).VerticalAlignment = Enum.Parse<VAlignment>(value3);
		}
		else
		{
			((Control)this).VerticalAlignment = (VAlignment)0;
		}
		StyleBoxFlat val = new StyleBoxFlat();
		if (args.TryGetValue("Color", out string value4))
		{
			val.BackgroundColor = Color.FromHex((ReadOnlySpan<char>)value4, (Color?)null);
		}
		if (args.TryGetValue("OutlineThickness", out string value5))
		{
			val.BorderThickness = new Thickness(float.Parse(value5));
		}
		else
		{
			val.BorderThickness = new Thickness(1f);
		}
		if (args.TryGetValue("OutlineColor", out string value6))
		{
			val.BorderColor = Color.FromHex((ReadOnlySpan<char>)value6, (Color?)null);
		}
		else
		{
			val.BorderColor = Color.White;
		}
		((PanelContainer)this).PanelOverride = (StyleBox)(object)val;
		return true;
	}
}
