using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components;

[ByRefEvent]
public record struct StorageOpenAttemptEvent(EntityUid User, bool Silent, bool Cancelled = false);
