using System.Collections.Generic;
using Content.Shared.Dataset;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StoryGen;

[Prototype(null, 1)]
public sealed class StoryTemplatePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public LocId LocId;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, ProtoId<LocalizedDatasetPrototype>> Variables = new Dictionary<string, ProtoId<LocalizedDatasetPrototype>>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
