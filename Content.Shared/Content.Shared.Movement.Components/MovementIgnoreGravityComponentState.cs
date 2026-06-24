using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Components;

[Serializable]
[NetSerializable]
public sealed class MovementIgnoreGravityComponentState : ComponentState
{
	public bool Weightless;

	public MovementIgnoreGravityComponentState(MovementIgnoreGravityComponent component)
	{
		Weightless = component.Weightless;
	}
}
