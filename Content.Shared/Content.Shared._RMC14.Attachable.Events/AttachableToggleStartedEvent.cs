using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Attachable.Events;

[ByRefEvent]
public readonly record struct AttachableToggleStartedEvent(EntityUid Holder, EntityUid User, string SlotId);
