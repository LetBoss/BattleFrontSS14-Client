using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Storage.Events;

[ByRefEvent]
public record struct StorageInsertFailedEvent(Entity<StorageComponent?> Storage, Entity<HandsComponent?> Player);
