using System;

namespace Robust.Shared.Network;

internal sealed class DummyHWId : IHWId
{
	public byte[] GetLegacy()
	{
		return Array.Empty<byte>();
	}

	public byte[] GetModern()
	{
		return Array.Empty<byte>();
	}
}
