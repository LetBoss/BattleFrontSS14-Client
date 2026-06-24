using System;

namespace Robust.Shared.Configuration;

[Flags]
public enum CVar : short
{
	NONE = 0,
	CHEAT = 1,
	SERVER = 2,
	NOT_CONNECTED = 4,
	REPLICATED = 8,
	ARCHIVE = 0x10,
	NOTIFY = 0x20,
	SERVERONLY = 0x40,
	CLIENTONLY = 0x80,
	CONFIDENTIAL = 0x100,
	CLIENT = 0x200
}
