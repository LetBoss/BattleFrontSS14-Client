using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.NPC.Prototypes;

[Prototype(null, 1)]
public sealed class NpcFactionPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<NpcFactionPrototype>> Friendly = new List<ProtoId<NpcFactionPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<NpcFactionPrototype>> Hostile = new List<ProtoId<NpcFactionPrototype>>();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
