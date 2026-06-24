using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Atmos;

[ByRefEvent]
public sealed class GetFireProtectionEvent : EntityEventArgs, IInventoryRelayEvent
{
	public float Multiplier;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public GetFireProtectionEvent()
	{
		Multiplier = 1f;
	}

	public void Reduce(float by)
	{
		Multiplier -= by;
		Multiplier = MathF.Max(Multiplier, 0f);
	}
}
