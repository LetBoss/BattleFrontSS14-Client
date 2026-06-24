using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles;

[Prototype(null, 1)]
public sealed class JobRequirementOverridePrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<JobPrototype>, HashSet<JobRequirement>> Jobs = new Dictionary<ProtoId<JobPrototype>, HashSet<JobRequirement>>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<AntagPrototype>, HashSet<JobRequirement>> Antags = new Dictionary<ProtoId<AntagPrototype>, HashSet<JobRequirement>>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
