using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components;

[Serializable]
[NetSerializable]
public enum HungerThreshold : byte
{
	Overfed = 8,
	Okay = 4,
	Peckish = 2,
	Starving = 1,
	Dead = 0
}
