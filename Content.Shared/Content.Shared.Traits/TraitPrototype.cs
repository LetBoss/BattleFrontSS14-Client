using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Traits;

[Prototype(null, 1)]
public sealed class TraitPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? TraitGear;

	[DataField(null, false, 1, false, false, null)]
	public int Cost;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<TraitCategoryPrototype>? Category;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocId Name { get; private set; } = LocId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public LocId? Description { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ComponentRegistry Components { get; private set; }
}
