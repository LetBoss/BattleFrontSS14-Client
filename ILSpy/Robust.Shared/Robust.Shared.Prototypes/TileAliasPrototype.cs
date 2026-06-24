using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Prototypes;

[Prototype(null, 1)]
public sealed class TileAliasPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public string Target { get; private set; }

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
