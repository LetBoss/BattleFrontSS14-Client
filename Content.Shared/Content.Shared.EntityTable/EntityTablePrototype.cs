using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable;

[Prototype(null, 1)]
public sealed class EntityTablePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public EntityTableSelector Table;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
