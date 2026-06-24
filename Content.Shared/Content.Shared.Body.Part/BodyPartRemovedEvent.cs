using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Part;

[ByRefEvent]
public readonly record struct BodyPartRemovedEvent(string Slot, Entity<BodyPartComponent> Part);
