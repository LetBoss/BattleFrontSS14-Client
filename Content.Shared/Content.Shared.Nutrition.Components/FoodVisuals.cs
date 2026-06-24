using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components;

[Serializable]
[NetSerializable]
public enum FoodVisuals : byte
{
	Visual,
	MaxUses
}
