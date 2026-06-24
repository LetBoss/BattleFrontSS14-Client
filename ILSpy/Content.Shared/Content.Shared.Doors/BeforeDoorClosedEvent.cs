using Robust.Shared.GameObjects;

namespace Content.Shared.Doors;

public sealed class BeforeDoorClosedEvent : CancellableEntityEventArgs
{
	public bool Partial;

	public bool PerformCollisionCheck;

	public BeforeDoorClosedEvent(bool performCollisionCheck, bool partial = false)
	{
		Partial = partial;
		PerformCollisionCheck = performCollisionCheck;
	}
}
