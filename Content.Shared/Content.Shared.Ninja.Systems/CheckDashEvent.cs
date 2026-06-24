using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

[ByRefEvent]
public record struct CheckDashEvent(EntityUid User, bool Cancelled = false);
