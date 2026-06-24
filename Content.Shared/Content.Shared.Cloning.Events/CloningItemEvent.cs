using Robust.Shared.GameObjects;

namespace Content.Shared.Cloning.Events;

[ByRefEvent]
public record struct CloningItemEvent(EntityUid CloneUid);
