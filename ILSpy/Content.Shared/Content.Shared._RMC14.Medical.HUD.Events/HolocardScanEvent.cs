using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.HUD.Events;

[ByRefEvent]
public record struct HolocardScanEvent(bool CanScan, SlotFlags TargetSlots) : IInventoryRelayEvent;
