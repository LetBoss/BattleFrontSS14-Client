using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Mining;

[Prototype(null, 1)]
public sealed class OrePrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? OreEntity;

	[DataField(null, false, 1, false, false, null)]
	public int MinOreYield = 1;

	[DataField(null, false, 1, false, false, null)]
	public int MaxOreYield = 1;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? OreSprite;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
