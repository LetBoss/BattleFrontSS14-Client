using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Access;

[Prototype(null, 1)]
public sealed class AccessLevelPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool CanAddToIdCard = true;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<IFFFactionComponent>? Faction;

	[DataField(null, false, 1, false, false, null)]
	public string AccessGroup = "";

	[DataField(null, false, 1, false, false, null)]
	public bool Hidden;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string? Name { get; set; }

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<AccessLevelPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	public string GetAccessLevelName()
	{
		string name = Name;
		if (name != null)
		{
			return Loc.GetString(name);
		}
		return ID;
	}
}
