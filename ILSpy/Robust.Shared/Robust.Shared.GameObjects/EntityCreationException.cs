using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.GameObjects;

[Serializable]
[Virtual]
public class EntityCreationException : Exception
{
	public EntityCreationException()
	{
	}

	public EntityCreationException(string message)
		: base(message)
	{
	}

	public EntityCreationException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
