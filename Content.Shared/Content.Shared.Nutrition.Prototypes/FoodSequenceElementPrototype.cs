using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Tag;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Nutrition.Prototypes;

[Prototype(null, 1)]
public sealed class FoodSequenceElementPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public List<SpriteSpecifier> Sprites { get; private set; } = new List<SpriteSpecifier>();

	[DataField(null, false, 1, false, false, null)]
	public Vector2 Scale { get; private set; } = Vector2.One;

	[DataField(null, false, 1, false, false, null)]
	public LocId? Name { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool Final { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<TagPrototype>> Tags { get; set; } = new List<ProtoId<TagPrototype>>();
}
