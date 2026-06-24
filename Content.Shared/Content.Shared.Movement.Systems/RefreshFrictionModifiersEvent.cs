using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

[ByRefEvent]
public record struct RefreshFrictionModifiersEvent : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

	public float Friction;

	public float FrictionNoInput;

	public float Acceleration;

	public void ModifyFriction(float friction, float noInput)
	{
		Friction *= friction;
		FrictionNoInput *= noInput;
	}

	public void ModifyFriction(float friction)
	{
		ModifyFriction(friction, friction);
	}

	public void ModifyAcceleration(float acceleration)
	{
		Acceleration *= acceleration;
	}
}
