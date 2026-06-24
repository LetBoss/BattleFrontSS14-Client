using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Marines.Skills;

[Prototype(null, 1)]
public sealed class SkillPresetPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntProtoId<SkillDefinitionComponent>, int> Skills = new Dictionary<EntProtoId<SkillDefinitionComponent>, int>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
