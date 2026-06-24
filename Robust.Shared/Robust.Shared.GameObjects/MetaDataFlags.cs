using System;

namespace Robust.Shared.GameObjects;

[Flags]
public enum MetaDataFlags : byte
{
	None = 0,
	SessionSpecific = 1,
	InContainer = 2,
	Detached = 4,
	Undetachable = 8,
	PvsPriority = 0x10,
	ExtraTransformEvents = 0x20
}
