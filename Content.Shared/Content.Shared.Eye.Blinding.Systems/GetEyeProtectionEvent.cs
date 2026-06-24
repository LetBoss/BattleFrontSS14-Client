using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class GetEyeProtectionEvent : EntityEventArgs, IInventoryRelayEvent
{
	public TimeSpan Protection;

	public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;
}
