using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public record struct MissingClosingBrace : IConError
{
	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected a closing brace, }.");
	}
}
