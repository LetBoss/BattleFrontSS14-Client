using System;

namespace Content.Shared.Shuttles.Components;

[Flags]
public enum IFFFlags : byte
{
	None = 0,
	HideLabel = 1,
	Hide = 2
}
