using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components;

[ByRefEvent]
public abstract class BaseBeforeStripEvent(TimeSpan initialTime, bool stealth = false) : EntityEventArgs, IInventoryRelayEvent
{
	public readonly TimeSpan InitialTime = initialTime;

	public float Multiplier = 1f;

	public TimeSpan Additive = TimeSpan.Zero;

	public bool Stealth = stealth;

	public TimeSpan Time => TimeSpan.FromSeconds(MathF.Max((float)InitialTime.Seconds * Multiplier + (float)Additive.Seconds, 0f));

	public SlotFlags TargetSlots { get; } = SlotFlags.GLOVES;
}
