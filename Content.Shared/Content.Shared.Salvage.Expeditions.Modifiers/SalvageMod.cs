using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Prototype("salvageMod", 1)]
public sealed class SalvageMod : IPrototype, ISalvageMod
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("desc", false, 1, false, false, null)]
	public LocId Description { get; private set; } = LocId.op_Implicit(string.Empty);

	[DataField("cost", false, 1, false, false, null)]
	public float Cost { get; private set; }
}
