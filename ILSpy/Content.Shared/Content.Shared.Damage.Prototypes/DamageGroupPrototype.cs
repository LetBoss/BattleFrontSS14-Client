using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes;

[Prototype(2)]
public sealed class DamageGroupPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	private LocId Name { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedName => Loc.GetString(LocId.op_Implicit(Name));

	[DataField("damageTypes", false, 1, true, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
	public List<string> DamageTypes { get; private set; }
}
