using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.ContentPack;

[Serializable]
[Virtual]
public class TypeCheckFailedException : Exception
{
	public TypeCheckFailedException()
	{
	}

	public TypeCheckFailedException(string message)
		: base(message)
	{
	}

	public TypeCheckFailedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
