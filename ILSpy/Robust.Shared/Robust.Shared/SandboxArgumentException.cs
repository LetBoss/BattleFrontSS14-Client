using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared;

[Serializable]
[Virtual]
public class SandboxArgumentException : Exception
{
	public SandboxArgumentException()
	{
	}

	public SandboxArgumentException(string message)
		: base(message)
	{
	}

	public SandboxArgumentException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
