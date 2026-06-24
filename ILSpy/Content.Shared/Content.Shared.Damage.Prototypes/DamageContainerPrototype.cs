using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes;

[Prototype(null, 1)]
public sealed class DamageContainerPrototype : IPrototype
{
	[DataField("supportedGroups", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageGroupPrototype>))]
	public List<string> SupportedGroups = new List<string>();

	[DataField("supportedTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
	public List<string> SupportedTypes = new List<string>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
