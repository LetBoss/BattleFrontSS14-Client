using Content.Shared.Interaction;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Inventory;

[ByRefEvent]
public record struct ShouldHandleVirtualItemInteractEvent(BeforeRangedInteractEvent Event, bool Handle = false);
