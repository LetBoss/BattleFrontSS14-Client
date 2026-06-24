using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Components;

[ByRefEvent]
public record struct EntityStorageInsertedIntoAttemptEvent(EntityUid ItemToInsert, bool Cancelled = false);
