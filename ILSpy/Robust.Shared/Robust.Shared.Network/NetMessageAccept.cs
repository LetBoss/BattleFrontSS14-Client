using System;

namespace Robust.Shared.Network;

[Flags]
public enum NetMessageAccept : byte
{
	None = 0,
	Server = 1,
	Client = 2,
	Both = 3,
	Handshake = 4
}
