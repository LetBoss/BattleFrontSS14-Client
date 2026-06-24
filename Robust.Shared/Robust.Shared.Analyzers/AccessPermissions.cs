using System;

namespace Robust.Shared.Analyzers;

[Flags]
public enum AccessPermissions : byte
{
	None = 0,
	Read = 1,
	Write = 2,
	Execute = 4,
	ReadWrite = 3,
	ReadExecute = 5,
	WriteExecute = 6,
	ReadWriteExecute = 7
}
