using System;
using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public record UnparseableValueError(Type T) : IConError
{
	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public FormattedMessage DescribeInner()
	{
		if (T.Constructable())
		{
			FormattedMessage formattedMessage = FormattedMessage.FromUnformatted("The type " + T.PrettyName() + " has no parser available and cannot be parsed.");
			formattedMessage.PushNewline();
			formattedMessage.AddText("Please contact a programmer with this error, they'd probably like to see it.");
			formattedMessage.PushNewline();
			formattedMessage.AddMarkupOrThrow("[bold][color=red]THIS IS A BUG.[/color][/bold]");
			return formattedMessage;
		}
		return FormattedMessage.FromUnformatted("The type " + T.PrettyName() + " cannot be parsed, as it cannot be constructed.");
	}
}
