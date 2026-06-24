using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Thief;

[Prototype(null, 1)]
public sealed class ThiefBackpackSetPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Content = new List<EntProtoId>();

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Description { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Sprite { get; private set; } = SpriteSpecifier.Invalid;
}
