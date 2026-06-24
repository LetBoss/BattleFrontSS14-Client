using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Prototypes;

[Serializable]
[Virtual]
public class PrototypeLoadException : Exception
{
	public PrototypeLoadException()
	{
	}

	public PrototypeLoadException(string message)
		: base(message)
	{
	}

	public PrototypeLoadException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
