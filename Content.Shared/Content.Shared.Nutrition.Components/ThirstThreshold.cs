using System;

namespace Content.Shared.Nutrition.Components;

[Flags]
public enum ThirstThreshold : byte
{
	Dead = 0,
	Parched = 1,
	Thirsty = 2,
	Okay = 4,
	OverHydrated = 8
}
