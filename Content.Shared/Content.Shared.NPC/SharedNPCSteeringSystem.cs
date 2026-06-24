using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.NPC;

public abstract class SharedNPCSteeringSystem : EntitySystem
{
	public const byte InterestDirections = 12;

	public const float InterestRadians = (float)Math.PI / 6f;

	public const float InterestDegrees = 30f;
}
