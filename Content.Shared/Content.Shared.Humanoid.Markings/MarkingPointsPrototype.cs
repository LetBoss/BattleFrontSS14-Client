using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings;

[Prototype(null, 1)]
public sealed class MarkingPointsPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool OnlyWhitelisted;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<MarkingCategories, MarkingPoints> Points { get; private set; }
}
