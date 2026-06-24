using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Nutrition;

[Prototype(null, 1)]
public sealed class FlavorPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("flavorType", false, 1, false, false, null)]
	public FlavorType FlavorType { get; private set; }

	[DataField("description", false, 1, false, false, null)]
	public string FlavorDescription { get; private set; }
}
