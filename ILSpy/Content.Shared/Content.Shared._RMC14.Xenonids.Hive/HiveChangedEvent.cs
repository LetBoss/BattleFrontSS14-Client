using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Hive;

[ByRefEvent]
public record struct HiveChangedEvent(Entity<HiveComponent>? Hive, EntityUid? OldHive);
