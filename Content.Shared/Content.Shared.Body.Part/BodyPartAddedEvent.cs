using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Part;

[ByRefEvent]
public readonly record struct BodyPartAddedEvent(string Slot, Entity<BodyPartComponent> Part);
