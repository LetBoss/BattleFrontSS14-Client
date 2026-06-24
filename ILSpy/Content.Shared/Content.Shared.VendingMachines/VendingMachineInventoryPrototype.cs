using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines;

[Prototype(null, 1)]
public sealed class VendingMachineInventoryPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("startingInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
	public Dictionary<string, uint> StartingInventory { get; private set; } = new Dictionary<string, uint>();

	[DataField("emaggedInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
	public Dictionary<string, uint>? EmaggedInventory { get; private set; }

	[DataField("contrabandInventory", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
	public Dictionary<string, uint>? ContrabandInventory { get; private set; }
}
