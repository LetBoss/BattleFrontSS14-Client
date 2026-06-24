using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random.Rules;

[Prototype(null, 1)]
public sealed class RulesPrototype : IPrototype
{
	[DataField("rules", false, 1, true, false, null)]
	public List<RulesRule> Rules = new List<RulesRule>();

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
