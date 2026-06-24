using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics.Systems;

[ByRefEvent]
public readonly struct PhysicsUpdateAfterSolveEvent(bool prediction, float deltaTime)
{
	public readonly bool Prediction = prediction;

	public readonly float DeltaTime = deltaTime;
}
