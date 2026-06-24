using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing;

[Serializable]
[NetSerializable]
public sealed class ClothingSpeedModifierComponentState : ComponentState
{
	public float WalkModifier;

	public float SprintModifier;

	public ClothingSpeedModifierComponentState(float walkModifier, float sprintModifier)
	{
		WalkModifier = walkModifier;
		SprintModifier = sprintModifier;
	}
}
