using Robust.Shared.GameObjects;

namespace Content.Shared.Containers.ItemSlots;

[ByRefEvent]
public record struct ItemSlotEjectAttemptEvent(EntityUid SlotEntity, EntityUid Item, EntityUid? User, ItemSlot Slot, bool Cancelled = false);
