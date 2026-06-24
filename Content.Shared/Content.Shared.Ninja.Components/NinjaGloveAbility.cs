using Content.Shared.Objectives.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Ninja.Components;

[DataRecord]
public record struct NinjaGloveAbility()
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<ObjectiveComponent>? Objective = null;

	[DataField(null, false, 1, true, false, null)]
	public ComponentRegistry Components = new ComponentRegistry();
}
