using System;
using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public record InvalidEnum<T>(string Value) : IConError where T : unmanaged, Enum
{
	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = FormattedMessage.FromUnformatted($"The value {Value} is not a valid {typeof(T).PrettyName()}.");
		formattedMessage.AddText("Valid values are: " + string.Join(", ", Enum.GetNames<T>()));
		return formattedMessage;
	}
}
