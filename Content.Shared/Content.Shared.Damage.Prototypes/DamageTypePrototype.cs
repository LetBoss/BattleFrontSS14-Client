using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes;

[Prototype(null, 1)]
public sealed class DamageTypePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	private LocId Name { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedName => Loc.GetString(LocId.op_Implicit(Name));

	[DataField("armorCoefficientPrice", false, 1, false, false, null)]
	public double ArmorPriceCoefficient { get; set; }

	[DataField("armorFlatPrice", false, 1, false, false, null)]
	public double ArmorPriceFlat { get; set; }
}
