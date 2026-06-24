using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Stacks;

[Prototype(null, 1)]
public sealed class StackPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Spawn { get; private set; } = EntProtoId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public int? MaxCount { get; private set; }
}
