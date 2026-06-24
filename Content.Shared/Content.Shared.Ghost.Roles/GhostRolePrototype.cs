using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ghost.Roles;

[Prototype(null, 1)]
public sealed class GhostRolePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId EntityPrototype;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? IconPrototype;

	[DataField(null, false, 1, true, false, null)]
	public string Rules;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public string Name { get; set; }

	[DataField(null, false, 1, true, false, null)]
	public string Description { get; set; }
}
