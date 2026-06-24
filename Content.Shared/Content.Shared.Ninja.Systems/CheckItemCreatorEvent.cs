using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

[ByRefEvent]
public record struct CheckItemCreatorEvent(EntityUid User, bool Cancelled = false);
