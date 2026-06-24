using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random;

[Prototype(null, 1)]
public sealed class WeightedRandomFillSolutionPrototype : IPrototype
{
	[DataField("fills", false, 1, true, false, null)]
	public List<RandomFillSolution> Fills = new List<RandomFillSolution>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
