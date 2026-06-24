using Robust.Shared.GameObjects;

namespace Content.Shared.Doors;

public sealed class DoorBoltsChangedEvent : EntityEventArgs
{
	public readonly bool BoltsDown;

	public DoorBoltsChangedEvent(bool boltsDown)
	{
		BoltsDown = boltsDown;
	}
}
