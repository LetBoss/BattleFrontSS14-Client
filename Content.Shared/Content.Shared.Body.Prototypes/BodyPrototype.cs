using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Body.Prototypes;

[Prototype(null, 1)]
public sealed class BodyPrototype : IPrototype, IInheritingPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = "";

	[DataField("root", false, 1, false, false, null)]
	public string Root { get; private set; } = string.Empty;

	[DataField("slots", false, 1, false, false, null)]
	public Dictionary<string, BodyPrototypeSlot> Slots { get; private set; } = new Dictionary<string, BodyPrototypeSlot>();

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<BodyPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	private BodyPrototype()
	{
	}

	public BodyPrototype(string id, string name, string root, Dictionary<string, BodyPrototypeSlot> slots)
	{
		ID = id;
		Name = name;
		Root = root;
		Slots = slots;
	}
}
