using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid.Markings;

[Serializable]
[NetSerializable]
public enum MarkingCategories : byte
{
	Special,
	Hair,
	FacialHair,
	Head,
	HeadTop,
	HeadSide,
	Eyes,
	Snout,
	Chest,
	UndergarmentTop,
	UndergarmentBottom,
	Arms,
	Legs,
	Tail,
	Overlay
}
