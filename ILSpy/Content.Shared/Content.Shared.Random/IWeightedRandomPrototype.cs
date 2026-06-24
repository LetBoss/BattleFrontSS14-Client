using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Random;

public interface IWeightedRandomPrototype : IPrototype
{
	[ViewVariables]
	Dictionary<string, float> Weights { get; }
}
