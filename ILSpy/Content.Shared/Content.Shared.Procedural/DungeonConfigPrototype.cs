using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class DungeonConfigPrototype : DungeonConfig, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
