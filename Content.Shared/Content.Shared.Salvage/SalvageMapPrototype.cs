using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Salvage;

[Prototype(null, 1)]
public sealed class SalvageMapPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath MapPath;

	[DataField(null, false, 1, true, false, null)]
	public LocId SizeString;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
