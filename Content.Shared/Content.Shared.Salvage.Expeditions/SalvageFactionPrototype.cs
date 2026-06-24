using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Salvage.Expeditions;

[Prototype(null, 1)]
public sealed class SalvageFactionPrototype : IPrototype
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("entries", false, 1, true, false, null)]
	public List<SalvageMobEntry> MobGroups = new List<SalvageMobEntry>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("configs", false, 1, false, false, null)]
	public Dictionary<string, string> Configs = new Dictionary<string, string>();

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("desc", false, 1, false, false, null)]
	public LocId Description { get; private set; } = LocId.op_Implicit(string.Empty);
}
