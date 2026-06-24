using System;

namespace Content.Shared.DoAfter;

[Flags]
public enum DuplicateConditions : byte
{
	None = 0,
	SameTool = 2,
	SameTarget = 4,
	SameEvent = 8,
	All = 0xE
}
