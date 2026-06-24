using Robust.Shared.GameObjects;

namespace Content.Shared.Storage;

[ByRefEvent]
public record struct StorageInteractUsingAttemptEvent(bool Cancelled = false);
