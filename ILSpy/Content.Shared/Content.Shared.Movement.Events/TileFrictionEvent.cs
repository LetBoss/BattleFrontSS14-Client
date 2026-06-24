using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public struct TileFrictionEvent(float modifier)
{
	public float Modifier = modifier;
}
