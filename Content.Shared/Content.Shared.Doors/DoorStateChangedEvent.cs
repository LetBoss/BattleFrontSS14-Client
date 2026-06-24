using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Doors;

public sealed class DoorStateChangedEvent : EntityEventArgs
{
	public readonly DoorState State;

	public DoorStateChangedEvent(DoorState state)
	{
		State = state;
	}
}
