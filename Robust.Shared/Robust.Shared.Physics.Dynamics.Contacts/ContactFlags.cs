using System;

namespace Robust.Shared.Physics.Dynamics.Contacts;

[Flags]
internal enum ContactFlags : byte
{
	None = 0,
	PreInit = 1,
	Island = 2,
	Filter = 4,
	Grid = 8,
	Deleting = 0x10,
	Deleted = 0x20
}
