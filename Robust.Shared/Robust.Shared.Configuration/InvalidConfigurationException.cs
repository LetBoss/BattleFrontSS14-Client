using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Configuration;

[Serializable]
[Virtual]
public class InvalidConfigurationException : Exception
{
	public InvalidConfigurationException()
	{
	}

	public InvalidConfigurationException(string message)
		: base(message)
	{
	}

	public InvalidConfigurationException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
