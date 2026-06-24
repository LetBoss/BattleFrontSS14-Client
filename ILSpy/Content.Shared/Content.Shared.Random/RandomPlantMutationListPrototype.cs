using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random;

[Prototype("RandomPlantMutationList", 1)]
public sealed class RandomPlantMutationListPrototype : IPrototype
{
	[DataField("mutations", false, 1, true, true, null)]
	public List<RandomPlantMutation> mutations = new List<RandomPlantMutation>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
