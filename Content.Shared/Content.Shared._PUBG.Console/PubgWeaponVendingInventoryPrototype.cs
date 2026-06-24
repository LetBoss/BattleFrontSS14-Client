using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Console;

[Prototype(null, 1)]
public sealed class PubgWeaponVendingInventoryPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Weapons = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> ChestRigs = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Backpacks = new List<EntProtoId>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
