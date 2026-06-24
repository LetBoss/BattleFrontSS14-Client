using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Guidebook;

[Virtual]
public class GuideEntry
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath Text;

	[IdDataField(1, null)]
	public string Id;

	[DataField(null, false, 1, true, false, null)]
	public string Name;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<GuideEntryPrototype>> Children = new List<ProtoId<GuideEntryPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public bool FilterEnabled;

	[DataField(null, false, 1, false, false, null)]
	public bool RuleEntry;

	[DataField(null, false, 1, false, false, null)]
	public int Priority;
}
