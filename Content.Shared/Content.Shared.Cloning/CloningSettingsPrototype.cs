using System.Collections.Generic;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Cloning;

[Prototype(null, 1)]
public sealed class CloningSettingsPrototype : IPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool ForceCloning = true;

	[DataField(null, false, 1, false, false, null)]
	public SlotFlags? CopyEquipment = SlotFlags.All;

	[DataField(null, false, 1, false, false, null)]
	public bool CopyInternalStorage = true;

	[DataField(null, false, 1, false, false, null)]
	public bool CopyImplants = true;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public HashSet<string> Components = new HashSet<string>();

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public HashSet<string> EventComponents = new HashSet<string>();

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ParentDataField(typeof(PrototypeIdArraySerializer<CloningSettingsPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	[NeverPushInheritance]
	public bool Abstract { get; private set; }
}
