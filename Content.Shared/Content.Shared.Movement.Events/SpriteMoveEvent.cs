using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public readonly struct SpriteMoveEvent
{
	public readonly bool IsMoving;

	public SpriteMoveEvent(bool isMoving)
	{
		IsMoving = false;
		IsMoving = isMoving;
	}
}
