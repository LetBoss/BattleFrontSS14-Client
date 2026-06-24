using System.Collections.Generic;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Implants;

[Prototype(null, 1)]
public sealed class ChameleonOutfitPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<JobPrototype>? Job;

	[DataField(null, false, 1, false, false, null)]
	public LocId? Name;

	[DataField(null, false, 1, false, false, null)]
	public LocId? LoadoutName;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<StartingGearPrototype>? StartingGear;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<JobIconPrototype>? Icon;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<DepartmentPrototype>>? Departments;

	[DataField(null, false, 1, false, false, null)]
	public bool HasMindShield;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, EntProtoId> Equipment { get; set; } = new Dictionary<string, EntProtoId>();
}
