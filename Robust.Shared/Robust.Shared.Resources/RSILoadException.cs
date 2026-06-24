using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Resources;

[Serializable]
[Virtual]
public class RSILoadException : Exception
{
	public RSILoadException()
	{
	}

	public RSILoadException(string message)
		: base(message)
	{
	}

	public RSILoadException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
