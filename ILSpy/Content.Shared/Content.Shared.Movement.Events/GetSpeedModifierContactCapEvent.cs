using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public record struct GetSpeedModifierContactCapEvent() : IInventoryRelayEvent
{
	SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

	public float MaxSprintSlowdown = 0f;

	public float MaxWalkSlowdown = 0f;

	public void SetIfMax(float valueSprint, float valueWalk)
	{
		MaxSprintSlowdown = MathF.Max(MaxSprintSlowdown, valueSprint);
		MaxWalkSlowdown = MathF.Max(MaxWalkSlowdown, valueWalk);
	}
}
