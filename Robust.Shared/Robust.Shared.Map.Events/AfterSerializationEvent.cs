using System.Collections.Generic;
using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Markdown.Mapping;

namespace Robust.Shared.Map.Events;

public readonly record struct AfterSerializationEvent(HashSet<EntityUid> Entities, MappingDataNode Node, FileCategory Category);
