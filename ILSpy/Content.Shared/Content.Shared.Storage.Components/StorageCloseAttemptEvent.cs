using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components;

[ByRefEvent]
public record struct StorageCloseAttemptEvent(EntityUid? User, bool Cancelled = false);
