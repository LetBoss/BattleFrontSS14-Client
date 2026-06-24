using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Guidebook.Richtext;

public sealed class Document : BoxContainer, IDocumentTag
{
	public Document()
	{
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
	}

	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		control = (Control?)(object)this;
		return true;
	}
}
