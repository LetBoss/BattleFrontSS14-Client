using System.Collections.Generic;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Loadout;

[Prototype(null, 1)]
public sealed class CivLoadoutOptionsPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Faction;

	[DataField(null, false, 1, true, false, null)]
	public CivTdmClass Class;

	[DataField(null, false, 1, true, false, null)]
	public Dictionary<string, List<EntProtoId>> Slots = new Dictionary<string, List<EntProtoId>>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
