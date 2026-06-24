using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Scoping;

[ByRefEvent]
public readonly record struct ScopedEvent(EntityUid User, Entity<ScopeComponent> Scope, SlotFlags TargetSlots = SlotFlags.HEAD) : IInventoryRelayEvent;
