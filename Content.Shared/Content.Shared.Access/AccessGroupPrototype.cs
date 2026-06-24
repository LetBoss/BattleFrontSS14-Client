using System.Collections.Generic;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Access;

[Prototype(null, 1)]
public sealed class AccessGroupPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public HashSet<ProtoId<AccessLevelPrototype>> Tags;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<IFFFactionComponent>? Faction;

	[DataField(null, false, 1, false, false, null)]
	public string AccessGroup = "";

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string? Name { get; set; }

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<AccessGroupPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	public string GetAccessGroupName()
	{
		string name = Name;
		if (name != null)
		{
			return Loc.GetString(name);
		}
		return ID;
	}
}
