using System;
using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public record struct WrongCommandReturn(Type Expected, Type Got) : IConError
{
	public string? Expression { get; set; } = null;

	public Vector2i? IssueSpan { get; set; } = null;

	public StackTrace? Trace { get; set; } = null;

	public FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected an command run that returns type " + Expected.PrettyName() + ", but got " + Got.PrettyName());
	}
}
