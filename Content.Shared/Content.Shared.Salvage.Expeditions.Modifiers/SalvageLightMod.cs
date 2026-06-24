using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Prototype("salvageLightMod", 1)]
public sealed class SalvageLightMod : IPrototype, IBiomeSpecificMod, ISalvageMod
{
	[DataField("color", false, 1, true, false, null)]
	public Color? Color;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("desc", false, 1, false, false, null)]
	public LocId Description { get; private set; } = LocId.op_Implicit(string.Empty);

	[DataField("cost", false, 1, false, false, null)]
	public float Cost { get; private set; }

	[DataField("biomes", false, 1, false, false, typeof(PrototypeIdListSerializer<SalvageBiomeModPrototype>))]
	public List<string>? Biomes { get; private set; }
}
