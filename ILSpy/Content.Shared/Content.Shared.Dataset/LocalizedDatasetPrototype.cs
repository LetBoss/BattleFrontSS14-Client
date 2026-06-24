using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Dataset;

[Prototype(null, 1)]
public sealed class LocalizedDatasetPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocalizedDatasetValues Values { get; private set; } = new LocalizedDatasetValues();
}
