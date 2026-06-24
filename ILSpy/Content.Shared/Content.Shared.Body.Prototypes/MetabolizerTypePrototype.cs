using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Body.Prototypes;

[Prototype(null, 1)]
public sealed class MetabolizerTypePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, true, false, null)]
	private LocId Name { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedName => Loc.GetString(LocId.op_Implicit(Name));
}
