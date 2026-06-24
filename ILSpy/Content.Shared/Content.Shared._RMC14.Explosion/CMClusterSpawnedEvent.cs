using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Explosion;

[ByRefEvent]
public readonly record struct CMClusterSpawnedEvent(List<EntityUid> Spawned, List<EntityUid> HitEntities, EntityUid OriginEntity, int? ExtraId = null);
