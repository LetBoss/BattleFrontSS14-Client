using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Contraband;

[Prototype(null, 1)]
public sealed class ContrabandSeverityPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public LocId ExamineText;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowDepartmentsAndJobs;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
