using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Shared.Log;

namespace Content.Client.Guidebook.Richtext;

public sealed class Table : TableContainer, IDocumentTag
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.table");

	public bool TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
	{
		((Control)this).HorizontalExpand = true;
		control = (Control?)(object)this;
		if (!args.TryGetValue("Columns", out string value) || !int.TryParse(value, out var result))
		{
			Sawmill.Error("Guidebook tag \"Table\" does not specify required property \"Columns.\"");
			control = null;
			return false;
		}
		base.Columns = result;
		return true;
	}
}
