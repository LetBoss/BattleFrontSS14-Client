using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Guidebook;

[Prototype(null, 1)]
public sealed class GuideEntryPrototype : GuideEntry, IPrototype, IInheritingPrototype, ICMSpecific
{
	public string ID => Id;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<GuideEntryPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool IsCM { get; private set; }
}
