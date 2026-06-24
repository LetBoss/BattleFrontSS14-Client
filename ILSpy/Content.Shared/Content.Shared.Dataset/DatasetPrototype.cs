using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Dataset;

[Prototype(null, 1)]
public sealed class DatasetPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("values", false, 1, false, false, null)]
	public IReadOnlyList<string> Values { get; private set; } = new List<string>();
}
