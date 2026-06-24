using System;
using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Errors;

public sealed class UnhandledExceptionError : IConError
{
	public Exception Exception;

	public string? Expression { get; set; }

	public Vector2i? IssueSpan { get; set; }

	public StackTrace? Trace { get; set; }

	public FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = new FormattedMessage();
		formattedMessage.AddText(Exception.ToString());
		return formattedMessage;
	}

	public UnhandledExceptionError(Exception exception)
	{
		Exception = exception;
	}
}
