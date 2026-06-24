using Robust.Shared.GameObjects;

namespace Content.Shared.Storage;

[ByRefEvent]
public record struct StorageInteractAttemptEvent(EntityUid User, bool Silent, bool Cancelled = false);
