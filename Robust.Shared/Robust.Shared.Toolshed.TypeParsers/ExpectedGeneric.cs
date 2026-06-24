using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public record struct ExpectedGeneric() : IConError
{
	public string? Expression { get; set; } = null;

	public Vector2i? IssueSpan { get; set; } = null;

	public StackTrace? Trace { get; set; } = null;

	public FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected a generic type, did you forget the angle brackets?");
	}
}
