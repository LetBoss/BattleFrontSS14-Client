using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Storage;

[ByRefEvent]
public record struct CMStorageItemFillEvent(Entity<ItemComponent> Item, StorageComponent Storage);
