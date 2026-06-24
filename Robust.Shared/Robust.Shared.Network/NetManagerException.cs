using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Network;

[Virtual]
public class NetManagerException : Exception
{
	public NetManagerException(string message)
		: base(message)
	{
	}
}
