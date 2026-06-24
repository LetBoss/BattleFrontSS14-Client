using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Gibbing.Events;

[ByRefEvent]
public record struct EntityGibbedEvent(EntityUid Target, List<EntityUid> DroppedEntities);
