using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.Loot;

[Prototype(null, 1)]
public sealed class SalvageLootPrototype : IPrototype
{
	[DataField("guaranteed", false, 1, false, false, null)]
	public bool Guaranteed;

	[DataField("loots", false, 1, false, false, null)]
	public List<IDungeonLoot> LootRules = new List<IDungeonLoot>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
