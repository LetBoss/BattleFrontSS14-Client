using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Armor;

[ByRefEvent]
public record struct RefreshArmorSpeedTierEvent(SlotFlags TargetSlots, string? SpeedTier = "") : IInventoryRelayEvent;
