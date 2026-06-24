using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components;

[ByRefEvent]
public record struct InsertIntoEntityStorageAttemptEvent(EntityUid ItemToInsert, EntityUid Container, bool Cancelled = false);
