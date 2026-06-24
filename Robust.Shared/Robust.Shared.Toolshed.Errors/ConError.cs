using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Errors;

public abstract class ConError : IConError
{
	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public abstract FormattedMessage DescribeInner();
}
