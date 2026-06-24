using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Guidebook.Richtext;

public sealed class Box : BoxContainer, IDocumentTag
{
	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).HorizontalExpand = true;
		control = (Control?)(object)this;
		if (args.TryGetValue("Margin", out string value))
		{
			((Control)this).Margin = new Thickness(float.Parse(value));
		}
		if (args.TryGetValue("Orientation", out string value2))
		{
			((BoxContainer)this).Orientation = Enum.Parse<LayoutOrientation>(value2);
		}
		else
		{
			((BoxContainer)this).Orientation = (LayoutOrientation)0;
		}
		if (args.TryGetValue("HorizontalAlignment", out string value3))
		{
			((Control)this).HorizontalAlignment = Enum.Parse<HAlignment>(value3);
		}
		else
		{
			((Control)this).HorizontalAlignment = (HAlignment)2;
		}
		if (args.TryGetValue("VerticalAlignment", out string value4))
		{
			((Control)this).VerticalAlignment = Enum.Parse<VAlignment>(value4);
		}
		return true;
	}
}
