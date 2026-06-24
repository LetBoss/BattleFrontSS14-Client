using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Components;

[Serializable]
[NetSerializable]
public record struct PhysicsLinearVelocityDeltaState : IComponentDeltaState<PhysicsComponentState>, IComponentDeltaState, IComponentState
{
	public Vector2 LinearVelocity;

	public void ApplyToFullState(PhysicsComponentState fullState)
	{
		fullState.LinearVelocity = LinearVelocity;
	}

	public PhysicsComponentState CreateNewFullState(PhysicsComponentState fullState)
	{
		return new PhysicsComponentState(fullState)
		{
			LinearVelocity = LinearVelocity
		};
	}
}
