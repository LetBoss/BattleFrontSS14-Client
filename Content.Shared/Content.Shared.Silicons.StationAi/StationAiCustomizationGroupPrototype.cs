using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Silicons.StationAi;

[Prototype(null, 1)]
public sealed class StationAiCustomizationGroupPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId Name;

	[DataField(null, false, 1, false, false, null)]
	public StationAiCustomizationType Category;

	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<StationAiCustomizationPrototype>> ProtoIds = new List<ProtoId<StationAiCustomizationPrototype>>();

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
