using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events;

[ByRefEvent]
public record struct GetMetabolicMultiplierEvent()
{
	public float Multiplier = 1f;
}
