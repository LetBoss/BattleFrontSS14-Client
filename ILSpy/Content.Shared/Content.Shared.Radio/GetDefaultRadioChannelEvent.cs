using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio;

public sealed class GetDefaultRadioChannelEvent : EntityEventArgs, IInventoryRelayEvent
{
	public string? Channel;

	public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
}
