using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Traits;

[Prototype(null, 1)]
public sealed class TraitCategoryPrototype : IPrototype
{
	public const string Default = "Default";

	[DataField(null, false, 1, false, false, null)]
	public int? MaxTraitPoints;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public LocId Name { get; private set; } = LocId.op_Implicit(string.Empty);
}
