using System.Collections.Generic;
using Content.Shared._RMC14.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Decals;

[Prototype(null, 1)]
public sealed class DecalPrototype : IPrototype, IInheritingPrototype, ICMSpecific
{
	[DataField("tags", false, 1, false, false, null)]
	public List<string> Tags = new List<string>();

	[DataField("showMenu", false, 1, false, false, null)]
	public bool ShowMenu = true;

	[DataField("snapCardinals", false, 1, false, false, null)]
	public bool SnapCardinals;

	[DataField(null, false, 1, false, false, null)]
	public bool DefaultCleanable;

	[DataField(null, false, 1, false, false, null)]
	public bool DefaultCustomColor;

	[DataField(null, false, 1, false, false, null)]
	public bool DefaultSnap = true;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("sprite", false, 1, false, false, null)]
	public SpriteSpecifier Sprite { get; private set; } = SpriteSpecifier.Invalid;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<DecalPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool IsCM { get; private set; }
}
