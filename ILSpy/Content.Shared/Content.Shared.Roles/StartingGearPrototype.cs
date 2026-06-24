using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles;

[Prototype(null, 1)]
public sealed class StartingGearPrototype : IPrototype, IInheritingPrototype, IEquipmentLoadout
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<StartingGearPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public Dictionary<string, EntProtoId> Equipment { get; set; } = new Dictionary<string, EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public List<EntProtoId> Inhand { get; set; } = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public Dictionary<string, List<EntProtoId>> Storage { get; set; } = new Dictionary<string, List<EntProtoId>>();
}
