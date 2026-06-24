using System.Collections.Generic;
using Content.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Xenoarchaeology.Artifact.Prototypes;

[Prototype(null, 1)]
public sealed class WeightedRandomXenoArchTriggerPrototype : IWeightedRandomPrototype, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, XenoArchTriggerPrototype>))]
	public Dictionary<string, float> Weights { get; private set; } = new Dictionary<string, float>();
}
