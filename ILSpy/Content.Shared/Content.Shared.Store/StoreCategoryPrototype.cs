using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store;

[Prototype(null, 1)]
public sealed class StoreCategoryPrototype : IPrototype
{
	private string _name = string.Empty;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = "";

	[DataField("priority", false, 1, false, false, null)]
	public int Priority { get; private set; }
}
