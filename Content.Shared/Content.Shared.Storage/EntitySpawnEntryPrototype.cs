using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage;

[Prototype(null, 1)]
public sealed class EntitySpawnEntryPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntitySpawnEntry> Entries = new List<EntitySpawnEntry>();

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
