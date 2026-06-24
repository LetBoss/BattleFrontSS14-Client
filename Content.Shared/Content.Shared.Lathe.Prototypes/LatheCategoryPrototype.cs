using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Lathe.Prototypes;

[Prototype(null, 1)]
public sealed class LatheCategoryPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public LocId Name;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
