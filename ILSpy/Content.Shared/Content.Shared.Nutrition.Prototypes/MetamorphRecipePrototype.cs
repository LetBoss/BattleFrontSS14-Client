using System.Collections.Generic;
using Content.Shared.Nutrition.FoodMetamorphRules;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nutrition.Prototypes;

[Prototype(null, 1)]
public sealed class MetamorphRecipePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<TagPrototype> Key = ProtoId<TagPrototype>.op_Implicit(string.Empty);

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Result;

	[DataField(null, false, 1, false, false, null)]
	public List<FoodMetamorphRule> Rules = new List<FoodMetamorphRule>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
