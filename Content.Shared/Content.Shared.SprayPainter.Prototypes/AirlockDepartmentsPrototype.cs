using System.Collections.Generic;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.SprayPainter.Prototypes;

[Prototype(null, 1)]
public sealed class AirlockDepartmentsPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<string, ProtoId<DepartmentPrototype>> Departments = new Dictionary<string, ProtoId<DepartmentPrototype>>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
