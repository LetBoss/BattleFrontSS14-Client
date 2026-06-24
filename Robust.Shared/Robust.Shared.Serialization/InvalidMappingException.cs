using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Serialization;

[Virtual]
public class InvalidMappingException : Exception
{
	public InvalidMappingException(string msg)
		: base(msg)
	{
	}
}
