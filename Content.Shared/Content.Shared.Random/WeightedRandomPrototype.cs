using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random;

[Prototype(null, 1)]
public sealed class WeightedRandomPrototype : IWeightedRandomPrototype, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("weights", false, 1, false, false, null)]
	public Dictionary<string, float> Weights { get; private set; } = new Dictionary<string, float>();
}
