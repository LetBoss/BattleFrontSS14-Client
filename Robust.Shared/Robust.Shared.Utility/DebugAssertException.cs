using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Utility;

[Virtual]
public class DebugAssertException : Exception
{
	public DebugAssertException()
	{
	}

	public DebugAssertException(string? message)
		: base(message)
	{
	}
}
