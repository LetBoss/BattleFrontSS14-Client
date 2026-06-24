using System.Collections.Generic;
using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Map.Events;

public readonly record struct BeforeSerializationEvent(HashSet<EntityUid> Entities, HashSet<MapId> MapIds, FileCategory Category = FileCategory.Unknown);
