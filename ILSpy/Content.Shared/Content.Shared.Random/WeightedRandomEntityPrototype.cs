using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Random;

[Prototype(null, 1)]
public sealed class WeightedRandomEntityPrototype : IWeightedRandomPrototype, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("weights", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, EntityPrototype>))]
	public Dictionary<string, float> Weights { get; private set; } = new Dictionary<string, float>();
}
